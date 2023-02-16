#!/bin/bash

# script for user input of commit message
read -p "Enter commit message: " commit_msg

# Check if commit message is empty
if [[ -z "$commit_msg" ]]; then
    echo "Commit message cannot be empty."
    exit 1
fi

git add -A
git commit -a -m "$commit_msg"
git pull
if [ $? -ne 0 ]; then
    echo "An error has occurred."
    exit 1
fi
git push
if [ $? -ne 0 ]; then
    echo "An error has occurred."
    exit 1
fi
