#!/bin/bash

BRANCH=$(git rev-parse --abbrev-ref HEAD)
BRANCH=${BRANCH:-"master"}
echo "Running services for branch $BRANCH"

docker run -p 8000:80 --mount type=bind,source=$(pwd)/Auth/users.db,target=/users.db docker.pkg.github.com/real-net/web-editor-backend/auth:$BRANCH 
