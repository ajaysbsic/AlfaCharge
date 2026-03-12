# Real Charger Testing Guide (AlfaCharge)

This guide explains how to connect a physical charger to your AlfaCharge backend and validate that OCPP communication is working.

## 1. Source-verified endpoints in this project

From the current code:

- API host profile: `http://localhost:5100`
- Optional HTTPS profile: `https://localhost:7188`
- OCPP WebSocket endpoint base: `/ocpp`
- SignalR hub (Admin live updates): `/hub/ocpp`
- Accepted WebSocket subprotocols:
  - `ocpp1.6`
  - `ocpp2.0.1`

OCPP URL formats accepted by server:

- `ws://<host>:5100/ocpp/<chargePointId>`
- `ws://<host>:5100/ocpp`
- `wss://<host>:7188/ocpp/<chargePointId>` (only when HTTPS is running with a trusted cert)

Note: The server can auto-generate an unknown ID when no ID is provided, but for production and reliable operations always use `.../ocpp/<chargePointId>`.

## 2. Important localhost rule

A charger device must not be configured with `localhost` unless the charger software runs on the same machine as AlfaCharge.

- Wrong for a physical charger: `ws://localhost:5100/ocpp/CP001`
- Correct on same LAN: `ws://192.168.1.50:5100/ocpp/CP001`

## 3. Prerequisites

- AlfaCharge API project builds and runs.
- Windows machine hosting API has a stable LAN IP.
- Charger supports OCPP `1.6J` and/or `2.0.1` over WebSocket.
- You know your charger identifier (`chargePointId`).

## 4. Start backend with correct profile

From solution root:

```powershell
dotnet run --project .\AlfaCharge.Api\AlfaCharge.Api.csproj --launch-profile http
```

Expected listening URL:

- `http://localhost:5100`

If you need TLS and your environment supports it:

```powershell
dotnet run --project .\AlfaCharge.Api\AlfaCharge.Api.csproj --launch-profile https
```

Expected URLs:

- `https://localhost:7188`
- `http://localhost:5100`

## 5. Find host machine LAN IP

On the API machine:

```powershell
ipconfig
```

Use the active adapter IPv4 address, for example `192.168.1.50`.

## 6. Charger configuration values

Configure these values in charger/EVSE management UI:

- CSMS URL (LAN, no TLS):
  - `ws://192.168.1.50:5100/ocpp/CP001`
- CSMS URL (TLS):
  - `wss://your-public-domain/ocpp/CP001` or `wss://192.168.1.50:7188/ocpp/CP001` (lab only)
- Charge Point ID: `CP001` (must match URL segment if charger requires both)
- OCPP Protocol: `1.6J` or `2.0.1`
- WebSocket subprotocol:
  - `ocpp1.6` for OCPP 1.6J
  - `ocpp2.0.1` for OCPP 2.0.1

If your charger has separate fields for endpoint and ID:

- Endpoint: `ws://192.168.1.50:5100/ocpp`
- Charge Point ID: `CP001`

## 7. Open Windows firewall ports on API host

Run PowerShell as Administrator:

```powershell
New-NetFirewallRule -DisplayName "AlfaCharge API 5100" -Direction Inbound -Action Allow -Protocol TCP -LocalPort 5100
New-NetFirewallRule -DisplayName "AlfaCharge API 7188" -Direction Inbound -Action Allow -Protocol TCP -LocalPort 7188
```

Verify:

```powershell
Get-NetFirewallRule -DisplayName "AlfaCharge API 5100","AlfaCharge API 7188"
```

If you only use HTTP for LAN tests, port `5100` is enough.

## 8. Router/NAT setup for internet-based chargers

Only needed when charger is outside your LAN.

1. Reserve a static LAN IP for the API host (DHCP reservation).
2. In router port-forwarding:
   - External TCP `5100` -> `192.168.1.50:5100` (not recommended for production)
   - External TCP `443` -> reverse proxy/API TLS endpoint (recommended)
3. Prefer a domain name and TLS certificate for `wss://`.
4. Configure charger CSMS URL with public DNS:
   - `wss://csms.example.com/ocpp/CP001`

Production recommendation:

- Do not expose raw `ws://` over internet.
- Use `wss://` with a CA-trusted certificate.

## 9. Verify server is reachable before connecting charger

From another machine on same LAN:

```powershell
Test-NetConnection 192.168.1.50 -Port 5100
```

Expected: `TcpTestSucceeded : True`

Quick HTTP check to API:

```powershell
curl http://192.168.1.50:5100/swagger
```

## 10. Live validation checklist

After charger connects, verify all of the following:

1. API console logs show OCPP connection start with CP ID and subprotocol.
2. Admin UI loads at `http://localhost:5122`.
3. In Admin:
   - Stations/charge point shows as connected.
   - OCPP logs contain BootNotification/Heartbeat.
4. Remote command test from Admin/API (for connected CP):
   - Send a harmless command such as TriggerMessage/Reset (as per your policy).
   - Confirm charger response appears in OCPP logs.

## 11. Common failure patterns and fixes

- Symptom: Charger cannot connect, timeout.
  - Fix: use LAN/public IP, not `localhost`.

- Symptom: HTTP 400 unsupported subprotocol.
  - Fix: charger must request `ocpp1.6` or `ocpp2.0.1` exactly.

- Symptom: Connection refused.
  - Fix: API not running, wrong port, or firewall blocked.

- Symptom: Charger connects but appears as unknown ID.
  - Fix: provide explicit `/ocpp/<chargePointId>` or explicit CP ID in charger config.

- Symptom: Works on LAN, fails over internet.
  - Fix: router forwarding missing, ISP CGNAT, or TLS/domain issues.

## 12. Recommended production topology

- Public DNS: `csms.example.com`
- Reverse proxy on `443` with valid cert
- Forward/proxy WebSocket traffic to internal API (`http://127.0.0.1:5100`)
- Charger URL:
  - `wss://csms.example.com/ocpp/<chargePointId>`

This avoids exposing non-standard raw ports publicly and improves charger compatibility.

## 13. Quick copy-paste templates

Replace placeholders:

- LAN test URL:
  - `ws://<LAN_IP>:5100/ocpp/<CP_ID>`
- Public TLS URL:
  - `wss://<PUBLIC_DNS>/ocpp/<CP_ID>`
- Subprotocol:
  - `ocpp1.6` or `ocpp2.0.1`

---

If you want, I can also add an Nginx/IIS reverse-proxy sample config for `wss://csms.example.com/ocpp/*` routing to `http://localhost:5100`.
