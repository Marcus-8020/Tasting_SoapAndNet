// ============================================================
// FIL: Services/LogService.cs
// FORMÅL: Skriver hendelser til loggfil og konsoll.
//         Tilsvarer logService.js i Node.js-versjonen.
//
// SAMMENLIGNING MED JAVASCRIPT:
//  JS:  fs.appendFileSync(fil, linje, "utf8")
//  C#:  File.AppendAllText(fil, linje)
//
//  JS:  fs.mkdirSync(mappe, { recursive: true })
//  C#:  Directory.CreateDirectory(mappe)   (lager rekursivt som standard)
//
//  JS:  new Date().toISOString()
//  C#:  DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffZ")
//
// I PRODUKSJON I .NET ville vi brukt ILogger<T> fra
// Microsoft.Extensions.Logging, injisert via dependency injection.
// Denne klassen er pedagogisk – den speiler JS-versjonen direkte.
// ============================================================

namespace ShopifyPosIntegrasjon.Services;

public class LogService
{
    // Stien til loggfilen.
    // Path.Combine() tilsvarer path.join() i Node.js.
    // "logs/integration.log" er relativt til der programmet kjøres fra
    // (prosjektmappen når du bruker "dotnet run").
    private readonly string _logFil = Path.Combine("logs", "integration.log");

    // Snarveimetoder – tilsvarer logg.info(), logg.advarsel(), logg.feil() i JS
    public void Info(string melding)     => LogHendelse("INFO",     melding);
    public void Advarsel(string melding) => LogHendelse("ADVARSEL", melding);
    public void Feil(string melding)     => LogHendelse("FEIL",     melding);

    // Tilsvarer "function loggHendelse(nivå, melding)" i JS
    // "private" = bare denne klassen kan kalle metoden
    private void LogHendelse(string nivå, string melding)
    {
        // Tidsstempel i ISO 8601-format – tilsvarer new Date().toISOString() i JS
        string tidsstempel = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffZ");

        // PadRight(8) tilsvarer padEnd(8) i JS – fyller med mellomrom til 8 tegn
        string linje = $"[{tidsstempel}] [{nivå.PadRight(8)}] {melding}";

        // Opprett logs/-mappen hvis den ikke finnes
        // Tilsvarer fs.mkdirSync(logsMappe, { recursive: true }) i JS
        string? logsMappe = Path.GetDirectoryName(_logFil);
        if (logsMappe != null && !Directory.Exists(logsMappe))
            Directory.CreateDirectory(logsMappe);

        // Legg til på slutten av filen (ikke overskriv)
        // Tilsvarer fs.appendFileSync() i JS
        // Environment.NewLine = "\r\n" på Windows, "\n" på Linux/Mac
        File.AppendAllText(_logFil, linje + Environment.NewLine);

        // Skriv også til konsollen
        Console.WriteLine($"  [LOG] {linje}");
    }
}
