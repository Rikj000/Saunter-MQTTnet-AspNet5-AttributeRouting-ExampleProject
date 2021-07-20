#region Using Imports
using MQTTnet.AspNetCore.AttributeRouting;
using MQTTnet.AspNetCore;
using MQTTnet.Client.Receiving;
using MQTTnet.Server;
using MQTTnet;
using System.Globalization;
using System.Reflection;
using System.Runtime.Versioning;
using System.Text;
using System.Threading.Tasks;
using System;
#endregion Using Imports

namespace Saunter_MQTTnet_AspNet5_AttributeRouting_ExampleProject.Services
{
    public class MqttService :
        IMqttServerConnectionValidator,
        IMqttApplicationMessageReceivedHandler,
        IMqttServerApplicationMessageInterceptor,
        IMqttServerStartedHandler,
        IMqttServerStoppedHandler,
        IMqttServerClientConnectedHandler,
        IMqttServerClientDisconnectedHandler,
        IMqttServerClientSubscribedTopicHandler,
        IMqttServerClientUnsubscribedTopicHandler
    {
        #region MQTT Service & Server Configuration

        #region Variable Declarations
        private static string _newLine = Environment.NewLine;
        public IMqttServer Server;
        #endregion Variable Declarations

        public void ConfigureMqttServerOptions(AspNetMqttServerOptionsBuilder options)
        {
            // Configure the MQTT Server options here
            options.WithoutDefaultEndpoint();
            options.WithConnectionValidator(this);
            options.WithApplicationMessageInterceptor(this);
            // Enable Attribute Routing
            // By default, messages published to topics that don't match any routes are rejected. 
            // Change this to true to allow those messages to be routed without hitting any controller actions.
            options.WithAttributeRouting(allowUnmatchedRoutes: true);
        }

        public void ConfigureMqttServer(IMqttServer mqtt)
        {
            Server = mqtt;
            mqtt.ApplicationMessageReceivedHandler = this;
            mqtt.StartedHandler = this;
            mqtt.StoppedHandler = this;
            mqtt.ClientConnectedHandler = this;
            mqtt.ClientDisconnectedHandler = this;
            mqtt.ClientSubscribedTopicHandler = this;
            mqtt.ClientUnsubscribedTopicHandler = this;
        }

        #endregion MQTT Service & Server Configuration

        #region Validation & Interception

        public Task ValidateConnectionAsync(MqttConnectionValidatorContext context)
        {
            return Task.Run(() => { Console.WriteLine("ValidateConnectionAsync Handler Triggered"); });
        }

        public Task HandleApplicationMessageReceivedAsync(MqttApplicationMessageReceivedEventArgs eventArgs)
        {
            return Task.Run(() =>
            {
                Console.WriteLine($"{DateTime.Now.ToString(CultureInfo.InvariantCulture)} - MQTT Message Received:{_newLine}" +
                                  $"- Topic = {eventArgs.ApplicationMessage.Topic + _newLine}" + 
                                  $"- Payload = {Encoding.UTF8.GetString(eventArgs.ApplicationMessage.Payload) + _newLine}" +
                                  $"- QoS = {eventArgs.ApplicationMessage.QualityOfServiceLevel + _newLine}" + 
                                  $"- Retain = {eventArgs.ApplicationMessage.Retain + _newLine}");
            });
        }
        
        public Task InterceptApplicationMessagePublishAsync(MqttApplicationMessageInterceptorContext context)
        {
            return Task.Run(() => { Console.WriteLine("InterceptApplicationMessagePublishAsync Handler Triggered"); });
        }

        #endregion Validation & Interception

        #region Handle Server Actions

        public Task HandleServerStartedAsync(EventArgs eventArgs)
        {
            return Task.Run(() => { Console.WriteLine("HandleServerStartedAsync Handler Triggered"); });
        }

        public Task HandleServerStoppedAsync(EventArgs eventArgs)
        {
            return Task.Run(() => { Console.WriteLine("HandleServerStoppedAsync Handler Triggered"); });
        }

        #endregion Handle Server Actions

        #region Handle Client Actions

        public Task HandleClientConnectedAsync(MqttServerClientConnectedEventArgs eventArgs)
        {
            Task.Run(async () =>
            {
                Console.WriteLine("HandleClientConnectedAsync Handler Triggered");
                
                var frameworkName =
                    GetType().Assembly.GetCustomAttribute<TargetFrameworkAttribute>()?.FrameworkName;

                var msg = new MqttApplicationMessageBuilder()
                    .WithPayload($"MQTTnet hosted on {frameworkName} has started up!")
                    .WithTopic("MqttService/subscribe/kiss-message");

                while (true)
                {
                    try
                    {
                        await Server.PublishAsync(msg.Build());
                        msg.WithPayload($"MQTTnet hosted on {frameworkName} is still running at {DateTime.Now}!");
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                    }
                    finally
                    {
                        await Task.Delay(TimeSpan.FromMinutes(5));
                    }
                }
            });

            var clientId = eventArgs.ClientId;
            return Task.Run(() =>
            {
                Server.SubscribeAsync(clientId, "test");
                
                Console.WriteLine($"{DateTime.Now.ToString(CultureInfo.InvariantCulture)} " +
                                  $"MQTT Client Connected:{_newLine} - ClientID = {clientId + _newLine}");
            });
        }

        public Task HandleClientDisconnectedAsync(MqttServerClientDisconnectedEventArgs eventArgs)
        {
            return Task.Run(() => { Console.WriteLine("HandleClientDisconnectedAsync Handler Triggered"); });
        }

        public Task HandleClientSubscribedTopicAsync(MqttServerClientSubscribedTopicEventArgs eventArgs)
        {
            return Task.Run(() => { Console.WriteLine("ClientSubscribedTopicHandler Handler Triggered"); });
        }

        public Task HandleClientUnsubscribedTopicAsync(MqttServerClientUnsubscribedTopicEventArgs eventArgs)
        {
            return Task.Run(() => { Console.WriteLine("ClientSubscribedTopicHandler Handler Triggered"); });
        }

        #endregion Handle Client Actions
    }
}