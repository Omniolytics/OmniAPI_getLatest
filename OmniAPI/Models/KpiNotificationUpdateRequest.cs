using System;

namespace OmniAPI.Models
{
    public class KpiNotificationUpdateRequest
    {
        public int? Id { get; set; }
        public int? BroilerId { get; set; }
        public string Kpi { get; set; }
        public int? DeviationP { get; set; }
        public bool? Enabled { get; set; }
<<<<<<< HEAD
        public int? PrimaryContact { get; set; }
        public int? SecondaryContact { get; set; }
=======
        public string PrimaryContact { get; set; }
        public string SecondaryContact { get; set; }
>>>>>>> origin/codex/add-endpoint-to-update-kpi-notifications-fulbow
        public int? Delay { get; set; }
    }
}
