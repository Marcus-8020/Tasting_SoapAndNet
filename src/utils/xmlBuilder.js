/**
 * ============================================================
 * FIL: src/utils/xmlBuilder.js
 * FORMÅL: Bygger en SOAP-lignende XML-melding fra en POS-ordre.
 *
 * HVA ER SOAP OG XML?
 *  - XML (eXtensible Markup Language) er et format for å strukturere data,
 *    litt som JSON men med åpne og lukkede tagger (som HTML).
 *  - SOAP er en protokoll (et sett regler) for å sende meldinger
 *    mellom systemer. SOAP bruker alltid XML som format.
 *  - Mange eldre systemer (inkl. mange PC-kasser) bruker SOAP
 *    fordi det er veldig strengt definert og forutsigbart.
 *
 * EKSEMPEL PÅ XML vs JSON:
 *  JSON:  { "navn": "Marcus" }
 *  XML:   <Navn>Marcus</Navn>
 *
 * NYT I DEL 2:
 *  - byggFraktXml(): ny hjelpefunksjon for fraktlinjer
 *  - byggRabattXml(): ny hjelpefunksjon for rabattkoder
 *  - byggSoapXml(): inkluderer nå Fraktlinjer og Rabatter i XML
 * ============================================================
 */

/**
 * byggProduktXml
 * --------------
 * Hjelpefunksjon som lager XML for ett enkelt produkt.
 *
 * @param {object} produkt - Et produkt-objekt fra POS-ordren
 * @returns {string} - XML-streng for produktet
 */
function byggProduktXml(produkt) {
  return `
        <Produkt>
            <Produktnavn>${produkt.produktnavn}</Produktnavn>
            <SKU>${produkt.sku}</SKU>
            <Antall>${produkt.antall}</Antall>
            <Enhetspris>${produkt.enhetspris}</Enhetspris>
            <Linjetotal>${produkt.linjetotal}</Linjetotal>
        </Produkt>`;
}

/**
 * byggFraktXml (DEL 2)
 * --------------------
 * Hjelpefunksjon som lager XML for én fraktlinje.
 * Kalles én gang per fraktlinje i ordren.
 *
 * @param {object} linje - En fraktlinje fra POS-ordren
 * @returns {string} - XML-streng for fraktlinjen
 */
function byggFraktXml(linje) {
  return `
        <Fraktlinje>
            <Metode>${linje.metode}</Metode>
            <Pris>${linje.pris}</Pris>
        </Fraktlinje>`;
}

/**
 * byggRabattXml (DEL 2)
 * ---------------------
 * Hjelpefunksjon som lager XML for én rabattkode.
 *
 * @param {object} rabatt - En rabatt fra POS-ordren
 * @returns {string} - XML-streng for rabatten
 */
function byggRabattXml(rabatt) {
  return `
        <Rabatt>
            <Kode>${rabatt.kode}</Kode>
            <Belop>${rabatt.belop}</Belop>
            <Type>${rabatt.type}</Type>
        </Rabatt>`;
}

/**
 * byggSoapXml
 * -----------
 * Bygger en komplett SOAP-envelope (konvolutt) med ordre-data.
 *
 * SOAP-struktur (forenklet):
 *  <Envelope>        ← Ytterste wrapper – alltid påkrevd i SOAP
 *    <Body>          ← Innholdet i meldingen
 *      <OrdreRequest> ← Vår egne data
 *        ...
 *      </OrdreRequest>
 *    </Body>
 *  </Envelope>
 *
 * @param {object} posOrdre - POS-ordre-objekt fra mapperen
 * @returns {string} - Komplett SOAP XML-streng
 */
function byggSoapXml(posOrdre) {
  const produkterXml  = posOrdre.produkter.map(byggProduktXml).join("");
  const fraktLinjerXml = posOrdre.fraktLinjer.map(byggFraktXml).join("");

  // Bygg rabatt-XML – men bare hvis det finnes rabatter
  const rabatterXml = posOrdre.rabatter.length > 0
    ? `<Rabatter>${posOrdre.rabatter.map(byggRabattXml).join("")}
            </Rabatter>`
    : "";

  return `<?xml version="1.0" encoding="UTF-8"?>
<soapenv:Envelope xmlns:soapenv="http://schemas.xmlsoap.org/soap/envelope/">
    <soapenv:Body>
        <OrdreRequest>
            <OrdreId>${posOrdre.ordreId}</OrdreId>
            <Kundenavn>${posOrdre.kundenavn}</Kundenavn>
            <KundeEpost>${posOrdre.kundeEpost}</KundeEpost>
            <Fraktlinjer>${fraktLinjerXml}
            </Fraktlinjer>
            <TotalFrakt>${posOrdre.totalFrakt}</TotalFrakt>
            ${rabatterXml}
            <TotalRabatt>${posOrdre.totalRabatt}</TotalRabatt>
            <TotalBelop>${posOrdre.totalBelop}</TotalBelop>
            <Produkter>${produkterXml}
            </Produkter>
        </OrdreRequest>
    </soapenv:Body>
</soapenv:Envelope>`;
}

// ---------------------------------------------------------------
// EKSPORT
// ---------------------------------------------------------------
module.exports = { byggSoapXml };

/**
 * ============================================================
 * OPPSUMMERING:
 *  - byggProduktXml(): lager XML for ett produkt
 *  - byggFraktXml():   lager XML for én fraktlinje (ny i del 2)
 *  - byggRabattXml():  lager XML for én rabattkode (ny i del 2)
 *  - byggSoapXml():    lager komplett SOAP-envelope med hele ordren
 *  - I .NET ville vi brukt en WSDL og auto-genererte klasser,
 *    men prinsippet er det samme: strukturert XML med faste feltnavn
 * ============================================================
 */
