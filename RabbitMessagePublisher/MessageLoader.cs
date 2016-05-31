using System;
using System.Collections.Generic;
using System.IO;

namespace RabbitMessagePublisher
{
    internal class MessageLoader
    {
        public MessageLoader()
        {
            
        }

        public IEnumerable<string> Read(string filePath)
        {
            List<string> messages = new List<string>();

            if(!File.Exists(filePath))
            {
                Console.WriteLine("The file path '{0}' does not exist.", filePath);
            }

            using(var fsReader = new StreamReader(File.OpenRead(filePath)))
            {
                fsReader.ReadLine();
                while(!fsReader.EndOfStream)
                {
                    string[] fields = CsvParser.Parse(fsReader.ReadLine());
                    CleanedMessage cleanedMessage = new CleanedMessage(fields[0], fields[1]);
                    messages.Add(cleanedMessage.ToString());
                }
            }
            return messages;
        }
    }

    internal class CsvParser
    {
        public static string[] Parse(string csvLine)
        {
            string[] csvFields = csvLine.Split(',');

            return csvFields;
        }      
    }
}