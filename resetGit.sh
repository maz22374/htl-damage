#!/bin/bash

echo "Warning: All local changes will be reset. Press CTRL+C to cancel."
read -p "Press [Enter] to continue"

current_branch=$(git rev-parse --abbrev-ref HEAD)

git fetch --all --prune
for branch in $(git branch -a | grep -v HEAD)
do
    echo "Resetting branch $branch..."
    git checkout $branch
    git clean -df
    git reset --hard origin/$branch
done

git checkout $current_branch
echo "You are now on branch $current_branch"
