namespace RestAPI.Models
{
    public class Consumable
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime? ReceiptDate { get; set; }
        public string PhotoPath { get; set; }
        public int Quantity { get; set; }
        public int? ResponsibleUserId { get; set; }
        public int? TempResponsibleUserId { get; set; }
        public int? ConsumableTypeId { get; set; }
    }
}