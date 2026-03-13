// ============================================================
// FIL: Models/ShopifyModels.cs
// FORMÅL: Datamodeller for det VI MOTTAR fra Shopify.
//         Tilsvarer strukturen i shopifyOrder.js.
//
// HVA ER EN "RECORD" I C#?
//  En record er en klasse optimalisert for å holde data.
//  Den er perfekt for dette – vi vil ikke endre dataene,
//  bare lese dem. Records er immutable (uforanderlige) som standard.
//
//  JavaScript:    const order = { id: 1001, ... }
//  C#:            var order = new ShopifyOrder(Id: 1001, ...);
//
// SAMMENLIGNING MED JAVASCRIPT:
//  JS:  customer.first_name   → C#: Customer.FirstName
//  JS:  line_items            → C#: LineItems
//  Samme data, men C# bruker PascalCase og typede klasser.
// ============================================================

namespace ShopifyPosIntegrasjon.Models;

// Tilsvarer hele shopifyOrder-objektet i JS
public record ShopifyOrder(
    int Id,
    string OrderNumber,
    ShopifyCustomer Customer,
    List<ShopifyLineItem> LineItems,
    List<ShopifyDiscountCode> DiscountCodes,
    List<ShopifyShippingLine> ShippingLines,
    decimal TotalPrice          // decimal brukes for penger i C# (ikke double/float)
);

// Tilsvarer customer-objektet i JS
public record ShopifyCustomer(
    string FirstName,
    string LastName,
    string Email
);

// Tilsvarer hvert element i line_items-arrayet
public record ShopifyLineItem(
    string Title,
    string Sku,
    int Quantity,
    decimal Price
);

// Tilsvarer hvert element i discount_codes-arrayet
public record ShopifyDiscountCode(
    string Code,
    decimal Amount,
    string Type     // "fixed_amount" eller "percentage"
);

// Tilsvarer hvert element i shipping_lines-arrayet
public record ShopifyShippingLine(
    string Title,
    decimal Price
);
