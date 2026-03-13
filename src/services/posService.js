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
 *
 * NYT I DEL 2:
 *  - skrivUtOrdre() viser nå alle fraktlinjer og rabatter
 * ============================================================
 */

const { byggSoapXml } = require("../utils/xmlBuilder");

/**
 * skrivUtOrdre
 * ------------
 * Skriver POS-ordren ut i et lesbart format i konsollen.
 * Oppdatert i del 2 til å vise fraktLinjer og rabatter.
 *
 * @param {object} posOrdre - Mappet POS-ordre
 */
function skrivUtOrdre(posOrdre) {
  console.log("\n=== POS-ORDRE (lesbart format) ===");
  console.log(`Ordre-ID:   ${posOrdre.ordreId}`);
  console.log(`Kunde:      ${posOrdre.kundenavn}`);
  console.log(`E-post:     ${posOrdre.kundeEpost}`);

  // DEL 2: Vis alle fraktlinjer (ikke bare én)
  console.log("\nFraktlinjer:");
  posOrdre.fraktLinjer.forEach((linje, index) => {
    const prisVisning = linje.pris === 0 ? "gratis" : `kr ${linje.pris},-`;
    console.log(`  ${index + 1}. ${linje.metode} (${prisVisning})`);
  });
  console.log(`  Total frakt: kr ${posOrdre.totalFrakt},-`);

  // DEL 2: Vis rabatter hvis de finnes
  if (posOrdre.rabatter.length > 0) {
    console.log("\nRabatter:");
    posOrdre.rabatter.forEach((rabatt) => {
      console.log(`  - Kode: ${rabatt.kode} → -kr ${rabatt.belop},- (${rabatt.type})`);
    });
    console.log(`  Total rabatt: -kr ${posOrdre.totalRabatt},-`);
  }

  console.log(`\nTOTAL:      kr ${posOrdre.totalBelop},-`);

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
 * @param {object} posOrdre - Mappet POS-ordre
 * @returns {boolean} - true hvis sending "lykkes"
 */
function sendTilPos(posOrdre) {
  console.log("\n=== SENDER TIL POS-SYSTEM ===");
  console.log("Bygger SOAP XML...");

  const soapXml = byggSoapXml(posOrdre);

  console.log("Kobler til POS-endepunkt: http://pos-system.intern/soap/ordre");
  console.log("Sender melding...\n");

  console.log("=== SOAP XML (dette ville blitt sendt) ===");
  console.log(soapXml);

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
 *  - skrivUtOrdre(): printer ordren lesbart inkl. alle fraktlinjer og rabatter
 *  - sendTilPos(): simulerer sending av SOAP-melding til PC-kassen
 *  - I en ekte integrasjon ville sendTilPos() gjort HTTP POST til SOAP-URL
 * ============================================================
 */
