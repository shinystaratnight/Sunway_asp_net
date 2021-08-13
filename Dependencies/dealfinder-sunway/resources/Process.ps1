# Import scripts
. .\scripts\framework.ps1
. .\scripts\bcp.ps1
. .\scripts\database.ps1
. .\scripts\ftp.ps1
. .\scripts\gzip.ps1
. .\scripts\logger.ps1
. .\scripts\resource.ps1
. .\scripts\slack.ps1

function Process-Import {
    [CmdletBinding()]
    param (
        [Parameter(Mandatory=$true)] [string] $Client,

        <#[string] $FtpServer,
        [string] $FtpUsername,
        [string] $FtpPassword,
        [string] $FtpRemoteFolder = "",#>
        [Parameter(Mandatory=$true)] [string] $FtpLocalFolder,

        [Parameter(Mandatory=$true)] [string] $DbServer,
        [Parameter(Mandatory=$true)] [string] $DbDatabase,
        [Parameter(Mandatory=$true)] [string] $DbUsername,
        [Parameter(Mandatory=$true)] [string] $DbPassword,

        [Parameter(Mandatory=$true)] [string] $BcpPath,
        [Parameter(Mandatory=$true)] [string] $BcpFolder,

        #[bool] $NoFTP,
        #[bool] $NoUnzip,

        [string] $FileName,

        [Parameter(Mandatory=$true)] [string] $SlackWebHookUrl,
        [Parameter(Mandatory=$true)] [string] $SlackChannel,
        [Parameter(Mandatory=$true)] [string] $SlackToken,

        [Parameter(Mandatory=$true)] [bool] $Force,
        [Parameter(Mandatory=$true)] [bool] $NoFlights,
        [Parameter(Mandatory=$true)] [bool] $NoProperties,

        [Parameter(Mandatory=$true)] [int] $RowTolerance
    )
    process {
        if (!(Test-Path $FtpLocalFolder)) {
            mkdir $FtpLocalFolder > $null;
        }
        if (!(Test-Path $BcpFolder)) {
            mkdir $BcpFolder > $null;
        }

        $templates = @(
            (New-Template -pattern "\-(return|outbound)\.csv\.gz" -label "^(?<label>.*?\-(?:return|outbound))" -processFlights)
            ,(New-Template -pattern "^ttss_hotels_(.*)\.csv\.gz" -label "^ttss_hotels_(?<label>.*?)\." -processProperties)
        )

        # MA - We don't need FTP support right now
        $NoFTP = $true

        $logger = New-Logger -fileName $errorlog
        $logger.LogStart();

        $processSupport = $false;
        $anyErrors = $false;
        
        $importGuid = [Guid]::NewGuid()
        #

        if ($NoFTP <# -or (Run-Ftp $FtpServer $FtpUsername $FtpPassword $FtpRemoteFolder $FtpLocalFolder $logger) #>) {     

            $files = Get-Resources -templates $templates -Path $FtpLocalFolder -NoFlights $NoFlights -NoProperties $NoProperties;
            $files | % {
                Write-Verbose "<RESOURCE>"
                Write-Verbose "`tPackage: $($_.Package)"
                Write-Verbose "`tFileName: $($_.FileName)"
                Write-Verbose "`tLabel: $($_.Label)"
                Write-Verbose "`tProcessFlights: $($_.ProcessFlights)"
                Write-Verbose "`tProcessProperties: $($_.ProcessProperties)"
                Write-Verbose "</RESOURCE>"
            }

            if ($files.Count -gt 0) {
                Log-Import -Server $DbServer -Database $DbDatabase -Username $DbUsername -Password $DbPassword -Guid $importGuid -Started $logger.Start

                $files | % {
                    if (!$FileName -Or ($FileName.ToLower() -eq $_.FileName.ToLower())) {
                        $package = Join-Path $FtpLocalFolder $_.Package
                        $file = Join-Path $BcpFolder $_.FileName
    
                        $importFileGuid = [Guid]::NewGuid()
    
                        if (Test-Path $package) {
                            $check = $_.CanProcess()
                            Write-Verbose "<RESOURCE-CHECK>"
                            Write-Verbose "`tPackage: $($_.Package)"
                            Write-Verbose "`tAvailable: $($check.Available)"
                            Write-Verbose "`tState: $($check.State)"
                            Write-Verbose "</RESOURCE-CHECK>"

                            if ($check.Available) {
                                $lastModified = (Get-Item $package).LastWriteTime
                                $history = Log-ImportFile $DbServer $DbDatabase $DbUsername $DbPassword $_.FileName $_.Label (?: $_.ProcessFlights 'F' 'P') $lastModified
                                
                                if ($history) {
                                    if ($history.Import -Or $Force) {
                                        Slack-NotifyImportStart -Resource $_ -Forced $Force -LastModified $lastModified;

                                        # Decompress the CSV content
                                        $gzip = $logger.StartSpan("ZIP-$($_.Label)");
                                        try {
                                            Write-Host "$(Get-Date) Decompressing $package to $file"
                                            DeGZip-File $package $file
                                            $gzip.EndSpan();    
                                        } catch {
                                            $gzip.FailSpan($_.Exception)
                                        }
                                    
                                        if (!$gzip.HasError -and (Test-Path $file)) {
                                            # Truncate the import tables
                                            $truncate = $logger.StartSpan("TRUNCATE-$($_.Label)");
                                            try {
                                                $importTable = (?: $_.ProcessFlights "Flight" "Property")
                                                Write-Host "$(Get-Date) Truncating $importTable import tables"

                                                Truncate `
                                                    -Server $DbServer `
                                                    -Database $DbDatabase `
                                                    -Username $DbUsername `
                                                    -Password $DbPassword `
                                                    -Table $importTable

                                                $truncate.EndSpan();
                                            } catch {
                                                $truncate.FailSpan($_.Exception)
                                            }  
        
                                            # Resolve these based on file type.
                                            $tableName = ?: $_.ProcessProperties "dbo.Pack_ImportProperty" "dbo.Pack_ImportFlight";
                                            $bcpFormatFile = Resolve-Path (?: $_.ProcessProperties "BCPFormatPackProperty.xml" "BCPFormatPackFlight.xml");
        
                                            Log-ImportFileHistory -Server $DbServer -Database $DbDatabase -Username $DbUsername -Password $DbPassword -ImportGuid $importGuid -ImportFileGuid $importFileGuid `
                                                                    -FileName $_.FileName -ZippedFileSize (Get-Item $package).Length -UnzippedFileSize (Get-Item $file).Length -UnzipStarted $gzip.Start `
                                                                    -UnzipEnded $gzip.End -UnzipTime $gzip.Delta -UnzipError $gzip.CapturedError
        
                                            # Run the BCP Import process
                                            $bcp = $logger.StartSpan("BCP-$($_.Label)")
                                            try {
                                                Write-Host "$(Get-Date) Running BCP for $file to $DbDatabase.$tableName using format file $bcpFormatFile"

                                                RunBCP `
                                                    -Logger $logger `
                                                    -Db $DbDatabase `
                                                    -TableName $tableName `
                                                    -DbUsername $DbUsername `
                                                    -DbPassword $DbPassword `
                                                    -ServerIP $DbServer `
                                                    -FormatFile $bcpFormatFile `
                                                    -Bcp $BcpPath `
                                                    -inFile $file
                                                    
                                                $bcp.EndSpan();
                                            } catch {
                                                $bcp.FailSpan($_.Exception)
                                            }
                                            Log-ImportFileHistory -Server $DbServer -Database $DbDatabase -Username $DbUsername -Password $DbPassword -ImportGuid $importGuid -ImportFileGuid $importFileGuid `
                                                                    -FileName $_.FileName -BcpStarted $bcp.Start -BcpEnded $bcp.End -BcpTime $bcp.Delta -BcpError $bcp.CapturedError
        
                                            if (!$bcp.HasError) {
                                                $lastImportStats = Get-LastFileImportStatistics `
                                                    -Server $DbServer `
                                                    -Database $DbDatabase `
                                                    -Username $DbUsername `
                                                    -Password $DbPassword `
                                                    -FileName $_.FileName `
                                                    -Type (?: $_.ProcessFlights "Flight" "Property")

                                                if ($lastImportStats.NewRows -gt 0) {
                                                    $processRows = $true
                                                    $toleranceFailure = $false

                                                    if ($lastImportStats.LastRows -gt 0) {
                                                        $result = (($lastImportStats.NewRows / $lastImportStats.LastRows) * 100)
                                                        if ($result -lt $RowTolerance) {
                                                            Slack-NotifyImportToleranceFailed -Resource $_ -Stats $lastImportStats -Tolerance $RowTolerance -Result $result -Override $Force

                                                            if ($Force) {
                                                                Write-Warning "$(Get-Date) Import file failed tolerance check, however Force flag is in use and the file will be processed"
                                                            } else {
                                                                Write-Error "$(Get-Date) Will not process import of file ($_.FileName) as the number of rows < $($RowTolerance)% of last successful import"
                                                                $processRows = $false
                                                                $toleranceFailure = $true
                                                            }
                                                        }
                                                    }

                                                    if ($processRows) {
                                                        if ($_.ProcessFlights) {
                                                            $processSupport = $true;
                                                            $flights = $logger.StartSpan("PROCESS-FLIGHTS-$($_.Label)");
                                                            try {
                                                                Write-Host "$(Get-Date) Importing flights"
                                                                # Post-process Flights
                                                                PostProcessFlights $DbServer $DbDatabase $DbUsername $DbPassword $importFileGuid
                                                                $flights.EndSpan();
                                                            } catch {
                                                                $flights.FailSpan($_.Exception)
                                                            }
                                                            Log-ImportFileHistory -Server $DbServer -Database $DbDatabase -Username $DbUsername -Password $DbPassword -ImportGuid $importGuid -ImportFileGuid $importFileGuid `
                                                                                    -FileName $_.FileName -ImportStarted $flights.Start -ImportEnded $flights.End -ImportTime $flights.Delta -ImportError $flights.CapturedError
                                                        }
                    
                                                        if ($_.ProcessProperties) {
                                                            $processSupport = $true;
                                                            $properties = $logger.StartSpan("PROCESS-PROPERTIES-$($_.Label)");
                                                            try {
                                                                Write-Host "$(Get-Date) Importing properties"
                                                                # Post-process Properties
                                                                PostProcessProperty $DbServer $DbDatabase $DbUsername $DbPassword $importFileGuid
                                                                $properties.EndSpan();
                                                            } catch {
                                                                $properties.FailSpan($_.Exception)
                                                            }
                                                            Log-ImportFileHistory -Server $DbServer -Database $DbDatabase -Username $DbUsername -Password $DbPassword -ImportGuid $importGuid -ImportFileGuid $importFileGuid `
                                                                                    -FileName $_.FileName -ImportStarted $properties.Start -ImportEnded $properties.End -ImportTime $properties.Delta -ImportError $properties.CapturedError
                                                        }
                                                    }
                                                }
                                            }
        
                                            $stats = Get-ImportFileHistoryStats -Server $DbServer -Database $DbDatabase -Username $DbUsername -Password $DbPassword -Guid $importFileGuid
        
                                            if ($SlackWebHookUrl) {
                                                # Report via Slack
                                                $fileErrors = Report-SlackFile -Logger $logger -FtpLocalFolder $FtpLocalFolder -BcpFolder $BcpFolder -File $_ -Guid $importFileGuid -Stats $stats
                                                if ($fileErrors) {
                                                    $anyErrors = $true;
                                                }
                                            }
                                        } else {
                                            Write-Error "$(Get-Date) Failed to decompress $package to $file"
                                        }
                                    } else {
                                        if ($history.Block) {
                                            Write-Warning "$(Get-Date) Will not import $file as this file is currently blocked"
                                        } else {
                                            Write-Warning "$(Get-Date) Will not import $file as this has already been imported"
                                        }
                                    }
                                } else {
                                    Write-Error "$(Get-Date) Unable to resolve import history for $file"
                                }
                            } elseif ($check.State -eq 'Unauthorised') {
                                Write-Error "$(Get-Date) Source package $package will be skipped because it is not available: $($check.State)"
                                Log-ImportFileHistory -Server $DbServer -Database $DbDatabase -Username $DbUsername -Password $DbPassword -ImportGuid $importGuid -ImportFileGuid $importFileGuid `
                                                        -FileName $_.FileName -UnzipError $check.State
    
                                $stats = Get-ImportFileHistoryStats -Server $DbServer -Database $DbDatabase -Username $DbUsername -Password $DbPassword -Guid $importFileGuid
                                Report-SlackFile -Logger $logger -FtpLocalFolder $FtpLocalFolder -BcpFolder $BcpFolder -File $_ -Guid $importFileGuid -Stats $stats -ToleranceFail $toleranceFailure
                            } else {
                                Write-Warning "$(Get-Date) Source package $package will be skipped because it is not available: $($check.State)"
                                Log-ImportFileHistory -Server $DbServer -Database $DbDatabase -Username $DbUsername -Password $DbPassword -ImportGuid $importGuid -ImportFileGuid $importFileGuid `
                                                        -FileName $_.FileName -UnzipError $check.State
                            }                           
                        } else {
                            Write-Error "$(Get-Date) Source package $package was not found"
                        }
                    }
                }
            }
        }

        $logger.LogEnd();
        
        Log-Import -Server $DbServer -Database $DbDatabase -Username $DbUsername -Password $DbPassword -Guid $importGuid -Ended $logger.End -Time $logger.Delta

        $logger.Clean();

        Write-Verbose "<LOGGER>"
        Write-Verbose "FileName: $($logger.FileName)"
        Write-Verbose "Start: $($logger.Start)"
        Write-Verbose "End: $($logger.End)"
        Write-Verbose "Delta: $($logger.Delta)"

        $logger.Spans | % {
            Write-Verbose "`t<SPAN>"
            Write-Verbose "`t`tName: $($_.Name)"
            Write-Verbose "`t`tStart: $($_.Start)"
            Write-Verbose "`t`tEnd: $($_.End)"
            Write-Verbose "`t`tDelta: $($_.Delta)"
            Write-Verbose "`t</SPAN>"
        }

        Write-Verbose "</LOGGER>"
    }
}