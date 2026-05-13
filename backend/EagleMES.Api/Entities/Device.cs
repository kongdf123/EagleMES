namespace EagleMES.Api.Entities
{
    public class Device
    {
        public int Id { get; set; }
        public string DeviceCode { get; set; } = string.Empty;
        public string DeviceName { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public double Temperature { get; set; }
        public DateTime LastUpdated { get; set; }
    }
}
