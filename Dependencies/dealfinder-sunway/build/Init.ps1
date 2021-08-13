[CmdletBinding()]
param(
    [Parameter(Mandatory=$true, Position = 0)]
    [string] $project,
    [Parameter(Position = 1)]
    [string] $package = $null,
    [Parameter(Position = 2)]
    [string] $jiraKey = $null,
    [Parameter(Position = 3)]
    [string] $module = $null,
    [Parameter(Position = 4)]
    [string] $template = 'Module',
    [Parameter(Position = 5)]
    [string] $origin = $null,
    [Parameter(Position = 6)]
    [string] $buildDirectory = $null
)
process {
    if (!$PSScriptRoot -and $buildDirectory) {
        $PSScriptRoot = $buildDirectory;
    }

    # Import the context factory and create it
    . $PSScriptRoot\template\ContextFactory.ps1;
    $factory = [ContextFactory]::new();

    # Create a default context
    $context = $factory.create(
        $project,
        $package,
        $jiraKey,
        $module,
        $template,
        $origin
    );
    # Execute the solution template.
    $context.Execute();

    Set-Location $context.TargetDirectory;

    $repo = (Test-Path .git);
    if ($repo) {
        # Create an initial commit if a JIRA key is provided
        if ($jiraKey) {
            $commit = "$($jiraKey): Initial commit of project structure from template '$template'.";
            & git add -A;
            & git commit -m $commit;
        }

        if ($origin) {
            # Set the upstream origin.
            & git remote add origin $origin;

            if ($jiraKey) {
                & git push --set-upstream origin master;
            }
        }

        # Checkout the develop branch.
        & git checkout -b develop;
    }

    # Reset the working location back.
    Set-Location $context.BuildDirectory;
    #endregion
}