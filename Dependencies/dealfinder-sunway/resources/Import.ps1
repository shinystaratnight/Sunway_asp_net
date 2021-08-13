[CmdletBinding()]
param (
    [string] $Client = $null,

    <#[string] $FtpServer,
    [string] $FtpUsername,
    [string] $FtpPassword,
    [string] $FtpRemoteFolder,#>
    [string] $FtpLocalFolder,

    [string] $DbServer,
    [string] $DbDatabase,
    [string] $DbUsername,
    [string] $DbPassword,

    [string] $BcpPath,
    [string] $BcpFolder,
    [int] $RowTolerance = 75,
    #[switch] $NoFTP,
    #[switch] $NoUnzip,

    [string] $SlackWebHookUrl,
    [string] $SlackChannel,
    [string] $SlackToken,

    [string] $FileName,

    [switch] $Force,
    [switch] $NoFlights,
    [switch] $NoProperties,

    [string] $SettingsFile = "import-settings.json"
)
process {
    . .\Process.ps1


    if ($SettingsFile) {
        if (Test-Path $SettingsFile) {
            $Settings = (Get-Content $SettingsFile | ConvertFrom-Json)

            $Client = ?? $Client $Settings.Client
            $DbServer = ?? $DbServer $Settings.DbServer
            $DbDatabase = ?? $DbDatabase $Settings.DbDatabase
            $DbUsername = ?? $DbUsername $Settings.DbUsername
            $DbPassword = ?? $DbPassword $Settings.DbPassword
            $FtpLocalFolder = ?? $FtpLocalFolder $Settings.FtpLocalFolder
            $BcpPath = ?? $BcpPath $Settings.BcpPath
            $BcpFolder = ?? $BcpFolder $Settings.BcpFolder
            $FileName = ?? $FileName $Settings.FileName
            $SlackWebHookUrl = ?? $SlackWebHookUrl $Settings.SlackWebHookUrl
            $SlackChannel = ?? $SlackChannel $Settings.SlackChannel
            $SlackToken = ?? $SlackToken $Settings.SlackToken
        } else {
            Write-Warning "$(Get-Date) Settings file $SettingsFile does not exist"
        }
    }

    Write-Verbose "<SETTINGS>"
    Write-Verbose "`tSettings file: $SettingsFile"
    Write-Verbose "`tClient: $Client"
    Write-Verbose "`tDatabase server: $DbServer"
    Write-Verbose "`tDatabase catalog: $DbDatabase"
    Write-Verbose "`tDatabase username: $DbUsername"
    Write-Verbose "`tDatabase password: $DbPassword"
    Write-Verbose "`tFTP local folder: $FtpLocalFolder"
    Write-Verbose "`tBCP path: $BcpPath"
    Write-Verbose "`tBCP folder: $BcpFolder"
    Write-Verbose "`tSpecific file to process: $FileName"
    Write-Verbose "`tSlack webhook URL: $SlackWebHookUrl"
    Write-Verbose "`tSlack channel: $SlackChannel"
    Write-Verbose "`tSlack token: $SlackToken"
    Write-Verbose "`tProcess flights: $(!$NoFlights)"
    Write-Verbose "`tProcess properties: $(!$NoProperties)"
    Write-Verbose "`tForce operations: $Force"
    Write-Verbose "<SETTINGS>"

    Process-Import `
        -Client $Client `
        -DbServer $DbServer `
        -DbDatabase $DbDatabase `
        -DbUsername $DbUsername `
        -DbPassword $DbPassword `
        -FtpLocalFolder $FtpLocalFolder `
        -BcpPath $BcpPath `
        -BcpFolder $BcpFolder `
        -FileName $FileName `
        -SlackWebHookUrl $SlackWebHookUrl `
        -SlackChannel $SlackChannel `
        -SlackToken $SlackToken `
        -Force $Force `
        -NoFlights $NoFlights `
        -NoProperties $NoProperties `
        -RowTolerance $RowTolerance
}