// ============================================================
// FIL: Program.cs
// FORMÅL: Tester tilkobling til PCKasseV3-databasen og
//         utforsker hva som finnes i den.
//
// KJØRE:
//   cd dotnet/PCKasseDbTest
//   dotnet run
// ============================================================

using Microsoft.Data.SqlClient;
using PCKasseDbTest.Database;

Console.OutputEncoding = System.Text.Encoding.UTF8;

Console.WriteLine("============================================================");
Console.WriteLine("  PCKASSE DB – TILKOBLINGSTEST");
Console.WriteLine("============================================================");
Console.WriteLine($"  Server:   localhost");
Console.WriteLine($"  Database: PCKasseV3");
Console.WriteLine($"  Auth:     Windows Authentication");
Console.WriteLine("============================================================\n");

// ---------------------------------------------------------------
// TEST 1: Kan vi i det hele tatt koble til?
// ---------------------------------------------------------------
Console.WriteLine("[TEST 1] Tester tilkobling...");

if (!DbForbindelse.TestTilkobling())
{
    Console.WriteLine("✗ Tilkobling FEILET.");
    Console.WriteLine("\nVanlige årsaker:");
    Console.WriteLine("  - SQL Server kjører ikke (sjekk Services / SQL Server Configuration Manager)");
    Console.WriteLine("  - Feil servernavn (prøv .\\SQLEXPRESS eller localhost\\SQLEXPRESS)");
    Console.WriteLine("  - Windows-brukeren din har ikke tilgang til databasen");
    Console.WriteLine("  - Databasenavnet 'PCKasseV3' stemmer ikke");
    return; // Ingen vits å fortsette
}

Console.WriteLine("✓ Tilkobling OK!\n");

// ---------------------------------------------------------------
// TEST 2: Vis info om databasen
// ---------------------------------------------------------------
Console.WriteLine("[TEST 2] Henter database-info...");
KjørQuery(
    "Databaseversjon",
    "SELECT @@VERSION AS Versjon, DB_NAME() AS [Database], SYSTEM_USER AS Bruker",
    reader =>
    {
        Console.WriteLine($"  Database:  {reader["Database"]}");
        Console.WriteLine($"  Bruker:    {reader["Bruker"]}");
        // Versjonsstrengen er lang – vis bare første linje
        var versjon = reader["Versjon"].ToString()?.Split('\n')[0].Trim();
        Console.WriteLine($"  SQL:       {versjon}");
    }
);

// ---------------------------------------------------------------
// TEST 3: Hvilke tabeller finnes i databasen?
// ---------------------------------------------------------------
Console.WriteLine("\n[TEST 3] Lister alle tabeller i PCKasseV3...");
Console.WriteLine("┌─────────────────────────────────────────┐");

var tabeller = new List<string>();
KjørQuery(
    "Tabelloversikt",
    @"SELECT TABLE_NAME, TABLE_TYPE
      FROM INFORMATION_SCHEMA.TABLES
      WHERE TABLE_TYPE = 'BASE TABLE'
      ORDER BY TABLE_NAME",
    reader =>
    {
        var navn = reader["TABLE_NAME"].ToString()!;
        tabeller.Add(navn);
        Console.WriteLine($"│  {navn,-39}│");
    }
);

Console.WriteLine("└─────────────────────────────────────────┘");
Console.WriteLine($"  Totalt: {tabeller.Count} tabeller\n");

// ---------------------------------------------------------------
// TEST 4: Vis kolonner for en spesifikk tabell
// Endre TABLE_NAME nedenfor til en tabell du fant i test 3
// ---------------------------------------------------------------
string testTabell = tabeller.FirstOrDefault() ?? "";

