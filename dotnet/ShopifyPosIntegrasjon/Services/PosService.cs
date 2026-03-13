// ============================================================
// FIL: Services/PosService.cs
// FORMÅL: Viser og "sender" en ordre til POS-systemet.
//         Tilsvarer posService.js i Node.js-versjonen.
//
// SAMMENLIGNING MED JAVASCRIPT:
//  JS:  posOrdre.fraktLinjer.forEach((linje, index) => { ... })
//  C#:  foreach (var linje in posOrdre.FraktLinjer) { ... }
//       (index: bruk en teller-variabel, eller Select med indeks)
//
//  JS:  console.log(`${a}: ${b}`)
//  C#:  Console.WriteLine($"{a}: {b}")
// ============================================================

namespace ShopifyPosIntegrasjon.Services;

public class PosService
{
    // XmlBuilder injiseres som en avhengighet (dependency)
    // I fremtidige prosjekter vil dette gjøres via DI-container
    private readonly XmlBuilder _xmlBuilder = new();

    // Tilsvarer "function skrivUtOrdre(posOrdre)" i JS
    public void SkrivUtOrdre(PosOrder posOrdre)
    {
        Console.WriteLine("\n=== POS-ORDRE (lesbart format) ===");
        Console.WriteLine($"Ordre-ID:   {posOrdre.OrdreId}");
        Console.WriteLine($"Kunde:      {posOrdre.Kundenavn}");
        Console.WriteLine($"E-post:     {posOrdre.KundeEpost}");

        // Vis alle fraktlinjer
        Console.WriteLine("\nFraktlinjer:");
        for (int i = 0; i < posOrdre.FraktLinjer.Count; i++)
        {
            var linje = posOrdre.FraktLinjer[i];
            // Ternary operator: condition ? verdi_hvis_true : verdi_hvis_false
            // Tilsvarer linje.pris === 0 ? "gratis" : `kr ${linje.pris},-` i JS
            string prisVisning = linje.Pris == 0 ? "gratis" : $"kr {linje.Pris},-";
            Console.WriteLine($"  {i + 1}. {linje.Metode} ({prisVisning})");
        }
        Console.WriteLine($"  Total frakt: kr {posOrdre.TotalFrakt},-");

        // Vis rabatter hvis de finnes
        if (posOrdre.Rabatter.Count > 0)
        {
            Console.WriteLine("\nRabatter:");
            foreach (var rabatt in posOrdre.Rabatter)
                Console.WriteLine($"  - Kode: {rabatt.Kode} → -kr {rabatt.Belop},- ({rabatt.Type})");
            Console.WriteLine($"  Total rabatt: -kr {posOrdre.TotalRabatt},-");
        }

        Console.WriteLine($"\nTOTAL:      kr {posOrdre.TotalBelop},-");

        Console.WriteLine("\nProduktlinjer:");
        for (int i = 0; i < posOrdre.Produkter.Count; i++)
        {
            var p = posOrdre.Produkter[i];
            Console.WriteLine(
                $"  {i + 1}. {p.Produktnavn} [{p.Sku}]" +
                $" – {p.Antall} stk x kr {p.Enhetspris},- = kr {p.Linjetotal},-"
            );
        }
    }

    // Tilsvarer "function sendTilPos(posOrdre)" i JS
    // bool = boolsk verdi (true/false), tilsvarer boolean i JS
    public bool SendTilPos(PosOrder posOrdre)
    {
        Console.WriteLine("\n=== SENDER TIL POS-SYSTEM ===");
        Console.WriteLine("Bygger SOAP XML...");

        string soapXml = _xmlBuilder.ByggSoapXml(posOrdre);

        Console.WriteLine("Kobler til POS-endepunkt: http://pos-system.intern/soap/ordre");
        Console.WriteLine("Sender melding...\n");

        // I virkeligheten: await httpClient.PostAsync(url, new StringContent(soapXml))
        Console.WriteLine("=== SOAP XML (dette ville blitt sendt) ===");
        Console.WriteLine(soapXml);

        Console.WriteLine("\n✓ POS-systemet svarte: 200 OK – Ordre mottatt og registrert");

        return true;
    }
}
