# Shopify → POS Integrasjon

Et læringsprosjekt som simulerer en fullstendig integrasjon mellom Shopify og et legacy PC-kasseystem (POS) via SOAP/XML. Bygget i faser – først i JavaScript, deretter konvertert til C#/.NET.

---

## Hva gjør det?

Prosjektet tar en Shopify-ordre og sender den videre til et POS-system gjennom en pipeline på fire steg:

```
Shopify-ordre → Validering → Mapping → SOAP XML → POS-system
```

1. **Validering** – sjekker at ordren er komplett før vi gjør noe med den
2. **Mapping** – oversetter Shopify sitt format til det POS-systemet forventer
3. **Logging** – skriver hendelser til fil underveis
4. **SOAP XML** – bygger meldingen som sendes til kassesystemet

Alle eksterne kall er simulert – ingen ekte Shopify-butikk eller kassesystem er nødvendig.

---

## Kjøre prosjektet

**JavaScript (Node.js):**
```bash
node main.js
```
Ingen installasjon nødvendig – bruker kun innebygde Node.js-moduler.

**C# (.NET):**
```bash
cd dotnet/ShopifyPosIntegrasjon
dotnet run
```
Krever [.NET SDK](https://dotnet.microsoft.com/download) installert.

Begge versjonene produserer identisk output.

---

## Struktur

```
shopify-pos-integrasjon/
│
├── main.js                          # JavaScript-inngangspunkt
├── src/
│   ├── data/shopifyOrder.js         # Simulert Shopify-ordre (testdata)
│   ├── validators/orderValidator.js # Validerer ordren
│   ├── mappers/shopifyToPos.js      # Oversetter Shopify → POS-format
│   ├── services/posService.js       # Viser og "sender" ordren
│   ├── services/logService.js       # Logger hendelser til fil
│   └── utils/xmlBuilder.js         # Bygger SOAP XML
│
├── dotnet/ShopifyPosIntegrasjon/    # C#-versjon (identisk logikk)
│   ├── Program.cs                   # = main.js
│   ├── Models/                      # Datastrukturer (records)
│   ├── Data/                        # Testdata
│   ├── Validators/                  # Validering
│   ├── Mappers/                     # Mapping
│   ├── Services/                    # LogService + PosService
│   └── Utils/                       # XmlBuilder
│
├── logs/                            # Loggfiler (opprettes automatisk)
└── docs/
    ├── arbeidsrapporter/            # Hva ble gjort i hver del
    └── forklaringer/                # Konseptforklaringer
```

---

## Hva støttes (Del 2)

- Produktlinjer med SKU, antall og pris
- Rabattkoder (`fixed_amount` og `percentage`)
- Flere fraktlinjer med ulike priser
- Logging til `logs/integration.log` med nivåer: INFO / ADVARSEL / FEIL
- Validering av alle felt før prosessering

---

## Faser

| Del | Innhold | Teknologi | Status |
|-----|---------|-----------|--------|
| Del 1 | Grunnleggende pipeline: validering, mapping, SOAP | JavaScript | ✓ Fullført |
| Del 2 | Rabatter, flere fraktlinjer, logg-tjeneste | JavaScript | ✓ Fullført |
| Del 3 | Konvertering til C#/.NET | C# / .NET 10 | ✓ Fullført |
| Del 4 | ASP.NET Core Web API – ekte Shopify webhooks | C# / ASP.NET | Planlagt |

---

## Teknologi

- **JavaScript:** Node.js, ingen npm-pakker
- **C#:** .NET 10, ingen NuGet-pakker
