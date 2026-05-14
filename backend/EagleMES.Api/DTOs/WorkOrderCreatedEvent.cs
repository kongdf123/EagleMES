namespace EagleMES.Api.DTOs
{
    public class WorkOrderCreatedEvent
    {
        public int Id { get; set; }
        public string OrderNo { get; set; } = string.Empty;
        public string ProductCode { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
