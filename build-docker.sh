#!/bin/bash
set -e

docker build \
  --no-cache -t scantrack-node \
  -f ScanTrackNode/Dockerfile \
  -t scantrack-node:latest \
  .

docker image ls scantrack-node