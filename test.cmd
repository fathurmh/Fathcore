@echo off
powershell -ExecutionPolicy ByPass -NoProfile -command "& """%~dp0test.ps1""" %*"
exit /b %ErrorLevel%
