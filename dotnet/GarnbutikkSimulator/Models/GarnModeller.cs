// ============================================================
// FIL: Models/GarnModeller.cs
// FORMÅL: Datamodeller som speiler PCKasse-strukturen,
//         tilpasset en garnbutikks behov.
//
// NØKKELKONSEPT – VARIANTER I GARN:
//  I PCKasse håndteres varianter via tre tabeller:
//    Articles   → selve produktet ("Drops Alaska")
//    Colors     → fargeregisteret  ("Hvit", "Sort", "Grå", ...)
//    Sizes      → størrelsesregisteret ("50g", "100g", ...)
//    SizeColors → krysstabell: én rad per gyldig kombinasjon
//                 ArticleNo + ColorCode + SizeCode = én unik variant
//
//  I Shopify er dette:
//    Product    → "Drops Alaska"
//    Variant    → { color: "Hvit", size: "50g", sku: "DROPS-ALA-01-50" }
//
//  UTFORDRINGEN: Vi må mappe mellom disse to systemene.
// ============================================================

namespace GarnbutikkSimulator.Models;

// ---------------------------------------------------------------
// PRODUKT-MODELLER (tilsvarer Articles-tabellen i PCKasse)
// ---------------------------------------------------------------

/// <summary>
/// Et garn-produkt slik det lagres i PCKasse (Articles-tabellen).
/// MERK: Selve produktet har INGEN fargeinformasjon – det ligger i SizeColors.
/// </summary>
public record GarnProdukt(
    string ArtikelNr,           // f.eks. "DROPS-ALA"
    string Navn,                // f.eks. "Drops Alaska"
    string Merkevare,           // f.eks. "Drops"
    decimal Pris,               // pris per enhet (eks. mva)
    decimal PrisInklMva,
    string GarnType,            // "Strikkegarn", "Heklegarn", "Broderi"
    string FiberInnhold,        // "100% ull", "75% ull 25% polyamid"
    int AnbefaltPinneStr,       // mm, f.eks. 5 (for 5mm pinner)
    string? Beskrivelse
);

/// <summary>
/// Én farge i fargeregisteret (Colors-tabellen i PCKasse).
/// Alle garn-merkevarer deler samme fargekatalog, ELLER har egne.
/// </summary>
public record Farge(
    string FargeKode,           // f.eks. "01", "02", "03"
    string FargeNavn,           // f.eks. "Hvit", "Naturhvit", "Lys grå"
    string HexKode              // f.eks. "#FFFFFF" – brukes i Shopify-visning
);

/// <summary>
/// Én størrelse (Sizes-tabellen i PCKasse).
/// For garn er dette typisk vekten per nøste.
/// </summary>
public record Størrelse(
    string StørrelsesKode,      // f.eks. "50G", "100G"
    string Beskrivelse,         // f.eks. "50 gram", "100 gram"
    int MeterPerEnhet           // meter garn per nøste – viktig for kunder
);

/// <summary>
/// ★ KJERNEMODELLEN – én unik variant (SizeColors-tabellen i PCKasse).
///
/// Dette er krysset mellom produkt + farge + størrelse.
/// Hvert nøste med en spesifikk farge OG størrelse er én SizeColor-rad.
///
/// Eksempel for Drops Alaska:
///   Alaska + Hvit    + 50g  → SizeColor #1  (EAN: 5700693xxxxxx)
///   Alaska + Hvit    + 100g → SizeColor #2
///   Alaska + Sort    + 50g  → SizeColor #3
///   Alaska + Sort    + 100g → SizeColor #4
///   ... (40 farger × 2 størrelser = 80 varianter)
/// </summary>
public record GarnVariant(
    string ArtikelNr,           // kobling til GarnProdukt
    string FargeKode,           // kobling til Farge
    string StørrelsesKode,      // kobling til Størrelse
    string? EanKode,            // strekkode – unik per variant
    int LagerAntall,            // antall nøster på lager for DENNE varianten
    string? Fargenummer         // ★ DYRELOT-GRUPPE: produksjonsparti
                                //   Drops bruker tall: "0100", "0200" osv.
                                //   Kunder bør kjøpe samme fargenummer for store prosjekter!
);

// ---------------------------------------------------------------
// ORDRE-MODELLER (tilsvarer Orders + OrderLines i PCKasse)
// ---------------------------------------------------------------

/// <summary>
/// En kundeordre – tilsvarer Orders-tabellen + Shopify-ordre.
/// </summary>
public record GarnOrdre(
    int OrdreId,
    string KundeNavn,
    string KundeEpost,
    DateTime Dato,
    List<GarnOrdrelinje> Linjer
)
{
    // Beregnes automatisk fra linjene – ikke lagret separat
    public decimal TotalBelop => Linjer.Sum(l => l.Linjetotal);
};

/// <summary>
/// Én linje i en ordre. Merk at den refererer til en VARIANT (ikke bare produkt).
/// Det holder ikke å si "2 nøster Alaska" – vi må si "2 nøster Alaska Hvit 50g".
/// </summary>
public record GarnOrdrelinje(
    string ProduktNavn,
    string FargeNavn,
    string Størrelse,
    string? Fargenummer,        // kunden ønsker dette fargenummeret (dyrelot)
    int Antall,
    decimal Enhetspris
)
{
    public decimal Linjetotal => Antall * Enhetspris;
};

// ---------------------------------------------------------------
// LAGER-MODELLER
// ---------------------------------------------------------------

/// <summary>
/// Lagerstatustatus for én variant – brukes for å vise oversikten.
/// </summary>
public record LagerStatus(
    string ProduktNavn,
    string FargeNavn,
    string Størrelse,
    string? Fargenummer,
    int Antall,
    LagerNivå Nivå            // beregnet status basert på antall
);

public enum LagerNivå
{
    Ok,         // > 5 stk
    Lavt,       // 1–5 stk – bør bestilles snart
    Utsolgt     // 0 stk – kan ikke selges
}

// ---------------------------------------------------------------
// SHOPIFY-MAPPING MODELLER
// ---------------------------------------------------------------

/// <summary>
/// Slik Shopify sender oss en ordre med garn-varianter.
/// Én Shopify-variant tilsvarer én PCKasse SizeColor.
/// </summary>
public record ShopifyGarnOrdre(
    string ShopifyOrdreId,
    string KundeEpost,
    List<ShopifyGarnLinje> Linjer
);

public record ShopifyGarnLinje(
    string ShopifyProduktId,    // f.eks. "gid://shopify/Product/123456"
    string VariantId,           // f.eks. "gid://shopify/ProductVariant/789012"
    string ProduktTittel,       // f.eks. "Drops Alaska"
    string FargeOption,         // f.eks. "Hvit"
    string StørrelsesOption,    // f.eks. "50g"
    string Sku,                 // f.eks. "DROPS-ALA-01-50G"
    int Antall,
    decimal Pris
);
