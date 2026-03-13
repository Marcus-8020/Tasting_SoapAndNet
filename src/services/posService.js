/**
 * ============================================================
 * FIL: src/services/posService.js
 * FORMÅL: Simulerer sending av en ordre til PC-kassen (POS-systemet).
 *
 * HVA ER EN "SERVICE"?
 *  I backend-arkitektur er en "service" en klasse eller modul
 *  som håndterer én type forretningslogikk.
 *  Denne servicen sin jobb er å:
 *   1. Ta imot en ferdig mappet POS-ordre
 *   2. Bygge SOAP XML av den
 *   3. "Sende" den til POS-systemet (her: bare skrive det ut)
 *
 * I VIRKELIGHETEN ville sendTilPos() gjort et HTTP POST-kall
 * til POS-systemets SOAP-endepunkt med XML-en som body.
 * ============================================================
 */

// Importer XML-byggeren fra utils-mappen
const { byggSoapXml } = require("../utils/xmlBuilder");

/**
 * skrivUtOrdre
 * ------------
 * Skriver POS-ordren ut i et lesbart format i konsollen.
 * Dette er det du ville sett i et adminpanel eller en logg.
 *
 * @param {object} posOrdre - Mappet POS-ordre
 */
function skrivUtOrdre(posOrdre) {
  console.log("\n=== POS-ORDRE (lesbart format) ===");
  console.log(`Ordre-ID:   ${posOrdre.ordreId}`);
  console.log(`Kunde:      ${posOrdre.kundenavn}`);
  console.log(`E-post:     ${posOrdre.kundeEpost}`);
  console.log(`Frakt:      ${posOrdre.fraktMetode} (kr ${posOrdre.fraktPris},-)`);
  console.log(`TOTAL:      kr ${posOrdre.totalBelop},-`);

  console.log("\nProduktlinjer:");
  posOrdre.produkter.forEach((produkt, index) => {
    console.log(
      `  ${index + 1}. ${produkt.produktnavn} [${produkt.sku}]` +
        ` – ${produkt.antall} stk x kr ${produkt.enhetspris},- = kr ${produkt.linjetotal},-`
    );
  });
}

/**
 * sendTilPos
 * ----------
 * Simulerer sending av ordren til PC-kassen via SOAP.
 * I en ekte integrasjon ville dette vært et HTTP-kall med XML som body.
 *
 * Flyten i en ekte versjon:
 *  1. Bygg SOAP XML  ← gjør vi her
 *  2. Send HTTP POST til POS-endepunkt  ← ville vært her
 *  3. Les respons fra POS  ← ville tolket XML-respons
 *  4. Returner suksess/feil til kalleren  ← returnerer her
 *
 * @param {object} posOrdre - Mappet POS-ordre
 * @returns {boolean} - true hvis sending "lykkes"
 */
function sendTilPos(posOrdre) {
  console.log("\n=== SENDER TIL POS-SYSTEM ===");
  console.log("Bygger SOAP XML...");

  // Bygg SOAP XML via xmlBuilder-verktøyet
  const soapXml = byggSoapXml(posOrdre);

  console.log("Kobler til POS-endepunkt: http://pos-system.intern/soap/ordre");
  console.log("Sender melding...\n");

  // I virkeligheten: await fetch(url, { method: "POST", body: soapXml })
  // Her: bare skriv ut XML-en som simulasjon
  console.log("=== SOAP XML (dette ville blitt sendt) ===");
  console.log(soapXml);

  // Simuler vellykket respons
  console.log("\n✓ POS-systemet svarte: 200 OK – Ordre mottatt og registrert");

  return true;
}

// ---------------------------------------------------------------
// EKSPORT
// ---------------------------------------------------------------
module.exports = { sendTilPos, skrivUtOrdre };

/**
 * ============================================================
 * OPPSUMMERING:
 *  - skrivUtOrdre(): printer ordren lesbart i konsollen
 *  - sendTilPos(): simulerer sending av SOAP-melding til PC-kassen
 *  - I en ekte integrasjon ville sendTilPos() gjort HTTP POST til SOAP-URL
 *  - Servicelaget holder "send"-logikken separat fra mapping og validering
 * ============================================================
 */
