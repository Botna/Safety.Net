$directory = Read-Host -Prompt "Please enter the directory of the mp4 chunks"

$mp4Chunks = [System.Collections.Generic.List[string]]::new()

Write-Host "Scanning:" $directory

$files = Get-ChildItem -Path $directory 


for($i = 0; $i -lt $files.Count -1; $i++){
    $validEntry = $files | Where-Object { $_ -like "*_$i.mp4" }
    if([string]::IsNullOrEmpty($validEntry))
    {
        Write-Host "Incorrectly formatted folderstructure.  Had extraneous files outside of chunks and header" -ForegroundColor Red
        exit 1
    }
    Write-Host "found a file with name: $validEntry"
    $mp4Chunks += $validEntry
}

$validHeader = $files | Where-Object { $_ -like "*_headerPart.mp4" }
    if([string]::IsNullOrEmpty($validHeader))
    {
        Write-Host "Incorrectly formatted folderstructure. Couldn't find _headerPart file" -ForegroundColor Red
        exit 1
    }
 Write-Host "Found header part: $validHeader"
 $outputFilename = $mp4Chunks[0] -creplace "_0", ""
 Write-Host "Consolidating files into file with name $outputFilename"

foreach($chunk in $mp4Chunks){
    $bytesToAppend = [System.IO.File]::ReadAllBytes("$chunk");

    [System.IO.File]::AppendAllBytes( $outputFilename, $bytesToAppend)
}

Write-Host "Built new file succesfully"

Write-Host "Overlaying header bytes onto file"

$bytesToAppend =[System.IO.File]::ReadAllBytes($validHeader)

$stream = [System.IO.File]::Open($outputFilename, [System.IO.FileMode]::Open, [System.IO.FileAccess]::ReadWrite)
$stream.Write($bytesToAppend, 0, $bytesToAppend.Length)
$stream.Close()