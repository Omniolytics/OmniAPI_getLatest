using System;

namespace OmniAPI.Models
{
    public class KpiNotificationUpdateRequest
    {
        public string Kpi { get; set; }
        public int? DeviationP { get; set; }
        public bool? Enabled { get; set; }
        public int? PrimaryContact { get; set; }
        public int? SecondaryContact { get; set; }
        public int? Delay { get; set; }
    }
}
