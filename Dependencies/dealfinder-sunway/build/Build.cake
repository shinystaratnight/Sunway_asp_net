#addin nuget:?package=Cake.Docker&version=0.10.0
#addin nuget:?package=Cake.FileHelpers&version=3.2.0
#addin nuget:?package=SharpZipLib&version=1.1.0
#addin nuget:?package=Cake.Compression&version=0.2.3
#tool "nuget:?package=GitVersion.CommandLine&version=4.0.0"

//////////////////////////////////////////////////////////////////////
// ARGUMENTS
//////////////////////////////////////////////////////////////////////

var target = Argument("target", "Default");
var configuration = Argument("configuration", "Debug");

bool deploy = HasArgument("deploy");
string nugetFeed = Argument("nugetFeedUrl", (string)null);
string nugetApiKey = Argument("nugetFeedApiKey", (string)null);
string octopusFeed = Argument("octopusFeedUrl", (string)null);
string octopusApiKey = Argument("octopusFeedApiKey", (string)null);
string dockerHost = Argument("dockerHost", (string)null);
string dockerUsername = Argument("dockerUsername", (string)null);
string dockerPassword = Argument("dockerPassword", (string)null);
string dockerFeed = Argument("dockerFeed", (string)null);

bool ci = HasArgument("ci");
bool docker = HasArgument("docker");

GitVersion version;

//////////////////////////////////////////////////////////////////////
// ENVIRONMENT VARIABLES
//////////////////////////////////////////////////////////////////////

//string nugetFeed = EnvironmentVariable("bamboo_NuGetFeedURL");
//string nugetApiKey = EnvironmentVariable("bamboo_NuGetFeedAPIKey");

//////////////////////////////////////////////////////////////////////
// SETUP
//////////////////////////////////////////////////////////////////////

var folders = new 
{
    artifacts = "../artifacts/",
    solution = "../",
    apps = "../apps/",
    libs = "../libs/",
    meta = "../meta/",
    tests = "../tests/",
    testResults = "../artifacts/test-results/"
};

//////////////////////////////////////////////////////////////////////
// TARGET: Clean
//////////////////////////////////////////////////////////////////////

Task("Clean")
    .Description("Cleans the working and build output directories")
    .Does(() =>
    {
        CleanDirectories(new DirectoryPath[] {
            folders.artifacts
        });

        CleanDirectories("../apps/**/" + configuration);
        CleanDirectories("../libs/**/" + configuration);
        CleanDirectories("../tests/**/" + configuration);
        CleanDirectories("../samples/**/" + configuration);
    });

//////////////////////////////////////////////////////////////////////
// TARGET: Version

Task("Version")
    .Description("Generates the version information using GitVersion")
    .Does(() =>
    {
        if (ci) 
        {
            GitVersion(new GitVersionSettings
            {
                RepositoryPath = "../",
                UpdateAssemblyInfo = false,
                OutputType = GitVersionOutput.BuildServer
            });
        }

        version = GitVersion(new GitVersionSettings
        {
            RepositoryPath = "../",
            UpdateAssemblyInfo = false,
            OutputType = GitVersionOutput.Json
        });

        var props = File("../Directory.Build.props");
        XmlPoke(props, "/Project/PropertyGroup[@Label='Project']/Version", version.NuGetVersion);
        XmlPoke(props, "/Project/PropertyGroup[@Label='Project']/InformationalVersion", version.InformationalVersion);

        Information("Version: " + version.NuGetVersion);
        Information("Product version: " + version.InformationalVersion);
    });

//////////////////////////////////////////////////////////////////////
// TARGET: Build
//////////////////////////////////////////////////////////////////////

Task("Build-Libs")
    .Description("Builds all projects in the solution")
    .IsDependentOn("Clean")
    .IsDependentOn("Version")
    .Does(() =>
    {
        DotNetCoreBuild(folders.solution, new DotNetCoreBuildSettings
        {
            Configuration = configuration,
            ArgumentCustomization = args =>
            {
                args.Append("/p:SemVer=" + version.NuGetVersion);
                return args;
            }
        });
    });
    
//////////////////////////////////////////////////////////////////////
// TARGET: Build
//////////////////////////////////////////////////////////////////////

