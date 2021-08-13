Import-Module Invoke-SqlCmd2

function PostProcessFlights{
    Param(
        $server,
        $database,
        $username,
        $password,
        $guid
    )
    $credentials = New-Object Management.Automation.PSCredential ($username, (ConvertTo-SecureString $password -AsPlainText -Force))

    Invoke-SqlCmd2 `
        -Query "update Pack_ImportFlight set cost=CostInPence/100 where cost is null" `
        -ServerInstance $server `
        -Database $database `
        -Credential $credentials `
        -QueryTimeout 1200

    Invoke-SqlCmd2 `
        -Query "update Pack_ImportFlight set DepartureDate=convert(date,right(DepartureDateString,4)+'-'+substring(DepartureDateString,4,2)+'-'+left(DepartureDateString,2)) where DepartureDate is null" `
        -ServerInstance $server `
        -Database $database `
        -Credential $credentials `
        -QueryTimeout 1200

    $parameters = @{
        ImportFileGuid = $guid;
    }

    Invoke-SqlCmd2 `
        -Query "exec Pack_GeneratePackFlight @ImportFileGuid" `
        -ServerInstance $server `
        -Database $database `
        -Credential $credentials `
        -QueryTimeout 3600 `
        -SqlParameters $parameters
}

function PostProcessProperty{
    Param(
        $server,
        $database,
        $username,
        $password,
        $guid
    )
    $credentials = New-Object Management.Automation.PSCredential ($username, (ConvertTo-SecureString $password -AsPlainText -Force))

    $parameters = @{
        ImportFileGuid = $guid;
    }

    Invoke-SqlCmd2 `
        -Query "exec Pack_GeneratePackProperty @ImportFileGuid" `
        -ServerInstance $server `
        -Database $database `
        -Credential $credentials `
        -QueryTimeout 14400 `
        -SqlParameters $parameters
}

function Truncate{
    Param(
        $server,
        $database,
        $username,
        $password,
        $table
    )
    $credentials = New-Object Management.Automation.PSCredential ($username, (ConvertTo-SecureString $password -AsPlainText -Force))

    Invoke-SqlCmd2 `
        -Query "exec Truncate$($table)ImportTables" `
        -ServerInstance $server `
        -Database $database `
        -Credential $credentials `
        -QueryTimeout 1200
}

function PostProcessSupport {
    param (
        $server,
        $database,
        $username,
        $password
    )
    $credentials = New-Object Management.Automation.PSCredential ($username, (ConvertTo-SecureString $password -AsPlainText -Force))

    Invoke-SqlCmd2 `
        -Query "exec Pack_BuildAllSupportTables" `
        -ServerInstance $server `
        -Database $database `
        -Credential $credentials `
        -QueryTimeout 1200
}

