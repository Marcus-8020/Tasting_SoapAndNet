# Hva er mapping og integrasjon?

> Forklaring på norsk for denne oppgaveserien

---

## Det enkle svaret

**Mapping** betyr å oversette data fra ett format til et annet.

To systemer bruker sjelden det samme "språket" for å beskrive de samme tingene. En integrasjon sin jobb er å stå i midten og oversette.

---

## Et konkret eksempel

**Shopify sier:**
```json
{
  "customer": {
    "first_name": "Marcus",
    "last_name": "Børresen"
  }
}
```

**PC-kassen forventer:**
```json
{
  "kundenavn": "Marcus Børresen"
}
```

Informasjonen er den **samme**, men formatet er **forskjellig**. Mapperen sin jobb er å gjøre om det ene til det andre.

---

## Hva er en integrasjon?

En integrasjon er koden som:

1. **Mottar** data fra system A (Shopify)
2. **Validerer** at dataen er komplett og gyldig
3. **Mapper** om dataen til format system B forstår
4. **Sender** dataen videre til system B (PC-kassen)

```
[Shopify] → [Validator] → [Mapper] → [POS-system]
```

---

## Hvorfor kan ikke systemene "bare kobles sammen"?

Fordi de er laget uavhengig av hverandre, av forskjellige firma, til forskjellige tider. De har aldri "snakket" med hverandre og vet ikke om hverandre.

Det er **integrasjonens jobb** å bygge broen.

---

## Hva er SOAP?

SOAP (Simple Object Access Protocol) er en gammel, men veldig utbredt standard for å sende meldinger mellom systemer.

- Bruker **XML** som format (ikke JSON)
- Veldig **strengt definert** – feltnavn og struktur må stemme nøyaktig
- Brukes mye i eldre systemer (banker, kassesystemer, offentlig sektor)
- Mer rigid enn moderne REST/JSON API-er, men det gjør det også forutsigbart

```xml
<soapenv:Envelope>
  <soapenv:Body>
    <OrdreRequest>
      <Kundenavn>Marcus Børresen</Kundenavn>
    </OrdreRequest>
  </soapenv:Body>
</soapenv:Envelope>
```

---

## Filstruktur i dette prosjektet

```
main.js                         ← Starter hele flyten
src/
  data/shopifyOrder.js          ← Simulert input fra Shopify
  validators/orderValidator.js  ← Sjekker at data er gyldig
  mappers/shopifyToPos.js       ← Oversetter format
  utils/xmlBuilder.js           ← Bygger SOAP XML
  services/posService.js        ← Sender til POS (simulert)
```

Denne strukturen ligner på hvordan ekte backend-prosjekter er organisert.
