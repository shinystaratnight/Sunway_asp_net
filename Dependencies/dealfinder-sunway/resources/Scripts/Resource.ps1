class Template {
    Template ([string] $pattern, [string] $label, [switch] $processFlights, [switch] $processProperties) {
        $this.Pattern = $pattern;
        $this.Label = $label;
        $this.ProcessFlights = $processFlights;
        $this.ProcessProperties = $processProperties;
    }

    [string] $Pattern;
    [string] $Label;
    [switch] $ProcessFlights;
    [switch] $ProcessProperties;
}

class ProcessResult {
    ProcessResult ([bool] $available, [string] $state) {
        $this.Available = $available;
        $this.State = $state;
    }

    [bool] $Available;
    [string] $State;
}

class Resource {
    Resource ([string] $package, [string] $fileName, [string] $basePath, [string] $label, [bool] $processFlights, [bool] $processProperties) {
        $this.Package = $package;
        $this.FullPackagePath = (Join-Path $basePath $package);
        $this.FileName = $fileName;
        $this.Label = $label;
        $this.ProcessFlights = $processFlights;
        $this.ProcessProperties = $processProperties;
    }

    [string] $Package;
    [string] $FileName;
    [string] $FullPackagePath;
    [string] $Label;
    [bool] $ProcessFlights;
    [bool] $ProcessProperties;
    
    [ProcessResult] CanProcess() {
        try {
            $fileStream = [IO.File]::Open($this.FullPackagePath, 'Open', 'Read')
            $fileStream.Close()
            $fileStream.Dispose()

            return [ProcessResult]::new($true, 'Available');
        } catch [System.UnauthorizedAccessException] {
            return [ProcessResult]::new($false, 'Unauthorised');
        } catch {
            return [ProcessResult]::new($false, 'Locked');
        }
    }
}

function New-Template ([string] $pattern, [string] $label, [switch] $processFlights, [switch] $processProperties) {
    return [Template]::new($pattern, $label, $processFlights, $processProperties);
}

function Get-Resources ([Template[]] $templates, [string] $path, [bool] $noFlights, [bool] $noProperties) {
    $resources = New-Object Collections.ArrayList;
    foreach ($template in $templates) {
        foreach ($file in (Get-ChildItem -Path $path -File)) {
            if ($file.Name -match $template.Pattern) {
                if (($noFlights -and $template.ProcessFlights) -or ($noProperties -and $template.ProcessProperties)) {
                    # We don't want flights or properties based on the incoming parameters
                    continue;
                }
                if ($file.Name -match $template.Label) {
                    $label = $matches['label'];
                } else {
                    $label = $template.Label;
                }

                $resource = New-Resource `
                    -package $file.Name `
                    -fileName $file.BaseName `
                    -basePath $path `
                    -label $label.ToUpper() `
                    -processFlights $template.ProcessFlights `
                    -processProperties $template.ProcessProperties;

                $resources.Add($resource) > $null;
            }
        }
    }
    return $resources;
}

function New-Resource ([string] $package, [string] $fileName, [string] $basePath, [string] $label, [bool] $processFlights, [bool] $processProperties) {
    return [Resource]::new($package, $fileName, $basePath, $label, $processFlights, $processProperties);
}