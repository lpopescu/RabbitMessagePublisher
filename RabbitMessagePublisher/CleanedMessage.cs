using Newtonsoft.Json;

namespace RabbitMessagePublisher
{
    internal class CleanedMessage
    {
        public int ProductId { get; set; }

        public int SubsidiaryId { get; set; }

        public CleanedMessage(int productId, int subsidiaryId)
        {
            this.ProductId = productId;
            this.SubsidiaryId = subsidiaryId;
        }

        public CleanedMessage(string productId, string subsidiaryId) : 
            this(int.Parse(productId), int.Parse(subsidiaryId))
        {
        }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}