class RemoteFile {
    RemoteFile ([string] $fileName) {
        $this.FileName = $fileName;
        $this.LastModified = $null;
        $this.Downloaded = $false;
    }

    [string] $FileName;
    [Nullable[DateTime]] $LastModified;
    [bool] $Downloaded;

    [void] SetDetails([DateTime] $lastModified, [bool] $downloaded) {
        $this.LastModified = $lastModified;
        $this.Downloaded = $downloaded;
    }
}

function Run-Ftp ($ftp, $user, $pass, $folder, $target, $logger) {
    $span = $logger.StartSpan("FTP")

    Write-Host "$(Get-Date) Connecting to ftp://$($ftp)$($folder)"

    try {
        #$logdate = Get-Date -Format "MM_dd_yyyy_HH_mm_ss"
        #"Running FTP at $logdate" | Add-Content $errorlog

        #SET CREDENTIALS
        $credentials = new-object System.Net.NetworkCredential($user, $pass)

        #SET FILE CHECKER
        $fileschanged = $false

        #SET FOLDER PATH
        $folderPath= "ftp://$ftp/$folder/"

        $remoteFiles = Get-FTPDir -url $folderPath -credentials $credentials

        $webclient = New-Object System.Net.WebClient 
        $webclient.Credentials = New-Object System.Net.NetworkCredential($user,$pass) 
        $counter = 0
        foreach ($file in ($remoteFiles | where { $_.FileName -like "*.csv.gz" })) {
            $source = $folderPath + $file.FileName  
            $destination = Join-Path $target $file.FileName
            Write-Host "$(Get-Date) Found remote file $source"

            $lastModified = Get-FtpLastModified -url $source -credentials $credentials
            $download = (!(Test-Path $destination) -or ($file.LastModified -gt [DateTime]::Now.AddDays(-1)));
            $file.SetDetails($lastModified, $download) > $null;

            if ($download) {
                Write-Host "$(Get-Date) Downloading remote file $source to $download"
                $webclient.DownloadFile($source, $destination) > $null;
                $fileschanged = $true
            } else {
                Write-Host "$(Get-Date) Skipping remote file $source as there is a newer/same file at $destination"
            }

            #PRINT FILE NAME AND COUNTER
            $counter++
        }

        Write-Verbose "<FTP>"
        $remoteFiles | % {
            Write-Verbose "`t<REMOTE-FILE>"
            Write-Verbose "`t`tFileName: $($_.FileName)"
            Write-Verbose "`t`tLast Modified: $($_.LastModified)"
            Write-Verbose "`t`tDownloaded: $($_.Downloaded)"
            Write-Verbose "`t</REMOTE-FILE>"
        }
        Write-Verbose "</FTP>"

        $span.EndSpan()

        return $fileschanged
    } catch {
        $span.FailSpan($_.Exception)

        return $false
    }
}

function Get-FtpDir ($url, $credentials) {
    $remoteFiles = New-Object System.Collections.ArrayList
    $request = [Net.WebRequest]::Create($url)
    $request.Method = [System.Net.WebRequestMethods+FTP]::ListDirectory
    if ($credentials) { $request.Credentials = $credentials }
    $response = $request.GetResponse()
    if ($response) {
        Using-Object ($reader = New-Object IO.StreamReader $response.GetResponseStream()) {
            if ($reader) {
                while(-not $reader.EndOfStream) {
                    $fileName = $reader.ReadLine()
                    $remoteFile = [RemoteFile]::new($fileName);
                    $remoteFiles.Add($remoteFile) > $null;
                }
                $reader.Close()
                $response.Close()
            }
        }
    }
    return $remoteFiles
}

function Get-FtpLastModified ($url, $credentials) {
    $timerequest = [Net.FtpWebRequest]::Create($url)
    $timerequest.Credentials = $credentials
    $timerequest.Method = [Net.WebRequestMethods+Ftp]::GetDateTimestamp
    $timerequest.UsePassive = $true
    $timeresponse = $timerequest.GetResponse()
    $lastmodified = [DateTime]$timeresponse.LastModified
    $timeresponse.Close()
    return $lastmodified
}

# Set the default cache policy for FTP requests
[Net.FtpWebRequest]::DefaultCachePolicy = New-Object System.Net.Cache.RequestCachePolicy([Net.Cache.RequestCacheLevel]::NoCacheNoStore)