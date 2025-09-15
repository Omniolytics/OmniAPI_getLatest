using System;

namespace OmniAPI
{
    public class PlacementWeightDto
    {
        public int id { get; set; }
        public int facilityId { get; set; }
        public int cycleId { get; set; }
        public double? totalBirdWeight { get; set; }
        public double? averageBox { get; set; }
        public int? noOfBirdsPerBox { get; set; }
        public DateTime? date { get; set; }
        public long? weightId { get; set; }
    }
}
