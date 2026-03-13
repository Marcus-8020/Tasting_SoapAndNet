// ============================================================
// FIL: Data/GarnTestData.cs
// FORMÅL: Realistiske testdata for en norsk garnbutikk.
//         Basert på faktiske produkter fra Drops og Sandnes Garn.
//
// HVORFOR DISSE MERKENE?
//  - Drops (Garnstudio) er norsk og markedsleder i Skandinavia
//  - Sandnes Garn er norsk og svært populær
//  - Begge er vanlige å finne i norske garnbutikker
// ============================================================

using GarnbutikkSimulator.Models;

namespace GarnbutikkSimulator.Data;

public static class GarnTestData
{
    // ---------------------------------------------------------------
    // STØRRELSER (Sizes-tabellen i PCKasse)
    // For garn = vekt per nøste
    // ---------------------------------------------------------------
    public static readonly List<Størrelse> Størrelser =
    [
        new("50G",  "50 gram",  200),   // de fleste nøster er 50g/~200m
        new("100G", "100 gram", 400),   // større nøster
        new("200G", "200 gram", 800),   // jumbo/kone
    ];

    // ---------------------------------------------------------------
    // FARGER (Colors-tabellen i PCKasse)
    // Drops bruker numeriske koder, Sandnes bruker navn
    // ---------------------------------------------------------------
    public static readonly List<Farge> Farger =
    [
        // Nøytrale – selges alltid mye av
        new("01",  "Hvit",           "#FFFFFF"),
        new("02",  "Naturhvit",      "#FDF5E6"),
        new("03",  "Lys grå",        "#D3D3D3"),
        new("04",  "Mellomgrå",      "#808080"),
        new("05",  "Mørk grå",       "#404040"),
        new("06",  "Sort",           "#111111"),

        // Klassiske farger
        new("07",  "Marineblå",      "#1F3A6E"),
        new("08",  "Kongeblå",       "#2C5F9E"),
        new("09",  "Petroleumsblå",  "#1A5F7A"),
        new("10",  "Lyseblå",        "#AED6F1"),
        new("11",  "Støvblå",        "#7FB5C6"),

        // Rødt/rosa-familien
        new("12",  "Rød",            "#C0392B"),
        new("13",  "Burgunder",      "#800020"),
        new("14",  "Rosa",           "#FFB6C1"),
        new("15",  "Gammelrosa",     "#C9A0A0"),
        new("16",  "Korall",         "#FF6B6B"),

        // Grønt
        new("17",  "Skogsgrønn",     "#2E7D32"),
        new("18",  "Mintgrønn",      "#98FB98"),
        new("19",  "Olivengrønn",    "#808000"),

        // Gult/oransje/brunt
        new("20",  "Gul",            "#FFD700"),
        new("21",  "Sennepsgul",     "#D4AC0D"),
        new("22",  "Oransje",        "#FF8C00"),
        new("23",  "Rust",           "#B7410E"),
        new("24",  "Kamel",          "#C19A6B"),
        new("25",  "Brun",           "#795548"),

        // Lilla/fiolett
        new("26",  "Lilla",          "#9B59B6"),
        new("27",  "Lavendel",       "#B39DDB"),
        new("28",  "Plomme",         "#6C3483"),
    ];

