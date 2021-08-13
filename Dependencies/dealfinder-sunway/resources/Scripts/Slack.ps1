Import-Module PSSlack

function Report-SlackFile ($logger, $FtpLocalFolder, $BcpFolder, $file, $guid, $stats, $toleranceFail) {
    $anyErrors = $false;
    $attachments = New-Object System.Collections.ArrayList

    $package = Join-Path $FtpLocalFolder $file.Package
    $unzipped = Join-Path $BcpFolder $file.FileName

    $bcp = ?? $logger.FindSpan("BCP-$($file.Label)") @{}
    $unzip = ?? $logger.FindSpan("ZIP-$($file.Label)") @{}

    $flights = $logger.FindSpan("PROCESS-FLIGHTS-$($file.Label)")
    $properties = $logger.FindSpan("PROCESS-PROPERTIES-$($file.Label)")
    $support = $logger.FindSpan("PROCESS-SUPPORT-$($file.Label)")

    $type = ?: $file.ProcessProperties "Properties" "Flights"

    $hasErrors = ($bcp.HasError -or $unzip.HasError -or $toleranceFail);
    if ($hasErrors) {
        $anyErrors = $true
    }

    $colour = ?: $hasErrors $_PSSlackColorMap.red $_PSSlackColorMap.yellowgreen
    $sourceFileSize = DisplayInBytes (Get-Item $package).Length
    $unzipFileSize = DisplayInBytes (Get-Item $unzipped).Length

    $data = @(
        @{ title = "Import Type"; value = $type; short = $true },
        @{ title = "Zipped File Size"; value = $sourceFileSize; short = $true }
        @{ title = "Unzip Time"; value = ("{0:hh\:mm\:ss\.fff}" -f (?? $unzip.Delta $([TimeSpan]::Zero))); short = $true },
        @{ title = "Unzipped File Size"; value = $unzipFileSize; short = $true }
        @{ title = "Unzip Error"; value = (?? $unzip.CapturedError "None"); short = $false },
        @{ title = "BCP Time"; value = ("{0:hh\:mm\:ss\.fff}" -f (?? $bcp.Delta $([TimeSpan]::Zero))); short = $true },
        @{ title = "BCP Error"; value = (?? $bcp.CapturedError "None"); short = $false }
    )

    if ($flights) {
        $spanError = ?: $flights.CapturedError $true $false
        if ($spanError) {
            $anyErrors = $true
            $colour = $_PSSlackColorMap.red
        }

        $spanData = @(
            @{ title = "Import Started"; value = $flights.Start.ToString("dd/MM/yyyy HH:mm"); short = $true },
            @{ title = "Import Ended"; value = $flights.End.ToString("dd/MM/yyyy HH:mm"); short = $true },
            @{ title = "Import Time"; value = ("{0:hh\:mm\:ss\.fff}" -f $flights.Delta); short = $true },
            @{ title = "Import Error"; value = (?? $flights.CapturedError "None"); short = $false },
            @{ title = "Total Rows"; value = $stats.TotalImportRows.ToString("N0"); short = $true },
            @{ title = "Records Added"; value = $stats.RecordsAdded.ToString("N0"); short = $true },
            @{ title = "Records Updated"; value = $stats.RecordsUpdated.ToString("N0"); short = $true },
            @{ title = "Records Removed"; value = $stats.RecordsDeleted.ToString("N0"); short = $true }
        )

        $data = [array]$data + $spanData
    }

    if ($properties) {
        $spanError = ?: $properties.CapturedError $true $false
        if ($spanError) {
            $anyErrors = $true
            $colour = $_PSSlackColorMap.red
        }

        $spanData = @(
            @{ title = "Import Started"; value = $properties.Start.ToString("dd/MM/yyyy HH:mm"); short = $true },
            @{ title = "Import Ended"; value = $properties.End.ToString("dd/MM/yyyy HH:mm"); short = $true },
            @{ title = "Import Time"; value = ("{0:hh\:mm\:ss\.fff}" -f $properties.Delta); short = $true },
            @{ title = "Import Error"; value = (?? $properties.CapturedError "None"); short = $false },
            @{ title = "Total Rows"; value = $stats.TotalImportRows.ToString("N0"); short = $true },
            @{ title = "Records Added"; value = $stats.RecordsAdded.ToString("N0"); short = $true },
            @{ title = "Records Updated"; value = $stats.RecordsUpdated.ToString("N0"); short = $true },
            @{ title = "Records Removed"; value = $stats.RecordsDeleted.ToString("N0"); short = $true },
            @{ title = "Missing PAX codes"; value = $stats.MissingPaxCodes.ToString("N0"); short = $true },
            @{ title = "Missing properties"; value = $stats.MissingTPProperties.ToString("N0"); short = $true },
            @{ title = "Missing meal bases"; value = $stats.MissingMealBases.ToString("N0"); short = $true },
            @{ title = "Missing currencies"; value = $stats.MissingCurrencies.ToString("N0"); short = $true }
        )

        $data = [array]$data + $spanData
    }

    $attachment = New-SlackMessageAttachment `
        -Color $colour `
        -Title "File Result: $($file.FileName)" `
        -Fields $data `
        -Fallback "Slack client does not support attachment data"

    $attachments.Add($attachment) > $null;

    $messageText = ?: $anyErrors "@channel - The last import failed for file $($file.FileName)" "The last import succeeded for file $($file.FileName)"

    $message = $attachments | New-SlackMessage `
        -Channel $SlackChannel `
        -AsUser -Username "$Client Deal Finder" `
        -Parse full `
        -Text $messageText

    $message | Send-SlackMessage -Token $SlackToken > $null; 
	
	foreach ($loggerAttachment in $logger.Attachments) {
		Slack-SendLoggerAttachment -LoggerAttachment $loggerAttachment > $null;
	}

    return $anyErrors;
}

