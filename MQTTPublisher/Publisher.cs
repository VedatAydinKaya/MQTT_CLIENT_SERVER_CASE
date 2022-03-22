using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Client.Options;
using System;
using System.Threading.Tasks;

namespace MQTTPublisher
{
    internal class Publisher
    {
        static async Task Main(string[] args)
        {
            var mqttFactory = new MqttFactory();
            var client=mqttFactory.CreateMqttClient();
            // we need to create and set up the options which the client  use in order to connect to the broker for this
            var options = new MqttClientOptionsBuilder()
                           .WithClientId(Guid.NewGuid().ToString())
                            .WithTcpServer("test.mosquitto.org",1883)
                            .WithCleanSession()
                            .Build();

            client.UseConnectedHandler(async e =>   //when the client gets connected this  particular event be triggered
            {
                Console.WriteLine("Connected  to the broker succesfully");

             /*   var topicFilter = new TopicFilterBuilder()
                                   .WithTopic("VedatAydinKaya")
                                   .Build();

                await client.SubscribeAsync(topicFilter);  */

            });

            client.UseDisconnectedHandler(e =>
            {
                Console.WriteLine("Disconnected from the broker succesfully");
            });


            await client.ConnectAsync(options);         // now we will try to establish connection

            Console.WriteLine("Please  press a key to publish the message");

           // Console.ReadLine();


           await PublishMessageAsync(client); 

            await client.DisconnectAsync(); // as soon as  the message has been published  we will disconnected the client

        }

        private static async Task PublishMessageAsync(IMqttClient client)
        {
            //string messagePayload = "Hello";
             string messagePayload = Console.ReadLine();
            

            var message =new MqttApplicationMessageBuilder()  // for message will have build
                          .WithTopic("VedatAydinKaya")
                          .WithPayload(messagePayload)
                          .WithAtLeastOnceQoS()           // quality of services which are defined in as part of the mqtt protocol 
                        .Build();

            if (client.IsConnected)
            {
                await client.PublishAsync(message);

               // Console.WriteLine($"Published message -{messagePayload}");
                Console.WriteLine($"{messagePayload}");

            }

        }
    }
}
