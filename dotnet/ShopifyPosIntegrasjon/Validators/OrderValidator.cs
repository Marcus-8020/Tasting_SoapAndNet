// ============================================================
// FIL: Validators/OrderValidator.cs
// FORMÅL: Validerer at en Shopify-ordre har alt vi trenger
//         FØR vi prøver å mappe eller sende den videre.
//         Tilsvarer orderValidator.js i Node.js-versjonen.
//
// SAMMENLIGNING MED JAVASCRIPT:
//  JS:  throw new Error("melding")
//  C#:  throw new InvalidOperationException("melding")
//
//  JS:  order.customer?.email
//  C#:  order.Customer?.Email   (?.  finnes i C# også – samme konsept!)
//
//  JS:  typeof x !== "number" || x <= 0
//  C#:  x <= 0                  (C# er typet – vi trenger ikke sjekke type)
// ============================================================

namespace ShopifyPosIntegrasjon.Validators;

public class OrderValidator
{
    // Tilsvarer "function validateOrder(order)" i JS
    // "void" betyr at metoden ikke returnerer noe (samme som JS-funksjoner uten return)
    public void Validate(ShopifyOrder order)
    {
        // Sjekk 1: Ordren må ha en gyldig ID
        // I C# kan ikke records være null hvis Nullable er satt opp riktig,
        // men vi sjekker Id-verdien eksplisitt
        if (order.Id <= 0)
            throw new InvalidOperationException("Ordre mangler eller har ingen ID");

        // Sjekk 2: Kunden må finnes og ha e-post
        // string.IsNullOrEmpty() = sjekker om streng er null eller ""
        // Tilsvarer !order.customer?.email i JS
        if (string.IsNullOrEmpty(order.Customer?.Email))
            throw new InvalidOperationException("Kunde-e-post mangler – kan ikke behandle ordren");

        // Sjekk 3: Det må finnes minst ett produkt
        if (order.LineItems == null || order.LineItems.Count == 0)
            throw new InvalidOperationException("Ordren inneholder ingen produkter (LineItems er tom)");

        // Sjekk 4: Totalprisen må være positiv
        if (order.TotalPrice <= 0)
            throw new InvalidOperationException("Totalprisen er ugyldig – må være et positivt tall");

        // Sjekk 5: Hvert produkt må ha SKU og gyldig pris
        // Tilsvarer order.line_items.forEach((item, index) => { ... }) i JS
        for (int i = 0; i < order.LineItems.Count; i++)
        {
            var item = order.LineItems[i];

            if (string.IsNullOrEmpty(item.Sku))
                throw new InvalidOperationException($"Produkt nr. {i + 1} mangler SKU");

            if (item.Price < 0)
                throw new InvalidOperationException($"Produkt nr. {i + 1} har ugyldig pris");
        }

        // Sjekk 6: Valider rabattkoder hvis de finnes
        // "?? []" tilsvarer "?? []" i JS – hvis null, bruk tom liste
        var rabatter = order.DiscountCodes ?? [];
        for (int i = 0; i < rabatter.Count; i++)
        {
            var rabatt = rabatter[i];

            if (string.IsNullOrEmpty(rabatt.Code))
                throw new InvalidOperationException($"Rabatt nr. {i + 1} mangler kode");

            if (rabatt.Amount < 0)
                throw new InvalidOperationException($"Rabatt \"{rabatt.Code}\" har ugyldig beløp");
        }

        // Hvis vi kommer hit uten feil: ordren er gyldig!
        Console.WriteLine("✓ Validering OK – ordren er gyldig");
    }
}
