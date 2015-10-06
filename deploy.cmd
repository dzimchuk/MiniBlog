@if "%SCM_TRACE_LEVEL%" NEQ "4" @echo off

:: ----------------------
:: KUDU Deployment Script
:: Version: 0.2.2
:: ----------------------

:: Prerequisites
:: -------------

:: Verify node.js installed
where node 2>nul >nul
IF %ERRORLEVEL% NEQ 0 (
  echo Missing node.js executable, please install node.js, if already installed make sure it can be reached from current environment.
  goto error
)

:: Setup
:: -----

setlocal enabledelayedexpansion

SET ARTIFACTS=%~dp0%..\artifacts

IF NOT DEFINED DEPLOYMENT_SOURCE (
  SET DEPLOYMENT_SOURCE=%~dp0%.
)

IF NOT DEFINED DEPLOYMENT_TARGET (
  SET DEPLOYMENT_TARGET=%ARTIFACTS%\wwwroot
)

IF NOT DEFINED NEXT_MANIFEST_PATH (
  SET NEXT_MANIFEST_PATH=%ARTIFACTS%\manifest

  IF NOT DEFINED PREVIOUS_MANIFEST_PATH (
    SET PREVIOUS_MANIFEST_PATH=%ARTIFACTS%\manifest
  )
)

IF NOT DEFINED KUDU_SYNC_CMD (
  :: Install kudu sync
  echo Installing Kudu Sync
  call npm install kudusync -g --silent
  IF !ERRORLEVEL! NEQ 0 goto error

  :: Locally just running "kuduSync" would also work
  SET KUDU_SYNC_CMD=%appdata%\npm\kuduSync.cmd
)
IF NOT DEFINED DEPLOYMENT_TEMP (
  SET DEPLOYMENT_TEMP=%temp%\___deployTemp%random%
  SET CLEAN_LOCAL_DEPLOYMENT_TEMP=true
)

IF DEFINED CLEAN_LOCAL_DEPLOYMENT_TEMP (
  IF EXIST "%DEPLOYMENT_TEMP%" rd /s /q "%DEPLOYMENT_TEMP%"
  mkdir "%DEPLOYMENT_TEMP%"
)

IF DEFINED MSBUILD_PATH goto MsbuildPathDefined
SET MSBUILD_PATH=%ProgramFiles(x86)%\MSBuild\14.0\Bin\MSBuild.exe
:MsbuildPathDefined

::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
:: Deployment
:: ----------

echo Handling .NET Web Site deployment.

:: 1. Restore NuGet packages
IF /I "MiniBlog.sln" NEQ "" (
  call :ExecuteCmd nuget restore "%DEPLOYMENT_SOURCE%\MiniBlog.sln"
  IF !ERRORLEVEL! NEQ 0 goto error
  
  call :ExecuteCmd nuget restore "%DEPLOYMENT_SOURCE%\Website\packages.config" -SolutionDirectory "%DEPLOYMENT_SOURCE%"
  IF !ERRORLEVEL! NEQ 0 goto error
)

:: 2. Build and package
call :ExecuteCmd "%MSBUILD_PATH%" "%DEPLOYMENT_SOURCE%\MiniBlog.sln" /p:Configuration=Release /verbosity:m /nologo %SCM_BUILD_ARGS%
IF !ERRORLEVEL! NEQ 0 goto error

call :BuildWebJob MiniBlog.Backup MiniBlog.Backup.csproj
IF !ERRORLEVEL! NEQ 0 goto error

call :BuildWebJob MiniBlog.BlogIndexer MiniBlog.BlogIndexer.csproj
IF !ERRORLEVEL! NEQ 0 goto error

call :BuildWebJob MiniBlog.ImageOptimizer MiniBlog.ImageOptimizer.csproj
IF !ERRORLEVEL! NEQ 0 goto error
rd /s /q "%DEPLOYMENT_TEMP%\Resources"

call :BuildWebJob MiniBlog.PostIndexer MiniBlog.PostIndexer.csproj
IF !ERRORLEVEL! NEQ 0 goto error

call :BuildWebJob MiniBlog.PostSync MiniBlog.PostSync.csproj
IF !ERRORLEVEL! NEQ 0 goto error

robocopy "%DEPLOYMENT_SOURCE%\PrecompiledWeb\localhost_36123" "%DEPLOYMENT_TEMP%" /E
IF !ERRORLEVEL! GTR 3 goto error

:: 2. KuduSync
IF /I "%IN_PLACE_DEPLOYMENT%" NEQ "1" (
  call :ExecuteCmd "%KUDU_SYNC_CMD%" -v 50 -f "%DEPLOYMENT_TEMP%" -t "%DEPLOYMENT_TARGET%" -n "%NEXT_MANIFEST_PATH%" -p "%PREVIOUS_MANIFEST_PATH%" -i ".git;.hg;.deployment;deploy.cmd;app_data\posts"
  IF !ERRORLEVEL! NEQ 0 goto error
)

::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::

:: Post deployment stub
IF DEFINED POST_DEPLOYMENT_ACTION call "%POST_DEPLOYMENT_ACTION%"
IF !ERRORLEVEL! NEQ 0 goto error

goto end

:BuildWebJob
set _folder_=%1
set _project_=%2
call :ExecuteCmd "%MSBUILD_PATH%" "%DEPLOYMENT_SOURCE%\%_folder_%\%_project_%" /nologo /verbosity:m /t:Build /t:pipelinePreDeployCopyAllFilesToOneFolder /p:_PackageTempDir="%DEPLOYMENT_TEMP%\%_folder_%";Configuration=Release /p:SolutionDir="%DEPLOYMENT_SOURCE%\.\\"
IF %ERRORLEVEL% NEQ 0 exit /b %ERRORLEVEL%
robocopy "%DEPLOYMENT_TEMP%\%_folder_%" "%DEPLOYMENT_TEMP%" /E /XD bin
IF %ERRORLEVEL% LEQ 3 (
	rd /s /q "%DEPLOYMENT_TEMP%\%_folder_%"
	exit /b 0
)
exit /b %ERRORLEVEL%

:: Execute command routine that will echo out when error
:ExecuteCmd
setlocal
set _CMD_=%*
call %_CMD_%
if "%ERRORLEVEL%" NEQ "0" echo Failed exitCode=%ERRORLEVEL%, command=%_CMD_%
exit /b %ERRORLEVEL%

:error
endlocal
echo An error has occurred during web site deployment.
call :exitSetErrorLevel
call :exitFromFunction 2>nul

:exitSetErrorLevel
exit /b 1

:exitFromFunction
()

:end
endlocal
echo Finished successfully.