function Report-Database {
    param (
        $server,
        $database,
        $username,
        $password,
        $files,
        $logger,
        $ftpLocalFolder,
        $bcpFolder
    )
    process {
        $guid = [Guid]::NewGuid()
        $credentials = New-Object Management.Automation.PSCredential ($username, (ConvertTo-SecureString $password -AsPlainText -Force))

        $query = "EXEC dbo.CreateImport 
            @Guid, @FtpStarted, @FtpEnded, @FtpTime, 
            @PropertiesStarted, @PropertiesEnded, @PropertiesTime, @PropertiesAdded, @PropertiesUpdated, @PropertiesRemoved,
            @FlightsStarted, @FlightsEnded, @FlightsTime, @FlightsAdded, @FlightsUpdated, @FlightsRemoved,
            @Started, @Ended, @TotalTime,
            @FtpError, @PropertiesError, @FlightsError, @ProcessError"

        $ftp = $logger.FindSpan("FTP")
        $flights = $logger.FindSpan("PROCESS-FLIGHTS")
        $properties = $logger.FindSpan("PROCESS-PROPERTIES")
        $support = $logger.FindSpan("PROCESS-SUPPORT")

        $ftpStarted = $null
        $ftpEnded = $null;
        $ftpTime = $null;
        $ftpError = $null;

        if ($ftp) {
            $ftpStarted = $ftp.Start;
            $ftpEnded = $ftp.End;
            $ftpTime = $ftp.Delta;
            $ftpError = $ftp.CapturedError;
        }

        $propertiesStarted = $null
        $propertiesEnded = $null;
        $propertiesTime = $null;
        $propertiesError = $null;

        if ($properties) {
            $propertiesStarted = $properties.Start;
            $propertiesEnded = $properties.End;
            $propertiesTime = $properties.Delta;
            $propertiesError = $properties.CapturedError;
        }

        $flightsStarted = $null
        $flightsEnded = $null;
        $flightsTime = $null;
        $flightsError = $null;

        if ($flights) {
            $flightsStarted = $flights.Start;
            $flightsEnded = $flights.End;
            $flightsTime = $flights.Delta;
            $flights.CapturedError;
        }

        if ($support) {
            $supportError = $support.CapturedError
        }

        $parameters = @{
            Guid = $guid;
            FtpStarted = $ftpStarted;
            FtpEnded = $ftpEnded;
            FtpTime = $ftpTime;
            PropertiesStarted = $propertiesStarted;
            PropertiesEnded = $propertiesEnded;
            PropertiesTime = $propertiesTime;
            PropertiesAdded = 0;
            PropertiesUpdated = 0;
            PropertiesRemoved = 0;
            FlightsStarted = $flightsStarted;
            FlightsEnded = $flightsEnded;
            FlightsTime = $flightsTime;
            FlightsAdded = 0;
            FlightsUpdated = 0;
            FlightsRemoved = 0;
            Started = $logger.Start;
            Ended = $logger.End;
            TotalTime = $logger.Delta;
            FtpError = $ftpError;
            PropertiesError = $propertiesError;
            FlightsError = $flightsError;
            ProcessError = $supportError;
        }

        Invoke-SqlCmd2 `
            -Query $query `
            -ServerInstance $server `
            -Database $database `
            -Credential $credentials `
            -SqlParameters $parameters

        foreach ($file in $files) {
            $package = Join-Path $FtpLocalFolder $file.Package
            $unzipped = Join-Path $BcpFolder $file.FileName
    
            $bcp = $logger.FindSpan("BCP-$($file.Label)")
            $unzip = $logger.FindSpan("ZIP-$($file.Label)")
            $type = ?: $file.ProcessProperties "P" "F"
            $capturedError = $null;
    
            $hasErrors = ($bcp.CapturedError -or $unzip.CapturedError);
            if ($hasErrors) {
                $capturedError = ?? $unzip.CapturedError $bcp.CapturedError
            }
    
            $unzipStarted = $null
            $unzipEnded = $null;
            $unzipTime = $null;
    
            if ($unzip) {
                $unzipStarted = $unzip.Start;
                $unzipEnded = $unzip.End;
                $unzipTime = $unzip.Delta;
            }
    
            $bcpStarted = $null
            $bcpEnded = $null;
            $bcpTime = $null;

            if ($bcp) {
                $bcpStarted = $bcp.Start;
                $bcpEnded = $bcp.End;
                $bcpTime = $bcp.Delta;
            }

            $sourceFileSize = (Get-Item $package).Length
            $unzipFileSize = (Get-Item $unzipped).Length

            $query = "EXEC dbo.CreateImportFile
                @Guid, @FileName, @Label, @Type,
                @ZippedFileSize, @UnzipStarted, @UnzipEnded, @UnzipTime, @UnzippedFileSize,
                @BcpStarted, @BcpEnded, @BcpTime,
                @Error"

            $parameters = @{
                Guid = $guid;
                FileName = $file.FileName;
                Label = $file.Label;
                Type = $type;
                ZippedFileSize = $sourceFileSize;
                UnzipStarted = $unzipStarted;
                UnzipEnded = $unzipEnded;
                UnzipTime = $unzipTime;
                UnzippedFileSize = $unzipFileSize;
                BcpStarted = $bcpStarted;
                BcpEnded = $bcpEnded;
                BcpTime = $bcpTime;
                Error = $capturedError;
            }

            Invoke-SqlCmd2 `
                -Query $query `
                -ServerInstance $server `
                -Database $database `
                -Credential $credentials `
                -SqlParameters $parameters
        }
    }
}

function Log-ImportFile {
    param (
        $server,
        $database,
        $username,
        $password,
        $fileName,
        $label,
        $type,
        $lastModified
    )
    $credentials = New-Object Management.Automation.PSCredential ($username, (ConvertTo-SecureString $password -AsPlainText -Force))

    $query = "EXEC dbo.LogImportFile @FileName, @Label, @Type, @LastModified"
    $parameters = @{
        FileName = $fileName;
        Label = $label;
        Type = $type;
        LastModified = $lastModified;
    }

    $result = Invoke-SqlCmd2 `
        -Query $query `
        -ServerInstance $server `
        -Database $database `
        -Credential $credentials `
        -SqlParameters $parameters `
        -As PSObject

    return $result;
}

