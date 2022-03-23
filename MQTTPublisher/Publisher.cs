using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Client.Options;
using System;
using System.Text;
using System.Threading.Tasks;

namespace MQTTPublisher
{
    internal class Publisher
    {
        static async Task Main(string[] args)
        {
            var mqttFactory = new MqttFactory();
            var client = mqttFactory.CreateMqttClient();
            // we need to create and set up the options which the client  use in order to connect to the broker for this
            var options = new MqttClientOptionsBuilder()
                           .WithClientId(Guid.NewGuid().ToString())
                            .WithTcpServer("test.mosquitto.org", 1883)
                            .WithCleanSession()
                            .Build();

            client.UseConnectedHandler(async e =>
            {
                Console.WriteLine("Connected  to the broker succesfully");

                var topicFilter = new TopicFilterBuilder()
                                    .WithTopic("sub")
                                    .Build();

                await client.SubscribeAsync(topicFilter);
            });

            client.UseDisconnectedHandler(e =>
            {
                Console.WriteLine("Disconnected from the broker succesfully");
            });

            client.UseApplicationMessageReceivedHandler(e =>     // we need to specify an event handler in case we receive a message 
            {
                // Console.WriteLine($"Received Message as Your Message -{Encoding.UTF8.GetString(e.ApplicationMessage.Payload)}");  // payload will be byte array we need to convert it back to string

                Console.WriteLine($"Received: {DateTime.Now:T} - {Encoding.UTF8.GetString(e.ApplicationMessage.Payload)}");
            });

            await client.ConnectAsync(options);         // now we will try to establish connection


            // Console.ReadLine();

            do
            {
                string messagePayload = Console.ReadLine();
                if (string.IsNullOrEmpty(messagePayload))// boş giriş yapıldığında pub kapanır
                    break;
                await PublishMessageAsync(client, messagePayload);
            } while (true);

            //await client.DisconnectAsync();    // as soon as  the message has been published  we will disconnected the client

        }

        private static async Task PublishMessageAsync(IMqttClient client, string messagePayload)
        {
            var message = new MqttApplicationMessageBuilder()  // for message will have build
                          .WithTopic("pub")
                          .WithPayload(messagePayload)
                          .WithAtLeastOnceQoS()           // quality of services which are defined in as part of the mqtt protocol 
                        .Build();

            if (client.IsConnected)
            {
                await client.PublishAsync(message);

                Console.WriteLine($"Sent: {DateTime.Now:T} - {messagePayload}");

            }

        }
    }
}
