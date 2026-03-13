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
 * I VIRKELIGHETEN ville denne XML-en bli sendt over HTTP til
 * et SOAP-endepunkt – men her skriver vi den bare ut til konsoll.
 * ============================================================
 */

/**
 * byggProduktXml
 * --------------
 * Hjelpefunksjon som lager XML for ett enkelt produkt.
 * Kalles én gang per produkt i ordren.
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
  // Bygg XML for alle produkter og slå dem sammen til én streng
  // .map() lager array av XML-strenger, .join("") slår dem sammen
  const produkterXml = posOrdre.produkter.map(byggProduktXml).join("");

  // Template literal (backticks) gjør det enkelt å bygge lange strenger
  // med variabler inni
  return `<?xml version="1.0" encoding="UTF-8"?>
<soapenv:Envelope xmlns:soapenv="http://schemas.xmlsoap.org/soap/envelope/">
    <soapenv:Body>
        <OrdreRequest>
            <OrdreId>${posOrdre.ordreId}</OrdreId>
            <Kundenavn>${posOrdre.kundenavn}</Kundenavn>
            <KundeEpost>${posOrdre.kundeEpost}</KundeEpost>
            <FraktMetode>${posOrdre.fraktMetode}</FraktMetode>
            <FraktPris>${posOrdre.fraktPris}</FraktPris>
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
 *  - byggProduktXml(): lager XML for ett produkt (intern hjelpefunksjon)
 *  - byggSoapXml(): lager komplett SOAP-envelope med hele ordren
 *  - XML-strukturen etterligner hva et ekte SOAP-kall ville sendt
 *  - I .NET ville vi brukt en WSDL og auto-genererte klasser,
 *    men prinsippet er det samme: strukturert XML med faste feltnavn
 * ============================================================
 */
