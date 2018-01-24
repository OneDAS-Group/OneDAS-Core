# OneDAS-Core
Contains the main executables and type definitions of OneDAS.

The documentation is hosted on [onedas.readthedocs.io](https://onedas.readthedocs.io) but incomplete and written for the old version of OneDAS. It will be updated continuously in the following weeks and months.

### Prerequisites

* Visual Studio 2017 Community (which workloads?)
* up-to-date browser, old browsers do not work
* Git for Windows (https://git-scm.com/download/win)
* node.js (https://nodejs.org/en/)
* **optional**: WiX Toolset (to open .wixproj and to build .msi packages)
  * WiX Toolset build tools v3.11 (http://wixtoolset.org/releases/)
  * WiX Toolset Visual Studio 2017 Extension (http://wixtoolset.org/releases/)"
* **optional**: CMake (for EtherCAT plugin)
  * CMake (https://cmake.org/download/)

### Initialization

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
init_solution.bat

:: Ethercat
cd %rootFolder%\%projectName%-Ethercat\support
init_solution.bat
```
