class Span {
    Span ([string] $name) {
        $this.Name = $name;
        $this.HasError = $false;
    }

    [string] $Name;
    [Nullable[DateTime]] $Start;
    [Nullable[DateTime]] $End;
    [Nullable[TimeSpan]] $Delta;
    [string] $CapturedError;
    [bool] $HasError;

    [void] StartSpan() {
        $this.Start = Get-Date;
    }

    [void] EndSpan() {
        $this.End = Get-Date;
        $this.Delta = ($this.End - $this.Start);
    }

    [void] FailSpan([Exception] $capturedError) {
        $exceptionType = $capturedError.GetType().Name;
        $exceptionMessage = $capturedError.Message;

        $this.EndSpan()
        $this.CapturedError = "$($exceptionType): $exceptionMessage"
        $this.HasError = $true
    }
}

class Attachment {
    Attachment ([string] $filePath, [string] $contentType, [bool] $clean) {
        $this.FilePath = $filePath;
        $this.FileName = Split-Path $filePath -Leaf;
        $this.ContentType = $contentType;
        $this.Clean = $clean;
    }

    [string] $FilePath;
    [string] $FileName;
    [string] $ContentType;
    [bool] $Clean;
}

class Logger {
    Logger([string] $fileName) {
        $this.FileName = $fileName;
        $this.Spans = New-Object Collections.ArrayList;
        $this.Attachments = New-Object Collections.ArrayList;
    }

    [string] $FileName;

    [Nullable[DateTime]] $Start;
    [Nullable[DateTime]] $End;
    [Nullable[TimeSpan]] $Delta;
    [Collections.ArrayList] $Spans;
    [Collections.ArrayList] $Attachments;

    [void] LogStart() {
        $this.Start = Get-Date;
    }

    [void] LogEnd() {
        $this.End = Get-Date;
        $this.Delta = ($this.End - $this.Start);
    }

    [Span] StartSpan($name) {
        $span = [Span]::new($name);
        $this.Spans.Add($span);
        $span.StartSpan();
        return $span;
    }

    [Span] FindSpan($name) {
        foreach ($span in $this.Spans) {
            if ($span.Name -eq $name) {
                return $span;
            }
        }
        
        return $null;
    }
    
    [Attachment] Attach($filePath, $contentType, $clean) {
        $attachment = [Attachment]::new($filePath, $contentType, $clean);
        $this.Attachments.Add($attachment);
        return $attachment;
    }

    [void] Clean() {
        foreach ($attachment in $this.Attachments) {
            if ($attachment.Clean) {
                Remove-Item $attachment.FilePath;
            }
        }
    }
}

function New-Logger ([string] $fileName) {
    return [Logger]::new($fileName);
}