Task("Build-Apps")
    .Description("Builds all apps in the solution")
    .IsDependentOn("Clean")
    .IsDependentOn("Version")
    .Does(() =>
    {
        var apps = GetFiles(folders.apps + "**/*.csproj");
        foreach (var app in apps)
        {
            string folder = System.IO.Path.GetDirectoryName(app.FullPath);
            string project = folder.Substring(folder.LastIndexOf('\\') + 1);

            string packageType = XmlPeek(app.FullPath, "/Project/PropertyGroup/PackageType") ?? "nuget";

            DotNetCorePublish(app.FullPath, new DotNetCorePublishSettings
            {
                Configuration = configuration,
                ArgumentCustomization = args =>
                {
                    args.Append("/p:SemVer=" + version.NuGetVersion);
                    return args;
                },
                OutputDirectory = folders.artifacts + "apps/" + packageType + "/" + project
            });
        }

        // Version the packages ready for shipping
        var nuspecs = GetFiles(folders.artifacts + "apps/**/*.nuspec");
        foreach (var nuspec in nuspecs) 
        {
            XmlPoke(nuspec, "/ns:package/ns:metadata/ns:version", version.NuGetVersion, new XmlPokeSettings
            {
                Namespaces = new Dictionary<string, string>
                {
                    { "ns", "http://schemas.microsoft.com/packaging/2012/06/nuspec.xsd" }
                }
            });
        }

        // Determine if there are any Cake build scripts to run
        var cakes = GetFiles(folders.artifacts + "apps/**/build.cake");
        foreach (var cake in cakes)
        {
            var settings = GetCakeSettings(Context, new Dictionary<string, string>
            {
                { "target", "Default" },
                { "configuration", "Configuration" }
            });

            CakeExecuteScript(cake, settings);
        }
    });

//////////////////////////////////////////////////////////////////////
// TARGET: Test
//////////////////////////////////////////////////////////////////////

Task("Test")
    .Description("Runs unit tests")
    .IsDependentOn("Build-Libs")
    .Does(() =>
    {
        var tests = GetFiles(folders.tests + "**/*.csproj");
        foreach (var test in tests)
        {
            string folder = System.IO.Path.GetDirectoryName(test.FullPath);
            string project = folder.Substring(folder.LastIndexOf('\\') + 1);
            string resultsFile = project + ".xml";
            string fullResultsFile = folders.testResults + project + ".xml";

            CreateDirectory(folders.testResults);
            DotNetCoreTest(test.FullPath, new DotNetCoreTestSettings
            {
                Configuration = configuration,
                NoBuild = true,
                ResultsDirectory = folders.testResults,
                Logger = "trx;LogFilename=" + resultsFile
            });
        }
    });

//////////////////////////////////////////////////////////////////////
// Target: Pack
//////////////////////////////////////////////////////////////////////

Task("Pack")
    .Description("Packs the output of projects")
    .IsDependentOn("Build-Libs")
    .IsDependentOn("Build-Apps")
    .IsDependentOn("Test")
    .Does(() =>
    {
        var projects = GetFiles(folders.libs + "**/*.csproj");
        foreach (var project in projects)
        {
            DotNetCorePack(project.FullPath, new DotNetCorePackSettings
            {
                ArgumentCustomization = args =>
                {
                    //args.Append("--include-symbols");
                    args.Append("/p:SemVer=" + version.NuGetVersion);
                    return args;
                },
                Configuration = configuration,
                OutputDirectory = folders.artifacts,
                NoBuild = true
            });
        }

        var specs = GetFiles(folders.meta + "**/*.nuspec");
        if (specs.Any())
        { 
            CreateDirectory(folders.artifacts + "meta/");
            CopyDirectory(folders.meta, folders.artifacts + "meta/");
            ReplaceTextInFiles(folders.artifacts + "meta/*.nuspec", "$version$", version.NuGetVersion);

            specs = GetFiles(folders.artifacts + "meta/*.nuspec");
            foreach (var spec in specs)
            {
                NuGetPack(spec, new NuGetPackSettings
                {
                    OutputDirectory = folders.artifacts
                });
            }
        }

        var appSpecs = GetFiles(folders.artifacts + "apps/**/*.nuspec");
        if (appSpecs.Any())
        { 
            foreach (var spec in appSpecs)
            {
                NuGetPack(spec, new NuGetPackSettings
                {
                    OutputDirectory = folders.artifacts + "apps"
                });
            }
        }

        if (DirectoryExists(folders.artifacts + "apps/zip")) 
        {
            var zipSpecs = GetSubDirectories(folders.artifacts + "apps/zip");
            if (zipSpecs.Any())
            {
                foreach (var spec in zipSpecs)
                {
                    string zip = spec.Segments.Last() + "." + version.NuGetVersion + ".zip";

                    Information(zip);
                    ZipCompress(spec.FullPath, (folders.artifacts + "apps/" + zip));
                }
            }
        }
    });

//////////////////////////////////////////////////////////////////////
// TARGET: Publish
//////////////////////////////////////////////////////////////////////

