@echo off
powershell -ExecutionPolicy ByPass -NoProfile -command "& """%~dp0test.ps1""" %*"
reportgenerator "-reports:./artifacts/TestResults/coverage.opencover.xml" "-targetdir:./artifacts/TestResults/report/"
start ./artifacts/TestResults/report/index.htm
exit /b %ErrorLevel%
