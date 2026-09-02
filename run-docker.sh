docker run -p 8080:8080 \
    -e CITY_NAME=Karlstad \
    -e NODE_URL=http://localhost:8080 \
    -e REGISTRY_URL=http://scantrack-registry-iths.northeurope.azurecontainer.io:8080 \
    scantrack