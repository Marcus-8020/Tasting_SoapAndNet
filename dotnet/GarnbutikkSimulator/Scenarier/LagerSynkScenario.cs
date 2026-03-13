// ============================================================
// FIL: Scenarier/LagerSynkScenario.cs
// FORMÅL: Simulerer den ANDRE retningen av integrasjonen:
//         PCKasse → Shopify (lageroppdatering etter kassasalg)
//
// PROBLEMET:
//  Vi har dekket Shopify → PCKasse (nettbutikkordre inn i kassen).
//  Men hva skjer når en FYSISK kunde kjøper noe i butikken?
//
//  PCKasse trekker fra lager automatisk ved kassasalg.
//  Shopify vet ingenting om dette – og vil fortsette å selge
//  en vare som allerede er solgt i butikken.
//
//  LØSNING: Etter hvert kassasalg (eller periodisk) sender vi
//  en oppdatering til Shopify via Admin REST API:
//    POST /admin/api/2024-01/inventory_levels/set.json
//
// SHOPIFY INVENTORY API – TO NØKKEL-IDer:
//  inventory_item_id: knyttet til en VARIANT (ikke produktet)
//  location_id:       hvilken butikk/lager som oppdateres
//  Disse IDene får vi fra Shopify Admin API ved oppsett.
// ============================================================

using GarnbutikkSimulator.Models;
using GarnbutikkSimulator.Data;
using System.Text.Json;

namespace GarnbutikkSimulator.Scenarier;

// Representerer en lageroppdatering klar til å sendes til Shopify
public record ShopifyLagerOppdatering(
    long   LocationId,         // ID for butikklokasjonen i Shopify
    long   InventoryItemId,    // ID for denne varianten i Shopify
    int    Available,          // nytt lagernivå
    string Sku,                // for logging/lesbarhet
    string ProduktInfo         // for logging/lesbarhet
);

public static class LagerSynkScenario
{
    // Simulerte Shopify-IDer (i virkeligheten hentes disse fra Shopify API)
    // Location = butikkens fysiske lokasjon i Shopify
    private const long SHOPIFY_LOCATION_ID = 12345678901L;

    // Mapping: SKU → Shopify inventory_item_id
    // Denne tabellen bygges opp én gang ved oppsett og lagres lokalt
    private static readonly Dictionary<string, long> InventoryItemIds = new()
    {
        { "DROPS-ALA-01-50G",  98001001L },
        { "DROPS-ALA-02-50G",  98001002L },
        { "DROPS-ALA-06-50G",  98001006L },
        { "DROPS-ALA-07-50G",  98001007L },
        { "DROPS-ALA-01-100G", 98001101L },
        { "DROPS-KAR-01-50G",  98002001L },
        { "DROPS-KAR-07-50G",  98002007L },
        { "SG-SMART-01-50G",   98003001L },
        { "SG-SMART-25-50G",   98003025L },
    };

