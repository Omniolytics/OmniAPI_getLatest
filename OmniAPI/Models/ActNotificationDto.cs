namespace OmniAPI.Models
{
    public class ActNotificationDto
    {
        public int Id { get; set; }
        public string Act { get; set; }
        public bool? Completed { get; set; }
        public bool? CompletedNotMet { get; set; }
        public bool? NotCompleted { get; set; }
        public string PrimaryContact { get; set; }
        public string SecondaryContact { get; set; }
        public int? Delay { get; set; }
        public int? BroilerId { get; set; }
    }
}
