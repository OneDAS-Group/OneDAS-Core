set "targetDirectory=.\..\OneDas.Hdf.Explorer\wwwroot\lib"
if exist "%targetDirectory%" rd "%targetDirectory%" /q /s

set "sourceDirectory=.\..\..\OneDAS-Support\web\node_modules"

set list="@aspnet\signalr-client\dist\browser"
set list=%list%;bootstrap\dist   
set list=%list%;chart.js\dist
set list=%list%;eonasdan-bootstrap-datetimepicker\build
set list=%list%;font-awesome\css
set list=%list%;font-awesome\fonts
set list=%list%;jquery\dist
set list=%list%;knockout\build
set list=%list%;moment\min
set list=%list%;popper.js\dist

for %%a in (%list%) do ( 
     xcopy /s /y "%sourceDirectory%\%%a\." "%targetDirectory%\%%a\"
)

set list=bootstrap-datetimepicker.min.js
set list=%list%;moment\moment.js

for %%a in (%list%) do ( 
     xcopy /s /y "%sourceDirectory%\%%a" "%targetDirectory%\%%a*"
)

pause