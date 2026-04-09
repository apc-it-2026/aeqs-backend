using SJ_QCMAPI.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace SJ_QCMAPI
{
    public class ConfirmShoesBase
    {
        /// <summary>
        /// 确认鞋管理首页视图
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject ConfirmShoesBaseView(object OBJ)
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
                string CONFIRM_PEOPLE = jarr.ContainsKey("CONFIRM_PEOPLE") ? jarr["CONFIRM_PEOPLE"].ToString() : "";
                string putin_date = jarr.ContainsKey("putin_date") ? jarr["putin_date"].ToString() : "";
                string end_date = jarr.ContainsKey("end_date") ? jarr["end_date"].ToString() : ""; 
                string STATUS = jarr.ContainsKey("STATUS") ? jarr["STATUS"].ToString() : ""; 
                string confirm_type = jarr.ContainsKey("confirm_type") ? jarr["confirm_type"].ToString() : ""; //类型

                string pageSize = jarr.ContainsKey("pageSize") ? jarr["pageSize"].ToString() : "";
                string pageIndex = jarr.ContainsKey("pageIndex") ? jarr["pageIndex"].ToString() : "";
                var sql = string.Empty;

                string strwhere = string.Empty;
                if (!string.IsNullOrEmpty(PROD_NO))
                {
                    strwhere += $@" and PROD_NO like '%{PROD_NO}%'";//ART
                }
                if (!string.IsNullOrEmpty(STATUS))
                {
                    strwhere += $@" and STATUS='{STATUS}'";//ART
                }
                if (!string.IsNullOrEmpty(SHOE_NO))
                {
                    strwhere += $@" and SHOE_NO like '%{SHOE_NO}%'";//鞋型
                }
                if (!string.IsNullOrEmpty(CONFIRM_PEOPLE))
                {
                    strwhere += $@" and CONFIRM_PEOPLE like '%{CONFIRM_PEOPLE}%'";//确认人
                }
                if (!string.IsNullOrEmpty(putin_date)&& string.IsNullOrEmpty(end_date))
                {
                    strwhere += $@" and INVALID_DATE='{putin_date}'";
                }
                if (!string.IsNullOrEmpty(end_date)&& string.IsNullOrEmpty(putin_date))
                {
                    strwhere += $@" and INVALID_DATE='{end_date}'";
                }
                if (!string.IsNullOrEmpty(putin_date) && !string.IsNullOrEmpty(end_date))
                {
                    strwhere += $@"AND ( INVALID_DATE  BETWEEN '{putin_date}' AND '{end_date}') ";
                }
                sql = $@"select ID,PROD_NO,SHOE_NO,DEVELOP_SEASON,QTY,RECEIVE_DATE,RECEIVE_PEOPLE,CONFIRM_DATE,RECONFIRM_DATE,INVALID_DATE,CONFIRM_PEOPLE,CONFIRM_RESULT,REDO_REASON,REMARKS,STATUS from  QCM_CONFIRM_SHOES_M  where 1=1 {strwhere} and confirm_type='{confirm_type}' order by id desc";
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
        /// 修改确认鞋型的时候展示
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject ConfirmShoesBaseViewByid(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase();
            try
            {
                DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);
                string Data = ReqObj.Data.ToString();

                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                string ID = jarr.ContainsKey("ID") ? jarr["ID"].ToString() : "";
                string sql = $@"select ID,PROD_NO,SHOE_NO,DEVELOP_SEASON,QTY,RECEIVE_DATE,RECEIVE_PEOPLE,CONFIRM_DATE,RECONFIRM_DATE,INVALID_DATE,CONFIRM_PEOPLE,CONFIRM_RESULT,REDO_REASON,REMARKS,STATUS from  QCM_CONFIRM_SHOES_M  where ID='{ID}'";
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
        /// 新增确认鞋型
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject ConfirmShoesBaseAdd(object OBJ)
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

                string PROD_NO = jarr.ContainsKey("PROD_NO") ? jarr["PROD_NO"].ToString() : "";
                string SHOE_NO = jarr.ContainsKey("SHOE_NO") ? jarr["SHOE_NO"].ToString() : "";
                string DEVELOP_SEASON = jarr.ContainsKey("DEVELOP_SEASON") ? jarr["DEVELOP_SEASON"].ToString() : "";
                string QTY = jarr.ContainsKey("QTY") ? jarr["QTY"].ToString() : "";
                string RECEIVE_DATE = jarr.ContainsKey("RECEIVE_DATE") ? jarr["RECEIVE_DATE"].ToString() : "";
                string RECEIVE_PEOPLE = jarr.ContainsKey("RECEIVE_PEOPLE") ? jarr["RECEIVE_PEOPLE"].ToString() : "";
                string CONFIRM_PEOPLE = jarr.ContainsKey("CONFIRM_PEOPLE") ? jarr["CONFIRM_PEOPLE"].ToString() : "";
                string CONFIRM_RESULT = jarr.ContainsKey("CONFIRM_RESULT") ? jarr["CONFIRM_RESULT"].ToString() : "";
                string REDO_REASON = jarr.ContainsKey("REDO_REASON") ? jarr["REDO_REASON"].ToString() : "";
                string REMARKS = jarr.ContainsKey("REMARKS") ? jarr["REMARKS"].ToString() : "";
                string confirm_type = jarr.ContainsKey("confirm_type") ? jarr["confirm_type"].ToString() : "";//类型0:原材料确认鞋管理；1：成品确认鞋管理；
                //string STATUS = jarr.ContainsKey("STATUS") ? jarr["STATUS"].ToString() : "";
                string CreactUserId = SJeMES_Framework_NETCore.Web.System.GetUserCodeByToken(ReqObj.UserToken);//查询登录人UserID

                string CONFIRM_DATE = jarr.ContainsKey("CONFIRM_DATE") ? jarr["CONFIRM_DATE"].ToString() : "";//确认日期
                string RECONFIRM_DATE = jarr.ContainsKey("RECONFIRM_DATE") ? jarr["RECONFIRM_DATE"].ToString() : "";//再次确认日期
              
                string INVALID_DATE = Convert.ToDateTime(CONFIRM_DATE).AddDays(90).ToString("yyyy-MM-dd");//失效日期=确认日期+90天

                //string GUID = Guid.NewGuid().ToString("N");
                string sql = $@"insert into QCM_CONFIRM_SHOES_M(confirm_type,PROD_NO,SHOE_NO,DEVELOP_SEASON,QTY,RECEIVE_DATE,RECEIVE_PEOPLE,CONFIRM_DATE,INVALID_DATE,CONFIRM_PEOPLE,CONFIRM_RESULT,REDO_REASON,REMARKS,STATUS,CREATEBY,CREATEDATE,CREATETIME) VALUES('{confirm_type}','{PROD_NO}','{SHOE_NO}','{DEVELOP_SEASON}','{QTY}','{RECEIVE_DATE}','{RECEIVE_PEOPLE}','{CONFIRM_DATE}','{INVALID_DATE}','{CONFIRM_PEOPLE}','{CONFIRM_RESULT}','{REDO_REASON}','{REMARKS}','{0}','{CreactUserId}','{DateTime.Now.ToString("yyyy-MM-dd")}','{DateTime.Now.ToString("HH:mm:ss")}')";
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
        /// 确认鞋型修改
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject ConfirmShoesBaseUpdate(object OBJ)
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
                string ID = jarr.ContainsKey("ID") ? jarr["ID"].ToString() : "";

                string PROD_NO = jarr.ContainsKey("PROD_NO") ? jarr["PROD_NO"].ToString() : "";
                string SHOE_NO = jarr.ContainsKey("SHOE_NO") ? jarr["SHOE_NO"].ToString() : "";
                string DEVELOP_SEASON = jarr.ContainsKey("DEVELOP_SEASON") ? jarr["DEVELOP_SEASON"].ToString() : "";
                string QTY = jarr.ContainsKey("QTY") ? jarr["QTY"].ToString() : "";
                string RECEIVE_DATE = jarr.ContainsKey("RECEIVE_DATE") ? jarr["RECEIVE_DATE"].ToString() : "";
                string RECEIVE_PEOPLE = jarr.ContainsKey("RECEIVE_PEOPLE") ? jarr["RECEIVE_PEOPLE"].ToString() : "";
                string CONFIRM_PEOPLE = jarr.ContainsKey("CONFIRM_PEOPLE") ? jarr["CONFIRM_PEOPLE"].ToString() : "";
                string CONFIRM_RESULT = jarr.ContainsKey("CONFIRM_RESULT") ? jarr["CONFIRM_RESULT"].ToString() : "";
                string REDO_REASON = jarr.ContainsKey("REDO_REASON") ? jarr["REDO_REASON"].ToString() : "";
                string REMARKS = jarr.ContainsKey("REMARKS") ? jarr["REMARKS"].ToString() : "";
                string STATUS = jarr.ContainsKey("STATUS") ? jarr["STATUS"].ToString() : "";
                string CreactUserId = SJeMES_Framework_NETCore.Web.System.GetUserCodeByToken(ReqObj.UserToken);//查询登录人UserID

                string CONFIRM_DATE = jarr.ContainsKey("CONFIRM_DATE") ? jarr["CONFIRM_DATE"].ToString() : "";//确认日期
                string RECONFIRM_DATE = jarr.ContainsKey("CONFIRM_DATE") ? jarr["CONFIRM_DATE"].ToString() : "";//再次确认日期
                string INVALID_DATE = Convert.ToDateTime(CONFIRM_DATE).AddDays(90).ToString("yyyy-MM-dd");//失效日期=确认日期+90天
                //判断一下是否过期
                int Num = DateTime.Compare(Convert.ToDateTime(CONFIRM_DATE), Convert.ToDateTime(INVALID_DATE));
                if (Num >0)//等于0就说明两个时间相等
                {
                    STATUS = "2";
                }
                string sql = $@"UPDATE  QCM_CONFIRM_SHOES_M set PROD_NO='{PROD_NO}',SHOE_NO='{SHOE_NO}',DEVELOP_SEASON='{DEVELOP_SEASON}',QTY='{QTY}',RECEIVE_PEOPLE='{RECEIVE_PEOPLE}',CONFIRM_PEOPLE='{CONFIRM_PEOPLE}',CONFIRM_RESULT='{CONFIRM_RESULT}',REDO_REASON='{REDO_REASON}',RECONFIRM_DATE='{RECONFIRM_DATE}',INVALID_DATE='{INVALID_DATE}',REMARKS='{REMARKS}',STATUS='{STATUS}',MODIFYBY='{CreactUserId}',MODIFYDATE='{DateTime.Now.ToString("yyyy-MM-dd")}',MODIFYTIME='{DateTime.Now.ToString("HH:mm:ss")}' where ID='{ID}'";
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
        /// 确认鞋型删除
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject ConfirmShoesBaseDelete(object OBJ)
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
                string ID = jarr.ContainsKey("ID") ? jarr["ID"].ToString() : "";
                string sql1 = $@"delete QCM_CONFIRM_SHOES_M where ID='{ID}'";
                DB.ExecuteNonQuery(sql1);
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
