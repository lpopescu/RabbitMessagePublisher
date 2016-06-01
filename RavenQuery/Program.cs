using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

using Raven.Client;
using Raven.Client.Document;

namespace RavenQuery
{
    class Program
    {
        static void Main(string[] args)
        {
            var docStore = new DocumentStore {Url = "http://localhost:8080", DefaultDatabase = "competitor-prices"};
            docStore.Initialize();
            RavenQueryStatistics stats = null;

            using(var session = docStore.OpenSession())
            {
                session.Query<CompetitorPriceDocument>().Statistics(out stats);
            }
            int incrementer = 1024;
            int startIndex = 0;
            Debug.WriteLine($"Total results{stats.TotalResults}");
            int total = 50000;

            List<RaventEntity> entities = new List<RaventEntity>();

            do
            {
                int amount = startIndex + incrementer;

                using(var session = docStore.OpenSession())
                {
                    var query = session.Query<CompetitorPriceDocument>()
                                       .Statistics(out stats)
                                       .Skip(startIndex)
                                       .Take(amount)
                                       .ToList()
                                       .Select(cp => new {Identifier = $"{cp.ProductId}~{cp.SubsidiaryId}", Count = 1});
                    var groups = query.GroupBy(cp => cp.Identifier);

                    foreach(var group in groups)
                    {
                        entities.Add(new RaventEntity {Id = group.Key, Count = group.Count()});
                        
                        Console.WriteLine($"Pid: {group.Key}  count: {group.Count()}");
                    }

                    startIndex = amount;
                }

                Console.WriteLine($"Total groups{startIndex}");

            } while (startIndex < total);

            var top10 = entities.Where(e => e.Count > 50).OrderByDescending(e => e.Count).Take(20);
            Console.WriteLine($"Top 10 {String.Join(";",top10)}" );

            var bottom10 = entities.Where(e => e.Count < 10).OrderBy(e => e.Count).Take(20);
            Console.WriteLine($"Bottom 10 {String.Join(";", bottom10)}");

            Console.ReadLine();
        }
    }

    internal class CompetitorPriceDocument
    {
        public int ProductId { get; set; }

        public int SubsidiaryId { get; set; }
    }

    internal class RaventEntity
    {
        public int Count { get; set; }

        public string Id { get; set; }

        public override string ToString()
        {
            return $"id: {Id}, count: {Count}";
        }
    }
}