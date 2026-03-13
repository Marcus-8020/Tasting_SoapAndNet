// ============================================================
// FIL: Utils/XmlBuilder.cs
// FORMÅL: Bygger SOAP XML-melding fra en POS-ordre.
//         Tilsvarer xmlBuilder.js i Node.js-versjonen.
//
// SAMMENLIGNING MED JAVASCRIPT:
//  JS:  posOrdre.produkter.map(byggProduktXml).join("")
//  C#:  string.Concat(posOrdre.Produkter.Select(ByggProduktXml))
//
//  JS:  Template literals (backticks): `<Tag>${verdi}</Tag>`
//  C#:  String interpolation: $"<Tag>{verdi}</Tag>"
//
// I .NET PRODUKSJON ville vi brukt:
//  - System.Xml.Linq (XDocument/XElement) for strukturert XML-bygging
//  - Eller auto-genererte klasser fra en WSDL-fil via "Add Service Reference"
//  Her bruker vi string-bygging for å speile JS-versjonen direkte.
// ============================================================

namespace ShopifyPosIntegrasjon.Utils;

public class XmlBuilder
{
    // Tilsvarer "function byggSoapXml(posOrdre)" i JS
    public string ByggSoapXml(PosOrder posOrdre)
    {
        // string.Concat() tilsvarer .map(...).join("") i JS
        string produkterXml   = string.Concat(posOrdre.Produkter.Select(ByggProduktXml));
        string fraktLinjerXml = string.Concat(posOrdre.FraktLinjer.Select(ByggFraktXml));

        // Bygg rabatt-XML kun hvis det finnes rabatter
        string rabatterXml = posOrdre.Rabatter.Count > 0
            ? "<Rabatter>" + string.Concat(posOrdre.Rabatter.Select(ByggRabattXml)) + "\n            </Rabatter>"
            : "";

        // @"..." er en verbatim string – linjeskift og innrykk bevares
        // Tilsvarer template literals (backticks) i JS
        return
            $"<?xml version=\"1.0\" encoding=\"UTF-8\"?>\n" +
            $"<soapenv:Envelope xmlns:soapenv=\"http://schemas.xmlsoap.org/soap/envelope/\">\n" +
            $"    <soapenv:Body>\n" +
            $"        <OrdreRequest>\n" +
            $"            <OrdreId>{posOrdre.OrdreId}</OrdreId>\n" +
            $"            <Kundenavn>{posOrdre.Kundenavn}</Kundenavn>\n" +
            $"            <KundeEpost>{posOrdre.KundeEpost}</KundeEpost>\n" +
            $"            <Fraktlinjer>{fraktLinjerXml}\n" +
            $"            </Fraktlinjer>\n" +
            $"            <TotalFrakt>{posOrdre.TotalFrakt}</TotalFrakt>\n" +
            $"            {rabatterXml}\n" +
            $"            <TotalRabatt>{posOrdre.TotalRabatt}</TotalRabatt>\n" +
            $"            <TotalBelop>{posOrdre.TotalBelop}</TotalBelop>\n" +
            $"            <Produkter>{produkterXml}\n" +
            $"            </Produkter>\n" +
            $"        </OrdreRequest>\n" +
            $"    </soapenv:Body>\n" +
            $"</soapenv:Envelope>";
    }

    // "private static" = intern hjelpemetode, ingen instans nødvendig
    // Tilsvarer "function byggProduktXml(produkt)" i JS
    private static string ByggProduktXml(PosProdukt produkt) =>
        $"\n        <Produkt>" +
        $"\n            <Produktnavn>{produkt.Produktnavn}</Produktnavn>" +
        $"\n            <SKU>{produkt.Sku}</SKU>" +
        $"\n            <Antall>{produkt.Antall}</Antall>" +
        $"\n            <Enhetspris>{produkt.Enhetspris}</Enhetspris>" +
        $"\n            <Linjetotal>{produkt.Linjetotal}</Linjetotal>" +
        $"\n        </Produkt>";

    private static string ByggFraktXml(PosFraktlinje linje) =>
        $"\n        <Fraktlinje>" +
        $"\n            <Metode>{linje.Metode}</Metode>" +
        $"\n            <Pris>{linje.Pris}</Pris>" +
        $"\n        </Fraktlinje>";

    private static string ByggRabattXml(PosRabatt rabatt) =>
        $"\n        <Rabatt>" +
        $"\n            <Kode>{rabatt.Kode}</Kode>" +
        $"\n            <Belop>{rabatt.Belop}</Belop>" +
        $"\n            <Type>{rabatt.Type}</Type>" +
        $"\n        </Rabatt>";
}
