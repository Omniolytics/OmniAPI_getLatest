namespace OmniAPI.Models
{
    public class KpiNotificationDto
    {
        public string KPI { get; set; }
        public decimal? DeviationP { get; set; }
        public bool? Enabled { get; set; }
        public string PrimaryContact { get; set; }
        public string SecondaryContact { get; set; }
        public int? Delay { get; set; }
    }
}
