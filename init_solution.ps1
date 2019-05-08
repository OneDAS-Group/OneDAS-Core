$baseDirectory = (Get-Location).Path

Set-Location -Path $baseDirectory\web
npm install
Set-Location -Path $baseDirectory

$sourceDirectory = $baseDirectory + "\web\node_modules"

# Core
$targetDirectory = $baseDirectory + "\src\OneDas.Core.WebClient\wwwroot\lib"
Remove-Item $targetDirectory -Recurse -ErrorAction Ignore

$directorySet = @(
    "bootstrap\dist",
    "font-awesome\css",
    "font-awesome\fonts",
    "jquery\dist"
)

foreach ($directory in $directorySet)
{
    $path = Join-Path -Path $sourceDirectory -ChildPath $directory
    $destination = Join-Path -Path $targetDirectory -ChildPath $directory

    Copy-Item -Path $path -Destination $destination -Recurse -Force
}

$filePathSet = @(
    "chart.js\dist\Chart.js",
    "chart.js\dist\Chart.min.js"
)

foreach ($filePath in $filePathSet)
{
    $path = Join-Path -Path $sourceDirectory -ChildPath $filePath
    $destination = Join-Path -Path $targetDirectory -ChildPath $filePath

    New-Item -Force $destination
    Copy-Item -Path $path -Destination $destination
}

# HDF.Explorer
$targetDirectory = $baseDirectory + "\src\OneDas.Hdf.Explorer\wwwroot\lib"
Remove-Item $targetDirectory -Recurse -ErrorAction Ignore

$directorySet = @(
    "@aspnet\signalr\dist\browser",
    "bootstrap\dist",
    "font-awesome\css",
    "font-awesome\fonts",
    "jquery\dist",
    "knockout\build",
    "moment\min"
    "tempusdominus-bootstrap-4\build"
)

foreach ($directory in $directorySet)
{
    $path = Join-Path -Path $sourceDirectory -ChildPath $directory
    $destination = Join-Path -Path $targetDirectory -ChildPath $directory

    Copy-Item -Path $path -Destination $destination -Recurse -Force
}

$filePathSet = @(
    "chart.js\dist\Chart.js",
    "chart.js\dist\Chart.min.js",
    "moment\moment.js"
)

foreach ($filePath in $filePathSet)
{
    $path = Join-Path -Path $sourceDirectory -ChildPath $filePath
    $destination = Join-Path -Path $targetDirectory -ChildPath $filePath

    New-Item -Force $destination
    Copy-Item -Path $path -Destination $destination
}