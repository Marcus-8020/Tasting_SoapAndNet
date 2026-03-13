// ============================================================
// FIL: Data/ShopifyOrders.cs
// FORMÅL: Simulert Shopify-ordre for testing.
//         Tilsvarer shopifyOrder.js i Node.js-versjonen.
//
// I VIRKELIGHETEN ville denne dataen kommet fra:
//  - En Shopify webhook (HTTP POST til et endepunkt vi lytter på)
//  - Eller et Shopify API-kall via HttpClient
//
// HVA ER EN STATISK KLASSE?
//  static class = en klasse du ikke kan lage instanser av.
//  Den fungerer som en samling av verktøy eller data.
//  Tilsvarer module.exports = { shopifyOrder } i Node.js.
// ============================================================

namespace ShopifyPosIntegrasjon.Data;

public static class ShopifyOrders
{
    // Tilsvarer "const shopifyOrder = { ... }" i shopifyOrder.js
    // "=>" er expression body – kort form for en metode med én retur-verdi
    public static ShopifyOrder GetSampleOrder() => new(
        Id: 1001,
        OrderNumber: "#1001",

        // C# bruker named arguments (Id: ..., FirstName: ...) –
        // tilsvarer { id: 1001, first_name: "Marcus" } i JS
        Customer: new ShopifyCustomer(
            FirstName: "Marcus",
            LastName:  "Børresen",
            Email:     "marcus@example.com"
        ),

        // "[ ... ]" er collection expression – ny kortform i C# 12
        // Tilsvarer [ { title: "...", ... }, ... ] i JS
        LineItems:
        [
            new ShopifyLineItem("Aurnor Premium Plan",  "AURNOR-001",    1, 299),
            new ShopifyLineItem("Market Insight Addon", "AURNOR-ADD-01", 2, 99),
        ],

        DiscountCodes:
        [
            new ShopifyDiscountCode("VAAR25", 25, "fixed_amount"),
        ],

        ShippingLines:
        [
            new ShopifyShippingLine("Standard frakt",    49),
            new ShopifyShippingLine("Digital levering",   0),
        ],

        // Totalberegning (samme som i JS-versjonen):
        //  Varer:  1×299 + 2×99 = 497
        //  Frakt:  49 + 0       =  49
        //  Rabatt: -25          = -25
        //  TOTAL:               = 521
        TotalPrice: 521
    );
}