    // ---------------------------------------------------------------
    // PRODUKTER (Articles-tabellen i PCKasse)
    // ---------------------------------------------------------------
    public static readonly List<GarnProdukt> Produkter =
    [
        new(
            ArtikelNr:        "DROPS-ALA",
            Navn:             "Drops Alaska",
            Merkevare:        "Drops",
            Pris:             42.00m,
            PrisInklMva:      52.50m,
            GarnType:         "Strikkegarn",
            FiberInnhold:     "100% ull",
            AnbefaltPinneStr: 5,
            Beskrivelse:      "Robust og populært ullgarn. Passer til ytterplagg, luer og votter."
        ),
        new(
            ArtikelNr:        "DROPS-KAR",
            Navn:             "Drops Karisma",
            Merkevare:        "Drops",
            Pris:             38.00m,
            PrisInklMva:      47.50m,
            GarnType:         "Strikkegarn",
            FiberInnhold:     "100% superwash ull",
            AnbefaltPinneStr: 4,
            Beskrivelse:      "Maskinvaskbart ullgarn. Perfekt til barneklær."
        ),
        new(
            ArtikelNr:        "DROPS-SKY",
            Navn:             "Drops Sky",
            Merkevare:        "Drops",
            Pris:             52.00m,
            PrisInklMva:      65.00m,
            GarnType:         "Strikkegarn",
            FiberInnhold:     "75% alpakka 25% polyamid",
            AnbefaltPinneStr: 4,
            Beskrivelse:      "Mykt og lett alpakkagarn med drape. Perfekt til sjal og gensere."
        ),
        new(
            ArtikelNr:        "SG-SMART",
            Navn:             "Sandnes Garn Smart",
            Merkevare:        "Sandnes Garn",
            Pris:             55.00m,
            PrisInklMva:      68.75m,
            GarnType:         "Strikkegarn",
            FiberInnhold:     "100% superwash ull",
            AnbefaltPinneStr: 3,
            Beskrivelse:      "Klassisk strikkegarn fra Sandnes. Maskinvaskbart og svært holdbart."
        ),
        new(
            ArtikelNr:        "SG-TYNN-SILK",
            Navn:             "Sandnes Garn Tynn Silk Mohair",
            Merkevare:        "Sandnes Garn",
            Pris:             98.00m,
            PrisInklMva:      122.50m,
            GarnType:         "Strikkegarn",
            FiberInnhold:     "57% kid mohair 43% silke",
            AnbefaltPinneStr: 3,
            Beskrivelse:      "Luksuriøst mohair/silke-garn med halo-effekt. Strikkes gjerne dobbelt."
        ),
    ];

