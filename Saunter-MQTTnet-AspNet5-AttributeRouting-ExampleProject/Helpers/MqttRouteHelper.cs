using System.Text.RegularExpressions;

namespace Saunter_MQTTnet_AspNet5_AttributeRouting_ExampleProject.Helpers
{
    public static class MqttRouteHelper
    {
        public static string ParseSaunterMqttRoute(string mqttNetRoute)
        {
            var saunterRoute = mqttNetRoute;
            
            var singleEventWildcard = new Regex(@"{\w+:\w+}");
            if (singleEventWildcard.IsMatch(saunterRoute))
                saunterRoute = singleEventWildcard.Replace(saunterRoute, "+");

            var multiLevelWildcard = new Regex(@"{\*\w+}");
            if (multiLevelWildcard.IsMatch(saunterRoute))
                saunterRoute = multiLevelWildcard.Replace(saunterRoute, "#");
            
            return saunterRoute;
        }
    }
}