// ============================================================
// FIL: Models/PosModels.cs
// FORMÅL: Datamodeller for det VI SENDER til POS-systemet.
//         Tilsvarer output-objektet fra mapShopifyToPos() i JS.
//
// Disse modellene bruker norske navn – akkurat som i JS-versjonen –
// fordi de representerer POS-systemets eget format.
// ============================================================

namespace ShopifyPosIntegrasjon.Models;

// Tilsvarer posOrdre-objektet som returneres fra mapShopifyToPos()
public record PosOrder(
    int OrdreId,
    string Kundenavn,
    string KundeEpost,
    List<PosProdukt> Produkter,
    List<PosFraktlinje> FraktLinjer,
    decimal TotalFrakt,
    List<PosRabatt> Rabatter,
    decimal TotalRabatt,
    decimal TotalBelop
);

// Tilsvarer hvert element i posOrdre.produkter
public record PosProdukt(
    string Produktnavn,
    string Sku,
    int Antall,
    decimal Enhetspris,
    decimal Linjetotal
);

// Tilsvarer hvert element i posOrdre.fraktLinjer
public record PosFraktlinje(
    string Metode,
    decimal Pris
);

// Tilsvarer hvert element i posOrdre.rabatter
public record PosRabatt(
    string Kode,
    decimal Belop,
    string Type
);
