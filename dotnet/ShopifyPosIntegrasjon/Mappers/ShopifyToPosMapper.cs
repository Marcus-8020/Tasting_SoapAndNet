// ============================================================
// FIL: Mappers/ShopifyToPosMapper.cs
// FORMÅL: Oversetter en Shopify-ordre til POS-format.
//         Tilsvarer shopifyToPos.js i Node.js-versjonen.
//
// SAMMENLIGNING MED JAVASCRIPT:
//  JS:  order.line_items.map(item => ({ ... }))
//  C#:  order.LineItems.Select(item => new PosProdukt(...)).ToList()
//
//  JS:  lineItems.reduce((sum, item) => sum + item.price, 0)
//  C#:  lineItems.Sum(item => item.Price)
//
//  LINQ (Language Integrated Query) er C# sin versjon av
//  array-metodene .map(), .filter(), .reduce() fra JavaScript.
// ============================================================

namespace ShopifyPosIntegrasjon.Mappers;

public class ShopifyToPosMapper
{
    // Tilsvarer "function mapShopifyToPos(order)" i JS
    public PosOrder Map(ShopifyOrder order)
    {
        // Hent alle fraktlinjer (med fallback til tom liste hvis null)
        var fraktLinjer = order.ShippingLines ?? [];

        // .Sum() tilsvarer .reduce((sum, l) => sum + l.price, 0) i JS
        decimal totalFrakt = fraktLinjer.Sum(l => l.Price);

        // Hent alle rabattkoder
        var rabatter = order.DiscountCodes ?? [];
        decimal totalRabatt = rabatter.Sum(r => r.Amount);

        decimal totalBelop = BeregnTotal(order.LineItems, totalFrakt, totalRabatt);

        // Bygg og returner PosOrder
        // new PosOrder(...) tilsvarer return { ordreId: ..., ... } i JS
        return new PosOrder(
            OrdreId:    order.Id,

            // String interpolation i C#: $"{a} {b}"
            // Tilsvarer template literals i JS: `${a} ${b}`
            Kundenavn:  $"{order.Customer.FirstName} {order.Customer.LastName}",

            KundeEpost: order.Customer.Email,

            // .Select() tilsvarer .map() i JS
            // .ToList() er nødvendig fordi Select() returnerer IEnumerable, ikke List
            Produkter: order.LineItems.Select(item => new PosProdukt(
                Produktnavn: item.Title,
                Sku:         item.Sku,
                Antall:      item.Quantity,
                Enhetspris:  item.Price,
                Linjetotal:  item.Quantity * item.Price
            )).ToList(),

            FraktLinjer: fraktLinjer.Select(l => new PosFraktlinje(
                Metode: l.Title,
                Pris:   l.Price
            )).ToList(),

            TotalFrakt: totalFrakt,

            Rabatter: rabatter.Select(r => new PosRabatt(
                Kode:  r.Code,
                Belop: r.Amount,
                Type:  r.Type
            )).ToList(),

            TotalRabatt: totalRabatt,
            TotalBelop:  totalBelop
        );
    }

    // Tilsvarer "function beregnTotal(lineItems, fraktTotal, rabattTotal)" i JS
    // "private static" = bare denne klassen kan bruke den, og vi trenger ingen instans
    private static decimal BeregnTotal(
        List<ShopifyLineItem> lineItems,
        decimal fraktTotal,
        decimal rabattTotal)
    {
        decimal varerTotal = lineItems.Sum(item => item.Quantity * item.Price);
        return varerTotal + fraktTotal - rabattTotal;
    }
}
