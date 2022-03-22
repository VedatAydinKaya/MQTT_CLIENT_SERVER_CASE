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
        static async Task Main(string[] args)
        {
            var mqttFactory = new MqttFactory();
            var client = mqttFactory.CreateMqttClient();
            var options = new MqttClientOptionsBuilder()
                           .WithClientId(Guid.NewGuid().ToString())
                            .WithTcpServer("test.mosquitto.org", 1883)
                            .WithCleanSession()
                            .Build();

            client.UseConnectedHandler( async e =>   
            {
                Console.WriteLine("Connected  to the broker succesfully");

                var topicFilter=new TopicFilterBuilder()
                                    .WithTopic("VedatAydinKaya")
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

                Console.WriteLine($"{Encoding.UTF8.GetString(e.ApplicationMessage.Payload)}");
            });              


            await client.ConnectAsync(options);

            Console.ReadLine();


         //  await PublishMessageAsync(client);

            await client.DisconnectAsync();

        }

        //private static async Task PublishMessageAsync(IMqttClient client)
        //{
        //    //string messagePayload = "Hello";
        //    string messagePayload = Console.ReadLine();


        //    var message = new MqttApplicationMessageBuilder()  // for message will have build
        //                  .WithTopic("VedatAydinKaya")
        //                  .WithPayload(messagePayload)
        //                  .WithAtLeastOnceQoS()           // quality of services which are defined in as part of the mqtt protocol 
        //                .Build();

        //    if (client.IsConnected)
        //    {
        //        await client.PublishAsync(message);

        //        // Console.WriteLine($"Published message -{messagePayload}");
        //        Console.WriteLine($"{messagePayload}");

        //    }

        //}



    }
}
