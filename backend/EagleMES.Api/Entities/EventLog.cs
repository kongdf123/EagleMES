namespace EagleMES.Api.Entities
{
    public class EventLog
    {
        public int Id { get; set; }
        public string EventType { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}
