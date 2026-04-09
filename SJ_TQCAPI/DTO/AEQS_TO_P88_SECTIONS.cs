using System;
using System.Collections.Generic;
using System.Text;

namespace SJ_TQCAPI.DTO
{
    class AEQS_TO_P88_SECTIONS
    {
        public string id { get; set; }
        public string union_id { get; set; }
        public string sections_type { get; set; }
        public string sections_title { get; set; }
        public decimal sections_result_id { get; set; }
        public decimal sections_qty_inspected { get; set; }
        public decimal sections_sampled_inspected { get; set; }
        public decimal sections_defective_parts { get; set; }
        public string sections_inspection_level { get; set; }
        public string sections_inspection_method { get; set; }
        public decimal sections_aql_minor { get; set; }
        public decimal sections_aql_major { get; set; }
        public decimal sections_aql_critical { get; set; }
        public string sections_barcodes_value { get; set; }
        public string sections_qty_type { get; set; }
        public decimal sections_max_minor_defects { get; set; }
        public decimal sections_max_major_defects { get; set; }
        public decimal sections_max_major_a_defects { get; set; }
        public decimal sections_max_major_b_defects { get; set; }
        public decimal sections_max_critical_defects { get; set; }
        public string sections_defects_label { get; set; }
        public string sections_defects_subsection { get; set; }
        public string sections_defects_code { get; set; }
        public decimal sections_defects_critical_level { get; set; }
        public decimal sections_defects_major_level { get; set; }
        public decimal sections_defects_minor_level { get; set; }
        public string sections_defects_comments { get; set; }
        public string defect_image { get; set; }
    }
}
