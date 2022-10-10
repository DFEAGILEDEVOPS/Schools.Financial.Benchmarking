call npm run-script build:newAndLegacy
@echo off
echo Exit Code is %errorlevel%
@echo on
if not "%errorlevel%"=="0"  EXIT /B %errorlevel%