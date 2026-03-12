# OCPP Server Implementation Documentation
## AlfaCharge CPMS - Complete Guide

---

## Table of Contents
1. [Architecture Overview](#architecture-overview)
2. [Abstract Factory Pattern Implementation](#abstract-factory-pattern-implementation)
3. [OCPP Protocol Support](#ocpp-protocol-support)
4. [Project Structure](#project-structure)
5. [How It Works](#how-it-works)
6. [Consuming in Web API](#consuming-in-web-api)
7. [Adding New OCPP Actions](#adding-new-ocpp-actions)
8. [Testing Guide](#testing-guide)

---

## 1. Architecture Overview

The AlfaCharge CPMS (Charge Point Management System) implements a WebSocket-based OCPP server that can communicate with electric vehicle charging stations using both OCPP 1.6 and OCPP 2.0.1/2.1 protocols.

### High-Level Architecture

```
???????????????????????????????????????????????????????????????????
?                        AlfaCharge.Api                           ?
?  ????????????????        ????????????????                      ?
?  ? Controllers  ??????????   Program.cs ?                      ?
?  ?  (REST API)  ?        ?   (DI Setup) ?                      ?
?  ????????????????        ????????????????                      ?
?         ?                        ?                               ?
?         ?                        ?                               ?
?  ????????????????????????????????????????????                  ?
?  ?    OcppConnectionManager (Singleton)     ?                  ?
?  ?  - Tracks active WebSocket connections   ?                  ?
?  ?  - Maps ChargePointId ? OcppConnection   ?                  ?
?  ????????????????????????????????????????????                  ?
???????????????????????????????????????????????????????????????????
                            ?
                            ?
???????????????????????????????????????????????????????????????????
?                   AlfaCharge.OcppServer                        ?
?                                                                  ?
?  ??????????????????????????????????????????????????????        ?
?  ?         OcppWebSocketServer (Singleton)            ?        ?
?  ?  - Accepts WebSocket connections at /ocpp          ?        ?
?  ?  - Negotiates subprotocol (ocpp1.6 or ocpp2.0.1)  ?        ?
?  ?  - Creates per-connection DI scope                 ?        ?
?  ??????????????????????????????????????????????????????        ?
?                            ?                                     ?
?                            ?                                     ?
?  ??????????????????????????????????????????????????????        ?
?  ?      IOcppHandlerFactory (Abstract Factory)        ?        ?
?  ?  ????????????????????   ????????????????????      ?        ?
?  ?  ? Ocpp16Factory    ?   ? Ocpp21Factory    ?      ?        ?
?  ?  ? (OCPP 1.6)       ?   ? (OCPP 2.0.1/2.1) ?      ?        ?
?  ?  ????????????????????   ????????????????????      ?        ?
?  ??????????????????????????????????????????????????????        ?
?                            ?                                     ?
?                            ?                                     ?
?  ??????????????????????????????????????????????????????        ?
?  ?         OcppMessageRouter (Scoped)                 ?        ?
?  ?  - Routes incoming OCPP messages                   ?        ?
?  ?  - Dispatches to versioned handlers                ?        ?
?  ?  - Logs all messages to database                   ?        ?
?  ??????????????????????????????????????????????????????        ?
?                            ?                                     ?
?                            ?                                     ?
?  ??????????????????????????????????????????????????????        ?
?  ?         Versioned Handlers (Scoped)                ?        ?
?  ?  - Ocpp16BootNotificationHandler                   ?        ?
?  ?  - Ocpp16TransactionHandler                        ?        ?
?  ?  - Ocpp16RemoteOpsHandler                          ?        ?
?  ?  - Ocpp201BootNotificationHandler                  ?        ?
?  ?  - Ocpp201TransactionHandler                       ?        ?
?  ?  - Ocpp201RemoteOpsHandler                         ?        ?
?  ?  ... and more                                      ?        ?
?  ??????????????????????????????????????????????????????        ?
???????????????????????????????????????????????????????????????????
                            ?
                            ?
???????????????????????????????????????????????????????????????????
?              AlfaCharge.Infrastructure                          ?
?  ??????????????????????????????????????????????????????        ?
?  ?         ApplicationDbContext (EF Core)             ?        ?
?  ?  - Stores OCPP logs, transactions, charge points   ?        ?
?  ?  - SQL Server database                             ?        ?
?  ??????????????????????????????????????????????????????        ?
???????????????????????????????????????????????????????????????????
```

---

## 2. Abstract Factory Pattern Implementation

### What is Abstract Factory Pattern?

The Abstract Factory pattern provides an interface for creating families of related objects without specifying their concrete classes. In our OCPP implementation, it allows us to support multiple OCPP protocol versions (1.6 and 2.0.1/2.1) without the router needing to know about version-specific implementation details.

### Why Use Abstract Factory Here?

1. **Protocol Versioning**: OCPP 1.6 and 2.0.1/2.1 have different message structures and action names
2. **Extensibility**: Easy to add support for future OCPP versions (e.g., 2.2)
3. **Separation of Concerns**: Router logic stays clean and version-agnostic
4. **Type Safety**: Compile-time guarantees that correct handler types are used

### Implementation Structure

```csharp
// Abstract Factory Interface
public interface IOcppHandlerFactory
{
    IBootNotificationHandler CreateBootNotificationHandler();
    IHeartbeatHandler CreateHeartbeatHandler();
    IAuthorizeHandler CreateAuthorizeHandler();
    IStatusNotificationHandler CreateStatusNotificationHandler();
    
    // Transactions
    IOcpp16TransactionHandler Create16TransactionHandler();
    IOcpp201TransactionHandler Create201TransactionHandler();
    
    // Remote Operations
    IRemoteOps16 CreateRemoteOps16();
    IRemoteOps201 CreateRemoteOps201();
    
    // ... more handler creators
}

// Concrete Factory for OCPP 1.6
public class Ocpp16HandlerFactory : IOcppHandlerFactory
{
    private readonly IServiceProvider _provider;
    
    public IBootNotificationHandler CreateBootNotificationHandler() 
        => _provider.GetRequiredService<Ocpp16BootNotificationHandler>();
    
    public IRemoteOps16 CreateRemoteOps16() 
        => _provider.GetRequiredService<IRemoteOps16>();
    
    public IRemoteOps201 CreateRemoteOps201() 
        => throw new NotImplementedException("2.x not supported by 1.6 factory");
}

// Concrete Factory for OCPP 2.0.1/2.1
public class Ocpp21HandlerFactory : IOcppHandlerFactory
{
    private readonly IServiceProvider _provider;
    
    public IBootNotificationHandler CreateBootNotificationHandler() 
        => _provider.GetRequiredService<Ocpp21BootNotificationHandler>();
    
    public IRemoteOps16 CreateRemoteOps16() 
        => throw new NotImplementedException("1.6 not supported by 2.x factory");
    
    public IRemoteOps201 CreateRemoteOps201() 
        => _provider.GetRequiredService<IRemoteOps201>();
}
```

### How Factory is Selected

```csharp
// In OcppWebSocketServer.cs
var version = OcppSubprotocols.Parse(subProtocol); // "ocpp1.6" or "ocpp2.0.1"

IOcppHandlerFactory handlerFactory = version switch
{
    OcppProtocolVersion.Ocpp16 => new Ocpp16HandlerFactory(services),
    OcppProtocolVersion.Ocpp201 => new Ocpp21HandlerFactory(services),
    _ => throw new NotSupportedException($"Protocol version {version} not supported.")
};
```

---

## 3. OCPP Protocol Support

### OCPP 1.6 vs OCPP 2.0.1/2.1 Key Differences

| Feature | OCPP 1.6 | OCPP 2.0.1/2.1 |
|---------|----------|----------------|
| **Remote Start** | `RemoteStartTransaction` | `RequestStartTransaction` |
| **Remote Stop** | `RemoteStopTransaction` | `RequestStopTransaction` |
| **Diagnostics** | `GetDiagnostics` | `GetLog` |
| **Status** | `StatusNotification` (connector-based) | `StatusNotification` (EVSE-based) |
| **Transactions** | `StartTransaction`, `StopTransaction` | `TransactionEvent` (unified) |
| **Authorization** | `Authorize` | `Authorize` (enhanced) |
| **Configuration** | `GetConfiguration`, `ChangeConfiguration` | Variable-based configuration |

### Currently Implemented Actions

#### CP ? CSMS (Charge Point to Central System)
? OCPP 1.6 & 2.0.1/2.1:
- BootNotification
- Heartbeat
- Authorize
- StatusNotification

? OCPP 1.6 Only:
- StartTransaction
- StopTransaction
- MeterValues
- DiagnosticsStatusNotification
- FirmwareStatusNotification

? OCPP 2.0.1/2.1 Only:
- TransactionEvent
- LogStatusNotification
- NotifyReport
- FirmwareStatusNotification (enhanced)

#### CSMS ? CP (Central System to Charge Point)
? OCPP 1.6:
- RemoteStartTransaction
- RemoteStopTransaction
- Reset
- GetDiagnostics
- UpdateFirmware
- UnlockConnector
- GetConfiguration
- ChangeConfiguration
- GetLocalListVersion
- SendLocalList
- ReserveNow
- CancelReservation
- ClearChargingProfile
- SetChargingProfile
- GetCompositeSchedule
- TriggerMessage

? OCPP 2.0.1/2.1:
- RequestStartTransaction
- RequestStopTransaction
- Reset
- GetLog (replaces GetDiagnostics)
- UpdateFirmware
- UnlockConnector
- SendLocalList
- ReserveNow
- CancelReservation
- ClearChargingProfile
- SetChargingProfile
- GetChargingProfiles
- GetCompositeSchedule
- ClearedChargingLimit
- TriggerMessage

---

## 4. Project Structure

```
AlfaCharge Solution
?
??? AlfaCharge.Api (ASP.NET Core Web API)
?   ??? Controllers/
?   ?   ??? ChargePointController.cs (REST API for CSMS?CP operations)
?   ??? Program.cs (DI configuration, WebSocket endpoint mapping)
?   ??? appsettings.json
?
??? AlfaCharge.OcppServer (OCPP Protocol Library)
?   ??? Contracts/
?   ?   ??? AbstractFactory/
?   ?   ?   ??? IOcppHandlerFactory.cs
?   ?   ??? IBootNotificationHandler.cs
?   ?   ??? IRemoteOps16.cs
?   ?   ??? IRemoteOps201.cs
?   ?   ??? ILocalAuthListOps16.cs
?   ?   ??? ILocalAuthListOps201.cs
?   ?   ??? IReservationOps16.cs
?   ?   ??? IReservationOps201.cs
?   ?   ??? IChargingProfileOps16.cs
?   ?   ??? IChargingProfileOps201.cs
?   ?   ??? ... more handler interfaces
?   ?
?   ??? Factory/
?   ?   ??? Ocpp16HandlerFactory.cs
?   ?   ??? Ocpp21HandlerFactory.cs
?   ?
?   ??? Versioned_Handlers/
?   ?   ??? Ocpp16/
?   ?   ?   ??? Ocpp16BootNotificationHandler.cs
?   ?   ?   ??? Ocpp16RemoteOpsHandler.cs
?   ?   ?   ??? Ocpp16LocalListHandler.cs
?   ?   ?   ??? ... more 1.6 handlers
?   ?   ??? Ocpp201/
?   ?       ??? Ocpp201BootNotificationHandler.cs
?   ?       ??? Ocpp201RemoteOpsHandler.cs
?   ?       ??? Ocpp201LocalListHandler.cs
?   ?       ??? ... more 2.x handlers
?   ?
?   ??? WebSockets/
?   ?   ??? OcppWebSocketServer.cs (WebSocket endpoint handler)
?   ?   ??? OcppConnectionManager.cs (Connection tracking)
?   ?   ??? OcppConnection.cs (Per-connection state)
?   ?
?   ??? Messages/
?   ?   ??? OcppMessageRouter.cs (Message routing logic)
?   ?
?   ??? Services/
?       ??? IOcppLogWriter.cs
?       ??? EfOcppLogWriter.cs (Logs to database)
?
??? AlfaCharge.Infrastructure (Data Access)
?   ??? DB/
?       ??? ApplicationDbContext.cs
?       ??? Services/
?
??? AlfaCharge.Domain (Entities)
    ??? Entities/
        ??? OCPPLog.cs
        ??? BootNotification.cs
        ??? ...
```

---

## 5. How It Works

### Message Flow: CP ? CSMS (Incoming)

```
1. Charge Point connects via WebSocket
   ??> ws://your-server/ocpp/CP001
   
2. OcppWebSocketServer accepts connection
   ??> Negotiates subprotocol: ocpp1.6 or ocpp2.0.1
   
3. Creates per-connection DI scope
   ??> Resolves ApplicationDbContext, IOcppLogWriter, etc.
   
4. Selects appropriate factory
   ??> Ocpp16HandlerFactory or Ocpp21HandlerFactory
   
5. Creates OcppMessageRouter with factory
   
6. Charge Point sends OCPP message
   ??> [2, "msg-123", "BootNotification", {...}]
   
7. Router parses message
   ??> Logs to database (inbound)
   ??> Extracts action: "bootnotification"
   ??> Normalizes to lowercase
   
8. Router dispatches to handler
   ??> handler = factory.CreateBootNotificationHandler()
   ??> response = await handler.HandleAsync(request)
   
9. Router builds CALLRESULT
   ??> [3, "msg-123", {...response...}]
   ??> Logs to database (outbound)
   
10. Sends response back to charge point
```

### Message Flow: CSMS ? CP (Outgoing)

```
1. API receives REST request
   ??> POST /api/chargepoint/CP001/remote-start
   
2. Controller resolves OcppConnectionManager
   
3. Gets connection by chargePointId
   ??> connection = manager.TryGet("CP001")
   
4. Determines protocol version
   ??> connection.ProtocolVersion ? Ocpp16 or Ocpp201
   
5. Calls appropriate handler via factory
   ??> For 1.6: RemoteStartTransactionAsync
   ??> For 2.x: RequestStartTransactionAsync
   
6. Handler uses OcppConnection.SendCallAsync
   ??> [2, "msg-456", "RemoteStartTransaction", {...}]
   ??> Logs to database
   
7. Waits for charge point response (with timeout)
   
8. Router receives CALLRESULT from CP
   ??> [3, "msg-456", {...response...}]
   ??> Completes pending TaskCompletionSource
   
9. Handler returns response to controller
   
10. Controller returns HTTP response to API caller
```

---

## 6. Consuming in Web API

### Example 1: Remote Start Transaction

```csharp
[HttpPost("chargepoint/{id}/remote-start")]
public async Task<IActionResult> RemoteStart(
    string id, 
    [FromBody] RemoteStartRequest request)
{
    // Get connection
    if (!_connections.TryGet(id, out var conn) || conn is null)
        return NotFound($"Charge point {id} not connected");
    
    // Check protocol version and call appropriate method
    if (conn.ProtocolVersion == OcppProtocolVersion.Ocpp16)
    {
        // OCPP 1.6: RemoteStartTransaction
        var handler = /* resolve IRemoteOps16 from factory */;
        var response = await handler.RemoteStartTransactionAsync(id, request, HttpContext.RequestAborted);
        return Ok(response);
    }
    else
    {
        // OCPP 2.0.1/2.1: RequestStartTransaction
        var handler = /* resolve IRemoteOps201 from factory */;
        var response = await handler.RequestStartTransactionAsync(id, request, HttpContext.RequestAborted);
        return Ok(response);
    }
}
```

### Example 2: Get Connected Charge Points

```csharp
[HttpGet("chargepoints")]
public IActionResult GetConnected()
{
    var result = _connections.ConnectedIds
        .Select(id => _connections.TryGet(id, out var conn) && conn != null
            ? new { ChargePointId = id, Protocol = conn.ProtocolVersion.ToString() }
            : new { ChargePointId = id, Protocol = "Unknown" })
        .ToList();
    
    return Ok(result);
}
```

### Example 3: Trigger Message (Testing)

```csharp
[HttpPost("chargepoint/{id}/trigger")]
public async Task<IActionResult> TriggerMessage(
    string id,
    [FromBody] TriggerRequest request) // { messageType: "StatusNotification" }
{
    if (!_connections.TryGet(id, out var conn) || conn is null)
        return NotFound($"Charge point {id} not connected");
    
    try
    {
        var payload = new { requestedMessage = request.MessageType };
        var response = await conn.SendCallAsync("TriggerMessage", payload, TimeSpan.FromSeconds(30));
        return Ok(new { success = true, response });
    }
    catch (Exception ex)
    {
        return StatusCode(500, new { error = ex.Message });
    }
}
```

---

## 7. Adding New OCPP Actions

### Step-by-Step Guide

#### Step 1: Define Handler Interface

```csharp
// In Contracts/INewFeatureOps16.cs
public interface INewFeatureOps16
{
    Task<string> SomeNewActionAsync(string chargePointId, object payload, CancellationToken ct);
}

// In Contracts/INewFeatureOps201.cs
public interface INewFeatureOps201
{
    Task<string> SomeNewActionAsync(string chargePointId, object payload, CancellationToken ct);
}
```

#### Step 2: Add to Factory Interface

```csharp
// In IOcppHandlerFactory.cs
public interface IOcppHandlerFactory
{
    // ...existing methods...
    
    INewFeatureOps16 CreateNewFeatureOps16();
    INewFeatureOps201 CreateNewFeatureOps201();
}
```

#### Step 3: Implement Concrete Handlers

```csharp
// In Versioned_Handlers/Ocpp16/Ocpp16NewFeatureHandler.cs
public class Ocpp16NewFeatureHandler : INewFeatureOps16
{
    private readonly OcppConnectionManager _connections;
    private readonly JsonSerializerOptions _jsonOptions;
    
    public async Task<string> SomeNewActionAsync(string cpId, object payload, CancellationToken ct)
    {
        if (!_connections.TryGet(cpId, out var conn))
            throw new InvalidOperationException($"CP {cpId} not connected");
        
        return await conn.SendCallAsync("SomeNewAction", payload, TimeSpan.FromSeconds(30), ct);
    }
}
```

#### Step 4: Update Factories

```csharp
// In Ocpp16HandlerFactory.cs
public INewFeatureOps16 CreateNewFeatureOps16() 
    => _provider.GetRequiredService<INewFeatureOps16>();

public INewFeatureOps201 CreateNewFeatureOps201() 
    => throw new NotImplementedException("2.x not supported");

// In Ocpp21HandlerFactory.cs
public INewFeatureOps16 CreateNewFeatureOps16() 
    => throw new NotImplementedException("1.6 not supported");

public INewFeatureOps201 CreateNewFeatureOps201() 
    => _provider.GetRequiredService<INewFeatureOps201>();
```

#### Step 5: Register in DI

```csharp
// In Program.cs
builder.Services.AddScoped<INewFeatureOps16, Ocpp16NewFeatureHandler>();
builder.Services.AddScoped<INewFeatureOps201, Ocpp201NewFeatureHandler>();
```

#### Step 6: Add Router Case (if CP?CSMS action)

```csharp
// In OcppMessageRouter.cs HandleCallAsync method
case "somenewaction":
{
    if (_connection.ProtocolVersion == OcppProtocolVersion.Ocpp16)
    {
        var req = JsonSerializer.Deserialize<Ocpp16SomeNewRequest>(payload.GetRawText());
        var handler = _handlerFactory.CreateNewFeatureOps16();
        var resp = await handler.HandleAsync(cpId, req);
        // ... log and return response
    }
    else
    {
        // Handle 2.x version
    }
    break;
}
```

---

## 8. Testing Guide

### Testing with Browser Console (WebSocket)

```javascript
// Connect to OCPP server
const ws = new WebSocket('ws://localhost:5000/ocpp/TEST-CP-001', 'ocpp1.6');

ws.onopen = () => {
    console.log('Connected!');
    
    // Send BootNotification
    const bootNotif = [
        2,  // CALL
        "msg-001",
        "BootNotification",
        {
            chargePointModel: "Test Model",
            chargePointVendor: "Test Vendor",
            firmwareVersion: "1.0.0"
        }
    ];
    ws.send(JSON.stringify(bootNotif));
};

ws.onmessage = (event) => {
    console.log('Received:', JSON.parse(event.data));
};

// Send Heartbeat
setTimeout(() => {
    const heartbeat = [2, "msg-002", "Heartbeat", {}];
    ws.send(JSON.stringify(heartbeat));
}, 5000);
```

### Testing CSMS?CP Operations via REST API

```bash
# Get connected charge points
curl http://localhost:5000/api/chargepoint

# Remote start transaction (OCPP 1.6)
curl -X POST http://localhost:5000/api/chargepoint/TEST-CP-001/remote-start \
  -H "Content-Type: application/json" \
  -d '{
    "connectorId": 1,
    "idTag": "USER123"
  }'

# Reset charge point
curl -X POST http://localhost:5000/api/chargepoint/TEST-CP-001/reset \
  -H "Content-Type: application/json" \
  -d '{
    "type": "Soft"
  }'
```

### Unit Testing Example

```csharp
[Fact]
public async Task Ocpp16RemoteStart_ShouldSendCorrectAction()
{
    // Arrange
    var mockConnection = new Mock<OcppConnection>();
    var manager = new OcppConnectionManager();
    manager.TryAdd(mockConnection.Object);
    
    var handler = new Ocpp16RemoteOpsHandler(manager, new JsonSerializerOptions());
    
    // Act
    await handler.RemoteStartTransactionAsync("CP001", new { connectorId = 1 }, CancellationToken.None);
    
    // Assert
    mockConnection.Verify(c => c.SendCallAsync(
        "RemoteStartTransaction", 
        It.IsAny<object>(), 
        It.IsAny<TimeSpan>(), 
        It.IsAny<CancellationToken>()), 
        Times.Once);
}
```

---

## Summary

This implementation provides:

? **Protocol Abstraction**: Abstract Factory pattern cleanly separates OCPP 1.6 and 2.0.1/2.1 logic  
? **Extensibility**: Easy to add new OCPP actions or versions  
? **Type Safety**: Compile-time guarantees for handler resolution  
? **Scalability**: Singleton connection manager, scoped per-connection handlers  
? **Observability**: All messages logged to database  
? **RESTful Control**: Web API controllers for CSMS-initiated operations  
? **Testing**: Simple WebSocket and REST API testing  

For questions or additional features, refer to OCPP specifications or extend the patterns described above.
