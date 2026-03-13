// ============================================================
// FIL: Scenarier/VariantScenario.cs
// FORMÅL: Viser variant-strukturen for et garn-produkt.
//         Svarer på: "Hva er egentlig en variant i PCKasse?"
//
// PROBLEMET ILLUSTRERT:
//   En kunde ringer og sier: "Har dere Drops Alaska?"
//   Svaret er IKKE ja/nei – det avhenger av farge og størrelse.
//   Drops Alaska finnes i 40+ farger × 2 størrelser = 80+ varianter.
//   Bare noen av dem er på lager.
// ============================================================

using GarnbutikkSimulator.Models;
using GarnbutikkSimulator.Data;

namespace GarnbutikkSimulator.Scenarier;

public static class VariantScenario
{
    public static void Kjør()
    {
        Console.WriteLine("\n╔══════════════════════════════════════════════════════════════╗");
        Console.WriteLine("║  SCENARIO 1: VARIANT-STRUKTUR                                ║");
        Console.WriteLine("╚══════════════════════════════════════════════════════════════╝");

        Console.WriteLine("""

  KONSEPT:
  I PCKasse-databasen er produkter og varianter tre separate tabeller:

    Articles      → "Drops Alaska"  (produktet)
    Colors        → "Hvit", "Sort"  (fargekatalogen)
    Sizes         → "50g", "100g"   (størrelseskatalogen)
    SizeColors    → Alaska+Hvit+50g, Alaska+Hvit+100g, Alaska+Sort+50g ...

  Hvert felt i SizeColors er én salgbar enhet med egen EAN og lagerstatus.
""");

        // Vis alle produkter
        Console.WriteLine("  PRODUKTER I BUTIKKEN:");
        Console.WriteLine("  " + new string('─', 62));
        Console.WriteLine($"  {"Artikkelnr",-14} {"Navn",-30} {"Pris",-10} {"Fiber",-20}");
        Console.WriteLine("  " + new string('─', 62));

        foreach (var p in GarnTestData.Produkter)
        {
            Console.WriteLine($"  {p.ArtikelNr,-14} {p.Navn,-30} kr {p.PrisInklMva,-7} {p.FiberInnhold,-20}");
        }

        // Velg ett produkt og vis alle varianter
        Console.WriteLine("\n  VARIANTER FOR 'DROPS ALASKA' (Articles → SizeColors joined):");
        Console.WriteLine("  " + new string('─', 70));
        Console.WriteLine($"  {"Farge",-16} {"Størrelse",-10} {"EAN",-16} {"Fargenr",-10} {"Lager",-8} Status");
        Console.WriteLine("  " + new string('─', 70));

        var alaskaVarianter = GarnTestData.Varianter
            .Where(v => v.ArtikelNr == "DROPS-ALA")
            .OrderBy(v => v.StørrelsesKode)
            .ThenBy(v => int.Parse(v.FargeKode));

        foreach (var v in alaskaVarianter)
        {
            var fargeNavn = GarnTestData.HentFargeNavn(v.FargeKode);
            var størrelse = GarnTestData.HentStørrelseBeskrivelse(v.StørrelsesKode);
            var nivå = GarnTestData.BeregnNivå(v.LagerAntall);
            var statusSymbol = nivå switch
            {
                LagerNivå.Ok      => "✓ OK",
                LagerNivå.Lavt    => "⚠ Lavt",
                LagerNivå.Utsolgt => "✗ Utsolgt",
                _                 => ""
            };

            Console.WriteLine(
                $"  {fargeNavn,-16} {størrelse,-10} {v.EanKode,-16} {v.Fargenummer,-10} {v.LagerAntall,-8} {statusSymbol}"
            );
        }

        // Tell opp
        var totalt   = GarnTestData.Varianter.Count(v => v.ArtikelNr == "DROPS-ALA");
        var påLager  = GarnTestData.Varianter.Count(v => v.ArtikelNr == "DROPS-ALA" && v.LagerAntall > 0);
        var utsolgte = GarnTestData.Varianter.Count(v => v.ArtikelNr == "DROPS-ALA" && v.LagerAntall == 0);

        Console.WriteLine("  " + new string('─', 70));
        Console.WriteLine($"  Totalt {totalt} varianter → {påLager} tilgjengelige, {utsolgte} utsolgte\n");

        // Vis fargenummer-problemet
        Console.WriteLine("  ★ FARGENUMMER (DYRELOT) – HVORFOR DET BETYR NOE:");
        Console.WriteLine("""
  Samme farge, forskjellig fargenummer = produsert i forskjellig parti.
  Fargen kan se litt ulik ut i dagslys.

  Eksempel – Drops Alaska Lys grå (farge 03):
""");

        // Simuler to lot-grupper
        Console.WriteLine($"  {"Fargenummer",-14} {"Antall på lager",-18} Anbefaling");
        Console.WriteLine("  " + new string('─', 50));
        Console.WriteLine($"  {"0100",-14} {"8 stk",-18} Bruk til prosjekter under 400m");
        Console.WriteLine($"  {"0200",-14} {"5 stk",-18} Bruk til prosjekter under 250m");
        Console.WriteLine();
        Console.WriteLine("  → Kunden bestiller 10 nøster: vi MÅ sjekke om vi har nok fra SAMME lot.");
        Console.WriteLine("    Shopify vet ikke om fargenummer – dette må integrasjonen håndtere.\n");
    }
}
