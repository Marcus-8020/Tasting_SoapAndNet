# Shopify → PCKasse Integrasjon

Et læringsprosjekt som simulerer en fullstendig integrasjon mellom Shopify og et PCKasse POS-system. Bygget i faser – fra grunnleggende JavaScript-pipeline til C#/.NET med ekte databasetilkobling og garnbutikk-simulator.

---

## Prosjektoversikt

```
Shopify-ordre → Validering → Mapping → SOAP XML → PCKasse
```

Alle eksterne kall er simulert frem til databasetestprosjektet – der kobles det mot en ekte lokal SQL Server-instans.

---

## Faser

| Del | Innhold | Teknologi | Status |
|-----|---------|-----------|--------|
| Del 1 | Grunnleggende pipeline: validering, mapping, SOAP | JavaScript | ✓ Fullført |
| Del 2 | Rabatter, flere fraktlinjer, logg-tjeneste | JavaScript | ✓ Fullført |
| Del 3 | Konvertering til C#/.NET | C# / .NET 10 | ✓ Fullført |
| Del 4 | DB-tilkobling mot lokal PCKasseV3 | C# / SQL Server | ✓ Fullført |
| Del 5 | Garnbutikk-simulator med variant-problematikk | C# | ✓ Fullført |
| Del 6 | ASP.NET Core Web API – ekte Shopify webhooks | C# / ASP.NET | Planlagt |

---

## Kjøre prosjektene

**JavaScript (Del 1+2):**
```bash
node main.js
```
Ingen installasjon nødvendig – bruker kun innebygde Node.js-moduler.

**C# – Shopify→POS integrasjon (Del 3):**
```bash
cd dotnet/ShopifyPosIntegrasjon
dotnet run
```
Identisk output som JavaScript-versjonen.

**C# – Database-tilkoblingstest (Del 4):**
```bash
cd dotnet/PCKasseDbTest
dotnet run
```
Krever lokal SQL Server med databasen `PCKasseV3` og Windows Authentication.

**C# – Garnbutikk-simulator (Del 5):**
```bash
cd dotnet/GarnbutikkSimulator
dotnet run
```
Ingen databasetilkobling nødvendig – kjører på in-memory testdata.

Alle C#-prosjekter krever [.NET SDK 10](https://dotnet.microsoft.com/download) installert.

---

## Prosjektstruktur

```
shopify-pos-integrasjon/
│
├── main.js                              # JavaScript-inngangspunkt (del 1+2)
├── src/
│   ├── data/shopifyOrder.js             # Simulert Shopify-ordre med rabatter og frakt
│   ├── validators/orderValidator.js     # 6-punkts validering inkl. rabattkoder
│   ├── mappers/shopifyToPos.js          # Oversetter Shopify → POS-format
│   ├── services/posService.js           # Viser og "sender" ordren
│   ├── services/logService.js           # Logger til logs/integration.log
│   └── utils/xmlBuilder.js             # Bygger SOAP XML med frakt og rabatter
│
├── dotnet/
│   ├── ShopifyPosIntegrasjon/           # C#-versjon av del 1+2 (del 3)
│   │   ├── Program.cs
│   │   ├── Models/                      # ShopifyModels.cs + PosModels.cs (records)
│   │   ├── Data/                        # ShopifyOrders.cs (testdata)
│   │   ├── Validators/OrderValidator.cs
│   │   ├── Mappers/ShopifyToPosMapper.cs
│   │   ├── Services/                    # LogService.cs + PosService.cs
│   │   └── Utils/XmlBuilder.cs
│   │
│   ├── PCKasseDbTest/                   # Database-tilkoblingstest (del 4)
│   │   ├── Database/DbForbindelse.cs    # Connection string + tilkoblingshelper
│   │   └── Program.cs                  # 5 tester (se under)
│   │
│   └── GarnbutikkSimulator/            # Garnbutikk-simulator (del 5)
│       ├── Models/GarnModeller.cs       # Produkt, Farge, Størrelse, GarnVariant, Ordre
│       ├── Data/GarnTestData.cs         # 5 produkter, 28 farger, 37 varianter
│       ├── Scenarier/
│       │   ├── VariantScenario.cs       # Scenario 1: variant-struktur
│       │   ├── LagerScenario.cs         # Scenario 2: lagerstyring
│       │   └── ShopifyMappingScenario.cs # Scenario 3: Shopify → PCKasse mapping
│       └── Program.cs
│
├── logs/                                # Loggfiler (opprettes automatisk ved kjøring)
└── docs/
    ├── arbeidsrapporter/                # DEL1, DEL2, DEL3 – hva ble gjort og lært
    └── forklaringer/                    # hva-er-mapping.md, hva-er-logging.md
```

