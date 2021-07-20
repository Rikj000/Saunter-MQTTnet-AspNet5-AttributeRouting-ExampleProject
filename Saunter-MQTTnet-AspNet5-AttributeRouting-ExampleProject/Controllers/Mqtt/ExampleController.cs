#region Using Imports
using Microsoft.Extensions.Logging;
using MQTTnet.AspNetCore.AttributeRouting;
using Saunter.Attributes;
using System.Text;
using System.Threading.Tasks;
using System;
using MQTTnet;
using Saunter_MQTTnet_AspNet5_AttributeRouting_ExampleProject.Models;
#endregion Using Imports

namespace Saunter_MQTTnet_AspNet5_AttributeRouting_ExampleProject.Controllers.Mqtt
{
    [AsyncApi] // Tells Saunter to scan the Controller
    [MqttController] // Generate MQTT Attribute Routing for the Controller
    [MqttRoute(nameof(ExampleController))] // Defines the Route Prefix for the Controller
    public class ExampleController : MqttBaseController // Inherit from MqttBaseController for convenience functions
    {
        #region Variable Declarations
        private const string Prefix = nameof(ExampleController) + "/"; // Defines the Route Prefix for the Topics
        private const string Pub = "publish/";
        private const string Sub = "subscribe/";
        private readonly AppSettings _appSettings;
        private readonly ILogger<ExampleController> _logger;
        #endregion Variable Declarations
        
        #region Publish & Subscribe Topic Declarations
        private const string PubTest = Pub + "test";
        private const string PubWeatherReport = Pub + "{zipCode:int}/temperature";
        #endregion Publish & Subscribe Topic Declarations
        
        // Initialize the MQTT Controller with full dependency injection support (Like normal AspNetCore controllers)
        public ExampleController(AppSettings appSettings, ILogger<ExampleController> logger)
        {
            _appSettings = appSettings;
            _logger = logger;
        }
        
        #region Publish & Subscribe Topics
        
        [MqttRoute(PubTest)] // Generate MQTT Attribute Routing for this Topic
        [Channel(Prefix + PubTest)] // Create a Channel & Generate AsyncAPI Documentation
        [PublishOperation(Summary = "Publishes a 'Test' Payload to the '" + Prefix + PubTest + "' Topic.")]
        public Task PublishTest()
        {
            var payloadMessage = BitConverter.ToString(Message.Payload);
            Console.WriteLine("Test Payload Received! Payload String Content: " +  Message.Payload);
            return Ok();
        }
        
        [MqttRoute(PubWeatherReport)] // Generate MQTT Attribute Routing for this Topic
        [Channel(Prefix + Pub + "+/temperature")] // Create a Channel & Generate AsyncAPI Documentation
        [PublishOperation(
            Summary = "Publishes a 'WeatherReport' Payload to the '" + Prefix + PubWeatherReport + "' Topic.")]
        public Task PublishWeatherReport(int zipCode)
        {
            // We have access to the MqttContext
            if (zipCode != 90210) { MqttContext.CloseConnection = true; }

            // We have access to the raw message
            var temperature = int.Parse(Encoding.ASCII.GetString(Message.Payload));
            _logger.LogInformation($"It's {temperature} degrees in Hollywood");

            // Example validation
            return temperature is <= 0 or >= 130 ? BadMessage() : Ok();
        }
        
        #endregion Publish & Subscribe Topics
    }
}