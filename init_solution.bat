:: WebServer
set "targetDirectory=.\src\OneDas.WebServer\wwwroot\lib"
if exist "%targetDirectory%" rd "%targetDirectory%" /q /s

set "sourceDirectory=.\..\OneDAS-Support\web\node_modules"

set list=@aspnet\signalr\dist\browser
set list=%list%;bootstrap\dist
set list=%list%;chart.js\dist
set list=%list%;font-awesome\css
set list=%list%;font-awesome\fonts
set list=%list%;jquery\dist
set list=%list%;knockout\build
set list=%list%;mathjs\dist
set list=%list%;moment\min
set list=%list%;popper.js\dist

for %%a in (%list%) do ( 
     xcopy /s /y "%sourceDirectory%\%%a\." "%targetDirectory%\%%a\"
)

set list=moment\moment.js
set list=%list%;pagerjs\pager.js

for %%a in (%list%) do ( 
     xcopy /s /y "%sourceDirectory%\%%a" "%targetDirectory%\%%a*"
)

:: HDF.Explorer
set "targetDirectory=.\src\OneDas.Hdf.Explorer\wwwroot\lib"
if exist "%targetDirectory%" rd "%targetDirectory%" /q /s

set "sourceDirectory=.\..\OneDAS-Support\web\node_modules"

set list=@aspnet\signalr\dist\browser
set list=%list%;bootstrap\dist   
set list=%list%;chart.js\dist
set list=%list%;font-awesome\css
set list=%list%;font-awesome\fonts
set list=%list%;jquery\dist
set list=%list%;knockout\build
set list=%list%;moment\min
set list=%list%;popper.js\dist
set list=%list%;tempusdominus-bootstrap-4\build

for %%a in (%list%) do ( 
     xcopy /s /y %sourceDirectory%\%%a\. %targetDirectory%\%%a\
)

set list=%list%;moment\moment.js

for %%a in (%list%) do ( 
     xcopy /s /y "%sourceDirectory%\%%a" "%targetDirectory%\%%a*"
)