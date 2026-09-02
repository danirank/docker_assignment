#!/bin/bash
set -e

docker build \
  -f ScanTrackNode/Dockerfile \
  -t scantrack-node:latest \
  .

docker image ls scantrack-node