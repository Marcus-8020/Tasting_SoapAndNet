# Arbeidsrapport – Del 2: Rabatter, frakt og logging

**Dato:** 2026-03-13
**Utvikler:** Marcus Børresen
**Teknologi:** Node.js (JavaScript)
**Status:** Fullført

---

## Mål for denne delen

Bygge videre på del 1 med tre nye funksjoner som gjør integrasjonen mer realistisk:

1. **Rabattstøtte** – Håndtere rabattkoder fra Shopify og trekke dem fra totalen
2. **Flere fraktlinjer** – Støtte for ordrer med mer enn én fraktlinje
3. **Logg-tjeneste** – Skrive hendelser til fil slik at vi kan spore hva som skjedde

---

## Hva ble bygget

### Nye filer

| Fil | Formål |
|-----|--------|
| `src/services/logService.js` | Logger hendelser til `logs/integration.log` |
| `docs/forklaringer/hva-er-logging.md` | Forklaring av logging-konseptet |

### Oppdaterte filer

| Fil | Hva ble endret |
|-----|----------------|
| `src/data/shopifyOrder.js` | Lagt til `discount_codes` og en ekstra fraktlinje |
| `src/validators/orderValidator.js` | Sjekk 6: validerer rabattkoder hvis de finnes |
| `src/mappers/shopifyToPos.js` | Ny `beregnFraktTotal()`, oppdatert `beregnTotal()` med rabatter |
| `src/services/posService.js` | `skrivUtOrdre()` viser nå fraktLinjer og rabatter |
| `src/utils/xmlBuilder.js` | Ny `byggFraktXml()` og `byggRabattXml()` |
| `main.js` | Logging ved hvert steg i flyten |

---

## Hva ble lært

### 1. Rabatter som eget konsept

Rabatter i Shopify er ikke bare en tallverdi – de er et objekt med:
- `code`: Koden kunden brukte ("VAAR25")
- `amount`: Beløpet som trekkes fra
- `type`: Om det er fast beløp eller prosent

I mapperen behandler vi dem separat fra produkter og frakt, og summerer dem
på slutten:

```
Varetotal + fraktTotal - rabattTotal = totalBelop
```

### 2. Arrays i stedet for enkeltverdi

I del 1 hardkodet vi `shipping_lines[0]` – vi antok alltid én fraktlinje.
Det er et vanlig feil-antagelse. I del 2 bruker vi `.reduce()` over hele arrayet:

```javascript
// Del 1 (skjørt – krasjer hvis shipping_lines er tomt):
const fraktlinje = order.shipping_lines[0];

// Del 2 (robust – håndterer 0, 1 eller flere linjer):
const totalFrakt = shippingLines.reduce((sum, linje) => sum + linje.price, 0);
```

Dette er et viktig prinsipp: **aldri anta at et array har nøyaktig ett element**.

### 3. Logging

Logging handler om å skrive ned hva som skjer mens det skjer.
En loggfil er uvurderlig i produksjon fordi:
- Programmet kjører gjerne uten at noen ser på
- Feil oppstår på uforutsigbare tidspunkter
- `console.log` forsvinner når terminalen lukkes

Vi valgte tre nivåer, inspirert av industristandarder:
- `INFO` – normal flyt
- `ADVARSEL` – uventet, men ikke kritisk
- `FEIL` – noe gikk galt

### 4. `??` (Nullish coalescing operator)

Vi bruker `??` flere steder for å håndtere valgfrie felt:

```javascript
const rabatter = order.discount_codes ?? [];
```

`??` betyr: "hvis venstre side er `null` eller `undefined`, bruk høyre side".
Dette er sikrere enn `||` fordi `||` også erstatter `0` og `""` – som vi ikke vil.

---

## Utfordringer og refleksjoner

- Det var nyttig å se at logging er teknisk sett bare `fs.appendFileSync()` –
  men det konseptuelle verdien er stor. Å skrive ned hva som skjer i rekkefølge
  er kjernen i all observabilitet.

- `beregnFraktTotal()` og `beregnTotal()` ble litt endret, men selve ansvaret
  er det samme: beregne tall, ikke ta beslutninger. Fortsatt ren separasjon.

- XML-byggeren fikk tre hjelpefunksjoner i stedet for én. Mønsteret er det
  samme – én funksjon per "type ting" som skal serialiseres til XML.

---

## Neste steg – Del 3 (.NET/C#)

Del 2 er verifisert i JavaScript. Neste fase er å konvertere til .NET:

- [ ] Lage et nytt C#-prosjekt (Console App eller Web API)
- [ ] Oversette modeller til C#-klasser med properties
- [ ] Bruke `ILogger<T>` i stedet for `logService.js`
- [ ] Vurdere å bruke `HttpClient` for ekte SOAP-kall

---

## Kjøre prosjektet

```bash
node main.js
```

**Forventet output:**
1. Valideringsmelding (OK)
2. Lesbar utskrift av POS-ordre med to fraktlinjer og én rabatt
3. SOAP XML med Fraktlinjer og Rabatter inkludert
4. Logglinjer skrives til `logs/integration.log`
