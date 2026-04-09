using SJ_QCMAPI.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace SJ_PRINTAPI
{
    public class QCM_Ex_Task_Print
    {
        /// <summary>
        /// 检测条码打印查询
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetEx_Task_Print_Main(object OBJ)
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
                //转译
                string task_no = jarr.ContainsKey("task_no") ? jarr["task_no"].ToString() : "";//查询条件 实验任务编号
                string sql = string.Empty;
                sql = $@"SELECT 
                        art_no,
                        order_po_qty,
                        b.name_t as shoe_name,
                        m.shoe_no,
                        MATERIAL_NAME,
                        category_name,
                        MAKINGS_ID,
                        product_level_value,
                        POSITION_NAME,
                        order_po,
                        MANUFACTURER_NAME,
                        season,
                        phase_creation_name,
                        MAKINGS_TYPE_NAME
                        FROM
                        qcm_ex_task_list_m m
                        LEFT JOIN BDM_RD_STYLE b on m.shoe_no=b.shoe_no
                        WHERE task_no='{task_no}'";
                DataTable dt1 = DB.GetDataTable(sql);

                //sql = $@"SELECT
                //     q.task_no,
                //     q.inspection_code,
                //        q.inspection_name,
                //     q.inspection_type,
                //     q.item_seq as seq,
                //     d.sample_qty 
                //    FROM
                //     qcm_ex_task_list_qrcode q 
                //     LEFT JOIN qcm_ex_task_list_d d on q.d_id=d.id
                //        WHERE q.task_no='{task_no}'";

                //Added by Ashok on 20240717
                sql = $@"SELECT
	                    q.task_no,
	                    q.inspection_code,
                        q.inspection_name,
	                    q.inspection_type,
	                    q.item_seq as seq,
	                    d.sample_qty
                      ,e.art_no 
                    FROM
	                    qcm_ex_task_list_qrcode q 
	                    LEFT JOIN qcm_ex_task_list_d d on q.d_id=d.id
                      inner JOIN qcm_ex_task_list_m e on e.task_no=d.task_no
                        WHERE q.task_no='{task_no}'";
                DataTable dt2 = DB.GetDataTable(sql);

                Dictionary<string, object> dic = new Dictionary<string, object>();
                dic.Add("Data1", dt1);
                dic.Add("Data2", dt2);

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

        /// <summary>
        /// 检测条码全部打印
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetEx_Task_Print_MainALL(object OBJ)
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
                //转译
                string task_no = jarr.ContainsKey("task_no") ? jarr["task_no"].ToString() : "";//查询条件 实验任务编号
                string sql = string.Empty;

                //sql = $@"SELECT
	               //     q.task_no,
	               //     q.inspection_code,
                //        q.inspection_name,
	               //     q.inspection_type,
	               //     q.item_seq as seq,
	               //     d.sample_qty 
                //    FROM
	               //     qcm_ex_task_list_qrcode q 
	               //     LEFT JOIN qcm_ex_task_list_d d on q.d_id=d.id
                //        WHERE q.task_no='{task_no}'";

                //Added by Ashok on 20240717
                sql = $@"SELECT
	                    q.task_no,
	                    q.inspection_code,
                        q.inspection_name,
	                    q.inspection_type,
	                    q.item_seq as seq,
	                    d.sample_qty
                      ,e.art_no 
                    FROM
	                    qcm_ex_task_list_qrcode q 
	                    LEFT JOIN qcm_ex_task_list_d d on q.d_id=d.id
                      inner JOIN qcm_ex_task_list_m e on e.task_no=d.task_no
                        WHERE q.task_no='{task_no}'";
                DataTable dt = DB.GetDataTable(sql);

                dt.Columns.Add("qr_code");
                foreach (DataRow item in dt.Rows)
                {
                    item["qr_code"] = item["task_no"] + "@" + item["inspection_type"] + "@" + item["inspection_code"] + "@" + item["seq"];
                }
                dt.Columns.Remove("inspection_type");
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
