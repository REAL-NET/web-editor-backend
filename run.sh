#!/bin/bash

BRANCH=$(git rev-parse --abbrev-ref HEAD)
BRANCH=${BRANCH:-"master"}
echo "Running services for branch $BRANCH"

docker run -p 8000:80 docker.pkg.github.com/real-net/web-editor-backend/gateway:$BRANCH &
docker run -p 8002:80 --mount type=bind,source=$(pwd)/Auth/users.db,target=/users.db docker.pkg.github.com/real-net/web-editor-backend/auth:$BRANCH &
docker run -p 8004:80 --mount type=bind,source=$(pwd)/Repo/serialized/,target=/serialized/ docker.pkg.github.com/real-net/web-editor-backend/repo:$BRANCH &
docker run -p 8006:80 --mount type=bind,source=$(pwd)/Storage/StorageDB.db,target=/StorageDB.db docker.pkg.github.com/real-net/web-editor-backend/storage:$BRANCH &
docker run -p 8008:80 docker.pkg.github.com/real-net/web-editor-backend/generator:$BRANCH &
docker run -p 8099:80 docker.pkg.github.com/real-net/web-editor-backend/test:$BRANCH
