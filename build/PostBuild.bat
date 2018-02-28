set "sourceDir=%~1."
set "localTargetDir=%localappdata%\OneDAS\plugin\%pluginName%"
set "systemTargetDir=C:\Windows\SysWOW64\config\systemprofile\AppData\Local\OneDAS\plugin\%pluginName%"

robocopy "%sourceDir%" "%localTargetDir%" /MIR /xf OneDas.Types.dll /R:0
robocopy "%sourceDir%" "%systemTargetDir%" /MIR /xf OneDas.Types.dll /R:0

if errorlevel 1 exit 0 else exit %errorlevel%