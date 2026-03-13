# Arbeidsrapport – Del 1: Grunnleggende integrasjon

**Dato:** 2026-03-13
**Utvikler:** Marcus Børresen
**Teknologi:** Node.js (JavaScript)
**Status:** Fullført

---

## Mål for denne delen

Bygge en forenklet, men realistisk integrasjon som viser hele flyten:

> Shopify-ordre → Validering → Mapping → SOAP XML → POS-system

Ingen ekte API-er brukes i denne delen. Alt er simulert slik at fokus er på **tankegangen**, ikke teknologien.

---

## Hva ble bygget

### Filer opprettet

| Fil | Formål |
|-----|--------|
| `src/data/shopifyOrder.js` | Simulert Shopify-ordre (testdata) |
| `src/validators/orderValidator.js` | Validerer at ordren er komplett |
| `src/mappers/shopifyToPos.js` | Oversetter fra Shopify-format til POS-format |
| `src/utils/xmlBuilder.js` | Bygger SOAP-lignende XML |
| `src/services/posService.js` | Simulerer sending til PC-kassen |
| `main.js` | Orkestrerer hele flyten |
| `docs/forklaringer/hva-er-mapping.md` | Forklaring av nøkkelbegreper |

---

## Hva ble lært

### 1. Integrasjonsflyt
En integrasjon er ikke én stor funksjon – den er en **kjede av steg**:
```
Hent data → Valider → Map → Send
```
Hvert steg har sitt eget ansvarsområde.

### 2. Mapping
Mapping handler om å **endre struktur, ikke informasjon**. Shopify og PC-kassen inneholder den samme informasjonen, men uttrykker den forskjellig.

Eksempel fra oppgaven:
- Shopify: `customer.first_name + customer.last_name` (to felt)
- POS: `kundenavn` (ett samlet felt)

### 3. Validering
Data fra eksterne systemer **kan ikke stoles på**. Validering tidlig i flyten hindrer at feil data smyger seg gjennom og forårsaker problemer i POS-systemet.

### 4. SOAP og XML
SOAP er bare et **strengt, XML-basert format** for meldinger. Det er ikke magi – det er en konvensjon for hvordan felter skal hete og struktureres.

### 5. Ansvarsfordeling (separasjon av bekymringer)
- `validators/` vet bare om data er gyldig
- `mappers/` vet bare om formatomvandling
- `services/` vet bare om å sende data videre
- `main.js` koordinerer dem alle

Dette er et kjent prinsipp i backend-utvikling.

---

## Utfordringer og refleksjoner

- Det var litt uvant å tenke på data som "to forskjellige dialekter" – men det er nøyaktig hva det er.
- `module.exports` / `require()` er Node.js sitt system for å dele kode mellom filer (tilsvarer `using`/`namespace` i .NET).
- Optional chaining (`?.`) er nyttig for å håndtere manglende felt uten å krasje.

---

## Neste steg – Del 2

Når Del 2 starter, bør vi:

- [ ] Legge til støtte for rabatter på produktlinjer
- [ ] Håndtere flere fraktmetoder med ulike priser
- [ ] Legge til en "logg-tjeneste" som skriver hendelser til fil
- [ ] Gjøre koden klar for overflytting til .NET (C#)

---

## Kjøre prosjektet

```bash
# Fra prosjektmappen:
node main.js
```

**Forventet output:**
1. Valideringsmelding (OK eller feil)
2. Lesbar utskrift av POS-ordre
3. SOAP XML som ville blitt sendt til PC-kassen