Task("Publish-Libs")
    .Description("Pushes the output of projects to a NuGet server")
    .WithCriteria(() => deploy)
    .IsDependentOn("Pack")
    .Does(() =>
    {
        if (string.IsNullOrEmpty(nugetFeed) || string.IsNullOrEmpty(nugetApiKey))
        {
            Error("NuGet feed URL and API key must be provided.");
        }
        else
        {
            var packages = GetFiles(folders.artifacts + "*.nupkg");
            foreach (var package in packages)
            {
                NuGetPush(package, new NuGetPushSettings
                {
                    Source = nugetFeed,
                    ApiKey = nugetApiKey
                });
            }
        }
    });

//////////////////////////////////////////////////////////////////////
// TARGET: Publish
//////////////////////////////////////////////////////////////////////

Task("Publish-Apps")
    .Description("Pushes the output of apps to a Octopus NuGet server")
    .WithCriteria(() => deploy)
    .IsDependentOn("Pack")
    .Does(() =>
    {
        if (string.IsNullOrEmpty(octopusFeed) || string.IsNullOrEmpty(octopusApiKey))
        {
            Error("NuGet feed URL and API key must be provided.");
        }
        else
        {
            var packages = GetFiles(folders.artifacts + "apps/*.nupkg");
            foreach (var package in packages)
            {
                NuGetPush(package, new NuGetPushSettings
                {
                    Source = octopusFeed,
                    ApiKey = octopusApiKey
                });
            }
        }
    });

//////////////////////////////////////////////////////////////////////
// TARGET: Docker Build
//////////////////////////////////////////////////////////////////////

Task("Docker")
    .WithCriteria(() => docker)
    .IsDependentOn("Version")
    .IsDependentOn("Publish-Apps")
    .Does(() => 
    {
        if (string.IsNullOrEmpty(dockerHost) 
            || string.IsNullOrEmpty(dockerFeed)
            || string.IsNullOrEmpty(dockerUsername)
            || string.IsNullOrEmpty(dockerPassword))
        {
            Error("Docker connection details are not provided.");
        }
        else
        {
            // Log into the Docker registry so we can push images
            DockerLogin(dockerUsername, dockerPassword, dockerHost);

            var docks = GetFiles(folders.artifacts + "apps/**/Dockerfile");
            foreach (var dock in docks)
            {
                string folder = System.IO.Path.GetDirectoryName(dock.FullPath);
                string project = folder.Substring(folder.LastIndexOf('\\') + 1);

                string tag = project.ToLower() + ":" + version.NuGetVersion;
                string publishTag = dockerHost + "/" + dockerFeed + "/" + tag;

                // Build the image
                DockerBuild(new DockerImageBuildSettings
                {
                    Tag = new[] { tag },
                    WorkingDirectory = folder
                }, folder);

                // Tag the image
                DockerTag(tag, publishTag);

                // Push the image
                DockerPush(publishTag);

                if (version.BranchName == "master" || version.BranchName.StartsWith("release"))
                {
                    string latestTag = project.ToLower() + ":latest";
                    string publishLatestTag = dockerHost + "/" + dockerFeed + "/" + latestTag;

                    // Tag the image
                    DockerTag(tag, publishLatestTag);

                    // Push the image
                    DockerPush(publishLatestTag);
                
                    // Remove the images
                    DockerRemove(new DockerImageRemoveSettings(), new[] { publishLatestTag });
                }

                // Remove the images
                DockerRemove(new DockerImageRemoveSettings(), new[] { tag, publishTag });
            }
        }
    });

//////////////////////////////////////////////////////////////////////
// TASK TARGETS
//////////////////////////////////////////////////////////////////////

Task("Default")
.IsDependentOn("Version")
.IsDependentOn("Pack")
.IsDependentOn("Publish-Libs")
.IsDependentOn("Publish-Apps")
.IsDependentOn("Docker")
;

//////////////////////////////////////////////////////////////////////
// EXECUTION
//////////////////////////////////////////////////////////////////////

RunTarget(target);

public CakeSettings GetCakeSettings(ICakeContext context, IDictionary<string, string> arguments = null)
{
    var settings = new CakeSettings { Arguments = arguments };

    if (context.Environment.Runtime.IsCoreClr)
    {
        var cakePath = System.IO.Path
            .Combine(context.Environment.ApplicationRoot.FullPath, "Cake.dll")
            .Substring(Context.Environment.WorkingDirectory.FullPath.Length + 1);

        settings.ToolPath = System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName;
        settings.ArgumentCustomization = args => string.Concat(cakePath, " ", args.Render());
    }

    return settings;
}