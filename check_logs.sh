#!/bin/bash

RG=$(az group list \
  --query "[].name" \
  --output tsv | grep -i "daniel" | head -n 1)

az container logs --name scantrack-karlstad-daniel --resource-group $RG --follow

