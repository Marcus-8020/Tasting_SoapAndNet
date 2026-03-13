/**
 * ============================================================
 * FIL: src/mappers/shopifyToPos.js
 * FORMÅL: "Oversetter" en Shopify-ordre til det formatet
 *         PC-kassen (POS-systemet) forventer å motta.
 *
 * MAPPING = OVERSETTELSE:
 *  Shopify og PC-kassen "snakker" ikke samme språk:
 *    Shopify bruker:    customer.first_name + customer.last_name
 *    PC-kassen vil ha:  customerName (ett samlet felt)
 *
 *  Det er denne jobben mapperen gjør.
 *  En mapper inneholder INGEN forretningslogikk – den bare
 *  strukturerer om data fra ett format til et annet.
 * ============================================================
 */

/**
 * beregnTotal
 * -----------
 * Regner ut totalbeløp basert på faktiske linjepriser + frakt.
 * Vi stoler IKKE blindt på total_price fra Shopify – vi verifiserer.
 *
 * @param {Array} lineItems   - Produktlinjene fra Shopify-ordren
 * @param {number} shippingPrice - Fraktpris
 * @returns {number} - Kalkulert totalbeløp
 */
function beregnTotal(lineItems, shippingPrice) {
  const varerTotal = lineItems.reduce((sum, item) => {
    return sum + item.quantity * item.price;
  }, 0); // starter summen på 0

  return varerTotal + shippingPrice;
}

/**
 * mapShopifyToPos
 * ---------------
 * Hovedfunksjonen i denne filen.
 * Tar inn en Shopify-ordre og returnerer et POS-ordre-objekt.
 *
 * @param {object} order - Validert Shopify-ordre
 * @returns {object} - Ordre i POS-format
 */
function mapShopifyToPos(order) {
  // Hent fraktlinja (første linje, eller fallback-verdier)
  const fraktlinje = order.shipping_lines[0] ?? { title: "Ukjent frakt", price: 0 };

  // Regn ut total på nytt i stedet for å stole på Shopify-input
  const kalkulertTotal = beregnTotal(order.line_items, fraktlinje.price);

  return {
    // Ordrenummer direkte fra Shopify
    ordreId: order.id,

    // Slå sammen fornavn og etternavn til ett felt
    // Template literals (backticks) gjør dette enkelt: `${a} ${b}`
    kundenavn: `${order.customer.first_name} ${order.customer.last_name}`,

    // E-post trenger ingen transformasjon
    kundeEpost: order.customer.email,

    // Map hvert produkt fra Shopify-format til POS-format
    // .map() lager et nytt array der hvert element er transformert
    produkter: order.line_items.map((item) => ({
      produktnavn: item.title,
      sku: item.sku,
      antall: item.quantity,
      enhetspris: item.price,
      linjetotal: item.quantity * item.price, // regn ut per linje
    })),

    // Fraktinfo
    fraktMetode: fraktlinje.title,
    fraktPris: fraktlinje.price,

    // Bruk vår egne kalkulerte total (sikrere enn å stole på input)
    totalBelop: kalkulertTotal,
  };
}

// ---------------------------------------------------------------
// EKSPORT: Gjør begge funksjonene tilgjengelige eksternt
// ---------------------------------------------------------------
module.exports = { mapShopifyToPos, beregnTotal };

/**
 * ============================================================
 * OPPSUMMERING:
 *  - beregnTotal(): regner ut faktisk totalbeløp fra linjer + frakt
 *  - mapShopifyToPos(): oversetter Shopify-ordre til POS-format
 *  - Alle feltnavn er på norsk i POS-formatet (som om POS-systemet er norsk)
 *  - Mapper inneholder ingen validering – det er validators sin jobb
 * ============================================================
 */