---

## PCKasseDbTest – hva testene gjør

Kjøres mot en lokal SQL Server-instans med `PCKasseV3`-databasen (ProCom PCKasse).

| Test | Hva den gjør |
|------|-------------|
| Test 1 | Tilkoblingstest – bekrefter at Windows Authentication fungerer |
| Test 2 | Viser SQL Server-versjon, databasenavn og innlogget bruker |
| Test 3 | Lister **alle tabeller** i databasen (155 tabeller i PCKasseV3) |
| Test 4 | Viser kolonnenavn og datatyper for den første tabellen som finnes |
| Test 5 | Henter topp 5 rader fra samme tabell |

**Connection string** (i `Database/DbForbindelse.cs`):
```
Server=localhost;Database=PCKasseV3;Trusted_Connection=True;TrustServerCertificate=True;
```

---

## GarnbutikkSimulator – hva scenariene viser

Simulerer en norsk garnbutikk med de spesifikke utfordringene knyttet til garn-varianter.

### Testdata
- **5 produkter:** Drops Alaska, Drops Karisma, Drops Sky, Sandnes Garn Smart, Sandnes Garn Tynn Silk Mohair
- **28 farger** med norske navn og hex-koder
- **37 varianter** (produkt × farge × størrelse) med EAN og fargenummer (dyrelot)
- Realistisk lagerfordeling: 20 OK, 11 lavt lager, 6 utsolgte

### Scenario 1 – Variant-struktur
Viser hvorfor ett garnprodukt egentlig er mange SKU-er. Drops Alaska finnes i 16 simulerte varianter (farger × størrelser), hver med egen EAN og fargenummer. Forklarer dyrelot-problematikken: samme farge fra to produksjonsbatcher kan se ulik ut i dagslys.

### Scenario 2 – Lagerstyring per variant
Full lageroversikt med status (OK / Lavt / Utsolgt) per variant. Simulerer en innkommende ordre og sjekker om butikken kan levere hver linje – med konkret svar (f.eks. "1 på lager, kunden vil ha 3"). Forklarer to synk-strategier mot Shopify: sanntid vs. periodisk.

### Scenario 3 – Shopify → PCKasse mapping
Tar en realistisk Shopify-webhook-ordre med SKU-er (`DROPS-ALA-01-50G`), parser dem og slår opp tilhørende PCKasse-variant (EAN, fargenummer, lagerstatus). Viser trinn-for-trinn hva integrasjonen må gjøre fra webhook ankommer til ordre er registrert i PCKasse.

**SKU-konvensjonen** som binder Shopify og PCKasse sammen:
```
[ArtikelNr]-[FargeKode]-[StørrelsesKode]
DROPS-ALA-01-50G  =  Drops Alaska, farge 01 (Hvit), 50g
SG-SMART-25-50G   =  Sandnes Garn Smart, farge 25 (Brun), 50g
```

---

## JavaScript → C# sammenligning

| JavaScript | C# |
|---|---|
| `require()` | `global using` |
| `const obj = { ... }` | `public record Navn(...)` |
| `.map(x => ...)` | `.Select(x => ...)` |
| `.reduce((s, x) => s + x, 0)` | `.Sum(x => x.Verdi)` |
| `throw new Error("...")` | `throw new InvalidOperationException("...")` |
| `` `${a} ${b}` `` | `$"{a} {b}"` |
| `?.` og `??` | Identisk i C# |
| `fs.appendFileSync()` | `File.AppendAllText()` |

---

## Teknologi

- **JavaScript:** Node.js, ingen npm-pakker
- **C#:** .NET 10, `Microsoft.Data.SqlClient` (kun i DB-testprosjektene)
- **Database:** Microsoft SQL Server 2025, Windows Authentication
