set "sourceDir=%~1."
set "localTargetDir=%localappdata%\OneDAS\Core\plugin\%pluginName%"

robocopy "%sourceDir%" "%localTargetDir%" /MIR /xf OneDas.Types.dll /R:0

if errorlevel 1 exit 0 else exit %errorlevel%