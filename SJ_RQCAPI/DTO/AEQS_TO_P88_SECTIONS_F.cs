using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SJ_RQCAPI.DTO
{
    class AEQS_TO_P88_SECTIONS_F
    {
        public string id { get; set; }
        public string union_id { get; set; }
        public string section_type { get; set; }
        public string section_title { get; set; }
        public string sections_defects_pictures_title { get; set; }
        public string sections_defects_pictures_full_filename { get; set; }
        public decimal sections_defects_pictures_number { get; set; }
        public string sections_defects_pictures_comment { get; set; }
    }
}
