@echo off
REM script for user input of commit message
set /p commit_msg="Enter commit message: "

REM Check if commit message is empty
IF "%commit_msg%"=="" (
echo Commit message cannot be empty.
pause
exit /b
)

git add -A
git commit -a -m "%commit_msg%"
git pull
IF ERRORLEVEL 1 GOTO error
git push
IF ERRORLEVEL 1 GOTO error
GOTO end
:error
echo An error has occurred.
pause
:end