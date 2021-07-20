# Saunter MQTTnet AspNet5 AttributeRouting ExampleProject 
An example project that implements following packages in a ASP.NET 5 MQTT Broker/Server:

## Current Implemented Package Status:
- [**MQTTnet**](https://github.com/chkr1011/MQTTnet) - `v3.0.16` - Working
- [**MQTTnet - AspNetCore - AttributeRouting**](https://github.com/Atlas-LiftTech/MQTTnet.AspNetCore.AttributeRouting) - `v3.0.16` (Forked, see [NuGet](https://www.nuget.org/packages/MQTTnet.AspNetCore.AttributeRouting.Forked/)) - Working
- [**Saunter**](https://github.com/tehmantra/saunter) - `v0.3.1` - Implemented, UI & generated json hosted, but not generating MQTT Publish/Subscribe Topic Documentation.

## Some Notes:
- Tested with [**MQTTnet.App**](https://github.com/chkr1011/MQTTnet.App) as the client, configured as following:
  - Transport protocol: `WebSocket`
  - TLS version: `no TLS`
  - Host: `localhost:5000/mqtt`
  - Port: `5000`
- A [**CatchAllController**](https://github.com/Rikj000/Saunter-MQTTnet-AspNet5-AttributeRouting-ExampleProject/blob/main/Saunter-MQTTnet-AspNet5-AttributeRouting-ExampleProject/Controllers/Mqtt/CatchAllController.cs) has been implemented, this will log all MQTT related messages going in/out of the MQTTnet Broker/Server
- A (MQTT) [**ExampleController**](https://github.com/Rikj000/Saunter-MQTTnet-AspNet5-AttributeRouting-ExampleProject/blob/main/Saunter-MQTTnet-AspNet5-AttributeRouting-ExampleProject/Controllers/Mqtt/ExampleController.cs) has been implemented with following Publish Topics:
  - `ExampleController/publish/<PostalCode>/temperature`
    - PostalCode: `90210`
    - Payload: `int`
  - `ExampleController/publish/test`
    - Payload: `string`
- The [**MqttService**](https://github.com/Rikj000/Saunter-MQTTnet-AspNet5-AttributeRouting-ExampleProject/blob/main/Saunter-MQTTnet-AspNet5-AttributeRouting-ExampleProject/Services/MqttService.cs) has been implemented with following Subscribe Topic:
  - `MqttService/subscribe/kiss-message`
    - It will send a Payload every 5 minutes after a client has connected
    - Payload: `$"MQTTnet hosted on {frameworkName} is still running at {DateTime.Now}!"`
  - Here you can also configure various MQTT Broker related settings / handlers
- [**Startup**](https://github.com/Rikj000/Saunter-MQTTnet-AspNet5-AttributeRouting-ExampleProject/blob/main/Saunter-MQTTnet-AspNet5-AttributeRouting-ExampleProject/Startup.cs) is where most of the other Package configuration happens