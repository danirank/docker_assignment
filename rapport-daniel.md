# Rapport — ScanTrack AB: Containerisera och deploya din nod

**Namn:** Daniel,

**Stad (nod):** Karlstad,

**Datum:** 2026-09-03,

**Kurs:** Administrera molnlösningar — ITHS

---

## 1. Arkitektur

Noden byggs först lokalt som en Docker-image utifrån projektets `Dockerfile`. Imagen testas lokalt och publiceras sedan till Azure Container Registry (ACR). Därefter skapas en Azure Container Instance (ACI) som hämtar imagen från ACR och kör noden i molnet. Noden exponeras på port `8080` och får ett publikt DNS-namn som används som `NODE_URL`.

```text
Dockerfile
    ↓
docker build
    ↓
Lokal Docker-image
    ↓
Test lokalt
    ↓
docker push
    ↓
Azure Container Registry (ACR)
    ↓
Azure Container Instance (ACI)
    ↓
Publikt DNS-namn / NODE_URL
```

Containern får även miljövariablerna `CITY_NAME`, `NODE_URL` och `REGISTRY_URL` när den startas.

---

## 2. NODE_URL-problemet

NODE_URL sattes till nodens publika DNS-adress innan containern skapades:

```bash
NODE_URL="http://${NODE_NAME}.${LOCATION}.azurecontainer.io:8080"
```

DNS-namnet bestämdes alltså i förväg genom att använda:

```bash
NODE_NAME="scantrack-${CITY}-daniel"
```

När ACI skapades användes samma DNS-label:

```bash
--dns-name-label "$NODE_NAME"
```

På så sätt kunde `NODE_URL` byggas i förväg utan att vara beroende av den publika IP-adress som ACI tilldelades.

---

## 3. Bevis

Test av `GET /status` på noden:

![status-respone](/images/status.png)

Livelogg vid skickande och mottagande av paket

![livelog](/images/livelog.png)

---

## Individuell reflektion

- **Vad var svårast?**
  Det svåraste var att få alla delar i lösningen att fungera tillsammans. Att bygga själva containern var relativt tydligt, men när noden skulle kommunicera med registret och andra noder blev det fler saker att hålla reda på. Det kunde till exempel vara svårt att veta om ett fel berodde på koden, Docker-imagen, miljövariablerna eller själva deploymenten i Azure. Felsökningen blev därför en viktig del av arbetet.

- **Vad förstår du nu som du inte förstod innan?**
  Jag förstår nu bättre hur Docker-images och containers fungerar, hur miljövariabler skickas in till en container och hur en container publiceras via ACR och körs i ACI. Jag har också fått en tydligare förståelse för hela flödet från lokal kod till en körande tjänst i molnet. Tidigare såg jag Docker, ACR och ACI mer som separata delar, men nu förstår jag bättre hur de hänger ihop i samma deploymentflöde.

- **Vad skulle du ha gjort annorlunda?**
  Jag hade testat varje del mer separat innan jag kopplade ihop hela lösningen. Det hade gjort det enklare att avgöra var ett fel faktiskt låg och minskat tiden som gick åt till felsökning. Jag hade även använt en user-assigned Managed Identity för att låta ACI hämta imagen från ACR, i stället för att skicka med användarnamn och lösenord till registret i skriptet. Det hade varit en säkrare lösning eftersom man då slipper hantera dessa autentiseringsuppgifter direkt i deploymentflödet.
- **Hur planerades projektet?**
  Jag ville först få en övergripande bild av hur hela lösningen skulle fungera och hur de olika delarna hängde ihop. Därefter arbetade jag stegvis och började med att få noden att fungera lokalt innan jag flyttade ut den i molnet. Arbetet delades upp i mindre delar: bygga och testa Docker-imagen, publicera den till ACR, starta noden i ACI, registrera noden mot registret och till sist testa kommunikationen. Det gjorde det lättare att fokusera på en del i taget och successivt bygga upp hela lösningen.
