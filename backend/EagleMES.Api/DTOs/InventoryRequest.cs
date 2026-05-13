namespace EagleMES.Api.DTOs
{
    public class InventoryRequest
    {
        public string MaterialCode { get; set; } = string.Empty;
        public int Quantity { get; set; }
    }
}
