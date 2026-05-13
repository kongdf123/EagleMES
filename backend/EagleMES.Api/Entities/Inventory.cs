namespace EagleMES.Api.Entities
{
    public class Inventory
    {
        public int Id { get; set; }
        public string MaterialCode { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public string Location { get; set; } = string.Empty;
    }
}
