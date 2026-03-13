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
 * NYT I DEL 2:
 *  - Logging via logService – hvert steg skrives til loggfil
 *
 * HVORDAN KJØRE:
 *   node main.js
 *
 * AVHENGIGHETER (ingen npm-pakker nødvendig):
 *   - src/data/shopifyOrder.js
 *   - src/validators/orderValidator.js
 *   - src/mappers/shopifyToPos.js
 *   - src/services/posService.js
 *   - src/services/logService.js
 * ============================================================
 */

// ---------------------------------------------------------------
// IMPORTS: Hent inn modulene vi trenger
// ---------------------------------------------------------------
const { shopifyOrder }    = require("./src/data/shopifyOrder");
const { validateOrder }   = require("./src/validators/orderValidator");
const { mapShopifyToPos } = require("./src/mappers/shopifyToPos");
const { skrivUtOrdre, sendTilPos } = require("./src/services/posService");
const { logg }            = require("./src/services/logService");

// ---------------------------------------------------------------
// HOVEDFUNKSJON: kjørIntegrasjon
// ---------------------------------------------------------------
function kjørIntegrasjon(ordre) {
  console.log("============================================================");
  console.log("  SHOPIFY → POS INTEGRASJON – STARTER");
  console.log("============================================================");
  console.log(`\nMottatt ordre fra Shopify: ${ordre.order_number}`);

  // Logg at vi starter – dette skrives til loggfilen
  logg.info(`Integrasjon startet for ordre ${ordre.order_number}`);

  try {
    // STEG 1: Validering
    console.log("\n[STEG 1] Validerer ordre...");
    validateOrder(ordre);
    logg.info(`Validering OK for ordre ${ordre.order_number}`);

    // STEG 2: Mapping
    console.log("\n[STEG 2] Mapper til POS-format...");
    const posOrdre = mapShopifyToPos(ordre);
    console.log("✓ Mapping fullført");
    logg.info(`Mapping fullført – total beregnet til kr ${posOrdre.totalBelop},-`);

    // Logg advarsel hvis ordren har rabatter (nyttig for å spore rabattbruk)
    if (posOrdre.rabatter.length > 0) {
      const koder = posOrdre.rabatter.map((r) => r.kode).join(", ");
      logg.advarsel(`Ordre ${ordre.order_number} har rabattkoder: ${koder} (total -kr ${posOrdre.totalRabatt},-)`);
    }

    // STEG 3: Skriv ut i lesbart format
    console.log("\n[STEG 3] Viser mappet ordre...");
    skrivUtOrdre(posOrdre);

    // STEG 4: Send til POS-systemet
    console.log("\n[STEG 4] Sender til POS-systemet...");
    const suksess = sendTilPos(posOrdre);

    if (suksess) {
      logg.info(`Ordre ${ordre.order_number} overført til POS – FULLFØRT`);
      console.log("\n============================================================");
      console.log("  INTEGRASJON FULLFØRT – Ordre overført til POS");
      console.log("============================================================");
    }

  } catch (feil) {
    // Logg feilen på FEIL-nivå – dette er det viktigste å logge
    logg.feil(`Integrasjon feilet for ordre ${ordre.order_number}: ${feil.message}`);

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
 *  - Importerer alle nødvendige moduler inkl. logService (ny i del 2)
 *  - kjørIntegrasjon() styrer hele flyten i rekkefølge
 *  - logg.info/advarsel/feil() skrives til logs/integration.log
 *  - try/catch fanger alle feil og logger dem før programmet stopper
 *  - Kjør med: node main.js
 * ============================================================
 */
