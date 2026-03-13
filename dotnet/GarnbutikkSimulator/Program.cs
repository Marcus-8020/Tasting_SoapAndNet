// ============================================================
// FIL: Program.cs
// FORMÅL: Kjører alle scenariene i simulatoren.
//
// KJØRE:
//   cd dotnet/GarnbutikkSimulator
//   dotnet run
// ============================================================

using GarnbutikkSimulator.Scenarier;
using GarnbutikkSimulator.Data;

Console.OutputEncoding = System.Text.Encoding.UTF8;

Console.WriteLine("╔══════════════════════════════════════════════════════════════╗");
Console.WriteLine("║          GARNBUTIKK INTEGRASJON – SIMULATOR                  ║");
Console.WriteLine("║          Shopify ↔ PCKasse med garn-varianter                ║");
Console.WriteLine("╚══════════════════════════════════════════════════════════════╝");

Console.WriteLine($"""

  Testdata lastet:
    {GarnTestData.Produkter.Count} produkter
    {GarnTestData.Farger.Count} farger
    {GarnTestData.Størrelser.Count} størrelser
    {GarnTestData.Varianter.Count} varianter (produkt × farge × størrelse)
    {GarnTestData.Varianter.Count(v => v.LagerAntall == 0)} utsolgte varianter
    {GarnTestData.Varianter.Count(v => v.LagerAntall > 0 && v.LagerAntall <= 5)} varianter med lavt lager
""");

// Kjør alle fire scenariene
VariantScenario.Kjør();
LagerScenario.Kjør();
ShopifyMappingScenario.Kjør();
LagerSynkScenario.Kjør();

Console.WriteLine("\n╔══════════════════════════════════════════════════════════════╗");
Console.WriteLine("║  FERDIG – alle scenarier kjørt                               ║");
Console.WriteLine("╠══════════════════════════════════════════════════════════════╣");
Console.WriteLine("║  1. Variant-struktur    (PCKasse SizeColors-modellen)         ║");
Console.WriteLine("║  2. Lagerstyring        (lagersjekk per variant)              ║");
Console.WriteLine("║  3. Shopify → PCKasse   (ordre inn via SKU-mapping)           ║");
Console.WriteLine("║  4. PCKasse → Shopify   (lagersynk etter kassasalg)           ║");
Console.WriteLine("╚══════════════════════════════════════════════════════════════╝\n");
