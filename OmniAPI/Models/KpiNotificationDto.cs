namespace OmniAPI.Models
{
    public class KpiNotificationDto
    {
        public int Id { get; set; }
        public int? BroilerId { get; set; }
        public string KPI { get; set; }
        public int? DeviationP { get; set; }
        public bool? Enabled { get; set; }
        public string PrimaryContact { get; set; }
        public string SecondaryContact { get; set; }
        public int? Delay { get; set; }
    }
}