    public static void Kjør()
    {
        Console.WriteLine("\n╔══════════════════════════════════════════════════════════════╗");
        Console.WriteLine("║  SCENARIO 4: PCKASSE → SHOPIFY (LAGERSYNK)                   ║");
        Console.WriteLine("╚══════════════════════════════════════════════════════════════╝");

        Console.WriteLine("""

  FLYT:
  En fysisk kunde kjøper garn i butikken → PCKasse registrerer salget
  → lager trekkes fra i PCKasse → integrasjonen oppdaterer Shopify

  Uten denne synken vil Shopify selge varer som er utsolgt i butikken.
""");

        // -------------------------------------------------------
        // DEL A: Simuler et kassasalg i PCKasse
        // -------------------------------------------------------
        Console.WriteLine("  A) KASSASALG I BUTIKKEN (PCKasse registrerer dette):");
        Console.WriteLine("  " + new string('─', 62));

        var kassaSalg = new[]
        {
            // (artikelNr, fargeKode, størrelse, antallSolgt)
            ("DROPS-ALA", "01", "50G",  4),   // 4 nøster Alaska Hvit 50g
            ("DROPS-ALA", "07", "50G",  1),   // 1 nøste Alaska Marineblå – men den er UTSOLGT!
            ("DROPS-KAR", "01", "50G",  6),   // 6 nøster Karisma Hvit
            ("SG-SMART",  "25", "50G",  3),   // 3 nøster Smart Brun
        };

        // Lag en kopi av lageret vi kan trekke fra
        var oppdatertLager = GarnTestData.Varianter
            .ToDictionary(v => $"{v.ArtikelNr}-{v.FargeKode}-{v.StørrelsesKode}", v => v.LagerAntall);

        foreach (var (art, farge, størr, antall) in kassaSalg)
        {
            var nøkkel     = $"{art}-{farge}-{størr}";
            var produktNavn = GarnTestData.HentProduktNavn(art);
            var fargeNavn   = GarnTestData.HentFargeNavn(farge);
            var før         = oppdatertLager.GetValueOrDefault(nøkkel, 0);

            if (før == 0)
            {
                Console.WriteLine($"  ⚠ ADVARSEL: {produktNavn} {fargeNavn} {størr} – allerede utsolgt! Salg blokkert.");
                continue;
            }

            var etter = Math.Max(0, før - antall);
            oppdatertLager[nøkkel] = etter;
            Console.WriteLine($"  Solgt: {antall}× {produktNavn} {fargeNavn} {størr}  →  lager: {før} → {etter}");
        }

        // -------------------------------------------------------
        // DEL B: Bygg Shopify lageroppdateringer
        // -------------------------------------------------------
        Console.WriteLine("\n  B) BYGGER SHOPIFY LAGEROPPDATERINGER:");
        Console.WriteLine("  " + new string('─', 62));

        var oppdateringer = new List<ShopifyLagerOppdatering>();

        foreach (var (nøkkel, nyttAntall) in oppdatertLager)
        {
            // Finn varianten i originaldataene
            var original = GarnTestData.Varianter
                .FirstOrDefault(v => $"{v.ArtikelNr}-{v.FargeKode}-{v.StørrelsesKode}" == nøkkel);

            if (original == null) continue;

            // Bare send oppdatering hvis lageret faktisk har endret seg
            if (original.LagerAntall == nyttAntall) continue;

            // Bygg SKU for oppslag av Shopify-ID
            var sku = $"{original.ArtikelNr}-{original.FargeKode}-{original.StørrelsesKode}";

            if (!InventoryItemIds.TryGetValue(sku, out long inventoryItemId))
            {
                Console.WriteLine($"  ⚠ Mangler Shopify inventory_item_id for SKU '{sku}' – hopper over");
                continue;
            }

            var produktInfo = $"{GarnTestData.HentProduktNavn(original.ArtikelNr)} " +
                              $"{GarnTestData.HentFargeNavn(original.FargeKode)} {original.StørrelsesKode}";

            oppdateringer.Add(new ShopifyLagerOppdatering(
                LocationId:      SHOPIFY_LOCATION_ID,
                InventoryItemId: inventoryItemId,
                Available:       nyttAntall,
                Sku:             sku,
                ProduktInfo:     produktInfo
            ));
        }

        foreach (var opd in oppdateringer)
            Console.WriteLine($"  → {opd.ProduktInfo,-40} nytt lager: {opd.Available}");

        // -------------------------------------------------------
        // DEL C: Vis JSON-payloaden som sendes til Shopify API
        // -------------------------------------------------------
        Console.WriteLine("\n  C) SHOPIFY API-KALL (dette ville blitt sendt for HVER variant):");
        Console.WriteLine("  " + new string('─', 62));
        Console.WriteLine("  Endepunkt: POST https://{butikk}.myshopify.com/admin/api/2024-01/inventory_levels/set.json");
        Console.WriteLine("  Header:    X-Shopify-Access-Token: {token}\n");

        foreach (var opd in oppdateringer)
        {
            // Bygg JSON-payload slik Shopify forventer det
            var payload = new
            {
                location_id       = opd.LocationId,
                inventory_item_id = opd.InventoryItemId,
                available         = opd.Available
            };

            // System.Text.Json serialiserer til JSON-streng
            var json = JsonSerializer.Serialize(payload, new JsonSerializerOptions { WriteIndented = true });

            Console.WriteLine($"  // {opd.Sku} – {opd.ProduktInfo}");
            Console.WriteLine("  " + json.Replace("\n", "\n  "));
            Console.WriteLine();
        }

        // -------------------------------------------------------
        // DEL D: Synk-strategi – to valg
        // -------------------------------------------------------
        Console.WriteLine("""
  D) SYNK-STRATEGI – VELG BASERT PÅ BUTIKKENS BEHOV:

  ┌─────────────────┬──────────────────────────────────────────────┐
  │ Sanntidssynk    │ Trigger etter hvert kassasalg                │
  │                 │ + Shopify alltid oppdatert                   │
  │                 │ - Krever polling eller DB-trigger mot PCKasse│
  ├─────────────────┼──────────────────────────────────────────────┤
  │ Periodisk synk  │ Kjør hvert 5–15 minutt                      │
  │                 │ + Enklere å implementere                     │
  │                 │ - Kort vindu der oversalg er mulig           │
  └─────────────────┴──────────────────────────────────────────────┘

  For en garnbutikk med lav trafikk anbefales periodisk synk hvert
  5 minutt – enkelt, robust og lavt risikonivå for oversalg.

  IMPLEMENTASJON I .NET:
    Sanntid:   SQL Server Change Tracking + bakgrunnstjeneste
    Periodisk: IHostedService med Timer (eller Azure Function/CRON)
""");
    }
}
