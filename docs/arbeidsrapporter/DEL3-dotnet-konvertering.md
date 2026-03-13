# Arbeidsrapport – Del 3: Konvertering til .NET (C#)

**Dato:** 2026-03-13
**Utvikler:** Marcus Børresen
**Teknologi:** .NET 10 (C#)
**Status:** Fullført ✓

---

## Mål for denne delen

Oversette hele del 2-løsningen fra JavaScript/Node.js til C# med identisk output,
slik at vi kan sammenligne de to versjonene side om side og forstå forskjellene.

---

## Hva ble bygget

```
dotnet/ShopifyPosIntegrasjon/
├── ShopifyPosIntegrasjon.csproj    ← prosjektfil (tilsvarer package.json)
├── GlobalUsings.cs                 ← samler alle "require()" på ett sted
├── Program.cs                      ← tilsvarer main.js
├── Models/
│   ├── ShopifyModels.cs            ← input-datastrukturer fra Shopify
│   └── PosModels.cs                ← output-format til POS
├── Data/
│   └── ShopifyOrders.cs            ← tilsvarer shopifyOrder.js
├── Validators/
│   └── OrderValidator.cs           ← tilsvarer orderValidator.js
├── Mappers/
│   └── ShopifyToPosMapper.cs       ← tilsvarer shopifyToPos.js
├── Services/
│   ├── LogService.cs               ← tilsvarer logService.js
│   └── PosService.cs               ← tilsvarer posService.js
└── Utils/
    └── XmlBuilder.cs               ← tilsvarer xmlBuilder.js
```

**Kjørekommando:**
```
cd dotnet/ShopifyPosIntegrasjon
dotnet run
```

---

## Hva ble lært

### 1. Direkte oversettelse av konsepter

De fleste JavaScript-konsepter har en direkte C#-ekvivalent:

| JavaScript | C# |
|---|---|
| `const obj = { id: 1, name: "..." }` | `public record Navn(int Id, string Name)` |
| `require("./modul")` | `global using Namespace;` |
| `module.exports = { fn }` | `public class` med `public`-metoder |
| `.map(x => ...)` | `.Select(x => ...)` |
| `.reduce((sum, x) => sum + x.pris, 0)` | `.Sum(x => x.Pris)` |
| `throw new Error("melding")` | `throw new InvalidOperationException("melding")` |
| `` `${a} ${b}` `` | `$"{a} {b}"` |
| `?.` og `??` | Identisk i C# |
| `fs.appendFileSync()` | `File.AppendAllText()` |

### 2. Records vs. objekter

I JavaScript bruker vi vanlige objekter `{ }` for datastruktur.
I C# bruker vi `record` – en klasse spesielt laget for å holde data.
Records er immutable (kan ikke endres etter opprettelse) og gir
automatisk `ToString()`, `Equals()` og mer gratis.

```csharp
// C# record – tilsvarer et JS-objekt, men med typer
public record ShopifyOrder(
    int Id,
    string OrderNumber,
    ShopifyCustomer Customer,
    ...
);
```

### 3. LINQ – C# sin versjon av array-metodene

JavaScript har `.map()`, `.filter()`, `.reduce()`.
C# har LINQ (Language Integrated Query) med `.Select()`, `.Where()`, `.Sum()`.
Syntaksen er annerledes, men konseptet er identisk.

### 4. Statisk typing

Den største forskjellen fra JS er at C# er **statisk typet** –
kompilatoren sjekker typene før programmet kjører.
Dette fanger mange feil allerede i editoren, ikke bare ved kjøring.

### 5. Top-level statements i moderne C#

I .NET 6+ kan `Program.cs` skrives uten `class Program { static void Main(...) }`.
Man skriver bare kode direkte – akkurat som i en JS-fil. Dette gjør
inngangspunktet mye mer lesbart for JavaScript-utviklere.

---

## Verifisering

Output fra `dotnet run` er identisk med `node main.js`:
- Samme valideringsflyt
- Samme POS-ordre-format
- Samme SOAP XML-struktur
- Samme logg-linjer (med tidsstempel)
- Total: kr 521,- (497 varer + 49 frakt − 25 rabatt)

---

## Neste steg

Mulige retninger videre:

- [ ] Konvertere til **ASP.NET Core Web API** – ta imot ekte Shopify webhooks via HTTP
- [ ] Bruke **ILogger<T>** og `Microsoft.Extensions.Logging` i stedet for egen LogService
- [ ] Legge til **dependency injection** (IServiceCollection) for løsere kobling
- [ ] Skrive **unit-tester** med xUnit for validator og mapper
