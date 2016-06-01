using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

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
                string queueName = ConfigurationManager.AppSettings["queueName"];
                string hostName = ConfigurationManager.AppSettings["hostName"];

                var rabbitmqFactory = new ConnectionFactory {HostName = hostName};
                using(var connection = rabbitmqFactory.CreateConnection())
                {
                    using(var channel = connection.CreateModel())
                    {
                        channel.QueueDeclare(queueName,
                            true,
                            false,
                            false,
                            null);

                        var messageLoader = new MessageLoader();
                        var jsonMessages = messageLoader.Read(filePath);
                        PublishMessages(channel, queueName, jsonMessages);
                    }
                }
            }
            Console.ReadLine();
        }

        private static void PublishMessages(IModel channel, string queueName, IEnumerable<string> messages)
        {
            int counter = 0;
            foreach(string message in messages)
            {
                {
                    var body = Encoding.UTF8.GetBytes(message);
                    counter++;
                    channel.BasicPublish("",
                        queueName,
                        null,
                        body);
                    Console.WriteLine($" Sent {counter} Content: {message}");
                }
            }
        }
    }
}