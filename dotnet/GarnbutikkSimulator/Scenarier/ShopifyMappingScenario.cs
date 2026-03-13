// ============================================================
// FIL: Scenarier/ShopifyMappingScenario.cs
// FORMÅL: Viser hvordan en Shopify-ordre med garn-varianter
//         mappes til PCKasse-format.
//
// KJERNEPROBLEMET:
//  Shopify:   Produkt "Drops Alaska" med variant { Color: "Hvit", Size: "50g" }
//  PCKasse:   Articles-rad + SizeColors-rad (krysstabell)
//
//  Koblingen skjer via SKU (artikkelnummer + fargekode + størrelseskode)
//  som vi selv definerer og holder konsistent mellom systemene.
// ============================================================

using GarnbutikkSimulator.Models;
using GarnbutikkSimulator.Data;

namespace GarnbutikkSimulator.Scenarier;

public static class ShopifyMappingScenario
{
    public static void Kjør()
    {
        Console.WriteLine("\n╔══════════════════════════════════════════════════════════════╗");
        Console.WriteLine("║  SCENARIO 3: SHOPIFY → PCKASSE MAPPING FOR GARN              ║");
        Console.WriteLine("╚══════════════════════════════════════════════════════════════╝");

        // -------------------------------------------------------
        // DEL A: Slik ser en Shopify-garn-ordre ut
        // -------------------------------------------------------
        var shopifyOrdre = new ShopifyGarnOrdre(
            ShopifyOrdreId: "gid://shopify/Order/5001234567",
            KundeEpost:     "kari@example.com",
            Linjer:
            [
                new(
                    ShopifyProduktId:  "gid://shopify/Product/1001",
                    VariantId:         "gid://shopify/ProductVariant/2001",
                    ProduktTittel:     "Drops Alaska",
                    FargeOption:       "Hvit",           // option1
                    StørrelsesOption:  "50g",            // option2
                    Sku:               "DROPS-ALA-01-50G",
                    Antall:            4,
                    Pris:              52.50m
                ),
                new(
                    ShopifyProduktId:  "gid://shopify/Product/1001",
                    VariantId:         "gid://shopify/ProductVariant/2002",
                    ProduktTittel:     "Drops Alaska",
                    FargeOption:       "Naturhvit",
                    StørrelsesOption:  "100g",
                    Sku:               "DROPS-ALA-02-100G",
                    Antall:            2,
                    Pris:              98.00m             // 100g koster mer
                ),
                new(
                    ShopifyProduktId:  "gid://shopify/Product/1003",
                    VariantId:         "gid://shopify/ProductVariant/3001",
                    ProduktTittel:     "Sandnes Garn Smart",
                    FargeOption:       "Brun",
                    StørrelsesOption:  "50g",
                    Sku:               "SG-SMART-25-50G",
                    Antall:            6,
                    Pris:              68.75m
                ),
            ]
        );

        Console.WriteLine("\n  A) SHOPIFY-ORDRE (råformat fra webhook):");
        Console.WriteLine($"  Ordre-ID: {shopifyOrdre.ShopifyOrdreId}");
        Console.WriteLine($"  Kunde:    {shopifyOrdre.KundeEpost}");
        Console.WriteLine("  " + new string('─', 70));
        Console.WriteLine($"  {"Produkt",-20} {"Farge",-14} {"Størr.",-8} {"SKU",-22} {"Stk",-5} Pris");
        Console.WriteLine("  " + new string('─', 70));

        foreach (var linje in shopifyOrdre.Linjer)
        {
            Console.WriteLine(
                $"  {linje.ProduktTittel,-20} {linje.FargeOption,-14} {linje.StørrelsesOption,-8}" +
                $" {linje.Sku,-22} {linje.Antall,-5} kr {linje.Pris:N2}"
            );
        }

        // -------------------------------------------------------
        // DEL B: SKU-konvensjonen som kobler systemene
        // -------------------------------------------------------
        Console.WriteLine("""

  B) SKU-KONVENSJONEN – NØKKELEN TIL KOBLINGEN:

  Vi bygger SKU slik:  [ArtikelNr]-[FargeKode]-[StørrelsesKode]
  Eksempler:
    DROPS-ALA-01-50G   = Drops Alaska, farge 01 (Hvit), 50g
    DROPS-ALA-02-100G  = Drops Alaska, farge 02 (Naturhvit), 100g
    SG-SMART-25-50G    = Sandnes Smart, farge 25 (Brun), 50g

  Denne SKU-en må eksistere BÅDE i Shopify og i PCKasse (EanNos/SizeColors).
  Det er vår integrasjon sin jobb å holde disse synkronisert.
""");

        // -------------------------------------------------------
        // DEL C: Map Shopify-ordre → PCKasse-format
        // -------------------------------------------------------
        Console.WriteLine("  C) MAPPING → PCKASSE-FORMAT:");
        Console.WriteLine("  " + new string('─', 70));

        bool mappingFeilet = false;
        foreach (var linje in shopifyOrdre.Linjer)
        {
            // Parse SKU: DROPS-ALA-01-50G → artikelNr=DROPS-ALA, farge=01, størrelse=50G
            var skuDeler = linje.Sku.Split('-');
            // SKU-format: [Merkevare]-[Produkt]-[FargeKode]-[StørrelsesKode]
            // DROPS-ALA-01-50G → ['DROPS','ALA','01','50G']
            string artikelNr    = string.Join("-", skuDeler[..^2]);  // alt unntatt siste 2
            string fargeKode    = skuDeler[^2];                       // nest siste
            string størrelseKode = skuDeler[^1];                      // siste

            // Finn variant i PCKasse-data (tilsvarer SELECT fra SizeColors + Articles)
            var variant = GarnTestData.Varianter.FirstOrDefault(v =>
                v.ArtikelNr       == artikelNr   &&
                v.FargeKode       == fargeKode   &&
                v.StørrelsesKode  == størrelseKode
            );

            if (variant == null)
            {
                Console.WriteLine($"  ✗ FEIL: SKU '{linje.Sku}' finnes ikke i PCKasse!");
                mappingFeilet = true;
                continue;
            }

            var produktNavn = GarnTestData.HentProduktNavn(artikelNr);
            var fargeNavn   = GarnTestData.HentFargeNavn(fargeKode);
            var lagerOk     = variant.LagerAntall >= linje.Antall;

            Console.WriteLine($"  SKU: {linje.Sku}");
            Console.WriteLine($"    → Produkt:    {produktNavn} [{artikelNr}]");
            Console.WriteLine($"    → Farge:      {fargeNavn} (kode: {fargeKode})");
            Console.WriteLine($"    → Størrelse:  {størrelseKode}");
            Console.WriteLine($"    → EAN:        {variant.EanKode}");
            Console.WriteLine($"    → Fargenr:    {variant.Fargenummer}");
            Console.WriteLine($"    → Lager:      {variant.LagerAntall} stk {(lagerOk ? "✓" : $"✗ (kunden vil ha {linje.Antall})")}");
            Console.WriteLine();
        }

        if (!mappingFeilet)
            Console.WriteLine("  ✓ Alle linjer mappet til PCKasse-format");

        // -------------------------------------------------------
        // DEL D: Hva integrasjonen må gjøre (oppsummering)
        // -------------------------------------------------------
        Console.WriteLine("""

  D) INTEGRASJONENS ANSVAR – STEG FOR STEG:

  Når Shopify-webhook ankommer:
    1. Parse JSON → ShopifyGarnOrdre-objekt
    2. For hver linje: parse SKU → hent variant fra PCKasse
    3. Sjekk lager per variant (SELECT fra SizeColors)
    4. Hvis nok på lager:
         → Opprett ordre i PCKasse (INSERT i Orders + OrderLines)
         → Trekk fra lager (oppdateres automatisk av PCKasse)
         → Oppdater Shopify med ordrebekreftelse
    5. Hvis IKKE nok på lager:
         → Logg advarsel
         → Send beskjed til butikken (e-post / Slack)
         → Vurder: dellevering eller avvisning?
    6. Etter salg: synkroniser lagerstatus tilbake til Shopify
""");
    }
}
