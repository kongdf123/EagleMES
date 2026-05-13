namespace EagleMES.Api.DTOs
{
    public class CreateWorkOrderRequest
    {
        public string OrderNo { get; set; } = string.Empty;
        public string ProductCode { get; set; } = string.Empty;
        public int Quantity { get; set; }
    }
}
