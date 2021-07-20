#region Using Imports
using Microsoft.Extensions.Logging;
using MQTTnet.AspNetCore.AttributeRouting;
using Saunter.Attributes;
using System.Threading.Tasks;
using MQTTnet;

#endregion Using Imports

namespace Saunter_MQTTnet_AspNet5_AttributeRouting_ExampleProject.Controllers.Mqtt
{
    [AsyncApi] // Tells Saunter to scan the Controller
    [MqttController] // Generate MQTT Attribute Routing for the Controller 
    public class CatchAllController : MqttBaseController // Inherit from MqttBaseController for convenience functions
    {
        #region Variable Declarations
        private const string WildCard = "{*topic}";
        private readonly ILogger<CatchAllController> _logger;
        #endregion Variable Declarations
        
        // Initialize the MQTT Controller with full dependency injection support (Like normal AspNetCore controllers)
        public CatchAllController(ILogger<CatchAllController> logger)
        {
            _logger = logger;
        }

        [MqttRoute(WildCard)] // Generate MQTT Attribute Routing for this Topic
        [Channel(WildCard)] // Create a Channel & Generate AsyncAPI Documentation
        [PublishOperation(typeof(Task), Summary = "Catches all Publishes done to the MQTTnet Broker.")]
        public Task WildCardMatchTopic(string topic)
        {
            // We have access to the MqttContext
            _logger.LogInformation($"Catch All Controller - Wildcard matched on Topic: '{topic}'");

            return Ok();
        }
    }
}