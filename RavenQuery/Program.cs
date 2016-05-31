using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Raven.Client.Document;

namespace RavenQuery
{
    class Program
    {
        static void Main(string[] args)
        {
            var docStore = new DocumentStore {ConnectionStringName = "localhost:8080"};
            using(var conn = docStore.OpenSession())
            {
                conn.Query<CleanedMessage>()
            }
        }
    }

    internal class CleanedMessage
    {
        public int ProductId { get; set; }

        public int SubsidiaryId { get; set; }
    }
}
