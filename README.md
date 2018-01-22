# OneDAS-Core
Contains the main executables and type definitions of OneDAS.

The documentation is hosted on [onedas.readthedocs.io](https://onedas.readthedocs.io) but incomplete and written for the old version of OneDAS. It will be updated continuously in the following weeks and months.

On Windows, run the following batch script to get started with OneDAS:

```bat
:: constants
set "projectName=OneDAS"
set "origin=https://github.com/OneDAS-Group/"

:: create parent folder
md %projectName%
cd .\%projectName%
set "rootFolder=%cd%"

:: clone projects
git clone "%origin%/%projectName%-Core"
git clone "%origin%/%projectName%-Documentation"
git clone "%origin%/%projectName%-Ethercat"
git clone "%origin%/%projectName%-Gamesa"
git clone "%origin%/%projectName%-Plugins"
git clone "%origin%/%projectName%-Support"

:: Support
cd %rootFolder%\%projectName%-Support\web
install_packages.bat

:: Core
cd %rootFolder%\%projectName%-Core\support
CopyNodeModules_OneDas.Hdf.Explorer.bat
CopyNodeModules_OneDas.WebServer.bat

:: Ethercat
cd %rootFolder%\%projectName%-Ethercat
git -C "%projectName%-Ethercat" submodule update --init --recursive
cd .\support
CMakeSOEM.bat
```