function Log-Import {
    param (
        [string] $server,
        [string] $database,
        [string] $username,
        [string] $password,
        [Guid] $guid,
        [Nullable[DateTime]] $started = $null,
        [Nullable[DateTime]] $ended = $null,
        [Nullable[TimeSpan]] $time = $null
    )
    $credentials = New-Object Management.Automation.PSCredential ($username, (ConvertTo-SecureString $password -AsPlainText -Force))

    $query = "EXEC dbo.LogImport @Guid, @Started, @Ended, @TotalTime"
    $parameters = @{
        Guid = $guid;
        Started = $started;
        Ended = $ended;
        TotalTime = $time;
    }

    Invoke-SqlCmd2 `
        -Query $query `
        -ServerInstance $server `
        -Database $database `
        -Credential $credentials `
        -SqlParameters $parameters
}

function Log-ImportFileHistory {
    param (
        [string] $server,
        [string] $database,
        [string] $username,
        [string] $password,
        [Guid] $importGuid,
        [Guid] $importFileGuid,
        [string] $filename,
        [Nullable[long]] $zippedFileSize = $null,
        [Nullable[DateTime]] $unzipStarted = $null,
        [Nullable[DateTime]] $unzipEnded = $null,
        [Nullable[TimeSpan]] $unzipTime = $null,
        [Nullable[long]] $unzippedFileSize = $null,
        [string] $unzipError = $null,
        [Nullable[DateTime]] $bcpStarted = $null,
        [Nullable[DateTime]] $bcpEnded = $null,
        [Nullable[TimeSpan]] $bcpTime = $null,
        [string] $bcpError = $null,
        [Nullable[DateTime]] $importStarted = $null,
        [Nullable[DateTime]] $importEnded = $null,
        [Nullable[TimeSpan]] $importTime = $null,
        [string] $importError = $null
    )
    $credentials = New-Object Management.Automation.PSCredential ($username, (ConvertTo-SecureString $password -AsPlainText -Force))

    $query = "EXEC dbo.LogImportFileHistory @ImportGuid, @ImportFileGuid, @FileName,
                   @ZippedFileSize, @UnzipStarted, @UnzipEnded, @UnzipTime, @UnzippedFileSize, @UnzipError,
                   @BcpStarted, @BcpEnded, @BcpTime, @BcpError,
                   @ImportStarted, @ImportEnded, @ImportTime, @ImportError"

    $parameters = @{
        ImportGuid = $ImportGuid;
        ImportFileGuid = $ImportFileGuid;
        FileName = $FileName;
        ZippedFileSize = $zippedFileSize;
        UnzipStarted = $unzipStarted;
        UnzipEnded = $unzipEnded;
        UnzipTime = $unzipTime;
        UnzippedFileSize = $unzippedFileSize;
        UnzipError = $unzipError;
        BcpStarted = $bcpStarted;
        BcpEnded = $bcpEnded;
        BcpTime = $bcpTime;
        BcpError = $bcpError;
        ImportStarted = $importStarted;
        ImportEnded = $importEnded;
        ImportTime = $importTime;
        ImportError = $importError;
    }

    Invoke-SqlCmd2 `
        -Query $query `
        -ServerInstance $server `
        -Database $database `
        -Credential $credentials `
        -SqlParameters $parameters
}

function Get-ImportFileHistoryStats {
    param (
        [string] $server,
        [string] $database,
        [string] $username,
        [string] $password,
        [Guid] $guid
    )
    $credentials = New-Object Management.Automation.PSCredential ($username, (ConvertTo-SecureString $password -AsPlainText -Force))

    $query = "SELECT TOP 1 [TotalImportRows], [RecordsAdded], [RecordsUpdated], [RecordsDeleted], [MissingPaxCodes], [MissingTPProperties], [MissingMealBases], [MissingCurrencies] FROM dbo.ImportFileHistory WHERE [Guid] = @Guid"
    $parameters = @{
        Guid = $guid;
    }

    $result = Invoke-SqlCmd2 `
        -Query $query `
        -ServerInstance $server `
        -Database $database `
        -Credential $credentials `
        -SqlParameters $parameters `
        -As PSObject

    return $result;
}

function Get-LastFileImportStatistics {
    param (
        [string] $server,
        [string] $database,
        [string] $username,
        [string] $password,
        [string] $fileName,
        [string] $type
    )
    $credentials = New-Object Management.Automation.PSCredential ($username, (ConvertTo-SecureString $password -AsPlainText -Force))

    $query = "EXEC dbo.GetImportStatisticsFor$($type)File @FileName"
    $parameters = @{
        FileName = $fileName;
    }

    $result = Invoke-SqlCmd2 `
        -Query $query `
        -ServerInstance $server `
        -Database $database `
        -Credential $credentials `
        -SqlParameters $parameters `
        -QueryTimeout 600 `
        -As PSObject

    return $result;
}