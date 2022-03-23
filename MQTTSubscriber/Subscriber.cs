using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Client.Options;
using System;
using System.Text;
using System.Threading.Tasks;

namespace MQTTSubscriber
{
    internal class Subscriber
    {
        private
        static async Task Main(string[] args)
        {
            var mqttFactory = new MqttFactory();
            var client = mqttFactory.CreateMqttClient();
            var options = new MqttClientOptionsBuilder()
                           .WithClientId(Guid.NewGuid().ToString())
                            .WithTcpServer("test.mosquitto.org", 1883)
                            .WithCleanSession()
                            .Build();

            client.UseConnectedHandler(async e =>
           {
               Console.WriteLine("Connected  to the broker succesfully");

               var topicFilter = new TopicFilterBuilder()
                                   .WithTopic("NODE_CLIENT")
                                   .Build();

               await client.SubscribeAsync(topicFilter);
           });

            client.UseDisconnectedHandler(e =>
            {
                Console.WriteLine("Disconnected from the broker succesfully");
            });

            client.UseApplicationMessageReceivedHandler(e =>     // we need to specify an event handler in case we receive a message 
            {
                  // payload will be byte array we need to convert it back to string
                Console.WriteLine($"Received: {DateTime.Now:T} - {Encoding.UTF8.GetString(e.ApplicationMessage.Payload)}");
            });

            await client.ConnectAsync(options);

            do
            {
                string messagePayload = Console.ReadLine();
                if (string.IsNullOrEmpty(messagePayload))   //  when empty entries from console pub topic closes
                {
                    await client.DisconnectAsync();
                    break;
                  
                }
                await PublishMessageAsync(client, messagePayload);
            } while (true);

           
        }
        private static async Task PublishMessageAsync(IMqttClient client, string messagePayload)
        {
            var message = new MqttApplicationMessageBuilder()  // for message will have build
                          .WithTopic("NODE_SERVER")
                          .WithPayload(messagePayload)
                          .WithAtLeastOnceQoS()          // quality of services which are defined in as part of the mqtt protocol 
                        .Build();
            if (client.IsConnected)
            {
                await client.PublishAsync(message);
                Console.WriteLine($"Sent: {DateTime.Now:T} - {messagePayload}");
            }
        }
    }
}
