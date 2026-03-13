/**
 * ============================================================
 * FIL: src/data/shopifyOrder.js
 * FORMÅL: Simulerer en ordre slik Shopify ville sendt den til oss.
 *         I virkeligheten ville denne dataen kommet via en webhook
 *         eller API-kall fra Shopify. Her "faker" vi den for å lære
 *         datastrukturen uten å trenge en ekte Shopify-butikk.
 *
 * NYT I DEL 2:
 *  - discount_codes: Rabattkoder brukt på ordren
 *  - Flere fraktlinjer i shipping_lines (f.eks. to forsendelser)
 *  - total_price er oppdatert til å reflektere rabatt
 * ============================================================
 *
 * NØKKELBEGREPER:
 *  - order_number:    Shopifys eget ordrenummer (tekstformat, f.eks "#1001")
 *  - customer:        Kundeinfo – Shopify skiller fornavn og etternavn
 *  - line_items:      Alle produkter i ordren (linje for linje)
 *  - discount_codes:  Rabattkoder som er brukt på ordren
 *  - shipping_lines:  Fraktinfo – kan være flere fraktlinjer
 *  - total_price:     Total inkl. frakt og etter rabatter
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

  // DEL 2: Rabattkoder brukt av kunden ved kjøp.
  // Shopify sender alltid dette som et array – det kan være 0 eller flere koder.
  // "type" kan være "fixed_amount" (fast beløp) eller "percentage" (prosent).
  discount_codes: [
    {
      code: "VAAR25",
      amount: 25,
      type: "fixed_amount", // kr 25 av på hele ordren
    },
  ],

  // DEL 2: To fraktlinjer – f.eks. fysisk vare og digital lisens sendes separat.
  // I del 1 hentet vi bare shipping_lines[0].
  // I del 2 håndterer vi alle linjene.
  shipping_lines: [
    {
      title: "Standard frakt",
      price: 49,
    },
    {
      title: "Digital levering",
      price: 0, // digitale produkter har ingen fraktkostnad
    },
  ],

  // Totalberegning:
  //  Varer:    1×299 + 2×99 = 497
  //  Frakt:    49 + 0       =  49
  //  Rabatt:   -25          = -25
  //  TOTAL:                  521
  total_price: 521,
};

// ---------------------------------------------------------------
// EKSPORT: Gjør shopifyOrder tilgjengelig for andre filer
// ---------------------------------------------------------------
module.exports = { shopifyOrder };

/**
 * ============================================================
 * OPPSUMMERING:
 *  - Definerer én fake Shopify-ordre med kunde, produkter,
 *    rabattkoder og flere fraktlinjer
 *  - Brukes som "input" til integrasjonen i main.js
 *  - Ingen logikk her – bare datastruktur
 * ============================================================
 */
