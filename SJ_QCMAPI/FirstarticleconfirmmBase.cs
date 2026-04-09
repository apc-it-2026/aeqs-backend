using SJ_QCMAPI.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace SJ_QCMAPI
{
    /// <summary>
    /// 首件确认
    /// </summary>
    public class FirstarticleconfirmmBase
    {
        /// <summary>
        /// 首件确认记录表数据展示/EXCEL导出
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject FirstarticleconfirmmView(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase();
            try
            {
                DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);
                string Data = ReqObj.Data.ToString();

                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);

                string PROD_NO = jarr.ContainsKey("PROD_NO") ? jarr["PROD_NO"].ToString() : "";
                string SHOE_NO = jarr.ContainsKey("SHOE_NO") ? jarr["SHOE_NO"].ToString() : "";
                string MODULE_NO = jarr.ContainsKey("MODULE_NO") ? jarr["MODULE_NO"].ToString() : "";
                string excel_no= jarr.ContainsKey("excel_no") ? jarr["excel_no"].ToString() : "";
                string putin_date = jarr.ContainsKey("putin_date") ? jarr["putin_date"].ToString() : "";

                string pageSize = jarr.ContainsKey("pageSize") ? jarr["pageSize"].ToString() : "";
                string pageIndex = jarr.ContainsKey("pageIndex") ? jarr["pageIndex"].ToString() : "";
                string strwhere = string.Empty;
                if (!string.IsNullOrEmpty(PROD_NO))
                {
                    strwhere += $@" and PROD_NO like '%{PROD_NO}%'";
                }
                if (!string.IsNullOrEmpty(SHOE_NO))
                {
                    strwhere += $@" and SHOE_NO like '%{SHOE_NO}%' ";
                }
                if (!string.IsNullOrEmpty(MODULE_NO))
                {
                    strwhere += $@" and MODULE_NO like '%{MODULE_NO}%'";
                }
                if (!string.IsNullOrEmpty(putin_date))//搜索时间
                {
                    strwhere += $@" and CREATEDATE='{putin_date.Trim()}'";
                }
                string sql = string.Empty;
                if (!string.IsNullOrEmpty(excel_no))
                {
                    sql = $@"select*from qcm_firstarticle_confirm_m where  1=1 {strwhere} order by id desc";
                }
                else
                {
                    sql = $@"select INSPECT_NO,PO_ORDER,PROD_NO,SHOE_NO,MODULE_NO,PHYSICAL_NAME,MACHINE,CODE_NUMBER,DEPARTMENT_NO,DEPARTMENT_NAME,PRODUCTIONLINE_NO,PRODUCTIONLINE_NAME,CREATEDATE,STATUS from qcm_firstarticle_confirm_m where  1=1 {strwhere} order by id desc";
                }
                 
                Dictionary<string, object> dic = new Dictionary<string, object>();

                DataTable dt = CommonBASE.GetPageDataTable(DB, sql, int.Parse(pageIndex), int.Parse(pageSize));
                int rowCount = CommonBASE.GetPageDataTableCount(DB, sql);
                dic.Add("Data", dt);
                dic.Add("rowCount", rowCount);

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
        /// 首件确认记录单明细数据展示
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject FirstarticleconfirmmXView(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase();
            try
            {
                DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);
                string Data = ReqObj.Data.ToString();

                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);

                string inspect_no = jarr.ContainsKey("inspect_no") ? jarr["inspect_no"].ToString() : "";
                
              
                string sql = $@"select inspect_no,inspect_seq,defect_no,defect_name from qcm_firstarticle_confirm_d where inspect_no='{inspect_no}'order by id desc";
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
        /// 首件确认记录表数据展示byid(修改时展示也用到)
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject FirstarticleconfirmmViewByid(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase();
            try
            {
                DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);
                string Data = ReqObj.Data.ToString();

                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);

                string INSPECT_NO = jarr.ContainsKey("INSPECT_NO") ? jarr["INSPECT_NO"].ToString() : "";
                string sql = $@"select PO_ORDER,PROD_NO,SHOE_NO,MODULE_NO,MACHINE,PHYSICAL_NAME,CODE_NUMBER,INSPECT_NO from qcm_firstarticle_confirm_m where INSPECT_NO='{INSPECT_NO}' order by id desc";
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
        /// 首件确认单添加数据
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject FirstarticleconfirmmAdd(object OBJ)
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

                string PO_ORDER = jarr.ContainsKey("PO_ORDER") ? jarr["PO_ORDER"].ToString() : "";
                string PROD_NO = jarr.ContainsKey("PROD_NO") ? jarr["PROD_NO"].ToString() : "";
                string SHOE_NO = jarr.ContainsKey("SHOE_NO") ? jarr["SHOE_NO"].ToString() : "";
                string MODULE_NO = jarr.ContainsKey("MODULE_NO") ? jarr["MODULE_NO"].ToString() : "";
                string MACHINE = jarr.ContainsKey("MACHINE") ? jarr["MACHINE"].ToString() : "";
                string PHYSICAL_NAME = jarr.ContainsKey("PHYSICAL_NAME") ? jarr["PHYSICAL_NAME"].ToString() : "";
                string CODE_NUMBER = jarr.ContainsKey("CODE_NUMBER") ? jarr["CODE_NUMBER"].ToString() : "";
                string CreactUserId = SJeMES_Framework_NETCore.Web.System.GetUserCodeByToken(ReqObj.UserToken);//查询登录人UserID
                //检验单号
                string INSPECT_NO =  DateTime.Now.ToString("yyyyMMdd");//20211016  00001
                string sql = $@"select max(INSPECT_NO) from qcm_firstarticle_confirm_m where INSPECT_NO like '{INSPECT_NO}%'";
                string max_inspection_no = DB.GetString(sql);
                //查询检验单号有没有相同的
                if (!string.IsNullOrEmpty(max_inspection_no))
                {
                    string seq = max_inspection_no.Replace(INSPECT_NO, "");//00002

                    int int_seq = Convert.ToInt32(seq) + 1;//3   00111

                    INSPECT_NO += int_seq.ToString().PadLeft(5, '0');
                    //throw new Exception("送检单号：【" + inspection_no + "】重复，请检查!");
                }
                else
                {
                    INSPECT_NO += "00001";
                }

                string sql2 = $@"insert into qcm_firstarticle_confirm_m(INSPECT_NO,PO_ORDER,PROD_NO,SHOE_NO,MODULE_NO,PHYSICAL_NAME,MACHINE,CODE_NUMBER,CREATEBY,CREATEDATE,CREATETIME) VALUES('{INSPECT_NO}','{PO_ORDER}','{PROD_NO}','{SHOE_NO}','{MODULE_NO}','{PHYSICAL_NAME}','{MACHINE}','{CODE_NUMBER}','{CreactUserId}','{DateTime.Now.ToString("yyyy-MM-dd")}','{DateTime.Now.ToString("HH:mm:ss")}')"; 
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
        /// 首件确认单修改数据
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject FirstarticleconfirmmUpdate(object OBJ)
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
                string INSPECT_NO = jarr.ContainsKey("INSPECT_NO") ? jarr["INSPECT_NO"].ToString() : "";

                string PO_ORDER = jarr.ContainsKey("PO_ORDER") ? jarr["PO_ORDER"].ToString() : "";
                string PROD_NO = jarr.ContainsKey("PROD_NO") ? jarr["PROD_NO"].ToString() : "";
                string SHOE_NO = jarr.ContainsKey("SHOE_NO") ? jarr["SHOE_NO"].ToString() : "";
                string MODULE_NO = jarr.ContainsKey("MODULE_NO") ? jarr["MODULE_NO"].ToString() : "";
                string MACHINE = jarr.ContainsKey("MACHINE") ? jarr["MACHINE"].ToString() : "";
                string PHYSICAL_NAME = jarr.ContainsKey("PHYSICAL_NAME") ? jarr["PHYSICAL_NAME"].ToString() : "";
                string CODE_NUMBER = jarr.ContainsKey("CODE_NUMBER") ? jarr["CODE_NUMBER"].ToString() : "";
               

                string CreactUserId = SJeMES_Framework_NETCore.Web.System.GetUserCodeByToken(ReqObj.UserToken);//查询登录人UserID
              
                string sql = $@"UPDATE  qcm_firstarticle_confirm_m set PO_ORDER='{PO_ORDER}',PROD_NO='{PROD_NO}',SHOE_NO='{SHOE_NO}',MODULE_NO='{MODULE_NO}',MACHINE='{MACHINE}',PHYSICAL_NAME='{PHYSICAL_NAME}',CODE_NUMBER='{CODE_NUMBER}',MODIFYBY='{CreactUserId}',MODIFYDATE='{DateTime.Now.ToString("yyyy-MM-dd")}',MODIFYTIME='{DateTime.Now.ToString("HH:mm:ss")}' where INSPECT_NO='{INSPECT_NO}'";
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
        /// 首件确认单删除数据
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject FirstarticleconfirmmDelete(object OBJ)
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
                string INSPECT_NO = jarr.ContainsKey("INSPECT_NO") ? jarr["INSPECT_NO"].ToString() : "";

                string sql = $@"select*from qcm_firstarticle_confirm_d where inspect_no='{INSPECT_NO}'";
                DataTable dt = DB.GetDataTable(sql);
                if (dt.Rows.Count > 0)
                {
                    throw new Exception("首件已确认生成明细，删除失败");
                }
                string sql2 = $@"delete qcm_firstarticle_confirm_m where INSPECT_NO='{INSPECT_NO}'";
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
    }
}
