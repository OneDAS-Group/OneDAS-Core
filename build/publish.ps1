Get-ChildItem -Include *.nupkg -Recurse | ForEach-Object `
{
	dotnet nuget push $_.fullname --source "https://www.myget.org/F/onedas/api/v3/index.json"
	Remove-Item $_.fullname
}