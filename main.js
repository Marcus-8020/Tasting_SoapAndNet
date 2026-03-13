/**
 * ============================================================
 * FIL: main.js
 * FORMÅL: Inngangspunkt for integrasjonen.
 *         Denne filen "orkestrerer" hele flyten:
 *           1. Hent Shopify-ordre (simulert)
 *           2. Valider ordren
 *           3. Map til POS-format
 *           4. Skriv ut og "send" til POS-systemet
 *
 * HVORDAN KJØRE:
 *   node main.js
 *
 * AVHENGIGHETER (ingen npm-pakker nødvendig):
 *   - src/data/shopifyOrder.js
 *   - src/validators/orderValidator.js
 *   - src/mappers/shopifyToPos.js
 *   - src/services/posService.js
 * ============================================================
 */

// ---------------------------------------------------------------
// IMPORTS: Hent inn modulene vi trenger
// require() er Node.js sin måte å importere filer på
// ---------------------------------------------------------------
const { shopifyOrder }    = require("./src/data/shopifyOrder");
const { validateOrder }   = require("./src/validators/orderValidator");
const { mapShopifyToPos } = require("./src/mappers/shopifyToPos");
const { skrivUtOrdre, sendTilPos } = require("./src/services/posService");

// ---------------------------------------------------------------
// HOVEDFUNKSJON: kjørIntegrasjon
// Samler hele flyten i én funksjon for å holde det ryddig.
// ---------------------------------------------------------------
function kjørIntegrasjon(ordre) {
  console.log("============================================================");
  console.log("  SHOPIFY → POS INTEGRASJON – STARTER");
  console.log("============================================================");
  console.log(`\nMottatt ordre fra Shopify: ${ordre.order_number}`);

  try {
    // STEG 1: Validering
    // Sjekk at ordren er komplett FØR vi gjør noe med den.
    // Hvis validering feiler, kastes en Error og vi hopper til catch.
    console.log("\n[STEG 1] Validerer ordre...");
    validateOrder(ordre);

    // STEG 2: Mapping
    // Oversett fra Shopify-format til POS-format.
    console.log("\n[STEG 2] Mapper til POS-format...");
    const posOrdre = mapShopifyToPos(ordre);
    console.log("✓ Mapping fullført");

    // STEG 3: Skriv ut i lesbart format
    // Vis hva POS-ordren ser ut som etter mapping.
    console.log("\n[STEG 3] Viser mappet ordre...");
    skrivUtOrdre(posOrdre);

    // STEG 4: Send til POS-systemet via SOAP
    // Dette er siste steg – her "forlater" dataen systemet vårt.
    console.log("\n[STEG 4] Sender til POS-systemet...");
    const suksess = sendTilPos(posOrdre);

    // Avslutt med oppsummering
    if (suksess) {
      console.log("\n============================================================");
      console.log("  INTEGRASJON FULLFØRT – Ordre overført til POS");
      console.log("============================================================");
    }

  } catch (feil) {
    // Noe gikk galt – logg feilen og stopp integrasjonen.
    // I et ekte system ville vi også sendt en varsling (e-post, Slack, etc.)
    console.error("\n[FEIL] Integrasjonen stoppet:");
    console.error(`  → ${feil.message}`);
    console.error("\nOrdren ble IKKE sendt til POS-systemet.");
  }
}

// ---------------------------------------------------------------
// KJØR: Start integrasjonen med vår simulerte Shopify-ordre
// ---------------------------------------------------------------
kjørIntegrasjon(shopifyOrder);

/**
 * ============================================================
 * OPPSUMMERING:
 *  - Importerer alle nødvendige moduler
 *  - kjørIntegrasjon() styrer hele flyten i rekkefølge
 *  - try/catch fanger alle feil og stopper integrasjonen trygt
 *  - Kjør med: node main.js
 * ============================================================
 */
