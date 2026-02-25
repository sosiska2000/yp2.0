namespace RestAPI.Models
{
    public class ConsumableAttribute
    {
        public int Id { get; set; }
        public int ConsumableId { get; set; }
        public string AttributeName { get; set; }
        public string AttributeValue { get; set; }
    }
}