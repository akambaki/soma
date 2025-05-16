# EDUX Stablecoin Listing & Technical Package – Expanded Edition v2.0

**Last updated:** 16 May 2025 · Emmanuel Mbongo

> **What changed?** The initiative is now based on a **fully‑collateralised stablecoin** pegged 1:1 to the West‑African CFA franc (**XOF**). This removes price volatility, simplifies accounting for tuition invoices, and satisfies forthcoming **MiCA** & **BCEAO** rules on asset‑referenced tokens.

---

## Quick Navigation

1. Stablecoin Rationale & Vision
2. White‑paper (20 sections) – EDUX Stablecoin
3. Technical & Integration Dossier (XRPL)
4. Reserve Architecture & Proof‑of‑Reserves
5. Fiat On/Off Ramps (XOF, EUR, USD)
6. Governance, Compliance & Licensing
7. Road‑map & KPIs
8. Risk Register
9. References

*(Use the canvas sidebar to jump to a chapter.)*

---

## 1  Rationale & Vision

**Old model:** Utility token (AYO) subject to price swings ⇒ unpredictable grant budgets.
**New model:** **EDUX**, a 100 % fiat‑backed, non‑interest‑bearing stablecoin.  Each EDUX is always worth **exactly 1 XOF** (≈ €0.00152).  Scholarships draw directly from EDUX pools, while sponsors enjoy zero FX risk.

**Vision –** Make EDUX the *de‑facto digital settlement layer* for all educational payments in West Africa.

---

## 2  EDUX Stablecoin – Full White‑paper (abridged key points)

### 2.1  Peg Mechanics

* **1 EDUX = 1 XOF**.  Reserves held 100 % in sight deposits at **Ecobank Benin** + **BCEAO T‑Bills ≤90 days**.
* Monthly **Mazars** attestation; real‑time Proof‑of‑Reserves hash posted on‑chain via `ReserveOracle.hook`.

### 2.2  Mint / Burn

| Action     | Trigger                             | XRPL Flow                                                               |
| ---------- | ----------------------------------- | ----------------------------------------------------------------------- |
| **Mint**   | Fiat deposit received (PSP webhook) | Authorised Minter sends `Payment` from Issuer → User, memo `MINT:<Ref>` |
| **Redeem** | User burns EDUX to Issuer address   | Issuer wires XOF back within T+0 (mobile‑money)                         |

### 2.3  Reserve Yield → Scholarships

Interest on T‑Bills (\~3.5 % p.a.) channels into **Scholarship Grant Pool**—targeting 1 % tuition subsidy per quarter.

### 2.4  Fees & Sustainability

* Mint: **0.20 %** (covers PSP).
* Redeem: **0.30 %**.
* 50 % of net fee profit → scholarship top‑ups; 50 % → operational treasury.

### 2.5  Governance

* **Issuer:** **EDUX Trust (Benin) S.A.** under OHADA.
* 5‑member Board incl. independent seat (Civil Society).
* **DAO** roadmap Q2 2027 after MiCA “significant” designation clarity.

### 2.6  Regulatory Positioning

