
@setlocal

if [%1] == [] (
@echo.
@echo Use this CMD to REGISTER DBJIPOINT.DLL as COM+ component
@echo.
@echo Just drag assembly dll and drop it on this cmd
@echo.
@echo For this CMD to work .NET 1.1 must be on the installed
@echo.
@echo 23AUG04 DBJ
@echo.
@echo $Revision: 3 $
@echo $JustDate: 26/08/04 $
@echo.
@echo.
goto FINAL_EXIT
)
:WORK
@SET FrameworkDir=%WINDIR%\Microsoft.NET\Framework
@REM @SET FrameworkVersion=v1.1.4322
@rem SET FrameworkVersion=v2.0.50727
@rem v4.0.30319
@SET "FrameworkVersion=v2.0.50727"
@set "REGISTRATOR=%FrameworkDir%\%FrameworkVersion%\regsvcs.exe"

%REGISTRATOR% %1

:FINAL_EXIT
@pause
@endlocal