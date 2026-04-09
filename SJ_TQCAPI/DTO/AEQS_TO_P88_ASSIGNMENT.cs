using System;
using System.Collections.Generic;
using System.Text;

namespace SJ_TQCAPI.DTO
{
    class AEQS_TO_P88_ASSIGNMENT
    {
        public string id { get; set; }
        public string union_id { get; set; }
        public decimal assignment_items_sampled_inspected { get; set; }
        public decimal assignment_items_inspection_result_id { get; set; }
        public decimal assignment_items_inspection_status_id { get; set; }
        public decimal assignment_items_qty_inspected { get; set; }
        public DateTime assignment_items_inspection_completed_date { get; set; }
        public decimal assignment_items_total_inspection_minutes { get; set; }
        public decimal assignment_items_sampling_size { get; set; }
        public decimal assignment_items_qty_to_inspect { get; set; }
        public decimal assignment_items_aql_minor { get; set; }
        public decimal assignment_items_aql_major { get; set; }
        public decimal assignment_items_aql_major_a { get; set; }
        public decimal assignment_items_aql_major_b { get; set; }
        public decimal assignment_items_aql_critical { get; set; }
        public string assignment_items_supplier_booking_msg { get; set; }
        public string assignment_items_conclusion_remarks { get; set; }
        public string assignment_items_assignment_inspector_username { get; set; }
        public DateTime assignment_items_assignment_date_inspection { get; set; }
        public string assignment_items_assignment_inspection_level { get; set; }
        public string assignment_items_assignment_inspection_method { get; set; }
        public decimal assignment_items_po_line_qty { get; set; }
        public DateTime assignment_items_po_line_etd { get; set; }
        public DateTime assignment_items_po_line_eta { get; set; }
        public string assignment_items_po_line_color { get; set; }
        public string assignment_items_po_line_size { get; set; }
        public string assignment_items_po_line_style { get; set; }
        public decimal assignment_items_po_line_po_exporter_id { get; set; }
        public string assignment_items_po_line_po_exporter_erp_business_id { get; set; }
        public string assignment_items_po_line_po_number { get; set; }
        public string assignment_items_po_line_customer_po { get; set; }
        public decimal assignment_items_po_line_importer_id { get; set; }
        public string assignment_items_po_line_importer_erp_business_id { get; set; }
        public decimal assignment_items_po_line_importer_project_id { get; set; }
        public string assignment_items_po_line_sku_sku_number { get; set; }
        public string assignment_items_po_line_sku_item_name { get; set; }
        public string assignment_items_po_line_sku_item_description { get; set; }

        //public int assignment_items_assignment_report_type_id { get; set; } 
        public string assignment_items_assignment_report_type_id { get; set; }
    }
}
