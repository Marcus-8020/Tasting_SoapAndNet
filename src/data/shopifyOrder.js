/**
 * ============================================================
 * FIL: src/data/shopifyOrder.js
 * FORMÅL: Simulerer en ordre slik Shopify ville sendt den til oss.
 *         I virkeligheten ville denne dataen kommet via en webhook
 *         eller API-kall fra Shopify. Her "faker" vi den for å lære
 *         datastrukturen uten å trenge en ekte Shopify-butikk.
 * ============================================================
 *
 * NØKKELBEGREPER:
 *  - order_number: Shopifys eget ordrenummer (tekstformat, f.eks "#1001")
 *  - customer:     Kundeinfo – Shopify skiller fornavn og etternavn
 *  - line_items:   Alle produkter i ordren (linje for linje)
 *  - shipping_lines: Fraktinfo – kan være flere fraktlinjer
 *  - total_price:  Total inkl. frakt
 */

// ---------------------------------------------------------------
// SHOPIFY-ORDRE: Slik ser rådata fra Shopify ut
// Dette er formatet VI mottar, ikke det PC-kassen forventer.
// ---------------------------------------------------------------
const shopifyOrder = {
  id: 1001,
  order_number: "#1001",

  // Shopify lagrer kunde som et objekt med separate felt
  customer: {
    first_name: "Marcus",
    last_name: "Børresen",
    email: "marcus@example.com",
  },

  // line_items = produktlinjene i ordren
  // Hvert produkt er sitt eget objekt
  line_items: [
    {
      title: "Aurnor Premium Plan",
      sku: "AURNOR-001",   // SKU = varenummer internt
      quantity: 1,
      price: 299,
    },
    {
      title: "Market Insight Addon",
      sku: "AURNOR-ADD-01",
      quantity: 2,
      price: 99,
    },
  ],

  // Shopify støtter flere fraktlinjer – vi henter alltid [0] (første)
  shipping_lines: [
    {
      title: "Standard Shipping",
      price: 49,
    },
  ],

  // total_price fra Shopify inkluderer alt (varer + frakt)
  total_price: 546,
};

// ---------------------------------------------------------------
// EKSPORT: Gjør shopifyOrder tilgjengelig for andre filer
// ---------------------------------------------------------------
module.exports = { shopifyOrder };

/**
 * ============================================================
 * OPPSUMMERING:
 *  - Definerer én fake Shopify-ordre med kunde, produkter og frakt
 *  - Brukes som "input" til integrasjonen i main.js
 *  - Ingen logikk her – bare datastruktur
 * ============================================================
 */
