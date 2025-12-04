# RFRabbitMQRPCClient

> 🇪🇸 Español | 🇺🇸 [English Version](README.md)

**RFRabbitMQRPCClient** es un cliente .NET diseñado para consumir servicios **RPC (Remote Procedure Call)** sobre **RabbitMQ**.  
Forma parte del ecosistema **RFRabbitMQ**, permitiendo implementar comunicación síncrona request/response de manera simple y confiable.

---

## 🚀 Características
- Cliente RPC simplificado basado en RabbitMQ.
- Compatible con **.NET 8, .NET 9 y .NET 10**.
- Manejo automático de:
  - Conexiones y canales  
  - Colas de respuesta  
  - Correlation IDs  
  - Tiempos de espera  
- Integración directa con la librería base `RFRabbitMQ`.
- Ideal para microservicios que requieren respuestas inmediatas.

---

## 📦 Instalación

### NuGet
```bash
Install-Package RFRabbitMQRPCClient
```

### .NET CLI
```bash
dotnet add package RFRabbitMQRPCClient
```

---

## 🔧 Configuración

Ejemplo en `appsettings.json`:

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

Lectura de configuración:

```csharp
var config = builder.Configuration.GetSection("RabbitMQ").Get<RabbitMQConfig>();
```

---

## 🖥️ Ejemplo de uso

### Crear cliente
```csharp
var client = new RpcClient(config);
```

### Enviar solicitud
```csharp
string response = await client.CallAsync("Hola servidor!");
Console.WriteLine(response);
```

### Llamada tipada
```csharp
var response = await client.CallAsync<MyRequest, MyResponse>(
    new MyRequest { Id = 10 }
);
```

---

## 🔍 Versionado
Versión actual: **1.3.1**

---

## 🔗 Dependencias
- `RFRabbitMQ` ≥ 1.3.1

---

## 📄 Licencia
Licencia MIT.

---

## 🌐 Repositorio
https://github.com/fabianlucena/rfn-rabbitmq-rpc-client
