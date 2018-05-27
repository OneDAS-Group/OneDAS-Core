dotnet build ./src/OneDas.Core/OneDas.Core.csproj                         -c $Env:Configuration /p:Build=$Env:APPVEYOR_BUILD_NUMBER /p:IsFinalBuild=$Env:APPVEYOR_REPO_TAG
dotnet build ./src/OneDas.Hdf.Types/OneDas.Hdf.Types.csproj               -c $Env:Configuration /p:Build=$Env:APPVEYOR_BUILD_NUMBER /p:IsFinalBuild=$Env:APPVEYOR_REPO_TAG
dotnet build ./samples/DataGatewaySample/DataGatewaySample.csproj         -c $Env:Configuration /p:Build=$Env:APPVEYOR_BUILD_NUMBER /p:IsFinalBuild=$Env:APPVEYOR_REPO_TAG

dotnet pack  ./src/OneDas.Core.WebServer/OneDas.Core.WebServer.csproj     -c $Env:Configuration /p:Build=$Env:APPVEYOR_BUILD_NUMBER /p:IsFinalBuild=$Env:APPVEYOR_REPO_TAG
dotnet pack  ./src/OneDas.Hdf.Explorer/OneDas.Hdf.Explorer.csproj         -c $Env:Configuration /p:Build=$Env:APPVEYOR_BUILD_NUMBER /p:IsFinalBuild=$Env:APPVEYOR_REPO_TAG

if ($isWindows)
{
  dotnet publish .\src\OneDas.Core.WebServer\OneDas.Core.WebServer.csproj -c $Env:Configuration /p:Build=$Env:APPVEYOR_BUILD_NUMBER /p:IsFinalBuild=$Env:APPVEYOR_REPO_TAG /p:RuntimeIdentifier=win7-x86 /p:BuildProjectReferences=false
  dotnet publish .\src\OneDas.Hdf.Explorer\OneDas.Hdf.Explorer.csproj     -c $Env:Configuration /p:Build=$Env:APPVEYOR_BUILD_NUMBER /p:IsFinalBuild=$Env:APPVEYOR_REPO_TAG /p:RuntimeIdentifier=win7-x64 /p:BuildProjectReferences=false

  msbuild ./src/OneDas.Core.Deployment/OneDas.Core.Deployment.wixproj     /p:Configuraton=$Env:Configuration /p:Build=$Env:APPVEYOR_BUILD_NUMBER /p:IsFinalBuild=$Env:APPVEYOR_REPO_TAG
  msbuild ./src/OneDas.Hdf.Deployment/OneDas.Hdf.Deployment.wixproj       /p:Configuraton=$Env:Configuration /p:Build=$Env:APPVEYOR_BUILD_NUMBER /p:IsFinalBuild=$Env:APPVEYOR_REPO_TAG
}