# RFRabbitMQRPCClient

> 🇺🇸 English | 🇪🇸 [Versión en Español](README.es.md)

**RFRabbitMQRPCClient** is a lightweight .NET client designed to consume **RPC (Remote Procedure Call)** services over **RabbitMQ**.  
It is part of the **RFRabbitMQ ecosystem**, providing a simple, reliable, and strongly‑typed way to perform synchronous request/response operations.

---

## 🚀 Features
- Simplified RPC client built on RabbitMQ.
- Supports **.NET 8, .NET 9, and .NET 10**.
- Manages:
  - Connections and channels  
  - Reply queues  
  - Correlation IDs  
  - Timeouts  
- Works seamlessly with the `RFRabbitMQ` base library.
- Ideal for microservices requiring synchronous responses.

---

## 📦 Installation

### NuGet
```bash
Install-Package RFRabbitMQRPCClient
```

### .NET CLI
```bash
dotnet add package RFRabbitMQRPCClient
```

---

## 🔧 Configuration

Add to `appsettings.json`:

```json
{
  "RabbitMQ": {
    "HostName": "localhost",
    "UserName": "guest",
    "Password": "guest",
    "QueueName": "rfrpc.queue",
    "TimeoutSeconds": 10
  }
}
```

Read configuration:

```csharp
var config = builder.Configuration.GetSection("RabbitMQ").Get<RabbitMQConfig>();
```

---

## 🖥️ Usage Example

### Initialize client
```csharp
var client = new RpcClient(config);
```

### Send request
```csharp
string response = await client.CallAsync("Hello server!");
Console.WriteLine(response);
```

### Strongly-typed call
```csharp
var response = await client.CallAsync<MyRequest, MyResponse>(
    new MyRequest { Id = 10 }
);
```

---

## 🔍 Versioning
Current version: **1.3.1**

---

## 🔗 Dependencies
- `RFRabbitMQ` ≥ 1.3.1

---

## 📄 License
MIT License.

---

## 🌐 Repository
https://github.com/fabianlucena/rfn-rabbitmq-rpc-client
