@echo off
chcp 65001
echo Warning: All local changes will be reset. Press CTRL+C to cancel.
pause

FOR /F "tokens=*" %%a IN ('git branch --show-current') DO (SET current_branch=%%a)

git fetch --all --prune
FOR /F "tokens=* delims=* " %%a IN ('git branch') DO (
    echo Resetting branch %%a...
    git checkout %%a
    git clean -df
    git reset --hard origin/%%a
)
git checkout %current_branch%
echo You are now on branch %current_branch%
