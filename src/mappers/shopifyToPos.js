/**
 * ============================================================
 * FIL: src/mappers/shopifyToPos.js
 * FORMÅL: "Oversetter" en Shopify-ordre til det formatet
 *         PC-kassen (POS-systemet) forventer å motta.
 *
 * MAPPING = OVERSETTELSE:
 *  Shopify og PC-kassen "snakker" ikke samme språk:
 *    Shopify bruker:    customer.first_name + customer.last_name
 *    PC-kassen vil ha:  kundenavn (ett samlet felt)
 *
 *  Det er denne jobben mapperen gjør.
 *  En mapper inneholder INGEN forretningslogikk – den bare
 *  strukturerer om data fra ett format til et annet.
 *
 * NYT I DEL 2:
 *  - beregnFraktTotal(): summerer ALLE fraktlinjer (ikke bare [0])
 *  - beregnTotal(): tar nå hensyn til rabatter
 *  - mapShopifyToPos(): inkluderer fraktLinjer og rabatter i output
 * ============================================================
 */

/**
 * beregnFraktTotal
 * ----------------
 * Summerer prisen på alle fraktlinjer.
 * I del 1 brukte vi bare shipping_lines[0].price direkte.
 * I del 2 itererer vi over alle linjene – sikrere og mer fleksibelt.
 *
 * @param {Array} shippingLines - Fraktlinjene fra Shopify-ordren
 * @returns {number} - Total fraktpris
 */
function beregnFraktTotal(shippingLines) {
  // .reduce() går gjennom hvert element og akkumulerer én verdi
  // Starter på 0, legger til .price for hver linje
  return shippingLines.reduce((sum, linje) => sum + linje.price, 0);
}

/**
 * beregnTotal
 * -----------
 * Regner ut totalbeløp basert på faktiske linjepriser, frakt og rabatter.
 * Vi stoler IKKE blindt på total_price fra Shopify – vi verifiserer selv.
 *
 * Formel: (sum av alle produktlinjer) + fraktTotal - rabattTotal
 *
 * @param {Array} lineItems     - Produktlinjene fra Shopify-ordren
 * @param {number} fraktTotal   - Total fraktpris (alle fraktlinjer summert)
 * @param {number} rabattTotal  - Total rabattbeløp (alle rabattkoder summert)
 * @returns {number} - Kalkulert totalbeløp
 */
function beregnTotal(lineItems, fraktTotal, rabattTotal) {
  const varerTotal = lineItems.reduce((sum, item) => {
    return sum + item.quantity * item.price;
  }, 0);

  return varerTotal + fraktTotal - rabattTotal;
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
  // DEL 2: Hent alle fraktlinjer og summer dem
  // I del 1: const fraktlinje = order.shipping_lines[0] ?? { title: "Ukjent frakt", price: 0 }
  // I del 2: vi itererer over hele arrayet
  const fraktLinjer = order.shipping_lines ?? [];
  const totalFrakt = beregnFraktTotal(fraktLinjer);

  // DEL 2: Hent alle rabattkoder
  // "??" = hvis discount_codes er null/undefined, bruk tom array
  const rabatter = order.discount_codes ?? [];
  const totalRabatt = rabatter.reduce((sum, kode) => sum + kode.amount, 0);

  // Regn ut total med både frakt og rabatter
  const kalkulertTotal = beregnTotal(order.line_items, totalFrakt, totalRabatt);

  return {
    // Ordrenummer direkte fra Shopify
    ordreId: order.id,

    // Slå sammen fornavn og etternavn til ett felt
    kundenavn: `${order.customer.first_name} ${order.customer.last_name}`,

    // E-post trenger ingen transformasjon
    kundeEpost: order.customer.email,

    // Map hvert produkt fra Shopify-format til POS-format
    produkter: order.line_items.map((item) => ({
      produktnavn: item.title,
      sku: item.sku,
      antall: item.quantity,
      enhetspris: item.price,
      linjetotal: item.quantity * item.price,
    })),

    // DEL 2: Alle fraktlinjer som et array (ikke bare én)
    // PC-kassen kan vise alle forsendelser separat
    fraktLinjer: fraktLinjer.map((linje) => ({
      metode: linje.title,
      pris: linje.price,
    })),
    totalFrakt,

    // DEL 2: Rabattkoder brukt på ordren
    rabatter: rabatter.map((kode) => ({
      kode: kode.code,
      belop: kode.amount,
      type: kode.type,
    })),
    totalRabatt,

    // Bruk vår egne kalkulerte total (sikrere enn å stole på input)
    totalBelop: kalkulertTotal,
  };
}

// ---------------------------------------------------------------
// EKSPORT: Gjør funksjonene tilgjengelige eksternt
// ---------------------------------------------------------------
module.exports = { mapShopifyToPos, beregnTotal, beregnFraktTotal };

/**
 * ============================================================
 * OPPSUMMERING:
 *  - beregnFraktTotal(): summerer alle fraktlinjer (ny i del 2)
 *  - beregnTotal(): regner totalbeløp inkl. rabatter (oppdatert i del 2)
 *  - mapShopifyToPos(): oversetter Shopify-ordre til POS-format,
 *    nå med fraktLinjer (array) og rabatter
 *  - Mapper inneholder ingen validering – det er validators sin jobb
 * ============================================================
 */
