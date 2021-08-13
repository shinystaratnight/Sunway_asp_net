function RunBCP{
    Param(
        $logger,
        $db,
        $tablename,
        $dbusername,
        $dbpassword,
        $serverip,
        $formatfile,
        $bcp,
        $inFile
    )

    $workingPath = Split-Path $inFile -Parent
    $workingFile = Split-Path $inFile -Leaf
    $errorFile = Join-Path $workingPath "bcp-error-$($workingFile).log"
    $outputFile = Join-Path $workingPath "bcp-output-$($workingFile).log"

    Write-Information $bcp

    $arglist = @(
        "$db.$tablename",
        "in $inFile",
        "-U $dbusername",
        "-P $dbpassword",
        "-S $serverip",
        "-f $formatfile",
        "-h TABLOCK",
        "-e $errorFile",
        "-m 100"
    )

    $process = Start-Process `
        -FilePath $bcp `
        -ArgumentList $arglist `
        -RedirectStandardOutput $outputFile `
        -WindowStyle Hidden `
        -PassThru

    Wait-Process -InputObject $process > $null;

    $errorMessage = '';

    if (Test-Path $outputFile) {
        $outputFileContent = [IO.File]::ReadAllText($outputFile)

        Remove-Item $outputFile > $null;

        if ($outputFileContent) {
            $outputFileContent = $outputFileContent.Trim()
            if ($outputFileContent) {
                $outputFileLines = $outputFileContent.Split([Environment]::NewLine)

                $outputFileLines | Where-Object { $_.StartsWith("Error") } | % {
                    $errorMessage += "$($_.Substring(8))`n"
                }

                $errorMessage = $errorMessage.Trim()
            }
        }
    }

    if (Test-Path $errorFile) {
        $errorFileContent = [IO.File]::ReadAllText($errorFile)

        if ($errorFileContent) {
            $errorFileContent = $errorFileContent.Trim()
            if ($errorFileContent) {
                if ($errorMessage) {
                    $errorMessage += "`n`n"
                }
                $errorMessage += $errorFileContent
            }
        }
    }

    if ($errorMessage) {
        Set-Content -Path $errorFile -Value $errorMessage
        $logger.Attach($errorFile, "plaintext", $true) > $null;

        throw "The BCP operation failed, see attachment for specific BCP error(s)"
    }
}