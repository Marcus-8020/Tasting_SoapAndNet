# Hva er logging?

> **Kort sagt:** Logging er å skrive ned hva programmet gjør mens det kjører –
> som en dagbok som aldri glemmer.

---

## Problemet logging løser

Tenk deg at integrasjonen din kjører automatisk kl. 03:47 om natten.
En ordre feiler. Ingen er tilstede. Hva skjedde?

Uten logging: Du vet ingenting. Du må gjette.
Med logging: Du åpner `logs/integration.log` og leser:

```
[2026-03-13T03:47:12.000Z] [INFO    ] Integrasjon startet for ordre #1044
[2026-03-13T03:47:12.001Z] [FEIL    ] Integrasjon feilet for ordre #1044: Produkt nr. 2 mangler SKU
```

Du vet nøyaktig hva som gikk galt, når det skjedde, og for hvilken ordre.

---

## Logg-nivåer

Ikke alle hendelser er like viktige. Nivåene hjelper deg å filtrere:

| Nivå       | Når brukes det?                               | Eksempel                          |
|------------|-----------------------------------------------|-----------------------------------|
| `INFO`     | Alt gikk bra. Normal fremdrift.               | "Validering OK for ordre #1001"   |
| `ADVARSEL` | Noe uventet, men vi fortsetter.               | "Ordre bruker rabattkode VAAR25"  |
| `FEIL`     | Noe gikk galt. Handling stoppet.             | "Mangler SKU på produkt nr. 2"   |

I produksjon setter man ofte varsling på `FEIL`-nivå – f.eks. en e-post eller Slack-melding.

---

## Konsoll vs. fil – hva er forskjellen?

```
console.log("Hei")   →  Vises i terminalen. Forsvinner når programmet lukkes.
fs.appendFileSync()  →  Skrives til fil. Ligger der for alltid.
```

I utvikling er konsollen fin. I produksjon trenger du **begge**:
- Konsollen for å se hva som skjer akkurat nå
- Filen for å gå tilbake og se hva som skjedde tidligere

Vår `logService.js` gjør begge deler automatisk.

---

## Slik er loggfilen strukturert

Hver linje har samme format:

```
[TIDSSTEMPEL] [NIVÅ    ] Melding
```

For eksempel:

```
[2026-03-13T14:32:01.000Z] [INFO    ] Integrasjon startet for ordre #1001
[2026-03-13T14:32:01.005Z] [INFO    ] Validering OK for ordre #1001
[2026-03-13T14:32:01.006Z] [ADVARSEL] Ordre #1001 har rabattkoder: VAAR25 (total -kr 25,-)
[2026-03-13T14:32:01.010Z] [INFO    ] Mapping fullført – total beregnet til kr 521,-
[2026-03-13T14:32:01.015Z] [INFO    ] Ordre #1001 overført til POS – FULLFØRT
```

**Tidsstempelet** er i ISO 8601-format (`2026-03-13T14:32:01.000Z`):
- Internasjonalt standardformat
- Kan sorteres alfabetisk (eldst øverst)
- `Z` på slutten betyr UTC (koordinert universaltid)

---

## appendFileSync vs. writeFileSync

To måter å skrive til fil i Node.js:

```javascript
fs.writeFileSync(fil, innhold)   // Overskriver filen – gammel logg forsvinner!
fs.appendFileSync(fil, innhold)  // Legger til på slutten – gammel logg beholdes
```

Vi bruker alltid `appendFileSync` for logging. Tenk deg at du hadde brukt
`writeFileSync` – da ville loggfilen nullstilles hver gang programmet starter.
Du ville mistet all historikk.

---

## Hvorfor `padEnd(8)` på nivåene?

I `logService.js` ser du denne linjen:

```javascript
const linje = `[${tidsstempel}] [${nivå.padEnd(8)}] ${melding}\n`;
```

`padEnd(8)` fyller ut teksten med mellomrom til den er 8 tegn lang:
- `"INFO"` (4 tegn) → `"INFO    "` (8 tegn)
- `"ADVARSEL"` (8 tegn) → `"ADVARSEL"` (8 tegn – ingen endring)
- `"FEIL"` (4 tegn) → `"FEIL    "` (8 tegn)

Dette gjør at meldingene starter på samme kolonne, noe som gjør loggen
mye lettere å lese raskt.

---

## Tilsvarende i .NET (C#)

I .NET bruker vi `ILogger<T>` fra `Microsoft.Extensions.Logging`:

```csharp
// JavaScript (vår løsning)          // .NET (C#)
logg.info("Validering OK");          _logger.LogInformation("Validering OK");
logg.advarsel("Rabatt brukt");       _logger.LogWarning("Rabatt brukt");
logg.feil("Mangler SKU");            _logger.LogError("Mangler SKU");
```

Prinsippet er identisk – kun syntaksen er forskjellig.
I .NET injiseres loggeren via dependency injection i stedet for `require()`.

---

## Neste steg

Når integrasjonen konverteres til .NET, vil vi:
1. Bruke `ILogger<T>` (innebygd i .NET)
2. Konfigurere logging i `appsettings.json`
3. Potensielt sende logger til et sentralt system (f.eks. Application Insights eller Seq)
