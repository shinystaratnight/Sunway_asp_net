function DeGZip-File{
    Param(
        $infile,
        $outfile = ($infile -replace '\.gz$','')
    )

    $bufferSize = 1024

    Using-Object ($inputFileStream = (New-Object System.IO.FileStream $inFile, ([IO.FileMode]::Open), ([IO.FileAccess]::Read), ([IO.FileShare]::Read))) {
        Using-Object ($outputFileStream = (New-Object System.IO.FileStream $outFile, ([IO.FileMode]::Create), ([IO.FileAccess]::Write), ([IO.FileShare]::None))) {
            Using-Object ($gzipStream = (New-Object System.IO.Compression.GzipStream $inputFileStream, ([IO.Compression.CompressionMode]::Decompress))) {
                $buffer = New-Object byte[]($bufferSize)
                while ($true) {
                    $read = $gzipStream.Read($buffer, 0, $bufferSize)
                    if ($read -le 0) {
                        break
                    }
                    $outputFileStream.Write($buffer, 0, $read)
                }
            }
        }
    }

    
    
    

    <#$buffer = New-Object byte[](1024)
    while($true){
        $read = $gzipstream.Read($buffer, 0, 1024)
        if ($read -le 0){break}
        $output.Write($buffer, 0, $read)
        }

    $gzipStream.Close()
    $output.Close()
    $input.Close()
    #>
}