function Slack-SendLoggerAttachment ($loggerAttachment) {
	$content = [IO.File]::ReadAllText($loggerAttachment.FilePath)
	
    Send-SlackFile `
        -Token $SlackToken `
        -Channel $SlackChannel `
        -Content $content `
        -FileType $loggerAttachment.ContentType `
        -FileName $loggerAttachment.FileName > $null;
}

function Slack-NotifyImportStart ($resource, $forced, $lastModified) {
    $attachments = New-Object System.Collections.ArrayList

    $type = ?: $resource.ProcessProperties "Properties" "Flights"
    $data = @(
        @{ title = "Package"; value = $resource.Package; short = $true; },
        @{ title = "Type"; value = $type; short = $true; },
        @{ title = "Label"; value = $resource.Label; short = $true; },
        @{ title = "Delivered"; value = $lastModified.ToString("dd/MM/yyyy HH:mm"); short = $true; },
        @{ title = "Run type"; value = (?: $forced "Forced" "Normal"); short = $false }
    )

    $attachment = New-SlackMessageAttachment `
        -Color (?: $forced $_PSSlackColorMap.yellow $_PSSlackColorMap.gray) `
        -Title "Import Process Started" `
        -Fields $data `
        -Fallback "Slack client does not support attachment data"
    
    $attachments.Add($attachment) > $null;

    $message = $attachments | New-SlackMessage `
        -Channel $SlackChannel `
        -AsUser -Username "$Client Deal Finder" `
        -Parse full `
        -Text "Import process started"

    $message | Send-SlackMessage -Token $SlackToken > $null; 
}

function Slack-NotifyImportToleranceFailed ($resource, $stats, $tolerance, $result, $override) {
    $attachments = New-Object System.Collections.ArrayList

    $result = [Math]::Round($result, 3)

    $type = ?: $resource.ProcessProperties "Properties" "Flights"
    $data = @(
        @{ title = "Package"; value = $resource.Package; short = $true; },
        @{ title = "Type"; value = $type; short = $true; },
        @{ title = "Previous rows"; value = $stats.LastRows.ToString("N0"); short = $true; },
        @{ title = "Latest rows"; value = $stats.NewRows.ToString("N0"); short = $true; },
        @{ title = "Tolerance"; value = "$tolerance%"; short = $true; },
        @{ title = "Result"; value = "$result%"; short = $true; }
    )

    $title = ?: $override "Import file failed tolerance check, but will be imported" "Import file failed tolerance check"
    $color = ?: $override $_PSSlackColorMap.yellow $_PSSlackColorMap.red
    $attachment = New-SlackMessageAttachment `
        -Color $color `
        -Title $title `
        -Fields $data `
        -Fallback "Slack client does not support attachment data"
    
    $attachments.Add($attachment) > $null;

    $message = $attachments | New-SlackMessage `
        -Channel $SlackChannel `
        -AsUser -Username "$Client Deal Finder" `
        -Parse full `
        -Text "Import process tolerance check"

    $message | Send-SlackMessage -Token $SlackToken > $null; 
}