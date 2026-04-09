using SJ_QCMAPI.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace SJ_QCMAPI
{
    public class InstrumentBase
    {
        /// <summary>
        /// 检验工具维护保养记录
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject InstrumentBaseView(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase();
            try
            {
                DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);
                string Data = ReqObj.Data.ToString();

                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                string INSPECT_ORDER = jarr.ContainsKey("INSPECT_ORDER") ? jarr["INSPECT_ORDER"].ToString() : "";
                string putin_date = jarr.ContainsKey("putin_date") ? jarr["putin_date"].ToString() : "";
                string end_date = jarr.ContainsKey("end_date") ? jarr["end_date"].ToString() : "";
            
                string pageSize = jarr.ContainsKey("pageSize") ? jarr["pageSize"].ToString() : "";
                string pageIndex = jarr.ContainsKey("pageIndex") ? jarr["pageIndex"].ToString() : "";
                var sql = string.Empty;

                string strwhere = string.Empty;
                if (!string.IsNullOrEmpty(INSPECT_ORDER))
                {
                    strwhere += $@" and INSPECT_TOOL_NAME like '%{INSPECT_ORDER}%'";//搜索工具的名称
                }
                if(!string.IsNullOrEmpty(putin_date) && !string.IsNullOrEmpty(end_date))
                {
                    strwhere += $@"AND ( INSPECT_DATE  BETWEEN '{putin_date}' AND '{end_date}') ";
                }
                sql = $@"select INSPECT_TOOL_CODE,INSPECT_TOOL_NAME,INSPECT_DATE,INSPECT_RESULT,INSPECT_ORDER from  QCM_INSPECT_TOOL_ORDER_M  where 1=1 {strwhere} order by id desc";
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
        /// 检验工具保养记录平板端口数据展示
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject InstrumentBaseViewS(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase();
            try
            {
                DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);
                string Data = ReqObj.Data.ToString();

                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                string INSPECT_TOOL_CODE = jarr.ContainsKey("INSPECT_TOOL_CODE") ? jarr["INSPECT_TOOL_CODE"].ToString() : "";
              
            
                string sql = $@"select INSPECT_TOOL_CODE,INSPECT_TOOL_NAME,REMARK from  BDM_INSPECT_TOOL_M where  INSPECT_TOOL_CODE='{INSPECT_TOOL_CODE}'";
                string sql2 = $@"select INSPECT_ITEM_CODE,INSPECT_ITEM_NAME,REMARK from BDM_INSPECT_ITEM_M";
                Dictionary<string, object> dic = new Dictionary<string, object>();
                DataTable dt = DB.GetDataTable(sql);
                DataTable dt1 = DB.GetDataTable(sql2);
                if (dt.Rows.Count == 0)
                {
                    throw new Exception("该工具条码不存在");
                }
                else if (dt1.Rows.Count == 0)
                {
                    ret.ErrMsg = "该工具条码没有相应的检验项";
                }
                dic.Add("Data", dt);
                dic.Add("Data1", dt1);
                ret.RetData1 = dic;
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
        /// 检验工具保养记录平板端口添加数据
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject InstrumentBaseViewAdd(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);
            try
            {
                string Data = ReqObj.Data.ToString();
                var requst = Newtonsoft.Json.JsonConvert.DeserializeObject<UpdateProjectListReqs>(Data);
                #region 逻辑
                DB.Open();
                DB.BeginTransaction();
                DateTime currDate = DateTime.Now;
                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                string userCode = SJeMES_Framework_NETCore.Web.System.GetUserCodeByToken(ReqObj.UserToken);

                if (requst.project_datas.Count > 0)
                {
                    string INSPECT_ORDER = DateTime.Now.ToString("yyyyMMdd");//20211016  00001
                    string sqla = $@"select max(INSPECT_ORDER) from QCM_INSPECT_TOOL_ORDER_M where INSPECT_ORDER like '{INSPECT_ORDER}%'";
                    string max_INSPECT_ORDER = DB.GetString(sqla);
                    //查询检验单号有没有相同的
                    if (!string.IsNullOrEmpty(max_INSPECT_ORDER))
                    {
                        string seq = max_INSPECT_ORDER.Replace(INSPECT_ORDER, "");//00002

                        int int_seq = Convert.ToInt32(seq) + 1;//3   00111

                        INSPECT_ORDER += int_seq.ToString().PadLeft(5, '0');
                        //throw new Exception("检验单号：【" + inspection_no + "】重复，请检查!");
                    }
                    else
                    {
                        INSPECT_ORDER += "00001";
                    }
                    foreach (var item in requst.project_datas)
                    {
                        Dictionary<string, object> insert_dic = new Dictionary<string, object>();
                        insert_dic.Add("INSPECT_ORDER", INSPECT_ORDER);
                        insert_dic.Add("INSPECT_TOOL_CODE", item.INSPECT_TOOL_CODE);
                        insert_dic.Add("INSPECT_TOOL_NAME", item.INSPECT_TOOL_NAME);
                        insert_dic.Add("INSPECT_DATE", DateTime.Now.ToString("yyyy-MM-dd"));
                        insert_dic.Add("INSPECT_RESULT", item.INSPECT_RESULT);

                        insert_dic.Add("CREATEBY", userCode);
                        insert_dic.Add("CREATEDATE", currDate.ToString("yyyy-MM-dd"));
                        insert_dic.Add("CREATETIME", currDate.ToString("HH:mm:ss"));

                        string sql = SJeMES_Framework_NETCore.Common.StringHelper.GetInsertSqlByDictionary("oracle", "QCM_INSPECT_TOOL_ORDER_M", insert_dic);
                        DB.ExecuteNonQuery(sql, insert_dic);
                    }
                }
                if (requst.project_datas2.Count > 0)
                {
                    string INSPECT_ORDER = DateTime.Now.ToString("yyyyMMdd");//20211016  00001
                    string sqlb = $@"select max(INSPECT_ORDER) from QCM_INSPECT_TOOL_ORDER_D where INSPECT_ORDER like '{INSPECT_ORDER}%'";
                    string max_INSPECT_ORDER = DB.GetString(sqlb);
                    //查询检验单号有没有相同的
                    if (!string.IsNullOrEmpty(max_INSPECT_ORDER))
                    {
                        string seq = max_INSPECT_ORDER.Replace(INSPECT_ORDER, "");//00002

                        int int_seq = Convert.ToInt32(seq) + 1;//3   00111

                        INSPECT_ORDER += int_seq.ToString().PadLeft(5, '0');
                        //throw new Exception("检验单号：【" + inspection_no + "】重复，请检查!");
                    }
                    else
                    {
                        INSPECT_ORDER += "00001";
                    }
                    foreach (var item in requst.project_datas2)
                    {
                        Dictionary<string, object> insert_dic = new Dictionary<string, object>();
                        insert_dic.Add("INSPECT_ORDER", INSPECT_ORDER);
                        insert_dic.Add("INSPECT_ITEM_CODE", item.INSPECT_ITEM_CODE);
                        insert_dic.Add("INSPECT_ITEM_NAME", item.INSPECT_ITEM_NAME);
                        insert_dic.Add("INSPECT_RESULT", item.INSPECT_RESULT);

                        insert_dic.Add("CREATEBY", userCode);
                        insert_dic.Add("CREATEDATE", currDate.ToString("yyyy-MM-dd"));
                        insert_dic.Add("CREATETIME", currDate.ToString("HH:mm:ss"));

                        string sql = SJeMES_Framework_NETCore.Common.StringHelper.GetInsertSqlByDictionary("oracle", "QCM_INSPECT_TOOL_ORDER_D", insert_dic);
                        DB.ExecuteNonQuery(sql, insert_dic);
                    }
                }
                DB.Commit();
                #endregion
                ret.ErrMsg = "保存成功！";
                ret.IsSuccess = true;
            }
            catch (Exception ex)
            {
                DB.Rollback();
                ret.IsSuccess = false;
                ret.ErrMsg = "保存失败，原因：" + ex.Message;
            }
            finally
            {
                DB.Close();
            }

            return ret;
        }
        /// <summary>
        /// 检验工具维护保养记录
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject InstrumentBaseViewList(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase();
            try
            {
                DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);
                string Data = ReqObj.Data.ToString();

                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                string INSPECT_ORDER = jarr.ContainsKey("INSPECT_ORDER") ? jarr["INSPECT_ORDER"].ToString() : "";

                string pageSize = jarr.ContainsKey("pageSize") ? jarr["pageSize"].ToString() : "";
                string pageIndex = jarr.ContainsKey("pageIndex") ? jarr["pageIndex"].ToString() : "";
                var sql = string.Empty;
                string strwhere = string.Empty;
                sql = $@"select INSPECT_ORDER,INSPECT_ITEM_CODE,INSPECT_ITEM_NAME,INSPECT_RESULT from  QCM_INSPECT_TOOL_ORDER_D  where INSPECT_ORDER='{INSPECT_ORDER}' order by id desc";
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
        /// 检验工具检验单主表删除
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject InstrumentBaseDelete(object OBJ)
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

                string INSPECT_ORDER = jarr.ContainsKey("INSPECT_ORDER") ? jarr["INSPECT_ORDER"].ToString() : "";//检验单号

                string sql = $@"delete QCM_INSPECT_TOOL_ORDER_M where INSPECT_ORDER='{INSPECT_ORDER}'";
                string sql2 = $@"delete  QCM_INSPECT_TOOL_ORDER_D where  INSPECT_ORDER='{INSPECT_ORDER}'";
                DB.ExecuteNonQuery(sql);
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
    public class UpdateProjectListReqs
    {
        /// <summary>
        /// 项目集合
        /// </summary>
        public List<QCM_INSPECT_TOOL_ORDER_M_Dto> project_datas { get; set; }
        public List<QCM_INSPECT_TOOL_ORDER_Dto> project_datas2{ get; set; }
    }
    public class QCM_INSPECT_TOOL_ORDER_M_Dto
    {
     
        /// <summary>
        /// 检验工具CODE
        /// </summary>
        public string INSPECT_TOOL_CODE { get; set; }
        /// <summary>
        /// 检验工具NAME
        /// </summary>
        public string INSPECT_TOOL_NAME { get; set; }
     
        /// <summary>
        /// 检验结果
        /// </summary>
        public string INSPECT_RESULT { get; set; }
    }
    public class QCM_INSPECT_TOOL_ORDER_Dto
    {
      
        /// <summary>
        /// 检验项CODE
        /// </summary>
        public string INSPECT_ITEM_CODE { get; set; }
        /// <summary>
        /// 检验项NAME
        /// </summary>
        public string INSPECT_ITEM_NAME { get; set; }
        /// <summary>
        /// 检验结果
        /// </summary>
        public string INSPECT_RESULT { get; set; }
    }

}
