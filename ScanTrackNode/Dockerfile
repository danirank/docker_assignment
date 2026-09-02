# Din Dockerfile — fyll i de saknade raderna
# Använd multi-stage build precis som på lektionen

# Stage 1: Bygg
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Done: Kopiera projektfilen och kör restore
 COPY ScanTrackNode/ScanTrackNode.csproj ./
 RUN dotnet restore

# Done: Kopiera resten av koden och publicera
# Done: Kopiera CSV-filen (den behövs i runtime-imagen också)
COPY ScanTrackNode/. .
COPY data/cities.csv /data/cities.csv

RUN dotnet publish -c Release -o /app/publish

# Stage 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

# Done: Kopiera det publicerade projektet från build-steget
COPY --from=build /app/publish .

# Porten appen lyssnar på inuti containern
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

# Done: Starta appen
ENTRYPOINT ["dotnet", "ScanTrackNode.dll"]
