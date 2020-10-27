$logger.LogInformation('Welcome to the OneDAS HDF5 file importer.')
$ErrorActionPreference = "Stop"

# general settings (example)
$sourceDir      = '/OneDAS' 
$projectName   = 'GROUP1_GROUP2_CAMPAIGNNAME'
$searchPattern  = "$($projectName)_V*_c8157e56"
$dataWriterId   = 'HDF_DW1'
$days           = 7

# FTP settings (example)
$hostName       = '192.168.0.100'
$port           = 21
$userName       = 'username'
$password       = 'password'

# start
$targetDir      = Join-Path -Path $dbRoot -ChildPath 'DB_NATIVE'
$dateTimeEnd    = (Get-Date).ToUniversalTime().Date.AddDays(-1)
$dateTimeBegin  = $dateTimeEnd.AddDays(-$days)
$ftpClient      = New-Object -TypeName FluentFTP.FtpClient `
                             -ArgumentList $hostName, $port, $username, $password

$ftpClient.Connect()
$logger.LogInformation("Connected to ftp://$($userName)@$($hostName):$($port).")

# get all items at sourceDir: (1) take only directories (2) that match the specified pattern, then (3) select only the FullName property
$searchOption   = [FluentFTP.FtpListOption]::Auto -bor [FluentFTP.FtpListOption]::Recursive

$directories    = $ftpClient.GetListing($sourceDir, $searchOption) | `
                   Where-Object `
                   { 
                        $_.Type -eq [FluentFTP.FtpFileSystemObjectType]::Directory `
                        -And [Regex]::IsMatch($_.FullName, $searchPattern.Replace('*', '.*')) 
                   } | `
                   Select-Object -ExpandProperty FullName

# go through all matched dirs 
foreach ($directory in $directories) 
{
    $version    = [Regex]::Match($directory, 'V[0-9]+(?!.*V[0-9]+)').Value
    $currentDir = (Join-Path -Path $directory -ChildPath $dataWriterId).Replace('\', '/')

    for ($i = 0; $i -lt $days; $i++) 
    {
        $currentBegin   = $dateTimeBegin.AddDays($i)
        $fileName       = "$($projectName)_$($version)_$($currentBegin.ToString('yyyy-MM-ddTHH-mm-ssZ')).h5"

        $sourceFile     = (Join-Path -Path $currentDir    -ChildPath $fileName).Replace('\', '/')
        $currTargetDir  =  Join-Path -Path $targetDir     -ChildPath $currentBegin.ToString("yyyy-MM")
        $targetFile     = (Join-Path -Path $currTargetDir -ChildPath $fileName).Replace('\', '/')

        if ($ftpClient.FileExists($sourceFile) -And (-Not (Test-Path $targetFile)))
        {
            $message = "Copying file '$fileName' ... "
            $logger.LogInformation($message)

            New-Item -Path $currTargetDir -ItemType Directory -Force

            try 
            {
                $ftpClient.DownloadFile($targetFile, $sourceFile, [FluentFTP.FtpVerify]::Retry)
                $logger.LogInformation("$($message)Done.")
            }
            catch 
            {
                if (Test-Path $targetFile)
                {
	                Remove-Item $targetFile
                }

                $logger.LogInformation("$($message)Failed.")
            }
        }
    }
}
