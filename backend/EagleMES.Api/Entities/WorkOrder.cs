namespace EagleMES.Api.Entities
{
    public class WorkOrder
    {
        public int Id { get; set; }
        public string OrderNo { get; set; } = string.Empty;
        public string ProductCode { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
    }
}
