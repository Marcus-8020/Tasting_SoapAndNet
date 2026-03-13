/**
 * ============================================================
 * FIL: src/validators/orderValidator.js
 * FORMÅL: Validerer at en Shopify-ordre har alt vi trenger
 *         FØR vi prøver å mappe eller sende den videre.
 *
 * HVORFOR VALIDERING ER VIKTIG:
 *  I ekte integrasjoner feiler data overraskende ofte:
 *  - Kunden mangler e-post
 *  - Ordren har ingen produkter
 *  - Pris er null eller negativ
 *  Uten validering krasjer systemet eller sender feil data til POS.
 * ============================================================
 */

/**
 * validateOrder
 * -------------
 * Sjekker at ordren inneholder det minimale vi trenger.
 * Kaster en Error (exception) hvis noe mangler – dette stopper
 * integrasjonen og gir en tydelig feilmelding i stedet for
 * at feil data smyger seg gjennom til PC-kassen.
 *
 * @param {object} order - Shopify-ordren som skal valideres
 * @throws {Error} - Hvis påkrevde felt mangler
 */
function validateOrder(order) {
  // Sjekk 1: Ordren må eksistere og ha en ID
  if (!order || !order.id) {
    throw new Error("Ordre mangler eller har ingen ID");
  }

  // Sjekk 2: Kunden må finnes og ha e-post
  // Bruker optional chaining (?.) for å unngå krasj hvis customer er undefined
  if (!order.customer?.email) {
    throw new Error("Kunde-e-post mangler – kan ikke behandle ordren");
  }

  // Sjekk 3: Det må finnes minst ett produkt i ordren
  if (!order.line_items || order.line_items.length === 0) {
    throw new Error("Ordren inneholder ingen produkter (line_items er tom)");
  }

  // Sjekk 4: Totalprisen må være et positivt tall
  if (typeof order.total_price !== "number" || order.total_price <= 0) {
    throw new Error("Totalprisen er ugyldig – må være et positivt tall");
  }

  // Sjekk 5: Hvert produkt må ha SKU og pris
  order.line_items.forEach((item, index) => {
    if (!item.sku) {
      throw new Error(`Produkt nr. ${index + 1} mangler SKU`);
    }
    if (typeof item.price !== "number" || item.price < 0) {
      throw new Error(`Produkt nr. ${index + 1} har ugyldig pris`);
    }
  });

  // Hvis vi kommer hit uten feil: ordren er gyldig!
  console.log("✓ Validering OK – ordren er gyldig");
}

// ---------------------------------------------------------------
// EKSPORT
// ---------------------------------------------------------------
module.exports = { validateOrder };

/**
 * ============================================================
 * OPPSUMMERING:
 *  - Én funksjon: validateOrder(order)
 *  - Sjekker ID, e-post, produkter, pris og SKU
 *  - Kaster feil tidlig slik at resten av systemet er trygt
 *  - Brukes i main.js FØR mapping skjer
 * ============================================================
 */
