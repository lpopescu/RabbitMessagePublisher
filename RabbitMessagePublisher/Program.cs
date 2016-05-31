using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using RabbitMQ.Client;

namespace RabbitMessagePublisher
{
    class Program
    {
        static void Main(string[] args)
        {
            if(!args.Any())
            {
                Console.WriteLine("The file path was not specified.");
                Console.WriteLine("usage: RabbitMessagePublisher [file_path]");
            }
            else
            {
                var filePath = args[0];

                var rabbitmqFactory = new ConnectionFactory {HostName = "localhost"};
                using(var connection = rabbitmqFactory.CreateConnection())
                {
                    using(var channel = connection.CreateModel())
                    {
                        channel.QueueDeclare(queue:"pricing-cleaner",
                            durable:true,
                            exclusive:false,
                            autoDelete:false,
                            arguments:null);

                        var messageLoader = new MessageLoader();
                        var jsonMessages = messageLoader.Read(filePath);
                        PublishMessages(channel, jsonMessages);

                    }
                }

            }
            Console.ReadLine();
        }

        private static int PublishMessages(IModel channel, IEnumerable<string> messages)
        {
            int counter = 0;
            foreach(string message in messages)
            {
                var body = Encoding.UTF8.GetBytes(message);
                counter++;
                channel.BasicPublish(exchange:"",
                    routingKey: "pricing-cleaner",
                    basicProperties:null,
                    body:body);
                Console.WriteLine($" Sent {counter} Content: {message}");
            }
            return counter;
        }
    }
}
