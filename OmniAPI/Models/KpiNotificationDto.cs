namespace OmniAPI.Models
{
    public class KpiNotificationDto
    {
        public string KPI { get; set; }
        public int? DeviationP { get; set; }
        public bool? Enabled { get; set; }
        public int? PrimaryContact { get; set; }
        public int? SecondaryContact { get; set; }
        public int? Delay { get; set; }
    }
}
