# SOMA Stablecoin Listing & Technical Package – **Hyper‑Verbose Edition v3.0**

**Last updated:** 16 May 2025 · Emmanuel Mbongo

> **Re‑brand note.** After extensive user‑testing with students, faculty, and sponsors across West Africa and the diaspora, we are renaming the stablecoin from **EDUX** to **SOMA**.  *Soma* means **“to learn, read, study”** in Swahili—a pan‑African lingua franca heard from Dakar to Dar‑es‑Salaam.  The name encapsulates our mission: **turning learning into a universal right, not a privilege.**  The crisp four‑letter ticker is easy to pronounce in French, English, Yoruba, and Fon, and the ASCII code (`534F4D41`) slots neatly into XRPL metadata without padding.

---

## Quick Navigation (clickable headings in the left sidebar)

1. Rationale & Vision (ultra‑detailed)
2. White‑paper (20 expanded sections) – *SOMA Stablecoin*
3. Technical & Integration Dossier (XRPL, code walk‑throughs, network diagrams)
4. Reserve Architecture & Proof‑of‑Reserves (bank flows + Merkle proofs)
5. Fiat On/Off Ramps (XOF, EUR, USD) with sequence diagrams
6. Governance, Compliance & Licensing (multi‑jurisdiction deep dive)
7. Road‑map & KPI Dashboard (Gantt + OKRs)
8. Risk Register (36 enumerated risks, Monte‑Carlo VaR)
9. References & Annexes (policy docs, legal memos, ISO messages)

---

## 1  Rationale & Vision – The Long Read

**1.1 Why abandon a utility token?**  Our pilot with AYO revealed two pain points: (a) **budget volatility**—a 15 % token price swing in one week scrambled semester invoices; (b) **accounting headaches**—schools requested **hard XOF figures**, not fluctuating token values.  A fiat‑pegged stablecoin surgically removes both frictions.

**1.2 Why peg to the CFA franc (XOF)?**  94 % of tuition invoices inside Sèmè City are denominated in XOF, and BCEAO regulations mandate local‑currency settlement for educational services.  Pegging to USD or EUR would merely re‑insert an FX layer.  **Local peg = local trust.**

**1.3 Why the name “SOMA”?**  *Soma* is a verb—*learn!*—and a call to action.  Unlike techy acronyms, it resonates emotionally with parents and students. It is also **short for: Social Opportunity, Monetary Access**.

**1.4 Vision 2030.**  By 2030 every learner in WAEMU can swipe a phone and pay tuition or receive a scholarship in **seconds**, with **zero FX drift**, and **auditable public ledgers** proving every centime is backed 1:1.

---

## 2  SOMA Stablecoin – White‑paper (Expanded Highlights)

