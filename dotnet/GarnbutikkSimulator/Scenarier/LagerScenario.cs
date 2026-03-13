// ============================================================
// FIL: Scenarier/LagerScenario.cs
// FORMÅL: Simulerer lagerproblematikken for en garnbutikk.
//
// PROBLEMENE VI LØSER HER:
//  1. Oversikt: hvilke varianter har lavt lager / er utsolgte?
//  2. Reservasjon: når en Shopify-ordre kommer inn, er varene der?
//  3. Kritisk situasjon: kunden bestiller 6 nøster, vi har 3
// ============================================================

using GarnbutikkSimulator.Models;
using GarnbutikkSimulator.Data;

namespace GarnbutikkSimulator.Scenarier;

public static class LagerScenario
{
    public static void Kjør()
    {
        Console.WriteLine("\n╔══════════════════════════════════════════════════════════════╗");
        Console.WriteLine("║  SCENARIO 2: LAGERSTYRING PER VARIANT                        ║");
        Console.WriteLine("╚══════════════════════════════════════════════════════════════╝");

        // -------------------------------------------------------
        // DEL A: Full lageroversikt med status
        // -------------------------------------------------------
        Console.WriteLine("\n  A) FULL LAGEROVERSIKT:");
        Console.WriteLine("  " + new string('─', 72));
        Console.WriteLine($"  {"Produkt",-22} {"Farge",-16} {"Størr.",-8} {"Stk",-6} Status");
        Console.WriteLine("  " + new string('─', 72));

        var lagerListe = GarnTestData.Varianter
            .Select(v => new LagerStatus(
                ProduktNavn:  GarnTestData.HentProduktNavn(v.ArtikelNr),
                FargeNavn:    GarnTestData.HentFargeNavn(v.FargeKode),
                Størrelse:    GarnTestData.HentStørrelseBeskrivelse(v.StørrelsesKode),
                Fargenummer:  v.Fargenummer,
                Antall:       v.LagerAntall,
                Nivå:         GarnTestData.BeregnNivå(v.LagerAntall)
            ))
            .OrderBy(l => l.Nivå)          // Utsolgt og lavt øverst
            .ThenBy(l => l.ProduktNavn)
            .ThenBy(l => l.FargeNavn);

        foreach (var l in lagerListe)
        {
            var (symbol, farge) = l.Nivå switch
            {
                LagerNivå.Utsolgt => ("✗ UTSOLGT", ""),
                LagerNivå.Lavt    => ("⚠ LAVT",    ""),
                _                 => ("✓",         "")
            };
            Console.WriteLine($"  {l.ProduktNavn,-22} {l.FargeNavn,-16} {l.Størrelse,-8} {l.Antall,-6} {symbol}");
        }

        // Oppsummering
        var utsolgt = lagerListe.Count(l => l.Nivå == LagerNivå.Utsolgt);
        var lavt    = lagerListe.Count(l => l.Nivå == LagerNivå.Lavt);
        var ok      = lagerListe.Count(l => l.Nivå == LagerNivå.Ok);
        Console.WriteLine("  " + new string('─', 72));
        Console.WriteLine($"  ✓ OK: {ok}  ⚠ Lavt: {lavt}  ✗ Utsolgt: {utsolgt}  (totalt {lagerListe.Count()} varianter)\n");

        // -------------------------------------------------------
        // DEL B: Sjekk om en spesifikk ordre kan oppfylles
        // -------------------------------------------------------
        Console.WriteLine("  B) KAN VI LEVERE DENNE SHOPIFY-ORDREN?");

        var testOrdre = new GarnOrdre(
            OrdreId:    5001,
            KundeNavn:  "Ingrid Hansen",
            KundeEpost: "ingrid@example.com",
            Dato:       DateTime.Today,
            Linjer:
            [
                new("Drops Alaska",  "Hvit",         "50 gram",  "0100", Antall: 6,  Enhetspris: 52.50m),
                new("Drops Alaska",  "Marineblå",    "50 gram",  null,   Antall: 4,  Enhetspris: 52.50m),
                new("Drops Karisma", "Korall",        "50 gram",  null,   Antall: 3,  Enhetspris: 47.50m),
            ]
        );

        Console.WriteLine($"\n  Ordre #{testOrdre.OrdreId} – {testOrdre.KundeNavn}");
        Console.WriteLine("  " + new string('─', 62));

        // Sjekk lager for hver linje mot våre data
        // Dette simulerer det en ekte integrasjon ville gjort mot PCKasse
        var lagerkart = GarnTestData.Varianter
            .ToDictionary(
                v => $"{v.ArtikelNr}|{v.FargeKode}|{v.StørrelsesKode}",
                v => v.LagerAntall
            );

        // Manuell kobling for simulasjonen (i ekte system: lookup via SKU/EAN)
        var ordreKoblinger = new Dictionary<string, (string artikelNr, string fargeKode, string størKode)>
        {
            { "Drops Alaska|Hvit|50 gram",      ("DROPS-ALA", "01", "50G") },
            { "Drops Alaska|Marineblå|50 gram", ("DROPS-ALA", "07", "50G") },
            { "Drops Karisma|Korall|50 gram",   ("DROPS-KAR", "12", "50G") },
        };

        bool kanLevere = true;
        foreach (var linje in testOrdre.Linjer)
        {
            var nøkkel = $"{linje.ProduktNavn}|{linje.FargeNavn}|{linje.Størrelse}";
            var kobling = ordreKoblinger[nøkkel];
            var lagerNøkkel = $"{kobling.artikelNr}|{kobling.fargeKode}|{kobling.størKode}";
            var påLager = lagerkart.GetValueOrDefault(lagerNøkkel, 0);

            var status = påLager >= linje.Antall
                ? $"✓ OK ({påLager} på lager)"
                : påLager == 0
                    ? $"✗ UTSOLGT"
                    : $"✗ DELVIS ({påLager} på lager, kunden vil ha {linje.Antall})";

            if (påLager < linje.Antall) kanLevere = false;

            Console.WriteLine(
                $"  {linje.Antall} × {linje.ProduktNavn} {linje.FargeNavn} {linje.Størrelse,-10} → {status}"
            );
        }

        Console.WriteLine("  " + new string('─', 62));
        Console.WriteLine(kanLevere
            ? $"  ✓ Ordren kan leveres fullt ut. Total: kr {testOrdre.TotalBelop:N2}"
            : $"  ✗ Ordren KAN IKKE leveres fullt ut – se detaljer over.");

        // -------------------------------------------------------
        // DEL C: Vis hva som skjer med Shopify-synk
        // -------------------------------------------------------
        Console.WriteLine("""

  C) LAGER-SYNK MELLOM SHOPIFY OG PCKASSE:

  Utfordring: Shopify viser lager basert på tall vi sender dit.
  Når en ordre legges i PCKasse-kassen, må vi:
    1. Trekke fra i PCKasse (skjer automatisk ved salg)
    2. Oppdatere Shopify via API (inventory_levels endpoint)

  Uten synk: Shopify selger Alaska Marineblå 50g selv om den er utsolgt.
  Med synk:  Shopify viser "Utsolgt" og hindrer bestilling.

  Synk-strategi (to valg):
    A) Sanntid    – oppdater Shopify etter hvert salg i kassen
    B) Periodisk  – kjør synk hvert X minutt (enklere, liten forsinkelse)
""");
    }
}