    // ---------------------------------------------------------------
    // VARIANTER (SizeColors-tabellen i PCKasse)
    // Én rad per gyldig kombinasjon av produkt + farge + størrelse
    //
    // ★ DYRELOT/FARGENUMMER:
    //   Drops bruker 4-sifrede tall: "0100" = lot 1, "0200" = lot 2
    //   Viktig for kunder som strikker store prosjekter – de trenger
    //   nok garn fra SAMME lot for å unngå fargeforskjeller.
    // ---------------------------------------------------------------
    public static readonly List<GarnVariant> Varianter =
    [
        // --- Drops Alaska 50g ---
        // Nøytrale (stor etterspørsel, godt lager)
        new("DROPS-ALA", "01", "50G", "5701700000011", Lager: 24, Fargenummer: "0100"),
        new("DROPS-ALA", "02", "50G", "5701700000028", Lager: 18, Fargenummer: "0100"),
        new("DROPS-ALA", "03", "50G", "5701700000035", Lager: 12, Fargenummer: "0200"),
        new("DROPS-ALA", "04", "50G", "5701700000042", Lager:  8, Fargenummer: "0200"),
        new("DROPS-ALA", "05", "50G", "5701700000059", Lager:  3, Fargenummer: "0200"), // ← LAVT LAGER
        new("DROPS-ALA", "06", "50G", "5701700000066", Lager: 20, Fargenummer: "0100"),
        new("DROPS-ALA", "07", "50G", "5701700000073", Lager:  0, Fargenummer: "0100"), // ← UTSOLGT
        new("DROPS-ALA", "08", "50G", "5701700000080", Lager:  6, Fargenummer: "0300"),
        new("DROPS-ALA", "12", "50G", "5701700000097", Lager:  4, Fargenummer: "0100"), // ← LAVT
        new("DROPS-ALA", "13", "50G", "5701700000103", Lager:  0, Fargenummer: "0200"), // ← UTSOLGT
        new("DROPS-ALA", "17", "50G", "5701700000110", Lager: 15, Fargenummer: "0100"),
        new("DROPS-ALA", "21", "50G", "5701700000127", Lager:  2, Fargenummer: "0300"), // ← LAVT

        // --- Drops Alaska 100g ---
        new("DROPS-ALA", "01", "100G", "5701700001011", Lager: 10, Fargenummer: "0100"),
        new("DROPS-ALA", "02", "100G", "5701700001028", Lager:  7, Fargenummer: "0100"),
        new("DROPS-ALA", "06", "100G", "5701700001066", Lager:  5, Fargenummer: "0200"),
        new("DROPS-ALA", "07", "100G", "5701700001073", Lager:  0, Fargenummer: "0100"), // ← UTSOLGT

        // --- Drops Karisma 50g ---
        new("DROPS-KAR", "01", "50G", "5701700002011", Lager: 30, Fargenummer: "0100"),
        new("DROPS-KAR", "06", "50G", "5701700002066", Lager: 22, Fargenummer: "0100"),
        new("DROPS-KAR", "07", "50G", "5701700002073", Lager:  8, Fargenummer: "0200"),
        new("DROPS-KAR", "12", "50G", "5701700002012", Lager:  1, Fargenummer: "0100"), // ← LAVT
        new("DROPS-KAR", "15", "50G", "5701700002015", Lager: 14, Fargenummer: "0200"),
        new("DROPS-KAR", "18", "50G", "5701700002018", Lager:  0, Fargenummer: "0300"), // ← UTSOLGT
        new("DROPS-KAR", "24", "50G", "5701700002024", Lager:  9, Fargenummer: "0100"),

        // --- Drops Sky 50g ---
        new("DROPS-SKY", "03", "50G", "5701700003003", Lager:  6, Fargenummer: "0100"),
        new("DROPS-SKY", "10", "50G", "5701700003010", Lager:  4, Fargenummer: "0100"),
        new("DROPS-SKY", "14", "50G", "5701700003014", Lager:  8, Fargenummer: "0200"),
        new("DROPS-SKY", "27", "50G", "5701700003027", Lager:  2, Fargenummer: "0100"), // ← LAVT

        // --- Sandnes Garn Smart 50g ---
        new("SG-SMART", "01", "50G", "7041763200011", Lager: 16, Fargenummer: "A23"),
        new("SG-SMART", "06", "50G", "7041763200066", Lager: 12, Fargenummer: "A23"),
        new("SG-SMART", "07", "50G", "7041763200073", Lager:  0, Fargenummer: "A22"), // ← UTSOLGT
        new("SG-SMART", "12", "50G", "7041763200012", Lager:  5, Fargenummer: "A23"),
        new("SG-SMART", "16", "50G", "7041763200016", Lager:  3, Fargenummer: "A24"), // ← LAVT
        new("SG-SMART", "25", "50G", "7041763200025", Lager: 11, Fargenummer: "A23"),

        // --- Sandnes Garn Tynn Silk Mohair 25g (liten nøste) ---
        new("SG-TYNN-SILK", "03", "50G", "7041763300003", Lager:  5, Fargenummer: "B12"),
        new("SG-TYNN-SILK", "14", "50G", "7041763300014", Lager:  8, Fargenummer: "B12"),
        new("SG-TYNN-SILK", "27", "50G", "7041763300027", Lager:  3, Fargenummer: "B13"), // ← LAVT
        new("SG-TYNN-SILK", "28", "50G", "7041763300028", Lager:  0, Fargenummer: "B12"), // ← UTSOLGT
    ];

    // ---------------------------------------------------------------
    // HJELPEMETODE: Beregn lagernivå (til bruk i lager-scenariet)
    // ---------------------------------------------------------------
    public static LagerNivå BeregnNivå(int antall) => antall switch
    {
        0              => LagerNivå.Utsolgt,
        <= 5           => LagerNivå.Lavt,
        _              => LagerNivå.Ok
    };

    // ---------------------------------------------------------------
    // HJELPEMETODE: Slå opp produktnavn og fargenavn fra koder
    // ---------------------------------------------------------------
    public static string HentProduktNavn(string artikelNr) =>
        Produkter.FirstOrDefault(p => p.ArtikelNr == artikelNr)?.Navn ?? artikelNr;

    public static string HentFargeNavn(string fargeKode) =>
        Farger.FirstOrDefault(f => f.FargeKode == fargeKode)?.FargeNavn ?? fargeKode;

    public static string HentStørrelseBeskrivelse(string kode) =>
        Størrelser.FirstOrDefault(s => s.StørrelsesKode == kode)?.Beskrivelse ?? kode;
}
