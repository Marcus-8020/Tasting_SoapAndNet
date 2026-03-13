// ============================================================
// FIL: Database/DbForbindelse.cs
// FORMÅL: Samler alt som har med database-tilkobling å gjøre
//         på ett sted. Resten av koden slipper å kjenne til
//         connection string-detaljene.
//
// CONNECTION STRING – FORKLARING AV DELENE:
//
//   Server=localhost
//     → SQL Server kjører på samme maskin (lokal instans)
//     → Alternativer: ".\SQLEXPRESS", "localhost\SQLEXPRESS", "PCnavn"
//
//   Database=PCKasseV3
//     → Hvilken database vi kobler til på serveren
//
//   Trusted_Connection=True
//     → Bruk Windows Authentication (ditt Windows-innlogging)
//     → Alternativ: User Id=sa;Password=... for SQL-innlogging
//
//   TrustServerCertificate=True
//     → Godta selv-signert sertifikat (nødvendig for lokal utvikling)
//     → I produksjon: sett opp et skikkelig sertifikat i stedet
// ============================================================

using Microsoft.Data.SqlClient;

namespace PCKasseDbTest.Database;

public static class DbForbindelse
{
    // Connection string samlet på ett sted.
    // Når du skal bytte til en annen server/database, endrer du kun her.
    public const string ConnectionString =
        "Server=localhost;" +
        "Database=PCKasseV3;" +
        "Trusted_Connection=True;" +
        "TrustServerCertificate=True;";

    /// <summary>
    /// Oppretter og returnerer en åpen SQL-tilkobling.
    /// Kallet husk å dispose (lukke) forbindelsen etterpå – bruk "using".
    /// </summary>
    public static SqlConnection ÅpneForbindelse()
    {
        // SqlConnection er klassen for én enkelt database-tilkobling
        var forbindelse = new SqlConnection(ConnectionString);

        // Open() etablerer den faktiske TCP-tilkoblingen til SQL Server
        // Kaster SqlException hvis noe er galt (feil server, tilgang nektet, osv.)
        forbindelse.Open();

        return forbindelse;
    }

    /// <summary>
    /// Tester at tilkoblingen virker uten å kjøre noen query.
    /// Returnerer true hvis OK, false hvis noe feiler.
    /// </summary>
    public static bool TestTilkobling()
    {
        try
        {
            using var forbindelse = ÅpneForbindelse();
            // State.Open betyr at tilkoblingen er aktiv
            return forbindelse.State == System.Data.ConnectionState.Open;
        }
        catch
        {
            return false;
        }
    }
}