if (!string.IsNullOrEmpty(testTabell))
{
    Console.WriteLine($"[TEST 4] Kolonner i tabellen '{testTabell}'...");
    KjørQuery(
        "Kolonner",
        @"SELECT COLUMN_NAME, DATA_TYPE, IS_NULLABLE, CHARACTER_MAXIMUM_LENGTH
          FROM INFORMATION_SCHEMA.COLUMNS
          WHERE TABLE_NAME = @TabellNavn
          ORDER BY ORDINAL_POSITION",
        reader =>
        {
            var kolonne  = reader["COLUMN_NAME"].ToString();
            var type     = reader["DATA_TYPE"].ToString();
            var nullable = reader["IS_NULLABLE"].ToString() == "YES" ? "NULL" : "NOT NULL";
            var lengde   = reader["CHARACTER_MAXIMUM_LENGTH"] == DBNull.Value
                           ? "" : $"({reader["CHARACTER_MAXIMUM_LENGTH"]})";
            Console.WriteLine($"  {kolonne,-30} {type}{lengde,-15} {nullable}");
        },
        // Parametre – sikker måte å sende verdier inn i en query
        // Aldri bruk string-concatenation direkte (SQL injection-risiko!)
        cmd => cmd.Parameters.AddWithValue("@TabellNavn", testTabell)
    );

    // ---------------------------------------------------------------
    // TEST 5: Hent de 5 første radene fra tabellen
    // ---------------------------------------------------------------
    Console.WriteLine($"\n[TEST 5] Topp 5 rader fra '{testTabell}'...");
    KjørRåQuery(
        $"SELECT TOP 5 * FROM [{testTabell}]",
        VisRaderSomTabell
    );
}

// ---------------------------------------------------------------
// TEST 6: Articles-tabellen – hva slags produkter finnes?
// ---------------------------------------------------------------
Console.WriteLine("\n[TEST 6] Kolonner i Articles-tabellen...");
VisKolonner("Articles");

Console.WriteLine("\n[TEST 6b] Topp 10 produkter fra Articles...");
KjørRåQuery("SELECT TOP 10 * FROM [Articles]", VisRaderSomTabell);

// ---------------------------------------------------------------
// TEST 7: Colors-tabellen – fargekatalogen
// ---------------------------------------------------------------
Console.WriteLine("\n[TEST 7] Alle farger i Colors-tabellen...");
KjørRåQuery("SELECT TOP 30 * FROM [Colors] ORDER BY 1", VisRaderSomTabell);

// ---------------------------------------------------------------
// TEST 8: Sizes-tabellen – størrelseskatalogen
// ---------------------------------------------------------------
Console.WriteLine("\n[TEST 8] Alle størrelser i Sizes-tabellen...");
KjørRåQuery("SELECT * FROM [Sizes] ORDER BY 1", VisRaderSomTabell);

// ---------------------------------------------------------------
// TEST 9: SizeColors – variant-tabellen (produkt × farge × størrelse)
// Dette er kjernetabellen for variant-håndtering i PCKasse.
// ---------------------------------------------------------------
Console.WriteLine("\n[TEST 9] Kolonner i SizeColors (variant-tabellen)...");
VisKolonner("SizeColors");

Console.WriteLine("\n[TEST 9b] Topp 10 varianter fra SizeColors...");
KjørRåQuery("SELECT TOP 10 * FROM [SizeColors]", VisRaderSomTabell);

// ---------------------------------------------------------------
// TEST 10: EanNos – strekkoder per variant
// Hver variant (SizeColor) bør ha én EAN-kode her.
// ---------------------------------------------------------------
Console.WriteLine("\n[TEST 10] Kolonner i EanNos (strekkoder)...");
VisKolonner("EanNos");

Console.WriteLine("\n[TEST 10b] Topp 10 rader fra EanNos...");
KjørRåQuery("SELECT TOP 10 * FROM [EanNos]", VisRaderSomTabell);

// ---------------------------------------------------------------
// TEST 11: Orders + OrderLines – ordrestrukturen
// Vi trenger å forstå denne for å skrive Shopify-ordrer inn hit.
// ---------------------------------------------------------------
Console.WriteLine("\n[TEST 11] Kolonner i Orders...");
VisKolonner("Orders");

Console.WriteLine("\n[TEST 11b] Kolonner i OrderLines...");
VisKolonner("OrderLines");

Console.WriteLine("\n[TEST 11c] Topp 5 siste ordrer (hvis noen finnes)...");
KjørRåQuery(
    "SELECT TOP 5 * FROM [Orders] ORDER BY 1 DESC",
    VisRaderSomTabell
);

Console.WriteLine("\n============================================================");
Console.WriteLine("  FERDIG – alle tester kjørt");
Console.WriteLine("============================================================");

// ---------------------------------------------------------------
// HJELPEFUNKSJONER
// ---------------------------------------------------------------

