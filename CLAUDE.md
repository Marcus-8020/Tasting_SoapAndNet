# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Running the Project

```bash
node main.js
```

No dependencies to install — the project uses only built-in Node.js modules.

## Architecture

This is a **Shopify → POS integration pipeline** (Phase 1 of a multi-part series). It simulates receiving Shopify orders and converting them for a legacy PC-based POS system via SOAP/XML.

**Pipeline flow:**

```
shopifyOrder.js (sample data)
  → orderValidator.js (validateOrder)
  → shopifyToPos.js (mapShopifyToPos)
  → posService.js (skrivUtOrdre + sendTilPos)
  → xmlBuilder.js (byggSoapXml)
```

**Module responsibilities:**

| File | Role |
|------|------|
| [main.js](main.js) | Orchestrator — runs `kjørIntegrasjon()`, handles top-level errors |
| [src/data/shopifyOrder.js](src/data/shopifyOrder.js) | Hardcoded sample Shopify order (simulates API response) |
| [src/validators/orderValidator.js](src/validators/orderValidator.js) | 5-point validation: ID, email, items, customer, totals |
| [src/mappers/shopifyToPos.js](src/mappers/shopifyToPos.js) | Field mapping: `customer.first_name` → `kundenavn`, `line_items` → `produkter`, etc. |
| [src/services/posService.js](src/services/posService.js) | Logs order to console and simulates SOAP transmission |
| [src/utils/xmlBuilder.js](src/utils/xmlBuilder.js) | Builds SOAP XML envelope for the POS system |

**Key design notes:**
- All external calls (Shopify API, POS endpoint) are simulated — no real network calls
- Code and comments are in Norwegian
- `posService.js` depends on `xmlBuilder.js`; all other modules are standalone
- Planned next phases: discount support, multiple shipping methods, logging service, .NET/C# migration
