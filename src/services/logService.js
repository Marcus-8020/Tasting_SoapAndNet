/**
 * ============================================================
 * FIL: src/services/logService.js
 * FORMÅL: Skriver hendelser (events) til en loggfil underveis
 *         i integrasjonsflyten, slik at vi kan spore hva som
 *         har skjedd – også etter at programmet er ferdig.
 *
 * HVORFOR LOGGE TIL FIL?
 *  Konsollen forsvinner når programmet lukkes.
 *  En loggfil består – du kan gå tilbake og se hva som skjedde
 *  kl. 03:47 i natt, selv om ingen var tilstede da.
 *  I produksjon er logging uunnværlig for å feilsøke og overvåke.
 *
 * LOGG-NIVÅER (fra minst til mest alvorlig):
 *  INFO     – Alt gikk bra. Vanlig fremdrift.
 *  ADVARSEL – Noe uventet skjedde, men vi fortsatte.
 *  FEIL     – Noe gikk galt. Integrasjonen stoppet.
 *
 * BRUKTE NODE.JS MODULER (ingen npm nødvendig):
 *  - fs   (file system): lese og skrive filer
 *  - path:              lage filstier som fungerer på alle OS
 * ============================================================
 */

const fs   = require("fs");    // innebygd Node.js-modul for filoperasjoner
const path = require("path");  // innebygd Node.js-modul for filstier

// ---------------------------------------------------------------
// KONFIGURASJON: Hvor skal loggfilen lagres?
// __dirname = mappen der DENNE filen ligger (src/services/)
// path.join() bygger en sti som fungerer på Windows, Mac og Linux
// Resultat: <prosjektmappe>/logs/integration.log
// ---------------------------------------------------------------
const LOG_FIL = path.join(__dirname, "../../logs/integration.log");

/**
 * loggHendelse (intern hjelpefunksjon)
 * ------------------------------------
 * Skriver én loglinje til filen.
 * Kalles ikke direkte – bruk logg.info(), logg.advarsel() etc. i stedet.
 *
 * Format på hver linje:
 *   [2026-03-13T14:32:01.000Z] [INFO] Melding her
 *
 * @param {string} nivå    - "INFO", "ADVARSEL" eller "FEIL"
 * @param {string} melding - Teksten som skal logges
 */
function loggHendelse(nivå, melding) {
  // toISOString() gir oss et standardisert tidsstempel: "2026-03-13T14:32:01.000Z"
  // Dette formatet er internasjonalt og kan sorteres alfabetisk
  const tidsstempel = new Date().toISOString();

  // Bygg én komplett loglinje inkl. linjeskift på slutten
  const linje = `[${tidsstempel}] [${nivå.padEnd(8)}] ${melding}\n`;
  // padEnd(8) gir alle nivåer samme bredde: "INFO    ", "ADVARSEL", "FEIL    "
  // Dette gjør loggen lettere å lese (kolonner er på linje)

  // Opprett logs/-mappen hvis den ikke finnes ennå
  // recursive: true betyr at den ikke krasjer hvis mappen allerede eksisterer
  const logsMappe = path.dirname(LOG_FIL);
  if (!fs.existsSync(logsMappe)) {
    fs.mkdirSync(logsMappe, { recursive: true });
  }

  // appendFileSync = legg til på slutten av filen (ikke overskriv)
  // Sync-versjonen blokkerer til skriving er ferdig – trygt for enkel logging
  fs.appendFileSync(LOG_FIL, linje, "utf8");

  // Skriv også til konsollen så vi ser det i terminalen
  console.log(`  [LOG] ${linje.trim()}`);
}

// ---------------------------------------------------------------
// OFFENTLIG API: Tre snarveifunksjoner – én per nivå
// Eksempel på bruk:
//   const { logg } = require("./logService");
//   logg.info("Ordre mottatt");
//   logg.advarsel("Fraktkode ukjent, bruker fallback");
//   logg.feil("Validering feilet: mangler SKU");
// ---------------------------------------------------------------
const logg = {
  info:     (melding) => loggHendelse("INFO",     melding),
  advarsel: (melding) => loggHendelse("ADVARSEL", melding),
  feil:     (melding) => loggHendelse("FEIL",     melding),
};

// ---------------------------------------------------------------
// EKSPORT
// ---------------------------------------------------------------
module.exports = { logg };

/**
 * ============================================================
 * OPPSUMMERING:
 *  - logg.info(), logg.advarsel(), logg.feil() er de tre nivåene
 *  - Skriver til logs/integration.log og til konsollen
 *  - Bruker fs.appendFileSync – legger til uten å slette gammelt
 *  - Lager logs/-mappen automatisk hvis den mangler
 *  - I .NET tilsvarer dette ILogger<T> med LogInformation(),
 *    LogWarning() og LogError()
 * ============================================================
 */
