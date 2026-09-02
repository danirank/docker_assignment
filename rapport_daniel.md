# Rapport — ScanTrack Node

**Namn:** Daniel,
**Stad (nod):**  Karlstad,
**Datum:**  2026-09-01,
**Kurs:**  Administrera molnlösningar — ITHS

---

## Del 1 — Tillvägagångssätt

Först satte jag variabler för region, stad, namn på noden och DNS-namn:

```bash
LOCATION="northeurope"
CITY="karlstad"

NODE_NAME="scantrack-${CITY}-daniel"
DNS_LABEL="scantracknode-daniel"
```

Därefter hämtade skriptet automatiskt en resursgrupp vars namn innehöll `daniel`.

Efter det hämtades resursgruppens ID och användes för att skapa ett unikt namn för mitt Azure Container Registry, ACR.

Sedan skapades ACR-resursen med Basic-nivån och admin-inloggning aktiverad.

När registret var skapat loggade jag in mot ACR, byggde Docker-imagen med mitt `build-docker.sh`-skript, taggade imagen med adressen till ACR och pushade den dit.

Till sist hämtades lösenordet till ACR och en Azure Container Instance skapades. Containern körde imagen från ACR, exponerade port `8080` och fick en publik IP-adress samt ett DNS-namn.

### Hur du byggde och testade Docker-imagen lokalt

Docker-imagen byggdes med:

```bash
./build-docker.sh
```

Som ihåller följande kommando: 

```bash
#!/bin/bash
set -e

docker build \
  -f ScanTrackNode/Dockerfile \
  -t scantrack-node:latest \
  .

docker image ls scantrack-node
```

Efter bygget körde jag 

```bash
./run-docker.sh
```
Som innehåller 

```bash
#!/bin/bash
docker run -p 8080:8080 \
    -e CITY_NAME=Karlstad \
    -e NODE_URL=http://localhost:8080 \
    -e REGISTRY_URL=http://scantrack-registry-iths.northeurope.azurecontainer.io:8080 \
    scantrack
```

Jag verifierade att Imagen körde genom /status -endpoint.
Svaret från endpointen visade att noden var igång och kunde ta emot HTTP-anrop.

![status-respone-local](/images/status_local.png)

### Hur du publicerade imagen till ACR

Jag använde Azure Container Registry.

Registret skapades med:

```bash
az acr create \
  --name "$ACR_NAME" \
  --resource-group "$RG" \
  --sku Basic \
  --location "$LOCATION" \
  --admin-enabled true
```

Sedan loggade jag in:

```bash
az acr login --name "$ACR_NAME"
```

Efter att Docker-imagen hade byggts taggades den:

```bash
docker tag scantrack-node:latest "$IMAGE"
```

och pushades till ACR:

```bash
docker push "$IMAGE"
```

Imagen fick namnet:

```text
${ACR_NAME}.azurecr.io/scantrack-node:v1
```

### Hur du startade noden i ACI

Noden startades med `az container create`.

Containern fick:

* Linux som operativsystem
* 1 CPU
* 1.5 GB minne
* port `8080`
* publik IP-adress
* ett publikt DNS-namn

Eftersom Docker-imagen låg i ACR skickades även inloggningsuppgifter till registret med.

Tre miljövariabler sattes:

```bash
CITY_NAME="$CITY"
NODE_URL="$NODE_URL"
REGISTRY_URL="$REGISTRY_URL"
```

`CITY_NAME` sattes till `karlstad`.

`NODE_URL` sattes till nodens publika adress:

```text
http://scantracknode-daniel.northeurope.azurecontainer.io:8080
```

`REGISTRY_URL` sattes till:

```text
http://scantrack-registry-iths.northeurope.azurecontainer.io:8080
```

Utifrån namnen på variablerna går det att se att noden får information om vilken stad den representerar, sin egen URL och adressen till registret.

### Hur du verifierade att noden fungerade

> Vad testade du? Fick du kontakt med de andra noderna?

Test av /status 

![status-respone-local](/images/status.png)




---

## Del 2 — Reflektion

### Vad har du lärt dig?

*Nämn tre konkreta saker du inte kunde innan den här veckan.*

1. Skapa ACR och ACI 
2. 
3.

---

### Vad var svårast?

*Vad tog längst tid eller krävde mest felsökning? Vad löste problemet?*

---

### Hur kan du ha nytta av det du lärt dig i framtiden?

*Tänk på en situation i ett riktigt jobb — hur och när skulle du använda det här?*

---

### Vad är skillnaden mellan att köra en app i en container och att köra den direkt på en server?

*Förklara med egna ord — som om du förklarar för en kollega som aldrig hört talas om Docker.*

---

### Varför skickar varje nod med historiken i paketet?

*Vad hade hänt om vi inte gjort det? Ge ett konkret exempel.*

---

## Del 3 — Gruppreflektion

### Hur fungerade samarbetet i gruppen?

*Vem gjorde vad? Hjälpte ni varandra eller jobbade ni parallellt?*

---

### Var det något som blockerade gruppen? Hur löste ni det?

---

### Vad skulle ni göra annorlunda om ni fick göra om det?

---

## Del 4 — Teknisk logg

*Klistra in tre kommandon du körde som du tycker var viktiga — och förklara vad varje rad gör.*

**Kommando 1:**
```bash

```
*Vad gör det:*

**Kommando 2:**
```bash

```
*Vad gör det:*

**Kommando 3:**
```bash

```
*Vad gör det:*

---

*Rapporten lämnas in på Classroom senast [DATUM]. Bifoga en skärmbild som visar att ett paket passerade din nod.*
