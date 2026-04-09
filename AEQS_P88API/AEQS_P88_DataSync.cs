using SJeMES_Framework_NETCore.DBHelper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace AEQS_P88API
{
    public class AEQS_P88_DataSync
    {
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetDataByReportType(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase();
            try
            {
                DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);
                string Data = ReqObj.Data.ToString();
                DB.Open();
                DB.BeginTransaction();
                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                //Input Params
                string from = jarr.ContainsKey("from") ? jarr["from"].ToString() : ""; //from
                string to = jarr.ContainsKey("to") ? jarr["to"].ToString() : ""; //to
                int report_type_id = jarr.ContainsKey("report_type_id") ? Convert.ToInt16(jarr["report_type_id"]) : 0; //report_type_id
                string po = jarr.ContainsKey("po") ? jarr["po"].ToString() : "";


                string wheresql = string.Empty;

                if (!string.IsNullOrEmpty(from) && !string.IsNullOrEmpty(to))
                {
                    wheresql += $@" and TO_CHAR(a.DATE_STARTED,'YYYY/MM/DD') BETWEEN '{from}' AND '{to}'";
                } 
                if (!string.IsNullOrEmpty(po))
                {
                    wheresql += $@"and b.assignment_items_po_line_po_number='{po}'";
                } 
                //String sql = $@"SELECT * FROM T_AEQS_TO_P88_LIST T WHERE TO_CHAR(T.DATE_STARTED,'YYYY/MM/DD') BETWEEN '{from}' AND '{to}'  AND T.ASSIGNMENT_ITEMS_ASSIGNMENT_REPORT_TYPE_ID={report_type_id} ORDER BY UNIQUE_KEY";
                String sql = $@"select  distinct a.UNIQUE_KEY
, a.STATUS
, a.DATE_STARTED
, a.DEFECTIVE_PARTS
, a.PASSFAILS_0_TITLE
, a.PASSFAILS_0_TYPE
, a.PASSFAILS_0_SUBSECTION
, a.PASSFAILS_0_LISTVALUES_VALUE
, NVL(a.Modifying_count,0) as MODIFY_COUNT
,to_char(a.SYNC_DATE,'yyyyMMdd') as SYNC_DATE
,a.IS_SYNC
,a.SYNC_STATUS_CODE STATUS_CODE
from t_aeqs_to_p88_list a left join t_aeqs_to_p88_assignment b on a.UNIQUE_KEY=b.union_id where 1=1 {wheresql}
 AND a.ASSIGNMENT_ITEMS_ASSIGNMENT_REPORT_TYPE_ID={report_type_id} and (a.is_sync is null or a.is_sync <> 'S')";
                DataTable dt = DB.GetDataTable(sql);

                Dictionary<string, object> dic = new Dictionary<string, object>();
                dic.Add("Data", dt);

                ret.RetData = Newtonsoft.Json.JsonConvert.SerializeObject(dic);
                ret.IsSuccess = true;

            }
            catch (Exception ex)
            {
                DB.Rollback();
                ret.IsSuccess = false;
                ret.ErrMsg = ex.Message;
            }
            finally
            {
                DB.Close();
            }
            return ret;
        }

        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetSyncData(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase();
            try
            {
                DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);
                string Data = ReqObj.Data.ToString();
                DB.Open();
                DB.BeginTransaction();
                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                //Input Params
                string from = jarr.ContainsKey("from") ? jarr["from"].ToString() : ""; //from
                string to = jarr.ContainsKey("to") ? jarr["to"].ToString() : ""; //to
                int report_type_id = jarr.ContainsKey("report_type_id") ? Convert.ToInt16(jarr["report_type_id"]) : 0; //report_type_id
                string is_sync = jarr.ContainsKey("is_sync") ? jarr["is_sync"].ToString() : "";
                string po = jarr.ContainsKey("po") ? jarr["po"].ToString() : "";
                string unique_key = jarr.ContainsKey("unique_key") ? jarr["unique_key"].ToString() : "";

                string wheresql = string.Empty;

                if (!string.IsNullOrEmpty(from) && !string.IsNullOrEmpty(to))
                {
                    wheresql += $@" and TO_CHAR(a.DATE_STARTED,'YYYY/MM/DD') BETWEEN '{from}' AND '{to}'";
                }
                if (!string.IsNullOrEmpty(is_sync))
                {
                    wheresql += $@" and (a.is_sync = '{is_sync}')";
                }
                if (!string.IsNullOrEmpty(po))
                {
                    wheresql += $@"and b.assignment_items_po_line_po_number='{po}'";
                }
                if (!string.IsNullOrEmpty(unique_key))
                {
                    wheresql += $@"and a.unique_key='{unique_key}'";
                }



                //String sql = $@"SELECT * FROM T_AEQS_TO_P88_LIST T WHERE TO_CHAR(T.DATE_STARTED,'YYYY/MM/DD') BETWEEN '{from}' AND '{to}'  AND T.ASSIGNMENT_ITEMS_ASSIGNMENT_REPORT_TYPE_ID={report_type_id} ORDER BY UNIQUE_KEY";
                String sql = $@"select  distinct a.UNIQUE_KEY
, a.STATUS
, a.DATE_STARTED
, a.DEFECTIVE_PARTS
, a.PASSFAILS_0_TITLE
, a.PASSFAILS_0_TYPE
, a.PASSFAILS_0_SUBSECTION
, a.PASSFAILS_0_LISTVALUES_VALUE
, NVL(a.Modifying_count,0) as MODIFY_COUNT
,to_char(a.SYNC_DATE,'yyyyMMdd') as SYNC_DATE
,a.IS_SYNC
,a.SYNC_STATUS_CODE STATUS_CODE
from t_aeqs_to_p88_list a left join t_aeqs_to_p88_assignment b on a.UNIQUE_KEY=b.union_id where 1=1 {wheresql}
 AND a.ASSIGNMENT_ITEMS_ASSIGNMENT_REPORT_TYPE_ID={report_type_id}";
                DataTable dt = DB.GetDataTable(sql);

                Dictionary<string, object> dic = new Dictionary<string, object>();
                dic.Add("Data", dt);

                ret.RetData = Newtonsoft.Json.JsonConvert.SerializeObject(dic);
                ret.IsSuccess = true;

            }
            catch (Exception ex)
            {
                DB.Rollback();
                ret.IsSuccess = false;
                ret.ErrMsg = ex.Message;
            }
            finally
            {
                DB.Close();
            }
            return ret;
        }

        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetDataByUniqueKey(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase();
            try
            {
                DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);
                string Data = ReqObj.Data.ToString();
                DB.Open();
                DB.BeginTransaction();
                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                //Input Params
                string vSource = jarr.ContainsKey("vSource") ? jarr["vSource"].ToString() : ""; //vSource

                string sql1 = $@"SELECT * FROM T_AEQS_TO_P88_SECTIONS S WHERE S.UNION_ID='{vSource}'";
                DataTable dt1 = DB.GetDataTable(sql1);

                string sql2 = $@"SELECT * FROM T_AEQS_TO_P88_PASSFAIL P WHERE P.UNION_ID='{vSource}'";
                DataTable dt2 = DB.GetDataTable(sql2);

                string sql3 = $@"SELECT * FROM T_AEQS_TO_P88_ASSIGNMENT A WHERE A.UNION_ID='{vSource}'";
                DataTable dt3 = DB.GetDataTable(sql3);

                Dictionary<string, object> dic = new Dictionary<string, object>();
                dic.Add("Data1", dt1);
                dic.Add("Data2", dt2);
                dic.Add("Data3", dt3);

                ret.RetData = Newtonsoft.Json.JsonConvert.SerializeObject(dic);
                ret.IsSuccess = true;

            }
            catch (Exception ex)
            {
                DB.Rollback();
                ret.IsSuccess = false;
                ret.ErrMsg = ex.Message;
            }
            finally
            {
                DB.Close();
            }
            return ret;
        }

        public static SJeMES_Framework_NETCore.WebAPI.ResultObject EditItem(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase();
            try
            {
                DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);
                string Data = ReqObj.Data.ToString();
                DB.Open();
                DB.BeginTransaction();
                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);

                int report_type_id = jarr.ContainsKey("report_type_id") ? Convert.ToInt16(jarr["report_type_id"]) : 0; //report_type_id
                string dt1_json = jarr["dt1"].ToString();
                string dt2_json = jarr["dt2"].ToString();
                string dt3_json = jarr["dt3"].ToString();
                string dt4_json = jarr["dt4"].ToString();
                string USER = SJeMES_Framework_NETCore.Web.System.GetUserCodeByToken(ReqObj.UserToken);

                DataTable dt1 = (DataTable)Newtonsoft.Json.JsonConvert.DeserializeObject(dt1_json, (typeof(DataTable)));
                DataTable dt2 = (DataTable)Newtonsoft.Json.JsonConvert.DeserializeObject(dt2_json, (typeof(DataTable)));
                DataTable dt3 = (DataTable)Newtonsoft.Json.JsonConvert.DeserializeObject(dt3_json, (typeof(DataTable)));
                DataTable dt4 = (DataTable)Newtonsoft.Json.JsonConvert.DeserializeObject(dt4_json, (typeof(DataTable)));
                Updatedt1(DB, dt1,USER);
                Updatedt2(DB, dt2,USER);
                Updatedt3(DB, dt3,USER); 
                Updatedt4(DB, dt4,USER);
                ret.IsSuccess = true;
                DB.Commit();

            }
            catch (Exception ex)
            {
                DB.Rollback();
                ret.IsSuccess = false;
                ret.ErrMsg = ex.Message;
            }
            finally
            {
                DB.Close();
            }
            return ret;
        }

        public static void Updatedt1(DataBase DB, DataTable dt1, string USER)
        {
             
              if (dt1.Rows.Count > 0)
              {
                DataRow dr = dt1.Rows[0];
                string UNIQUE_KEY = dr["UNIQUE_KEY2"].ToString();
                string DATE_STARTED = dr["DATE_STARTED2"].ToString();
                string DEFECTIVE_PARTS = dr["DEFECTIVE_PARTS2"].ToString();
                //string ASSIGNMENT_ITEMS_SAMPLED_INSPECTED = dr["ASSIGNMENT_ITEMS_SAMPLED_INSPECTED"].ToString();
                //string ASSIGNMENT_ITEMS_INSPECTION_RESULT_ID = dr["ASSIGNMENT_ITEMS_INSPECTION_RESULT_ID"].ToString();
                //string ASSIGNMENT_ITEMS_INSPECTION_STATUS_ID = dr["ASSIGNMENT_ITEMS_INSPECTION_STATUS_ID"].ToString();
                //string ASSIGNMENT_ITEMS_QTY_INSPECTED = dr["ASSIGNMENT_ITEMS_QTY_INSPECTED"].ToString();
                //string ASSIGNMENT_ITEMS_TOTAL_INSPECTION_MINUTES = dr["ASSIGNMENT_ITEMS_TOTAL_INSPECTION_MINUTES"].ToString();
                //string ASSIGNMENT_ITEMS_SAMPLING_SIZE = dr["ASSIGNMENT_ITEMS_SAMPLING_SIZE"].ToString();
                //string ASSIGNMENT_ITEMS_QTY_TO_INSPECT = dr["ASSIGNMENT_ITEMS_QTY_TO_INSPECT"].ToString();
                //string ASSIGNMENT_ITEMS_AQL_MINOR = dr["ASSIGNMENT_ITEMS_AQL_MINOR"].ToString();
                //string ASSIGNMENT_ITEMS_AQL_MAJOR = dr["ASSIGNMENT_ITEMS_AQL_MAJOR"].ToString();
                //string ASSIGNMENT_ITEMS_AQL_MAJOR_A = dr["ASSIGNMENT_ITEMS_AQL_MAJOR_A"].ToString();
                //string ASSIGNMENT_ITEMS_AQL_MAJOR_B = dr["ASSIGNMENT_ITEMS_AQL_MAJOR_B"].ToString();
                //string ASSIGNMENT_ITEMS_AQL_CRITICAL = dr["ASSIGNMENT_ITEMS_AQL_CRITICAL"].ToString();
                //string ASSIGNMENT_ITEMS_SUPPLIER_BOOKING_MSG = dr["ASSIGNMENT_ITEMS_SUPPLIER_BOOKING_MSG"].ToString();
                //string ASSIGNMENT_ITEMS_CONCLUSION_REMARKS = dr["ASSIGNMENT_ITEMS_CONCLUSION_REMARKS"].ToString();
                //string ASSIGNMENT_ITEMS_ASSIGNMENT_INSPECTOR_USERNAME = dr["ASSIGNMENT_ITEMS_ASSIGNMENT_INSPECTOR_USERNAME"].ToString();
                //string ASSIGNMENT_ITEMS_PO_LINE_SKU_SKU_NUMBER = dr["ASSIGNMENT_ITEMS_PO_LINE_SKU_SKU_NUMBER"].ToString();
                //string ASSIGNMENT_ITEMS_PO_LINE_SKU_ITEM_NAME = dr["ASSIGNMENT_ITEMS_PO_LINE_SKU_ITEM_NAME"].ToString();
                //string ASSIGNMENT_ITEMS_PO_LINE_SKU_ITEM_DESCRIPTION = dr["ASSIGNMENT_ITEMS_PO_LINE_SKU_ITEM_DESCRIPTION"].ToString();
                string PASSFAILS_0_TITLE = dr["PASSFAILS_0_TITLE2"].ToString();
                string PASSFAILS_0_SUBSECTION = dr["PASSFAILS_0_SUBSECTION2"].ToString();
                string PASSFAILS_0_LISTVALUES_VALUE = dr["PASSFAILS_0_LISTVALUES_VALUE2"].ToString();
                string MODIFYCOUNT = dr["modify_count2"].ToString();
                string NEWCOUNT = (Convert.ToInt32(MODIFYCOUNT) + 1).ToString();

                string sql = string.Empty;
                sql = $@"select * from T_AEQS_TO_P88_LIST a where a.Unique_Key='{UNIQUE_KEY}'";
                DataTable dt2 = DB.GetDataTable(sql);

                DataRow dr2 = dt2.Rows[0]; 
                string DATE_STARTED2 = dr2["DATE_STARTED"].ToString();
                string DEFECTIVE_PARTS2 = dr2["DEFECTIVE_PARTS"].ToString();
                string PASSFAILS_0_TITLE2 = dr2["PASSFAILS_0_TITLE"].ToString();
                string PASSFAILS_0_SUBSECTION2 = dr2["PASSFAILS_0_SUBSECTION"].ToString();
                string PASSFAILS_0_LISTVALUES_VALUE2 = dr2["PASSFAILS_0_LISTVALUES_VALUE"].ToString();
                string User_Account = USER;

                string sql1 = string.Empty;
                string sql2 = string.Empty;
                //sql1 = $@"update t_aeqs_to_p88_list set DEFECTIVE_PARTS='{DEFECTIVE_PARTS}',ASSIGNMENT_ITEMS_SAMPLED_INSPECTED='{ASSIGNMENT_ITEMS_SAMPLED_INSPECTED}',ASSIGNMENT_ITEMS_INSPECTION_RESULT_ID='{ASSIGNMENT_ITEMS_INSPECTION_RESULT_ID}',ASSIGNMENT_ITEMS_INSPECTION_STATUS_ID='{ASSIGNMENT_ITEMS_INSPECTION_STATUS_ID}'
                //            ,ASSIGNMENT_ITEMS_QTY_INSPECTED='{ASSIGNMENT_ITEMS_QTY_INSPECTED}',ASSIGNMENT_ITEMS_TOTAL_INSPECTION_MINUTES='{ASSIGNMENT_ITEMS_TOTAL_INSPECTION_MINUTES}',ASSIGNMENT_ITEMS_SAMPLING_SIZE='{ASSIGNMENT_ITEMS_SAMPLING_SIZE}',ASSIGNMENT_ITEMS_QTY_TO_INSPECT='{ASSIGNMENT_ITEMS_QTY_TO_INSPECT}',ASSIGNMENT_ITEMS_AQL_MINOR='{ASSIGNMENT_ITEMS_AQL_MINOR}'
                //            ,ASSIGNMENT_ITEMS_AQL_MAJOR='{ASSIGNMENT_ITEMS_AQL_MAJOR}',ASSIGNMENT_ITEMS_AQL_MAJOR_A='{ASSIGNMENT_ITEMS_AQL_MAJOR_A}',ASSIGNMENT_ITEMS_AQL_MAJOR_B='{ASSIGNMENT_ITEMS_AQL_MAJOR_B}',ASSIGNMENT_ITEMS_AQL_CRITICAL='{ASSIGNMENT_ITEMS_AQL_CRITICAL}',ASSIGNMENT_ITEMS_SUPPLIER_BOOKING_MSG='{ASSIGNMENT_ITEMS_SUPPLIER_BOOKING_MSG}'
                //            ,ASSIGNMENT_ITEMS_CONCLUSION_REMARKS='{ASSIGNMENT_ITEMS_CONCLUSION_REMARKS}',ASSIGNMENT_ITEMS_ASSIGNMENT_INSPECTOR_USERNAME='{ASSIGNMENT_ITEMS_ASSIGNMENT_INSPECTOR_USERNAME}',ASSIGNMENT_ITEMS_PO_LINE_SKU_SKU_NUMBER='{ASSIGNMENT_ITEMS_PO_LINE_SKU_SKU_NUMBER}',ASSIGNMENT_ITEMS_PO_LINE_SKU_ITEM_NAME='{ASSIGNMENT_ITEMS_PO_LINE_SKU_ITEM_NAME}',ASSIGNMENT_ITEMS_PO_LINE_SKU_ITEM_DESCRIPTION='{ASSIGNMENT_ITEMS_PO_LINE_SKU_ITEM_DESCRIPTION}'
                //            ,PASSFAILS_0_TITLE='{PASSFAILS_0_TITLE}',PASSFAILS_0_SUBSECTION='{PASSFAILS_0_SUBSECTION}',PASSFAILS_0_LISTVALUES_VALUE='{PASSFAILS_0_LISTVALUES_VALUE}'
                //             where UNIQUE_KEY = '{UNIQUE_KEY}'";
                //sql1 = $@"update t_aeqs_to_p88_list set DEFECTIVE_PARTS='{DEFECTIVE_PARTS}',PASSFAILS_0_TITLE='{PASSFAILS_0_TITLE}',PASSFAILS_0_SUBSECTION='{PASSFAILS_0_SUBSECTION}',
                //          PASSFAILS_0_LISTVALUES_VALUE='{PASSFAILS_0_LISTVALUES_VALUE}',MODIFYING_COUNT='{NEWCOUNT}' where UNIQUE_KEY = '{UNIQUE_KEY}'";
                sql1 = $@"update t_aeqs_to_p88_list set DATE_STARTED=to_date('{DATE_STARTED}','yyyy/MM/dd HH24:mi:ss'),DEFECTIVE_PARTS='{DEFECTIVE_PARTS}',PASSFAILS_0_TITLE='{PASSFAILS_0_TITLE}',PASSFAILS_0_SUBSECTION='{PASSFAILS_0_SUBSECTION}',
                          PASSFAILS_0_LISTVALUES_VALUE='{PASSFAILS_0_LISTVALUES_VALUE}',MODIFYING_COUNT='{NEWCOUNT}' where UNIQUE_KEY = '{UNIQUE_KEY}'";
                DB.ExecuteNonQuery(sql1);

                sql2 = $@"insert into t_aeqs_to_p88_list_log( UNIQUE_KEY,DATE_STARTED,DEFECTIVE_PARTS,PASSFAILS_0_TITLE,PASSFAILS_0_SUBSECTION,PASSFAILS_0_LISTVALUES_VALUE,MODIFIED_BY,MODIFIED_DATE) values('{UNIQUE_KEY}',to_date('{DATE_STARTED2}','yyyy/MM/dd HH24:mi:ss'),'{DEFECTIVE_PARTS2}','{PASSFAILS_0_TITLE2}','{PASSFAILS_0_SUBSECTION2}',
                          '{PASSFAILS_0_LISTVALUES_VALUE2}','{User_Account}',sysdate)";
                DB.ExecuteNonQuery(sql2);
            }
        }

        public static void Updatedt2(DataBase DB, DataTable dt2, string USER)
        {
            if (dt2.Rows.Count > 0)
            {
                for (int i = 0; i < dt2.Rows.Count; i++)
                {
                    DataRow dr = dt2.Rows[i];
                    string ID = dr["ID"].ToString();
                    string UNION_ID = dr["UNION_ID"].ToString();
                    string SECTIONS_RESULT_ID = dr["SECTIONS_RESULT_ID"].ToString();
                    string SECTIONS_QTY_INSPECTED = dr["SECTIONS_QTY_INSPECTED"].ToString();
                    string SECTIONS_SAMPLED_INSPECTED = dr["SECTIONS_SAMPLED_INSPECTED"].ToString();
                    string SECTIONS_DEFECTIVE_PARTS = dr["SECTIONS_DEFECTIVE_PARTS"].ToString();
                    string SECTIONS_AQL_MINOR = dr["SECTIONS_AQL_MINOR"].ToString();
                    string SECTIONS_AQL_MAJOR = dr["SECTIONS_AQL_MAJOR"].ToString();
                    string SECTIONS_AQL_CRITICAL = dr["SECTIONS_AQL_CRITICAL"].ToString();
                    string SECTIONS_BARCODES_VALUE = dr["SECTIONS_BARCODES_VALUE"].ToString();
                    string SECTIONS_QTY_TYPE = dr["SECTIONS_QTY_TYPE"].ToString();
                    string SECTIONS_MAX_MINOR_DEFECTS = dr["SECTIONS_MAX_MINOR_DEFECTS"].ToString();
                    string SECTIONS_MAX_MAJOR_DEFECTS = dr["SECTIONS_MAX_MAJOR_DEFECTS"].ToString();
                    string SECTIONS_MAX_MAJOR_A_DEFECTS = dr["SECTIONS_MAX_MAJOR_A_DEFECTS"].ToString();
                    string SECTIONS_MAX_MAJOR_B_DEFECTS = dr["SECTIONS_MAX_MAJOR_B_DEFECTS"].ToString();
                    string SECTIONS_MAX_CRITICAL_DEFECTS = dr["SECTIONS_MAX_CRITICAL_DEFECTS"].ToString();
                    string chkval = string.Empty;
                    if (dr["SECTIONS_DEFECTS_LABEL"].ToString().Contains("'"))
                        chkval = dr["SECTIONS_DEFECTS_LABEL"].ToString().Replace(@"'", "");
                    else
                        chkval = dr["SECTIONS_DEFECTS_LABEL"].ToString();
                    string SECTIONS_DEFECTS_LABEL = chkval;
                    string SECTIONS_DEFECTS_SUBSECTION = dr["SECTIONS_DEFECTS_SUBSECTION"].ToString();
                    string SECTIONS_DEFECTS_CODE = dr["SECTIONS_DEFECTS_CODE"].ToString();
                    string SECTIONS_DEFECTS_CRITICAL_LEVEL = dr["SECTIONS_DEFECTS_CRITICAL_LEVEL"].ToString();
                    string SECTIONS_DEFECTS_MAJOR_LEVEL = dr["SECTIONS_DEFECTS_MAJOR_LEVEL"].ToString();
                    string SECTIONS_DEFECTS_MINOR_LEVEL = dr["SECTIONS_DEFECTS_MINOR_LEVEL"].ToString();
                    string SECTIONS_DEFECTS_COMMENTS = dr["SECTIONS_DEFECTS_COMMENTS"].ToString();

                    string sql = string.Empty;
                    sql = $@"select * from t_aeqs_to_p88_sections where ID = '{ID}' and UNION_ID = '{UNION_ID}'";
                    DataTable dt = DB.GetDataTable(sql);

                    DataRow dr2 = dt.Rows[0];
                    string ID2 = dr2["ID"].ToString();
                    string UNION_ID2 = dr2["UNION_ID"].ToString();
                    string SECTIONS_RESULT_ID2 = dr2["SECTIONS_RESULT_ID"].ToString();
                    string SECTIONS_QTY_INSPECTED2 = dr2["SECTIONS_QTY_INSPECTED"].ToString();
                    string SECTIONS_SAMPLED_INSPECTED2 = dr2["SECTIONS_SAMPLED_INSPECTED"].ToString();
                    string SECTIONS_DEFECTIVE_PARTS2 = dr2["SECTIONS_DEFECTIVE_PARTS"].ToString();
                    string SECTIONS_AQL_MINOR2 = dr2["SECTIONS_AQL_MINOR"].ToString();
                    string SECTIONS_AQL_MAJOR2 = dr2["SECTIONS_AQL_MAJOR"].ToString();
                    string SECTIONS_AQL_CRITICAL2 = dr2["SECTIONS_AQL_CRITICAL"].ToString();
                    string SECTIONS_BARCODES_VALUE2 = dr2["SECTIONS_BARCODES_VALUE"].ToString();
                    string SECTIONS_QTY_TYPE2 = dr2["SECTIONS_QTY_TYPE"].ToString();
                    string SECTIONS_MAX_MINOR_DEFECTS2 = dr2["SECTIONS_MAX_MINOR_DEFECTS"].ToString();
                    string SECTIONS_MAX_MAJOR_DEFECTS2 = dr2["SECTIONS_MAX_MAJOR_DEFECTS"].ToString();
                    string SECTIONS_MAX_MAJOR_A_DEFECTS2 = dr2["SECTIONS_MAX_MAJOR_A_DEFECTS"].ToString();
                    string SECTIONS_MAX_MAJOR_B_DEFECTS2 = dr2["SECTIONS_MAX_MAJOR_B_DEFECTS"].ToString();
                    string SECTIONS_MAX_CRITICAL_DEFECTS2 = dr2["SECTIONS_MAX_CRITICAL_DEFECTS"].ToString();
                    string chkval2 = string.Empty;
                    if (dr["SECTIONS_DEFECTS_LABEL"].ToString().Contains("'"))
                        chkval2 = dr["SECTIONS_DEFECTS_LABEL"].ToString().Replace(@"'", "");
                    else
                        chkval2 = dr["SECTIONS_DEFECTS_LABEL"].ToString();
                    string SECTIONS_DEFECTS_LABEL2 = chkval;
                    string SECTIONS_DEFECTS_SUBSECTION2 = dr["SECTIONS_DEFECTS_SUBSECTION"].ToString();
                    string SECTIONS_DEFECTS_CODE2 = dr["SECTIONS_DEFECTS_CODE"].ToString();
                    string SECTIONS_DEFECTS_CRITICAL_LEVEL2 = dr["SECTIONS_DEFECTS_CRITICAL_LEVEL"].ToString();
                    string SECTIONS_DEFECTS_MAJOR_LEVEL2 = dr["SECTIONS_DEFECTS_MAJOR_LEVEL"].ToString();
                    string SECTIONS_DEFECTS_MINOR_LEVEL2 = dr["SECTIONS_DEFECTS_MINOR_LEVEL"].ToString();
                    string SECTIONS_DEFECTS_COMMENTS2 = dr["SECTIONS_DEFECTS_COMMENTS"].ToString();
                    string User_Account = USER;

                    string sql1 = string.Empty;
                    sql1 = $@"insert into t_aeqs_to_p88_sections_log(ID,UNION_ID,SECTIONS_RESULT_ID,SECTIONS_QTY_INSPECTED,SECTIONS_SAMPLED_INSPECTED,SECTIONS_DEFECTIVE_PARTS,SECTIONS_AQL_MINOR,SECTIONS_AQL_MAJOR,SECTIONS_AQL_CRITICAL
                                     ,SECTIONS_BARCODES_VALUE,SECTIONS_QTY_TYPE,SECTIONS_MAX_MINOR_DEFECTS,SECTIONS_MAX_MAJOR_DEFECTS,SECTIONS_MAX_MAJOR_A_DEFECTS,SECTIONS_MAX_MAJOR_B_DEFECTS,SECTIONS_MAX_CRITICAL_DEFECTS
                                     ,SECTIONS_DEFECTS_LABEL,SECTIONS_DEFECTS_SUBSECTION,SECTIONS_DEFECTS_CODE,SECTIONS_DEFECTS_CRITICAL_LEVEL,SECTIONS_DEFECTS_MAJOR_LEVEL,SECTIONS_DEFECTS_MINOR_LEVEL,SECTIONS_DEFECTS_COMMENTS,MODIFIED_BY,MODIFIED_DATE) 
                                      values('{ID2}','{UNION_ID2}','{SECTIONS_RESULT_ID2}','{SECTIONS_QTY_INSPECTED2}','{SECTIONS_SAMPLED_INSPECTED2}','{SECTIONS_DEFECTIVE_PARTS2}','{SECTIONS_AQL_MINOR2}','{SECTIONS_AQL_MAJOR2}','{SECTIONS_AQL_CRITICAL2}',
                                    '{SECTIONS_BARCODES_VALUE2}','{SECTIONS_QTY_TYPE2}','{SECTIONS_MAX_MINOR_DEFECTS2}','{SECTIONS_MAX_MAJOR_DEFECTS2}','{SECTIONS_MAX_MAJOR_A_DEFECTS2}','{SECTIONS_MAX_MAJOR_B_DEFECTS2}','{SECTIONS_MAX_CRITICAL_DEFECTS2}',
                                    '{SECTIONS_DEFECTS_LABEL2}','{SECTIONS_DEFECTS_SUBSECTION2}','{SECTIONS_DEFECTS_CODE2}','{SECTIONS_DEFECTS_CRITICAL_LEVEL2}','{SECTIONS_DEFECTS_MAJOR_LEVEL2}','{SECTIONS_DEFECTS_MINOR_LEVEL2}','{SECTIONS_DEFECTS_COMMENTS2}','{User_Account}',sysdate)";
                    DB.ExecuteNonQuery(sql1);

                    string sql2 = string.Empty;
                        sql2 = $@"update t_aeqs_to_p88_sections set SECTIONS_RESULT_ID='{SECTIONS_RESULT_ID}',SECTIONS_QTY_INSPECTED='{SECTIONS_QTY_INSPECTED}',SECTIONS_SAMPLED_INSPECTED='{SECTIONS_SAMPLED_INSPECTED}',SECTIONS_DEFECTIVE_PARTS='{SECTIONS_DEFECTIVE_PARTS}'
                                 ,SECTIONS_AQL_MINOR='{SECTIONS_AQL_MINOR}',SECTIONS_AQL_MAJOR='{SECTIONS_AQL_MAJOR}',SECTIONS_AQL_CRITICAL='{SECTIONS_AQL_CRITICAL}',SECTIONS_BARCODES_VALUE='{SECTIONS_BARCODES_VALUE}',SECTIONS_QTY_TYPE='{SECTIONS_QTY_TYPE}'
                                 ,SECTIONS_MAX_MINOR_DEFECTS='{SECTIONS_MAX_MINOR_DEFECTS}',SECTIONS_MAX_MAJOR_DEFECTS='{SECTIONS_MAX_MAJOR_DEFECTS}',SECTIONS_MAX_MAJOR_A_DEFECTS='{SECTIONS_MAX_MAJOR_A_DEFECTS}',SECTIONS_MAX_MAJOR_B_DEFECTS='{SECTIONS_MAX_MAJOR_B_DEFECTS}',SECTIONS_MAX_CRITICAL_DEFECTS='{SECTIONS_MAX_CRITICAL_DEFECTS}'
                                 ,SECTIONS_DEFECTS_LABEL='{SECTIONS_DEFECTS_LABEL}',SECTIONS_DEFECTS_SUBSECTION='{SECTIONS_DEFECTS_SUBSECTION}',SECTIONS_DEFECTS_CODE='{SECTIONS_DEFECTS_CODE}',SECTIONS_DEFECTS_CRITICAL_LEVEL='{SECTIONS_DEFECTS_CRITICAL_LEVEL}',SECTIONS_DEFECTS_MAJOR_LEVEL='{SECTIONS_DEFECTS_MAJOR_LEVEL}'
                                 ,SECTIONS_DEFECTS_MINOR_LEVEL='{SECTIONS_DEFECTS_MINOR_LEVEL}',SECTIONS_DEFECTS_COMMENTS='{SECTIONS_DEFECTS_COMMENTS}'
                                  where ID = '{ID}' and UNION_ID = '{UNION_ID}'";
                        DB.ExecuteNonQuery(sql2);
                        
                    

                }
            }
        }

        public static void Updatedt3(DataBase DB, DataTable dt3, string USER)
        {
            if (dt3.Rows.Count > 0)
            {
                for (int i = 0; i < dt3.Rows.Count; i++)
                {
                    DataRow dr = dt3.Rows[i];
                    string ID = dr["ID1"].ToString();
                    string UNION_ID = dr["UNION_ID1"].ToString();
                    string PASSFAILS_TITLE = dr["PASSFAILS_TITLE"].ToString();
                    string PASSFAILS_VALUE = dr["PASSFAILS_VALUE"].ToString();
                    string PASSFAILS_TYPE = dr["PASSFAILS_TYPE"].ToString();
                    string PASSFAILS_SUBSECTION = dr["PASSFAILS_SUBSECTION"].ToString();
                    string PASSFAILS_CHECKLISTSUBSECTION = dr["PASSFAILS_CHECKLISTSUBSECTION"].ToString();
                    string PASSFAILS_STATUS = dr["PASSFAILS_STATUS"].ToString();
                    string PASSFAILS_COMMENT = dr["PASSFAILS_COMMENT"].ToString();

                    string sql = string.Empty;
                    sql = $@"select * from t_aeqs_to_p88_passfail where ID = '{ID}' and UNION_ID = '{UNION_ID}'";
                    DataTable dt = DB.GetDataTable(sql);

                    DataRow dr2 = dt.Rows[0];
                    string ID2 = dr2["ID"].ToString();
                    string UNION_ID2 = dr2["UNION_ID"].ToString();
                    string PASSFAILS_TITLE2 = dr2["PASSFAILS_TITLE"].ToString();
                    string PASSFAILS_VALUE2 = dr2["PASSFAILS_VALUE"].ToString();
                    string PASSFAILS_TYPE2 = dr2["PASSFAILS_TYPE"].ToString();
                    string PASSFAILS_SUBSECTION2 = dr2["PASSFAILS_SUBSECTION"].ToString();
                    string PASSFAILS_CHECKLISTSUBSECTION2 = dr2["PASSFAILS_CHECKLISTSUBSECTION"].ToString();
                    string PASSFAILS_STATUS2 = dr2["PASSFAILS_STATUS"].ToString();
                    string PASSFAILS_COMMENT2 = dr2["PASSFAILS_COMMENT"].ToString();
                    string User_Account = USER;

                    string sql2 = string.Empty;
                    sql2 = $@"insert into t_aeqs_to_p88_passfail_log(ID,UNION_ID,PASSFAILS_TITLE,PASSFAILS_VALUE,PASSFAILS_TYPE,PASSFAILS_SUBSECTION,PASSFAILS_CHECKLISTSUBSECTION,PASSFAILS_STATUS,PASSFAILS_COMMENT,MODIFIED_BY,MODIFIED_DATE) values('{ID2}','{UNION_ID2}','{PASSFAILS_TITLE2}','{PASSFAILS_VALUE2}','{PASSFAILS_TYPE2}','{PASSFAILS_SUBSECTION2}'
                                 ,'{PASSFAILS_CHECKLISTSUBSECTION2}','{PASSFAILS_STATUS2}','{PASSFAILS_COMMENT2}','{User_Account}',sysdate) ";

                    DB.ExecuteNonQuery(sql2);


                    string sql3 = string.Empty;
                        sql3 = $@"update t_aeqs_to_p88_passfail set PASSFAILS_TITLE='{PASSFAILS_TITLE}',PASSFAILS_VALUE='{PASSFAILS_VALUE}',PASSFAILS_TYPE='{PASSFAILS_TYPE}',PASSFAILS_SUBSECTION='{PASSFAILS_SUBSECTION}'
                                 ,PASSFAILS_CHECKLISTSUBSECTION='{PASSFAILS_CHECKLISTSUBSECTION}',PASSFAILS_STATUS='{PASSFAILS_STATUS}',PASSFAILS_COMMENT='{PASSFAILS_COMMENT}'
                                  where ID = '{ID}' and UNION_ID = '{UNION_ID}'";
                        DB.ExecuteNonQuery(sql3);
                     
                }
            }
        }

        public static void Updatedt4(DataBase DB, DataTable dt4, string USER)
        {
            if (dt4.Rows.Count > 0)
            {
                for (int i = 0; i < dt4.Rows.Count; i++)
                {
                    DataRow dr = dt4.Rows[i];
                    string ID = dr["ID2"].ToString();
                    string UNION_ID = dr["UNION_ID_A"].ToString();
                    string ASSIGNMENT_ITEMS_SAMPLED_INSPECTED = dr["ASSIGNMENT_ITEMS_SAMPLED_INSPECTED"].ToString();
                    string ASSIGNMENT_ITEMS_INSPECTION_RESULT_ID = dr["ASSIGNMENT_ITEMS_INSPECTION_RESULT_ID"].ToString();
                    string ASSIGNMENT_ITEMS_INSPECTION_STATUS_ID = dr["ASSIGNMENT_ITEMS_INSPECTION_STATUS_ID"].ToString();
                    string ASSIGNMENT_ITEMS_QTY_INSPECTED = dr["ASSIGNMENT_ITEMS_QTY_INSPECTED"].ToString();
                    string ASSIGNMENT_ITEMS_TOTAL_INSPECTION_MINUTES = dr["ASSIGNMENT_ITEMS_TOTAL_INSPECTION_MINUTES"].ToString();
                    string ASSIGNMENT_ITEMS_SAMPLING_SIZE = dr["ASSIGNMENT_ITEMS_SAMPLING_SIZE"].ToString();
                    string ASSIGNMENT_ITEMS_QTY_TO_INSPECT = dr["ASSIGNMENT_ITEMS_QTY_TO_INSPECT"].ToString();
                    string ASSIGNMENT_ITEMS_AQL_MINOR = dr["ASSIGNMENT_ITEMS_AQL_MINOR"].ToString();
                    string ASSIGNMENT_ITEMS_AQL_MAJOR = dr["ASSIGNMENT_ITEMS_AQL_MAJOR"].ToString();
                    string ASSIGNMENT_ITEMS_AQL_MAJOR_A = dr["ASSIGNMENT_ITEMS_AQL_MAJOR_A"].ToString();
                    string ASSIGNMENT_ITEMS_AQL_MAJOR_B = dr["ASSIGNMENT_ITEMS_AQL_MAJOR_B"].ToString();
                    string ASSIGNMENT_ITEMS_AQL_CRITICAL = dr["ASSIGNMENT_ITEMS_AQL_CRITICAL"].ToString();
                    string ASSIGNMENT_ITEMS_SUPPLIER_BOOKING_MSG = dr["ASSIGNMENT_ITEMS_SUPPLIER_BOOKING_MSG"].ToString();
                    string ASSIGNMENT_ITEMS_CONCLUSION_REMARKS = dr["ASSIGNMENT_ITEMS_CONCLUSION_REMARKS"].ToString();
                    string ASSIGNMENT_ITEMS_ASSIGNMENT_INSPECTOR_USERNAME = dr["ASSIGNMENT_ITEMS_ASSIGNMENT_INSPECTOR_USERNAME"].ToString();
                    string ASSIGNMENT_ITEMS_PO_LINE_SKU_SKU_NUMBER = dr["ASSIGNMENT_ITEMS_PO_LINE_SKU_SKU_NUMBER"].ToString();
                    string ASSIGNMENT_ITEMS_PO_LINE_SKU_ITEM_NAME = dr["ASSIGNMENT_ITEMS_PO_LINE_SKU_ITEM_NAME"].ToString();
                    string ASSIGNMENT_ITEMS_PO_LINE_SKU_ITEM_DESCRIPTION = dr["ASSIGNMENT_ITEMS_PO_LINE_SKU_ITEM_DESCRIPTION"].ToString();

                    string sql = string.Empty;
                    sql = $@"select * from t_aeqs_to_p88_assignment where ID = '{ID}' and UNION_ID = '{UNION_ID}'";
                    DataTable dt = DB.GetDataTable(sql);

                    DataRow dr2 = dt.Rows[0];
                    string ID2 = dr2["ID"].ToString();
                    string UNION_ID2 = dr2["UNION_ID"].ToString();
                    string ASSIGNMENT_ITEMS_SAMPLED_INSPECTED2 = dr2["ASSIGNMENT_ITEMS_SAMPLED_INSPECTED"].ToString();
                    string ASSIGNMENT_ITEMS_INSPECTION_RESULT_ID2 = dr2["ASSIGNMENT_ITEMS_INSPECTION_RESULT_ID"].ToString();
                    string ASSIGNMENT_ITEMS_INSPECTION_STATUS_ID2 = dr2["ASSIGNMENT_ITEMS_INSPECTION_STATUS_ID"].ToString();
                    string ASSIGNMENT_ITEMS_QTY_INSPECTED2 = dr2["ASSIGNMENT_ITEMS_QTY_INSPECTED"].ToString();
                    string ASSIGNMENT_ITEMS_TOTAL_INSPECTION_MINUTES2 = dr2["ASSIGNMENT_ITEMS_TOTAL_INSPECTION_MINUTES"].ToString();
                    string ASSIGNMENT_ITEMS_SAMPLING_SIZE2 = dr2["ASSIGNMENT_ITEMS_SAMPLING_SIZE"].ToString();
                    string ASSIGNMENT_ITEMS_QTY_TO_INSPECT2 = dr2["ASSIGNMENT_ITEMS_QTY_TO_INSPECT"].ToString();
                    string ASSIGNMENT_ITEMS_AQL_MINOR2 = dr2["ASSIGNMENT_ITEMS_AQL_MINOR"].ToString();
                    string ASSIGNMENT_ITEMS_AQL_MAJOR2 = dr2["ASSIGNMENT_ITEMS_AQL_MAJOR"].ToString();
                    string ASSIGNMENT_ITEMS_AQL_MAJOR_A2 = dr2["ASSIGNMENT_ITEMS_AQL_MAJOR_A"].ToString();
                    string ASSIGNMENT_ITEMS_AQL_MAJOR_B2 = dr2["ASSIGNMENT_ITEMS_AQL_MAJOR_B"].ToString();
                    string ASSIGNMENT_ITEMS_AQL_CRITICAL2 = dr2["ASSIGNMENT_ITEMS_AQL_CRITICAL"].ToString();
                    string ASSIGNMENT_ITEMS_SUPPLIER_BOOKING_MSG2 = dr2["ASSIGNMENT_ITEMS_SUPPLIER_BOOKING_MSG"].ToString();
                    string ASSIGNMENT_ITEMS_CONCLUSION_REMARKS2 = dr2["ASSIGNMENT_ITEMS_CONCLUSION_REMARKS"].ToString();
                    string ASSIGNMENT_ITEMS_ASSIGNMENT_INSPECTOR_USERNAME2 = dr2["ASSIGNMENT_ITEMS_ASSIGNMENT_INSPECTOR_USERNAME"].ToString();
                    string ASSIGNMENT_ITEMS_PO_LINE_SKU_SKU_NUMBER2 = dr2["ASSIGNMENT_ITEMS_PO_LINE_SKU_SKU_NUMBER"].ToString();
                    string ASSIGNMENT_ITEMS_PO_LINE_SKU_ITEM_NAME2 = dr2["ASSIGNMENT_ITEMS_PO_LINE_SKU_ITEM_NAME"].ToString();
                    string ASSIGNMENT_ITEMS_PO_LINE_SKU_ITEM_DESCRIPTION2 = dr2["ASSIGNMENT_ITEMS_PO_LINE_SKU_ITEM_DESCRIPTION"].ToString();
                    string User_Account = USER;
                    string sql3 = string.Empty; 
                    sql3 = $@"insert into t_aeqs_to_p88_assignment_log(ID,UNION_ID,ASSIGNMENT_ITEMS_SAMPLED_INSPECTED,ASSIGNMENT_ITEMS_INSPECTION_RESULT_ID,ASSIGNMENT_ITEMS_INSPECTION_STATUS_ID,ASSIGNMENT_ITEMS_QTY_INSPECTED,ASSIGNMENT_ITEMS_TOTAL_INSPECTION_MINUTES,ASSIGNMENT_ITEMS_SAMPLING_SIZE
                               ,ASSIGNMENT_ITEMS_QTY_TO_INSPECT,ASSIGNMENT_ITEMS_AQL_MINOR,ASSIGNMENT_ITEMS_AQL_MAJOR,ASSIGNMENT_ITEMS_AQL_MAJOR_A,ASSIGNMENT_ITEMS_AQL_MAJOR_B,ASSIGNMENT_ITEMS_AQL_CRITICAL,ASSIGNMENT_ITEMS_SUPPLIER_BOOKING_MSG,ASSIGNMENT_ITEMS_CONCLUSION_REMARKS,ASSIGNMENT_ITEMS_ASSIGNMENT_INSPECTOR_USERNAME,
                                ASSIGNMENT_ITEMS_PO_LINE_SKU_SKU_NUMBER,ASSIGNMENT_ITEMS_PO_LINE_SKU_ITEM_NAME,ASSIGNMENT_ITEMS_PO_LINE_SKU_ITEM_DESCRIPTION,MODIFIED_BY,MODIFIED_DATE) 
                                values('{ID2}','{UNION_ID2}','{ASSIGNMENT_ITEMS_SAMPLED_INSPECTED2}','{ASSIGNMENT_ITEMS_INSPECTION_RESULT_ID2}','{ASSIGNMENT_ITEMS_INSPECTION_STATUS_ID2}','{ASSIGNMENT_ITEMS_QTY_INSPECTED2}','{ASSIGNMENT_ITEMS_TOTAL_INSPECTION_MINUTES2}','{ASSIGNMENT_ITEMS_SAMPLING_SIZE2}',
                                '{ASSIGNMENT_ITEMS_QTY_TO_INSPECT2}','{ASSIGNMENT_ITEMS_AQL_MINOR2}','{ASSIGNMENT_ITEMS_AQL_MAJOR2}','{ASSIGNMENT_ITEMS_AQL_MAJOR_A2}','{ASSIGNMENT_ITEMS_AQL_MAJOR_B2}','{ASSIGNMENT_ITEMS_AQL_CRITICAL2}','{ASSIGNMENT_ITEMS_SUPPLIER_BOOKING_MSG2}'
                                ,'{ASSIGNMENT_ITEMS_CONCLUSION_REMARKS2}','{ASSIGNMENT_ITEMS_ASSIGNMENT_INSPECTOR_USERNAME2}','{ASSIGNMENT_ITEMS_PO_LINE_SKU_SKU_NUMBER2}','{ASSIGNMENT_ITEMS_PO_LINE_SKU_ITEM_NAME2}','{ASSIGNMENT_ITEMS_PO_LINE_SKU_ITEM_DESCRIPTION2}','{User_Account}',sysdate)";
                    DB.ExecuteNonQuery(sql3);

                    string sql4 = string.Empty;
                        //sql4 = $@"update t_aeqs_to_p88_passfail set PASSFAILS_TITLE='{PASSFAILS_TITLE}',PASSFAILS_VALUE='{PASSFAILS_VALUE}',PASSFAILS_TYPE='{PASSFAILS_TYPE}',PASSFAILS_SUBSECTION='{PASSFAILS_SUBSECTION}'
                        //             ,PASSFAILS_CHECKLISTSUBSECTION='{PASSFAILS_CHECKLISTSUBSECTION}',PASSFAILS_STATUS='{PASSFAILS_STATUS}',PASSFAILS_COMMENT='{PASSFAILS_COMMENT}'
                        //              where ID = '{ID}' and UNION_ID = '{UNION_ID}'";
                        sql4 = $@"update t_aeqs_to_p88_assignment set ASSIGNMENT_ITEMS_SAMPLED_INSPECTED='{ASSIGNMENT_ITEMS_SAMPLED_INSPECTED}',ASSIGNMENT_ITEMS_INSPECTION_RESULT_ID='{ASSIGNMENT_ITEMS_INSPECTION_RESULT_ID}',ASSIGNMENT_ITEMS_INSPECTION_STATUS_ID='{ASSIGNMENT_ITEMS_INSPECTION_STATUS_ID}'
                                ,ASSIGNMENT_ITEMS_QTY_INSPECTED='{ASSIGNMENT_ITEMS_QTY_INSPECTED}',ASSIGNMENT_ITEMS_TOTAL_INSPECTION_MINUTES='{ASSIGNMENT_ITEMS_TOTAL_INSPECTION_MINUTES}',ASSIGNMENT_ITEMS_SAMPLING_SIZE='{ASSIGNMENT_ITEMS_SAMPLING_SIZE}',ASSIGNMENT_ITEMS_QTY_TO_INSPECT='{ASSIGNMENT_ITEMS_QTY_TO_INSPECT}',ASSIGNMENT_ITEMS_AQL_MINOR='{ASSIGNMENT_ITEMS_AQL_MINOR}'
                                ,ASSIGNMENT_ITEMS_AQL_MAJOR='{ASSIGNMENT_ITEMS_AQL_MAJOR}',ASSIGNMENT_ITEMS_AQL_MAJOR_A='{ASSIGNMENT_ITEMS_AQL_MAJOR_A}',ASSIGNMENT_ITEMS_AQL_MAJOR_B='{ASSIGNMENT_ITEMS_AQL_MAJOR_B}',ASSIGNMENT_ITEMS_AQL_CRITICAL='{ASSIGNMENT_ITEMS_AQL_CRITICAL}',ASSIGNMENT_ITEMS_SUPPLIER_BOOKING_MSG='{ASSIGNMENT_ITEMS_SUPPLIER_BOOKING_MSG}'
                                ,ASSIGNMENT_ITEMS_CONCLUSION_REMARKS='{ASSIGNMENT_ITEMS_CONCLUSION_REMARKS}',ASSIGNMENT_ITEMS_ASSIGNMENT_INSPECTOR_USERNAME='{ASSIGNMENT_ITEMS_ASSIGNMENT_INSPECTOR_USERNAME}',ASSIGNMENT_ITEMS_PO_LINE_SKU_SKU_NUMBER='{ASSIGNMENT_ITEMS_PO_LINE_SKU_SKU_NUMBER}',ASSIGNMENT_ITEMS_PO_LINE_SKU_ITEM_NAME='{ASSIGNMENT_ITEMS_PO_LINE_SKU_ITEM_NAME}',ASSIGNMENT_ITEMS_PO_LINE_SKU_ITEM_DESCRIPTION='{ASSIGNMENT_ITEMS_PO_LINE_SKU_ITEM_DESCRIPTION}'
                                 where ID = '{ID}' and UNION_ID = '{UNION_ID}'";
                        DB.ExecuteNonQuery(sql4);
                       
                     
                }
            }
        }

        public static SJeMES_Framework_NETCore.WebAPI.ResultObject DeleteDataByUniqueKey(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase();
            try
            {
                DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);
                string Data = ReqObj.Data.ToString();
                DB.Open();
                DB.BeginTransaction();
                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                //Input Params
                string vSource = jarr.ContainsKey("UNIQUE_KEY") ? jarr["UNIQUE_KEY"].ToString() : ""; //vSource 

                String sql1 = $@"DELETE FROM T_AEQS_TO_P88_PASSFAIL P WHERE P.UNION_ID='{vSource}'";
                DB.ExecuteNonQuery(sql1);

                String sql2 = $@"DELETE FROM T_AEQS_TO_P88_ASSIGNMENT A WHERE A.UNION_ID='{vSource}'";
                DB.ExecuteNonQuery(sql2);

                String sql3 = $@"DELETE FROM t_aeqs_to_p88_list S WHERE S.unique_key='{vSource}'";
                DB.ExecuteNonQuery(sql3);

                String sql4 = $@"DELETE FROM T_AEQS_TO_P88_SECTIONS S WHERE S.UNION_ID='{vSource}'";
                DB.ExecuteNonQuery(sql4);

                String sql5 = $@"select ID from t_aeqs_to_p88_sections where union_id ='{vSource}'";
                DataTable dt = DB.GetDataTable(sql5);

                string ID = string.Empty;
                foreach (DataRow dr in dt.Rows)
                {
                    string name = dr["ID"].ToString();
                    ID += "'" + name + "'" + ",";
                }
                ID = ID.TrimEnd(',');
                string sql6 = $@"DELETE FROM T_AEQS_TO_P88_SECTIONS_F S WHERE S.UNION_ID='{vSource}' or S.UNION_ID in ({ID})";
                DB.ExecuteNonQuery(sql6);

                ret.IsSuccess = true;
                DB.Commit();

            }
            catch (Exception ex)
            {
                DB.Rollback();
                ret.IsSuccess = false;
                ret.ErrMsg = ex.Message;
            }
            finally
            {
                DB.Close();
            }
            return ret;
        }
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject CheckModifyUser(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase();
            try
            {
                DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);
                string Data = ReqObj.Data.ToString();
                DB.Open();
                DB.BeginTransaction();
                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);

                string User = SJeMES_Framework_NETCore.Web.System.GetUserCodeByToken(ReqObj.UserToken);




                //String sql = $@"SELECT * FROM T_AEQS_TO_P88_LIST T WHERE TO_CHAR(T.DATE_STARTED,'YYYY/MM/DD') BETWEEN '{from}' AND '{to}'  AND T.ASSIGNMENT_ITEMS_ASSIGNMENT_REPORT_TYPE_ID={report_type_id} ORDER BY UNIQUE_KEY";
                String sql = $@"select * from T_AEQS_TO_P88_MODIFIER a where a.modifier_code='{User}'";

                DataTable dt = DB.GetDataTable(sql);
                 if (dt.Rows.Count>0)
                {
                    ret.IsSuccess = true;
                }
                else
                {
                    ret.IsSuccess = false;
                } 

            }
            catch (Exception ex)
            {
                DB.Rollback();
                ret.IsSuccess = false;
                ret.ErrMsg = ex.Message;
            }
            finally
            {
                DB.Close();
            }
            return ret;
        }

        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetLogData(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase();
            try
            {
                DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);
                string Data = ReqObj.Data.ToString();
                DB.Open();
                DB.BeginTransaction();
                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                string from = jarr.ContainsKey("from") ? jarr["from"].ToString() : ""; //from
                string to = jarr.ContainsKey("to") ? jarr["to"].ToString() : ""; //to
                //int report_type_id = jarr.ContainsKey("report_type_id") ? Convert.ToInt16(jarr["report_type_id"]) : 0; //report_type_id 
                string unique_key = jarr.ContainsKey("unique_key") ? jarr["unique_key"].ToString() : "";


                string wheresql1 = string.Empty;
                string wheresql2 = string.Empty;
                string wheresql3 = string.Empty;

                if (!string.IsNullOrEmpty(from) && !string.IsNullOrEmpty(to))
                {
                    wheresql1 += $@" and TO_CHAR(a.MODIFIED_DATE,'YYYY/MM/DD') BETWEEN '{from}' AND '{to}'";
                }
                
                if (!string.IsNullOrEmpty(unique_key))
                {
                    wheresql2 += $@"and a.unique_key='{unique_key}'";
                    wheresql3 += $@"and a.union_id='{unique_key}'";
                }
                 
                String sql = $@"select* from t_aeqs_to_p88_list_log a where 1=1 {wheresql2}{wheresql1}";

                DataTable dt1 = DB.GetDataTable(sql);

                String sql2 = $@"select* from t_aeqs_to_p88_assignment_log a where 1=1 {wheresql3}{wheresql1}";

                DataTable dt2 = DB.GetDataTable(sql2);

                String sql3 = $@"select* from t_aeqs_to_p88_sections_log a where 1=1 {wheresql3}{wheresql1}";

                DataTable dt3 = DB.GetDataTable(sql3);

                String sql4 = $@"select* from t_aeqs_to_p88_passfail_log a where 1=1 {wheresql3}{wheresql1}";

                DataTable dt4 = DB.GetDataTable(sql4);

                Dictionary<string, object> dic = new Dictionary<string, object>();

                dic.Add("Data1", dt1);
                dic.Add("Data2", dt2);
                dic.Add("Data3", dt3);
                dic.Add("Data4", dt4);

                ret.RetData = Newtonsoft.Json.JsonConvert.SerializeObject(dic);
                ret.IsSuccess = true;

            }
            catch (Exception ex)
            {
                DB.Rollback();
                ret.IsSuccess = false;
                ret.ErrMsg = ex.Message;
            }
            finally
            {
                DB.Close();
            }
            return ret;
        }


        public static SJeMES_Framework_NETCore.WebAPI.ResultObject LoadUnique_Key(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase();
            try
            {
                DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);
                string Data = ReqObj.Data.ToString();
                DB.Open();
                DB.BeginTransaction();
                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);  
                String sql = $@"select UNIQUE_KEY from t_aeqs_to_p88_list";

                DataTable dt = DB.GetDataTable(sql);
                Dictionary<string, object> dic = new Dictionary<string, object>();

                dic.Add("Data", dt); 
                ret.RetData = Newtonsoft.Json.JsonConvert.SerializeObject(dic);
                ret.IsSuccess = true;

            }
            catch (Exception ex)
            {
                DB.Rollback();
                ret.IsSuccess = false;
                ret.ErrMsg = ex.Message;
            }
            finally
            {
                DB.Close();
            }
            return ret;
        }

    }
}
