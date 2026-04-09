using System;
using System.Collections.Generic;
using System.Text;

namespace SJ_QCMAPI
{
    public class BdnListClass
    {
        public string id { get; set; }
        public string inspection_no { get; set; }
        public string testitem_code { get; set; }
        public string testitem_name { get; set; }
        public string testtype_no { get; set; }
        public string testtype_name { get; set; }
        public string sample_num { get; set; }
        public string check_type { get; set; }
        public string t_check_item { get; set; }
        public string t_check_value { get; set; }
        public string d_check_item { get; set; }
        public string d_check_value { get; set; }
        public string currency_formula { get; set; }
        public string custom_formula { get; set; }
        public string unit { get; set; }
        public string art_remarks { get; set; }
        public string result_value { get; set; }
        public string check_result { get; set; }
        public string test_remarks { get; set; }
        public string reference_level { get; set; }
    }
}
