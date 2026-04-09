using System;
using System.Collections.Generic;
using System.Text;

namespace SJ_RQCAPI.DTO
{
    class AEQS_TO_P88_LIST
    {
        public string unique_key { get; set; }
        public string status { get; set; }
        public DateTime date_started { get; set; }
        public decimal defective_parts { get; set; }
        public decimal assignment_items_assignment_report_type_id { get; set; }
        public string passfails_0_title { get; set; }
        public string passfails_0_type { get; set; }
        public string passfails_0_subsection { get; set; }
        public string passfails_0_listvalues_value { get; set; }
        public string assignment_items_fields_string_12 { get; set; }
    }
}
