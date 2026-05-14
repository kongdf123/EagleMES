namespace EagleMES.Api.Entities
{
    public class PurchaseOrder
    {
        public int Id { get; set; }
        public string PoNo { get; set; } = string.Empty;
        public int SupplierId { get; set; }
        public string MaterialName { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}
