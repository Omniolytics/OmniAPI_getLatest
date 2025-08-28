using System.Collections.Generic;

namespace OmniAPI
{
    public class FarmDto
    {
        public int ID { get; set; }
        public int? UserID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public List<tbl_Cycles> tbl_Cycles { get; set; }
    }
}

