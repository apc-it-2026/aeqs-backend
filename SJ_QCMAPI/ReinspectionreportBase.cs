using SJ_QCMAPI.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace SJ_QCMAPI
{
    /// <summary>
    /// 重检报告
    /// </summary>
    public class ReinspectionreportBase
    {
        /// <summary>
        /// 重检报告首页视图数据展示
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject ReinspectionreportView(object OBJ)
        {

            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase();
            try
            {
                DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);
                string Data = ReqObj.Data.ToString();
                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                string OUTSOURCING_INSPECTION_NO = jarr.ContainsKey("OUTSOURCING_INSPECTION_NO") ? jarr["OUTSOURCING_INSPECTION_NO"].ToString() : "";
                string PO_ORDER = jarr.ContainsKey("PO_ORDER") ? jarr["PO_ORDER"].ToString() : "";
                string PROD_NO = jarr.ContainsKey("PROD_NO") ? jarr["PROD_NO"].ToString() : "";
                string excel_no = jarr.ContainsKey("excel_no") ? jarr["excel_no"].ToString() : "";


                string pageSize = jarr.ContainsKey("pageSize") ? jarr["pageSize"].ToString() : "";
                string pageIndex = jarr.ContainsKey("pageIndex") ? jarr["pageIndex"].ToString() : "";
                string wheres = string.Empty;
                if (!string.IsNullOrEmpty(OUTSOURCING_INSPECTION_NO))
                {
                    wheres += $" and OUTSOURCING_INSPECTION_NO like '%{OUTSOURCING_INSPECTION_NO}%'";
                }
                if (!string.IsNullOrEmpty(PO_ORDER))
                {
                    wheres += $" and PO_ORDER like'%{PO_ORDER}%'";
                }
                if (!string.IsNullOrEmpty(PROD_NO))
                {
                    wheres += $" and PROD_NO like'%{PROD_NO}%'";
                }
                string sql = string.Empty;
                if (!string.IsNullOrEmpty(excel_no))
                {
                    sql = $@"select*from QCM_REINSPECTION_REPORT_M where 1=1 {wheres} order by id desc";
                }
                else
                {
                    sql = $@"select ID,GUID,OUTSOURCING_INSPECTION_NO,SUPPLIERS_CODE,SUPPLIERS_NAME,SUPPLIERS_TYPE,PO_ORDER,PROD_NO,WH_QTY,SPOT_CHECK_QTY,BAD_QTY,BAD_RATE,NOT_ACCEPT_QTY,SHOE_NO,ACCEPT_QTY,GENERAL_TESTTYPE_NO,CATEGORY_NO from QCM_REINSPECTION_REPORT_M where 1=1 {wheres} order by id desc";
                }
                Dictionary<string, object> dic = new Dictionary<string, object>();

                DataTable dt = CommonBASE.GetPageDataTable(DB, sql, int.Parse(pageIndex), int.Parse(pageSize));
                int rowCount = CommonBASE.GetPageDataTableCount(DB, sql);
                dic.Add("Data", dt);
                dic.Add("rowCount", rowCount);
                ret.RetData = Newtonsoft.Json.JsonConvert.SerializeObject(dic);
            }
            catch (Exception ex)
            {
                //DB.Rollback();
                ret.IsSuccess = false;
                ret.ErrMsg = ex.Message;
            }
            return ret;
        }
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject ReinspectionreportXView(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase();
            try
            {
                DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);
                string Data = ReqObj.Data.ToString();

                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);

                //string OUTSOURCING_INSPECTION_NO = jarr.ContainsKey("OUTSOURCING_INSPECTION_NO") ? jarr["OUTSOURCING_INSPECTION_NO"].ToString() : "";
                string GUID = jarr.ContainsKey("GUID") ? jarr["GUID"].ToString() : "";
                string ID = jarr.ContainsKey("ID") ? jarr["ID"].ToString() : "";


                //string sql = $@"select*from QCM_REINSPECTION_REPORT_D where OUTSOURCING_INSPECTION_NO='{OUTSOURCING_INSPECTION_NO}'";
                string sql = $@"select TESTITEM_CATEGORY,TESTITEM_CODE,TESTITEM_NAME,TESTTYPE_NO,TESTTYPE_NAME,SAMPLE_NUM,AQL_LEVEL,PROBLEM_POINT,INS_RES,GUID_IMG,REMARK from QCM_REINSPECTION_REPORT_D where GUID='{GUID}'";
                Dictionary<string, object> dic = new Dictionary<string, object>();
                DataTable dt = DB.GetDataTable(sql);

                dic.Add("Data", dt);

                ret.RetData = Newtonsoft.Json.JsonConvert.SerializeObject(dic);
                ret.IsSuccess = true;
            }
            catch (Exception ex)
            {
                //DB.Rollback();
                ret.IsSuccess = false;
                ret.ErrMsg = ex.Message;
            }
            return ret;
        }
        /// <summary>
        /// 重检报告工艺检测项详情表数据展示byid(修改时展示也用到)
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject ReinspectionreportViewByid(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase();
            try
            {
                DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);
                string Data = ReqObj.Data.ToString();

                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);

                //string OUTSOURCING_INSPECTION_NO = jarr.ContainsKey("OUTSOURCING_INSPECTION_NO") ? jarr["OUTSOURCING_INSPECTION_NO"].ToString() : "";
                string ID = jarr.ContainsKey("ID") ? jarr["ID"].ToString() : "";
                string GUID = jarr.ContainsKey("GUID") ? jarr["GUID"].ToString() : "";
                string OUTSOURCING_INSPECTION_NO = jarr.ContainsKey("OUTSOURCING_INSPECTION_NO") ? jarr["OUTSOURCING_INSPECTION_NO"].ToString() : "";
                //string sql = $@"select*from QCM_REINSPECTION_REPORT_M where GUID='{GUID}' and ID='{ID}'";
                string sql = $@"select OUTSOURCING_INSPECTION_NO,SUPPLIERS_TYPE,PO_ORDER,PROD_NO,WH_QTY,SPOT_CHECK_QTY,SHOE_NO,SUPPLIERS_CODE from QCM_REINSPECTION_REPORT_M where OUTSOURCING_INSPECTION_NO='{OUTSOURCING_INSPECTION_NO}'";
                DataTable dt = DB.GetDataTable(sql);
                Dictionary<string, object> dic = new Dictionary<string, object>();
                dic.Add("Data", dt);

                ret.RetData = Newtonsoft.Json.JsonConvert.SerializeObject(dic);
                ret.IsSuccess = true;
            }
            catch (Exception ex)
            {
                //DB.Rollback();
                ret.IsSuccess = false;
                ret.ErrMsg = ex.Message;
            }
            return ret;
        }
        /// <summary>
        /// 重检添加数据
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject ReinspectionreportAdd(object OBJ)
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

                //string OUTSOURCING_INSPECTION_NO = jarr.ContainsKey("OUTSOURCING_INSPECTION_NO") ? jarr["OUTSOURCING_INSPECTION_NO"].ToString() : "";
                string SUPPLIERS_TYPE = jarr.ContainsKey("SUPPLIERS_TYPE") ? jarr["SUPPLIERS_TYPE"].ToString() : "";
                string SUPPLIERS_CODE = jarr.ContainsKey("SUPPLIERS_CODE") ? jarr["SUPPLIERS_CODE"].ToString() : "";
                string SUPPLIERS_NAME = jarr.ContainsKey("SUPPLIERS_NAME") ? jarr["SUPPLIERS_NAME"].ToString() : "";
                string PO_ORDER = jarr.ContainsKey("PO_ORDER") ? jarr["PO_ORDER"].ToString() : "";
                string PROD_NO = jarr.ContainsKey("PROD_NO") ? jarr["PROD_NO"].ToString() : "";
                string WH_QTY = jarr.ContainsKey("WH_QTY") ? jarr["WH_QTY"].ToString() : "";
                string SPOT_CHECK_QTY = jarr.ContainsKey("SPOT_CHECK_QTY") ? jarr["SPOT_CHECK_QTY"].ToString() : "";
                string SHOE_NO = jarr.ContainsKey("SHOE_NO") ? jarr["SHOE_NO"].ToString() : "";
             
               //string CATEGORY_NO = jarr.ContainsKey("CATEGORY_NO") ? jarr["CATEGORY_NO"].ToString() : "";
               
                string CreactUserId = SJeMES_Framework_NETCore.Web.System.GetUserCodeByToken(ReqObj.UserToken);//查询登录人UserID
                //检验单号
                string OUTSOURCING_INSPECTION_NO = DateTime.Now.ToString("yyyyMMdd");//20211016  00001
                string sql = $@"select max(OUTSOURCING_INSPECTION_NO) from QCM_REINSPECTION_REPORT_M where OUTSOURCING_INSPECTION_NO like '{OUTSOURCING_INSPECTION_NO}%'";
                string max_OUTSOURCING_INSPECTION_NO = DB.GetString(sql);
                //查询检验单号有没有相同的
                if (!string.IsNullOrEmpty(max_OUTSOURCING_INSPECTION_NO))
                {
                    string seq = max_OUTSOURCING_INSPECTION_NO.Replace(OUTSOURCING_INSPECTION_NO, "");//00002

                    int int_seq = Convert.ToInt32(seq) + 1;//3   00111

                    OUTSOURCING_INSPECTION_NO += int_seq.ToString().PadLeft(5, '0');
                    //throw new Exception("送检单号：【" + inspection_no + "】重复，请检查!");
                }
                else
                {
                    OUTSOURCING_INSPECTION_NO += "00001";
                }
                string GUID = Guid.NewGuid().ToString("N");
                string sql2 = $@"insert into QCM_REINSPECTION_REPORT_M(GUID,OUTSOURCING_INSPECTION_NO,SUPPLIERS_TYPE,SHOE_NO,SUPPLIERS_CODE,SUPPLIERS_NAME,PO_ORDER,PROD_NO,WH_QTY,SPOT_CHECK_QTY,CREATEBY,CREATEDATE,CREATETIME) VALUES('{GUID}','{OUTSOURCING_INSPECTION_NO}','{SUPPLIERS_TYPE}','{SHOE_NO}','{SUPPLIERS_CODE}','{SUPPLIERS_NAME}','{PO_ORDER}','{PROD_NO}','{WH_QTY}','{SPOT_CHECK_QTY}','{CreactUserId}','{DateTime.Now.ToString("yyyy-MM-dd")}','{DateTime.Now.ToString("HH:mm:ss")}')";
                DB.ExecuteNonQuery(sql2);
                DB.Commit();
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
        /// 重检修改数据
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject ReinspectionreportUpdate(object OBJ)
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

                string OUTSOURCING_INSPECTION_NO = jarr.ContainsKey("OUTSOURCING_INSPECTION_NO") ? jarr["OUTSOURCING_INSPECTION_NO"].ToString() : "";
                string GUID = jarr.ContainsKey("GUID") ? jarr["GUID"].ToString() : "";
                string ID = jarr.ContainsKey("ID") ? jarr["ID"].ToString() : "";

                string SUPPLIERS_TYPE = jarr.ContainsKey("SUPPLIERS_TYPE") ? jarr["SUPPLIERS_TYPE"].ToString() : "";
                string SUPPLIERS_NAME = jarr.ContainsKey("SUPPLIERS_NAME") ? jarr["SUPPLIERS_NAME"].ToString() : "";
                string SUPPLIERS_CODE = jarr.ContainsKey("SUPPLIERS_CODE") ? jarr["SUPPLIERS_CODE"].ToString() : "";

                string PO_ORDER = jarr.ContainsKey("PO_ORDER") ? jarr["PO_ORDER"].ToString() : "";
                string PROD_NO = jarr.ContainsKey("PROD_NO") ? jarr["PROD_NO"].ToString() : "";
                string WH_QTY = jarr.ContainsKey("WH_QTY") ? jarr["WH_QTY"].ToString() : "";
                string SPOT_CHECK_QTY = jarr.ContainsKey("SPOT_CHECK_QTY") ? jarr["SPOT_CHECK_QTY"].ToString() : "";

                string SHOE_NO = jarr.ContainsKey("SHOE_NO") ? jarr["SHOE_NO"].ToString() : "";
                
                //string CATEGORY_NO = jarr.ContainsKey("CATEGORY_NO") ? jarr["CATEGORY_NO"].ToString() : "";

                string CreactUserId = SJeMES_Framework_NETCore.Web.System.GetUserCodeByToken(ReqObj.UserToken);//查询登录人UserID
                string sql = $@"UPDATE  QCM_REINSPECTION_REPORT_M set SUPPLIERS_TYPE='{SUPPLIERS_TYPE}',SUPPLIERS_CODE='{SUPPLIERS_CODE}',SHOE_NO='{SHOE_NO}',SUPPLIERS_NAME='{SUPPLIERS_NAME}',PO_ORDER='{PO_ORDER}',PROD_NO='{PROD_NO}',WH_QTY='{WH_QTY}',SPOT_CHECK_QTY='{SPOT_CHECK_QTY}',MODIFYBY='{CreactUserId}',MODIFYDATE='{DateTime.Now.ToString("yyyy-MM-dd")}',MODIFYTIME='{DateTime.Now.ToString("HH:mm:ss")}' where OUTSOURCING_INSPECTION_NO='{OUTSOURCING_INSPECTION_NO}'";
                DB.ExecuteNonQuery(sql);
                DB.Commit();
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
        /// 重检删除数据
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject ReinspectionreportDelete(object OBJ)
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
                string OUTSOURCING_INSPECTION_NO = jarr.ContainsKey("OUTSOURCING_INSPECTION_NO") ? jarr["OUTSOURCING_INSPECTION_NO"].ToString() : "";
                string ID = jarr.ContainsKey("ID") ? jarr["ID"].ToString() : "";
                string GUID = jarr.ContainsKey("GUID") ? jarr["GUID"].ToString() : "";

                //string sql = $@"select*from QCM_REINSPECTION_REPORT_D where OUTSOURCING_INSPECTION_NO='{OUTSOURCING_INSPECTION_NO}'";
                string sql = $@"select*from QCM_REINSPECTION_REPORT_D where OUTSOURCING_INSPECTION_NO='{OUTSOURCING_INSPECTION_NO}'";
                DataTable dt = DB.GetDataTable(sql);
                if (dt.Rows.Count > 0)
                {
                    throw new Exception("首件已确认生成明细，删除失败");
                }
                string sql2 = $@"delete QCM_REINSPECTION_REPORT_M where OUTSOURCING_INSPECTION_NO='{OUTSOURCING_INSPECTION_NO}'";
                //string sql = $@"delete QCM_REINSPECTION_REPORT_M where OUTSOURCING_INSPECTION_NO='{OUTSOURCING_INSPECTION_NO}'";

                DB.ExecuteNonQuery(sql2);
                DB.Commit();
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
        /// 厂商类型一级下拉框
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject ReinspectionreportONEXLK(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase();
            try
            {
                DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);
                string Data = ReqObj.Data.ToString();
                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                Dictionary<string, object> dic = new Dictionary<string, object>();
                string sql = $@"select vendor_type_no,vendor_type_name from bdm_vendor_type_m";
                DataTable dt = DB.GetDataTable(sql);
                dic.Add("data", dt);
                ret.IsSuccess = true;
                ret.RetData = Newtonsoft.Json.JsonConvert.SerializeObject(dic);
            }
            catch (Exception ex)
            {
                ret.IsSuccess = false;
                ret.ErrMsg = ex.Message;
            }

            return ret;
        }
        /// <summary>
        /// 供应商信息表 二级级下拉框
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject ReinspectionreportTWOXLK(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase();
            try
            {
                DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);
                string Data = ReqObj.Data.ToString();
                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                string VENDOR_TYPE_NO = jarr.ContainsKey("VENDOR_TYPE_NO") ? jarr["VENDOR_TYPE_NO"].ToString() : "";
                Dictionary<string, object> dic = new Dictionary<string, object>();
                string sql = $@"select SUPPLIERS_CODE,SUPPLIERS_NAME from base003m where VENDOR_TYPE_NO='{VENDOR_TYPE_NO}'";
                DataTable dt = DB.GetDataTable(sql);
                dic.Add("data", dt);
                ret.IsSuccess = true;
                ret.RetData = Newtonsoft.Json.JsonConvert.SerializeObject(dic);
            }
            catch (Exception ex)
            {
                ret.IsSuccess = false;
                ret.ErrMsg = ex.Message;
            }

            return ret;
        }
        /// <summary>
        /// 重检报告图片资源表
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject ReinspectionreporttListIMG(object OBJ)
        {

            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase();
            try
            {
                DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);
                string Data = ReqObj.Data.ToString();
                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);

                string GUID_IMG = jarr.ContainsKey("GUID_IMG") ? jarr["GUID_IMG"].ToString() : "";//关联键
                string sql = $@"select IMG_NAME,IMG_URL from QCM_REINSPECTION_REPORT_IMAGEURL where GUID_IMG='{GUID_IMG}'";
                DataTable dt = DB.GetDataTable(sql);
                Dictionary<string, object> dic = new Dictionary<string, object>();
                dic.Add("Data", dt);
                ret.IsSuccess = true;
                ret.RetData = Newtonsoft.Json.JsonConvert.SerializeObject(dic);
            }
            catch (Exception ex)
            {
                ret.IsSuccess = false;
                ret.ErrMsg = ex.Message;
            }
            return ret;
        }
    }
}
