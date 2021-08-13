[CmdletBinding()]
Param([bool] $local = $TRUE, [string] $branch = $NULL)

function ReadBuildProps
{
    $buildProps = ConvertFrom-StringData (Get-Content ../build.version -raw);
    $buildObject = New-Object PSObject -Property $buildProps | Select-Object major, minor, patch;
    return $buildObject;
}

function ReadBranchName {
    Set-Location ../
    $branchName = (& git rev-parse --abbrev-ref HEAD).Trim();
    Set-Location ./build
    Write-Host "[GIT] Branch name = $($branchName)"
    return $branchName;
}

function ReadCommitCount {
    Set-Location ../
    $count = (& git rev-list --all --count).Trim();
    Write-Host "[GIT] Commit count = $($count)"
    Set-Location ./build

    return $count;
}

function CreateVersion (
    [string] $major,
    [string] $minor,
    [string] $patch,
    [string] $branch,
    [string] $commits,
    [bool] $local = $TRUE) {

    $version = [string]::Concat($major, ".", $minor, ".", $patch);
    $prerelease = $NULL;
    $commits = $commits.PadLeft(5, "0");
    $branch = $branch.Replace('/', '-').Replace("feature", "ft");

    if ($branch -ne "release") {
        if ($local) {
            $prerelease = $branch;
        } else {
            $prerelease = "$($branch)-$($commits)";
        }
    }

    if ($prerelease -ne $NULL) {
        $semVer = [string]::Concat($version, "-", $prerelease);
    } else {
        $semVer = $version;
    }

    $msVersion = [string]::Concat($version, ".0");

    return @{
        Version = $version
        SemanticVersion = $semVer
        MsVersion = $msVersion
        PreRelease = $prerelease
    };
}

function WriteVersion (
    [string] $version,
    [string] $semanticVersion,
    [string] $msVersion,
    [string] $prerelease) {

    Set-Content ../version.version "version=$($version)`nsemanticVersion=$($semanticVersion)`nmsVersion=$($msVersion)`nprerelease=$($prerelease)";
}

function ResolveVersion ([bool] $local = $TRUE) {
    $parts = ReadBuildProps;
    if (!$branch) {
        $branch = ReadBranchName;
    }
    $count = ReadCommitCount;
    $version = CreateVersion -major $parts.major -minor $parts.minor -patch $parts.patch -branch $branch -commits $count -local $local;

    WriteVersion -version $version.Version -semanticVersion $version.SemanticVersion -msVersion $version.MsVersion -prerelease $version.PreRelease;
}

ResolveVersion -local $local