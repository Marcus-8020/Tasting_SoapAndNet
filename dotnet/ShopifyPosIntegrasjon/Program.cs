// ============================================================
// FIL: Program.cs
// FORMÅL: Inngangspunkt for integrasjonen.
//         Tilsvarer main.js i Node.js-versjonen.
//
// SAMMENLIGNING MED JAVASCRIPT:
//  JS:  function kjørIntegrasjon(ordre) { ... }
//       kjørIntegrasjon(shopifyOrder);
//
//  C#:  void KjørIntegrasjon(ShopifyOrder ordre) { ... }
//       KjørIntegrasjon(ordre);
//
// HVA ER "TOP-LEVEL STATEMENTS"?
//  I moderne C# (fra .NET 6) trenger vi ikke lenger å skrive:
//    class Program { static void Main(string[] args) { ... } }
//  Vi kan bare skrive kode direkte øverst i filen – akkurat som i JS.
//
// HVORDAN KJØRE:
//  Fra mappen dotnet/ShopifyPosIntegrasjon/:
//    dotnet run
// ============================================================

// Sett opp tjenestene vi trenger
// Tilsvarer alle require()-kallene i main.js
var logg       = new LogService();
var validator  = new OrderValidator();
var mapper     = new ShopifyToPosMapper();
var posService = new PosService();

// Hent simulert Shopify-ordre
// Tilsvarer: const { shopifyOrder } = require("./src/data/shopifyOrder")
var ordre = ShopifyOrders.GetSampleOrder();

// Start integrasjonen
KjørIntegrasjon(ordre);

// ---------------------------------------------------------------
// LOKALE FUNKSJONER: definert etter "top-level"-koden
// Tilsvarer "function kjørIntegrasjon(ordre) { ... }" i JS
// ---------------------------------------------------------------
void KjørIntegrasjon(ShopifyOrder ordre)
{
    Console.WriteLine("============================================================");
    Console.WriteLine("  SHOPIFY → POS INTEGRASJON – STARTER");
    Console.WriteLine("============================================================");
    Console.WriteLine($"\nMottatt ordre fra Shopify: {ordre.OrderNumber}");

    logg.Info($"Integrasjon startet for ordre {ordre.OrderNumber}");

    // try/catch tilsvarer try/catch i JS – nøyaktig samme konsept
    try
    {
        // STEG 1: Validering
        Console.WriteLine("\n[STEG 1] Validerer ordre...");
        validator.Validate(ordre);
        logg.Info($"Validering OK for ordre {ordre.OrderNumber}");

        // STEG 2: Mapping
        Console.WriteLine("\n[STEG 2] Mapper til POS-format...");
        var posOrdre = mapper.Map(ordre);
        Console.WriteLine("✓ Mapping fullført");
        logg.Info($"Mapping fullført – total beregnet til kr {posOrdre.TotalBelop},-");

        // Logg advarsel hvis ordren har rabatter
        if (posOrdre.Rabatter.Count > 0)
        {
            // string.Join() tilsvarer .map(r => r.kode).join(", ") i JS
            var koder = string.Join(", ", posOrdre.Rabatter.Select(r => r.Kode));
            logg.Advarsel($"Ordre {ordre.OrderNumber} har rabattkoder: {koder} (total -kr {posOrdre.TotalRabatt},-)");
        }

        // STEG 3: Skriv ut
        Console.WriteLine("\n[STEG 3] Viser mappet ordre...");
        posService.SkrivUtOrdre(posOrdre);

        // STEG 4: Send til POS
        Console.WriteLine("\n[STEG 4] Sender til POS-systemet...");
        bool suksess = posService.SendTilPos(posOrdre);

        if (suksess)
        {
            logg.Info($"Ordre {ordre.OrderNumber} overført til POS – FULLFØRT");
            Console.WriteLine("\n============================================================");
            Console.WriteLine("  INTEGRASJON FULLFØRT – Ordre overført til POS");
            Console.WriteLine("============================================================");
        }
    }
    catch (Exception feil)
    {
        // Exception i C# tilsvarer Error i JS
        // feil.Message tilsvarer feil.message i JS
        logg.Feil($"Integrasjon feilet for ordre {ordre.OrderNumber}: {feil.Message}");

        Console.Error.WriteLine("\n[FEIL] Integrasjonen stoppet:");
        Console.Error.WriteLine($"  → {feil.Message}");
        Console.Error.WriteLine("\nOrdren ble IKKE sendt til POS-systemet.");
    }
}