/// <summary>
/// Kjører en SQL-query og kaller tilbake for hver rad.
/// Håndterer åpning/lukking av forbindelsen automatisk.
/// </summary>
void KjørQuery(
    string beskrivelse,
    string sql,
    Action<SqlDataReader> perRad,
    Action<SqlCommand>? konfigurerKommando = null)
{
    try
    {
        // "using" sørger for at forbindelsen lukkes automatisk
        // selv om det oppstår en feil (tilsvarer finally { conn.Close() })
        using var forbindelse = DbForbindelse.ÅpneForbindelse();
        using var kommando = new SqlCommand(sql, forbindelse);

        // La kalleren legge til parametre hvis ønskelig
        konfigurerKommando?.Invoke(kommando);

        // ExecuteReader() kjører SELECT og gir oss et "resultatvindu"
        using var reader = kommando.ExecuteReader();

        int antall = 0;
        while (reader.Read()) // Les én rad om gangen
        {
            perRad(reader);
            antall++;
        }

        if (antall == 0)
            Console.WriteLine("  (ingen rader)");
    }
    catch (SqlException ex)
    {
        // SqlException inneholder SQL Server sin egen feilkode og melding
        Console.WriteLine($"  ✗ SQL-feil [{ex.Number}]: {ex.Message}");
    }
}

/// <summary>
/// Kjører en query uten forhåndsdefinert rad-handling.
/// </summary>
void KjørRåQuery(string sql, Action<SqlDataReader> behandle)
{
    try
    {
        using var forbindelse = DbForbindelse.ÅpneForbindelse();
        using var kommando = new SqlCommand(sql, forbindelse);
        using var reader = kommando.ExecuteReader();
        behandle(reader);
    }
    catch (SqlException ex)
    {
        Console.WriteLine($"  ✗ SQL-feil [{ex.Number}]: {ex.Message}");
    }
}

/// <summary>
/// Viser kolonnestruktur for en tabell – kortform for TEST 4-mønsteret.
/// </summary>
void VisKolonner(string tabellNavn)
{
    KjørQuery(
        tabellNavn,
        @"SELECT COLUMN_NAME, DATA_TYPE, IS_NULLABLE, CHARACTER_MAXIMUM_LENGTH
          FROM INFORMATION_SCHEMA.COLUMNS
          WHERE TABLE_NAME = @TabellNavn
          ORDER BY ORDINAL_POSITION",
        reader =>
        {
            var kolonne  = reader["COLUMN_NAME"].ToString();
            var type     = reader["DATA_TYPE"].ToString();
            var nullable = reader["IS_NULLABLE"].ToString() == "YES" ? "NULL" : "NOT NULL";
            var lengde   = reader["CHARACTER_MAXIMUM_LENGTH"] == DBNull.Value
                           ? "" : $"({reader["CHARACTER_MAXIMUM_LENGTH"]})";
            Console.WriteLine($"  {kolonne,-30} {type}{lengde,-15} {nullable}");
        },
        cmd => cmd.Parameters.AddWithValue("@TabellNavn", tabellNavn)
    );
}

/// <summary>
/// Printer alle rader og kolonner som en enkel tabell i konsollen.
/// Nyttig for å utforske ukjente tabeller.
/// </summary>
void VisRaderSomTabell(SqlDataReader reader)
{
    // Hent kolonnenavn fra metadata
    var kolonneNavn = Enumerable.Range(0, reader.FieldCount)
                                .Select(i => reader.GetName(i))
                                .ToList();

    // Skriv kolonneoverskrifter
    Console.WriteLine("  " + string.Join(" | ", kolonneNavn.Select(k => k.PadRight(15))));
    Console.WriteLine("  " + new string('-', kolonneNavn.Count * 18));

    // Skriv rader
    int antall = 0;
    while (reader.Read())
    {
        var verdier = Enumerable.Range(0, reader.FieldCount)
                                .Select(i => reader.IsDBNull(i) ? "NULL" : reader[i].ToString() ?? "")
                                .Select(v => v.Length > 15 ? v[..12] + "..." : v);
        Console.WriteLine("  " + string.Join(" | ", verdier.Select(v => v.PadRight(15))));
        antall++;
    }

    if (antall == 0)
        Console.WriteLine("  (ingen rader)");
}