* **Benin / WAEMU:** E‑money equivalent. Applying for **Établissement de Monnaie Électronique** licence (#P/EME‑2025‑014).
* **EU:** MiCA Title III “E‑money Token” ⇒ require white‑paper filing & capital buffer ≥2 % reserves.
* **US:** FinCEN MSB; NY DFS BitLicense not required (no NY marketing).

*(White‑paper full text = 32 pages; see GitHub.)*

---

## 3  Technical & Integration Dossier (XRPL)

| Parameter                 | Value                                                               |
| ------------------------- | ------------------------------------------------------------------- |
| **Currency Code**         | `45545558` (ASCII “EDUX”)                                           |
| **Decimals**              | 2 (fits centime)                                                    |
| **Issuer Address**        | `rEDUXIssuerB6JkP4M4kgw1EZdux9NvFtX`                                |
| **Flags**                 | `RequireAuth`, `DisallowXRP`, `EnableClawback`, master key disabled |
| **Authorized Minters**    | 2 hot wallets (`rMint1...`, `rMint2...`) with 10 M daily cap each   |
| **Hooks**                 | `ReserveOracle.hook` (Proof‑of‑Reserves)                            |
| **Deposit Confirmations** | 1 ledger (≈4 s)                                                     |
| **Explorer**              | [https://xrpscan.com/token/EDUX](https://xrpscan.com/token/EDUX)    |

### 3.1  Mint Workflow (Python sketch)

```python
# called by PSP webhook
if verify_webhook(psp_payload):
    amount = psp_payload["xof_amount"]
    mint_tx = Payment(
        account=MINTER_WALLET.classic_address,
        destination=user_xrpl,
        amount={"currency":"EDUX","issuer":ISSUER_ADDR,"value":f"{amount/100:.2f}"},
        memos=[{"MemoData":f"MINT:{psp_ref}".encode().hex()}]
    )
    send_reliable_submission(sign_and_autofill(mint_tx))
```

### 3.2  Burn / Redemption Flow

User sends `Payment` of EDUX to **Issuer** with `MemoData="REDEEM:<IBAN|MoMo>"`.  Hook logs TxHash; backend wires fiat.

---

## 4  Reserve Architecture & Proof‑of‑Reserves

1. **Bank Layer:** Ecobank escrow account; daily CSV.
2. **Treasury Layer:** BCEAO T‑Bills (≤90 d); custodian BNP‑Paribas Securities.
3. **Reconciliation:** AWS Glue ETL hourly; delta $≤0.01 %$ → alert.
4. **Attestation:** Mazars signs SHA‑256 Merkle root; hash pushed to XRPL via `ReserveOracle.hook`.

---

## 5  Fiat On/Off Ramps (Updated)

* **XOF:** Wave, CinetPay (mobile‑money), Ecobank API.
* **EUR:** SEPA Instant via ClearBank, Stripe Treasury.
* **USD:** ACH & FedWire via Modern Treasury; USDC via Circle.

Fees lowered (0.2 %/0.3 %) thanks to stablecoin peg.

---

## 6  Governance, Compliance & Licensing

* **Licence Path:** EME (Benin) → MiCA EMT (EU) → FinCEN MSB (US).
* **Capital Buffer:** 2.5 % of reserves held in Tier‑1 government bonds.
* **Travel Rule:** Notabene TRISA memos for tx ≥1 000 USD‑equiv.
* **OFAC Screening:** ComplyAdvantage real‑time API.

---

## 7  Road‑map & KPIs

| Date     | Milestone                    | KPI                           |
| -------- | ---------------------------- | ----------------------------- |
| Aug 2025 | EDUX Main‑net launch         | ±0.1 % peg deviation          |
| Sep 2025 | Bitrue Listing               | ≥US\$3 M 24 h vol             |
| Q4 2025  | 500 M EDUX in circulation    | Monthly attestations on‑chain |
| 2026     | MiCA compliance audit passed | AMF registration              |
| 2027     | DAO upgrade vote             | On‑chain governance live      |

---

## 8  Risk Register (Stablecoin‑specific)

| Risk                | Likelihood | Impact | Mitigation                                                 |
| ------------------- | ---------- | ------ | ---------------------------------------------------------- |
| Bank default        | Low        | High   | Multi‑bank, deposit insurance, T‑Bill ladder               |
| Peg deviation       | Low        | Med    | 30 % reserve in instant cash, arbitrage market‑making fund |
| Regulatory ban      | Med        | High   | Engage BCEAO sandbox, EU MiCA filing                       |
| Mass redemption run | Low        | High   | ≥110 % liquid reserves, pause mint if stress               |

---

## 9  References

1. BCEAO Instruction n°008‑05‑2023 – E‑money rules.
2. MiCA Regulation (EU) 2023/1114.
3. Ripple Labs. *Issued Currencies & Clawback* (2024).
4. Mazars. *Stablecoin Attestation Framework* (2024).

---

*End of EDUX Stablecoin Package v2.0*
