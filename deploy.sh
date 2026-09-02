#!/bin/bash
set -e

LOCATION="northeurope"
CITY="karlstad"

NODE_NAME="scantrack-${CITY}-daniel"
DNS_LABEL="scantracknode-daniel"

RG=$(az group list \
  --query "[].name" \
  --output tsv | grep -i "daniel" | head -n 1)

REGISTRY_URL="http://scantrack-registry-iths.northeurope.azurecontainer.io:8080"

# Hämta resursgruppens id och gör ett kort unikt suffix för ACR-namnet
RG_ID=$(az group show \
  --name "$RG" \
  --query id \
  --output tsv | md5sum | cut -c1-8)

ACR_NAME="acrscantrack${RG_ID}"
IMAGE="${ACR_NAME}.azurecr.io/scantrack-node:v1"
NODE_URL="http://${DNS_LABEL}.${LOCATION}.azurecontainer.io:8080"

# Skapa Container Registry
az acr create \
  --name "$ACR_NAME" \
  --resource-group "$RG" \
  --sku Basic \
  --location "$LOCATION" \
  --admin-enabled true

# Logga in, tagga och pusha imagen
az acr login --name "$ACR_NAME"

#Bygg image
./build-docker.sh

docker tag scantrack-node:latest "$IMAGE"
docker push "$IMAGE"


# Hämta lösenord till ACR
ACR_PASSWORD=$(az acr credential show \
  --name "$ACR_NAME" \
  --query "passwords[0].value" \
  --output tsv)

# Skapa och starta noden
az container create \
  --name "$NODE_NAME" \
  --resource-group "$RG" \
  --location "$LOCATION" \
  --image "$IMAGE" \
  --os-type Linux \
  --cpu 1 \
  --memory 1.5 \
  --ports 8080 \
  --ip-address Public \
  --dns-name-label "$DNS_LABEL" \
  --registry-login-server "${ACR_NAME}.azurecr.io" \
  --registry-username "$ACR_NAME" \
  --registry-password "$ACR_PASSWORD" \
  --environment-variables \
    CITY_NAME="Karlstad" \
    NODE_URL="$NODE_URL" \
    REGISTRY_URL="$REGISTRY_URL"