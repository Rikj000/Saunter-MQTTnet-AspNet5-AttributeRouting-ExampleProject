using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Namotion.Reflection;
using Saunter.AsyncApiSchema.v2;
using Saunter.Generation.Filters;

namespace Saunter_MQTTnet_AspNet5_AttributeRouting_ExampleProject.Filters
{
    public class MqttNetAspNetCoreAttributeRoutingDocumentFilter : IDocumentFilter
    {
        public void Apply(AsyncApiDocument document, DocumentFilterContext context)
        {
            var mqttRouteTableFactory = Type.GetType("MQTTnet.AspNetCore.AttributeRouting.MqttRouteTableFactory, MQTTnet.AspNetCore.AttributeRouting");
            var mqttRouteTable = Type.GetType("MQTTnet.AspNetCore.AttributeRouting.MqttRouteTable, MQTTnet.AspNetCore.AttributeRouting");
            var mqttRoute = Type.GetType("MQTTnet.AspNetCore.AttributeRouting.MqttRoute, MQTTnet.AspNetCore.AttributeRouting");
            var mqttRouteTemplate = Type.GetType("MQTTnet.AspNetCore.AttributeRouting.RouteTemplate, MQTTnet.AspNetCore.AttributeRouting");
            var mqttTemplateSegment = Type.GetType("MQTTnet.AspNetCore.AttributeRouting.TemplateSegment, MQTTnet.AspNetCore.AttributeRouting");

            var create = mqttRouteTableFactory.GetMethod("Create", BindingFlags.Static | BindingFlags.NonPublic, null, CallingConventions.Any,
                new[] {typeof(IEnumerable<Assembly>)}, null);


            var routeTable = create.Invoke(null, new object[] { new [] {Assembly.GetEntryAssembly() } });

            var routes = mqttRouteTable.GetProperty("Routes").GetValue(routeTable);


            foreach (var route in (IEnumerable) routes)
            {
                var template = mqttRoute.GetProperty("Template").GetValue(route);
                var segments = mqttRouteTemplate.GetProperty("Segments").GetValue(template);

                // MQTTnet route templates are not uri safe, which is required by the asyncapi spec.
                var channelItemName = new List<string>();
                foreach (var segment in (IEnumerable)segments)
                {
                    var isParameter = (bool) mqttTemplateSegment.GetProperty("IsParameter").GetValue(segment);
                    var isCatchAll = (bool) mqttTemplateSegment.GetProperty("IsCatchAll").GetValue(segment);
                    var value = (string) mqttTemplateSegment.GetProperty("Value").GetValue(segment);

                    channelItemName.Add(isCatchAll ? "#" : isParameter ? "+" : value);
                }


                var handler = (MethodInfo) mqttRoute.GetProperty("Handler").GetValue(route);
                var parameters = handler.GetParameters();

                ISchema payload = null;
                if (parameters != null && parameters.Any())
                {
                    // TODO: improve SchemaGenerator interface...
                    payload = context.SchemaGenerator.GenerateSchema(parameters.First().ParameterType, context.SchemaRepository);
                }


                document.Channels.Add(string.Join("/", channelItemName), new ChannelItem
                {
                    Publish = new Operation
                    {
                        OperationId = handler.Name,
                        Summary = handler.GetXmlDocsSummary(),
                        Message = new Message
                        {
                            Payload = payload,
                        }
                    }
                });
            }
        }
    }
}