> *Full 44‑page PDF hosted at [https://soma.finance/whitepaper.pdf](https://soma.finance/whitepaper.pdf). Key high‑density takeaways below.*

### 2.1 Peg Mechanics (Verbose)

* **Hard peg:** **1 SOMA = 1 XOF**.  The peg is enforced by an automated **Mint & Redeem Arb‑Bot** (see §3) and 100 %+ on‑ledger proof of reserves.
* **Collateral composition:**

  * 70 % demand deposits at **Ecobank Benin** and **UBA Benin** (two‑bank model ↓ counter‑party risk).
  * 25 % BCEAO **T‑Bills ≤ 90 days** (liquid, zero credit risk, positive yield).
  * 5 % instant‑settlement cash at **Wave Money** for real‑time mobile payouts.

### 2.2 Mint / Burn Process – Step Level Narrative

| # | Actor                   | Off‑chain Event                                                                          | On‑chain Footprint | Latency Target |
| - | ----------------------- | ---------------------------------------------------------------------------------------- | ------------------ | -------------- |
| 1 | Sponsor                 | Sends **50 000 XOF** via Wave                                                            | –                  | ≤10 s          |
| 2 | PSP                     | Webhook `status=ACCEPTED`                                                                | –                  | ≤500 ms        |
| 3 | **Minter Bot**          | Generates XRPL `Payment` **50 000 SOMA** → sponsor; inserts `MemoData="MINT:WAVE:TX123"` | Tx hash in ledger  | ≤5 s           |
| 4 | Proof‑of‑Reserve Oracle | Adds deposit to Merkle tree leaf; posts root hash in `ReserveOracle.hook`                | `SetHook` emit     | ≤30 s          |
| 5 | Dashboard               | Grafana panel updates “Backed 100.12 %”                                                  | –                  | ≤60 s          |
| 6 | User                    | Receives SMS + XRPLScan link                                                             | –                  | ≤2 min         |

### 2.3 Reserve Yield Allocations

Annualised 90‑day T‑Bill yield (\~3.5 %) produces predictable inflow.  Policy: **80 %** of net interest **auto‑routes** to *Scholarship Grant Pool* each quarter; **20 %** covers auditing & ops.

### 2.4 Fee Model (Exploded)

* **Mint fee 0.20 %** = PSP <0.15 %> + 0.05 % protocol surplus.
* **Redeem fee 0.30 %** = Bank wire <0.20 %> + 0.10 % protocol surplus.
* **Net margin** channels 50/50 into (1) grant pool top‑up, (2) OPEX hedge.

### 2.5 Governance (Deep‑dive)

| Layer                   | Pre‑2027                        | Post‑2027 (DAO Phase α)               |
| ----------------------- | ------------------------------- | ------------------------------------- |
| Board                   | 5 members, majority independent | DAO Treasury Council (token‑weighted) |
| Reserve Policy          | Board vote monthly              | On‑chain vote, 7‑day delay            |
| Smart‑contract Upgrades | Multi‑sig 4‑of‑7                | DAO vote + 48 h timelock              |
| Emergency Pause         | 4‑of‑7 multisig                 | DAO Security Council (quorum 4)       |

### 2.6 Regulatory Status (Verbose)

1. **Benin:** Licence dossier filed 15 Apr 2025; BCEAO sandbox slot Q3‑2025; deposit‑insurance MoU signed.
2. **EU MiCA:** White‑paper pre‑notification submitted to AMF (France) 30 Apr 2025; seeking EMT status; 2 % capital buffer parked at Caisse des Dépôts.
3. **US:** FinCEN MSB; no “significant” EMT activity → no OCC special purpose charter needed.

*(White‑paper continues with sections on Legal Opinion excerpts, ESG alignment, Carbon footprint, Accessibility features, etc.)*

---

## 3  Technical & Integration Dossier (XRPL) – Super‑Verbose

| Parameter          | Value                                                                      | Commentary                                         |
| ------------------ | -------------------------------------------------------------------------- | -------------------------------------------------- |
| **Currency Code**  | `534F4D41` (ASCII “SOMA”)                                                  | 4 chars → no trailing spaces, keeps code clean     |
| **Decimals**       | 2                                                                          | Mirrors “centime” precision; avoids dust           |
| **Issuer Address** | `rSOMAissuerAGqnAnw8F8g5uAPQi2wZ2ub`                                       | Master key disabled post‑genesis                   |
| **Hot Minters**    | `rMintAlpha...`, `rMintBeta...`                                            | Daily cap 10 M each; signer list rotated quarterly |
| **Flags**          | `RequireAuth`, `DisallowXRP`, `AllowClawback`                              | Clawback only for legal compulsion                 |
| **Hooks**          | `ReserveOracle.hook` (Proof‑of‑Reserves), `ScholarTopUp.hook` (yield flow) | WASM audited by CertiK                             |
| **Explorer**       | [https://xrpscan.com/token/SOMA](https://xrpscan.com/token/SOMA)           | Custom badge “ED‑Stable”                           |

### 3.1 Network Diagram

*(ASCII diagram omitted in text)*  Three rippled validator nodes in Cotonou, Lagos, Paris + 2 watcher nodes on AWS.

### 3.2 Comprehensive Mint Algorithm (Python)

```python
from decimal import Decimal
from xrpl.models.transactions import Payment, TrustSet
from xrpl.transaction import safe_sign_and_autofill_transaction as sign_tx

async def mint_soma(user_addr: str, amount_xof: int, psp_ref: str):
    """Mints SOMA after PSP webhook confirmation."""
    soma_val = Decimal(amount_xof) / 100  # centime → SOMA
    payment = Payment(
        account=MINTER_WALLET.classic_address,
        destination=user_addr,
        amount={"currency": "SOMA", "issuer": ISSUER_ADDR, "value": f"{soma_val:.2f}"},
        memos=[{"MemoData": f"MINT:{psp_ref}".encode().hex()}]
    )
    stx = sign_tx(payment, MINTER_WALLET, XRPL_CLIENT)
    result = send_reliable_submission(stx, XRPL_CLIENT)
    post_to_reserve_oracle(amount_xof, result.result["hash"])
    return result
```

### 3.3 Proof‑of‑Reserves Merkle Tree Logic (Pseudo‑Go)

```go
func UpdateRoot(deposit int, txHash string) {
    leaf := sha256(NewLeaf(deposit, txHash))
    merkle.Lock()
    merkle.Append(leaf)
    root := merkle.Root()
    merkle.Unlock()
    SubmitHookSet(root) // pushes to ReserveOracle.hook
}
```

### 3.4 Exchange Integration Cheat‑Sheet

* **Cold wallet**: XRPL address + destination tag 1001.
* **Min deposit**: 10 SOMA (else dust).
* **Confirmations**: 1 ledger (\~4 s).
* **Currency code**: `534F4D41`.
* **Clawback policy**: only under court order; event emitted `CLAW:<case#>`.

---

## 4  Reserve Architecture & Proof‑of‑Reserves (Verbose)

1. **Banking Layer** – Dual escrow accounts; SWIFT gpi “UETR” captured in memo fields to link fiat leg to XRPL mint tx.
2. **Treasury Layer** – Custody at BNP Paribas Securities; daily MT 535 reconciliations.
3. **Liquidity Buffer** – Wave Money “float” wallet 3 bn XOF; reconciled via REST every 15 min.
4. **Attestation Pipeline** – Mazars signs Merkle root each month, uploads PDF + root to IPFS; `ReserveOracle.hook` stores IPFS CID & root hash in hook state.
5. **Real‑time Dashboard** – Grafana + Prometheus scraping hook state; shows *Backed 100.07 %, updated 42 s ago*.

---

## 5  Fiat On/Off Ramps – Sequence Diagrams & Fee Tables

### 5.1 XOF Ramp (Wave)

*(Mermaid sequence diagram omitted)* – End‑to‑end <25 s.

### 5.2 EUR Ramp (SEPA Instant)

Fee table: 0.21 % all‑in; settlement <10 s; max ticket €100 k.

### 5.3 USD Ramp (ACH / FedWire / USDC)

*Same as earlier but with Mint/Burn Bot hedging via Bitrue’s USDC/XRP pair to neutralise FX.*

---

## 6  Governance, Compliance & Licensing (Ultra‑Verbose)

* **Legal Structure**: **SOMA Trust S.A.** – a Special Purpose Vehicle under OHADA law; trust deed locks reserves for exclusive benefit of holders.
* **Board Charter**: 30‑page charter; committees: Audit, Risk, CSR.
* **Audit Firms**: Financial – Mazars; Smart‑contract – CertiK; Cyber – NCC Group.
* **Licences Road‑map**: EME (Benin) filed, MiCA EMT filing Q2 2026 (post‑launch), application for Canadian MSB Q4 2026.

---

## 7  Road‑map & KPI Dashboard

*(Gantt chart omitted)*  Key Objective Q4 2025: **500 M SOMA** in circulation, **1 % tuition subsidy** autopaid.

---

## 8  Risk Register – Condensed Table

*(Full 36‑risk CSV available in repo)*  Monte‑Carlo sim: 99 % VaR on peg deviation ≤0.22 % for 10‑day horizon.

---

## 9  References & Annexes

* BCEAO Circulars, MiCA Articles, Ripple Hooks RFCs, sample ISO 20022 pain.001 messages, full Clifford Chance legal opinion, CertiK security report (draft).

---

*End of SOMA Stablecoin Package v3.0 – Hyper‑Verbose Edition*
