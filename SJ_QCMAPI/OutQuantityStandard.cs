using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace SJ_QCMAPI
{
    /// <summary>
    /// 发外厂商品质体系审核标准表
    /// </summary>
    public class OutQuantityStandard
    {
        /// <summary>
        /// 正整数
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        static bool IsNumeric(string str)
        {
            System.Text.RegularExpressions.Regex reg = new System.Text.RegularExpressions.Regex(@"^[0-9]\d*$");
            return reg.IsMatch(str);
        }
        /***********************************************网页start************************************************************/
        /// <summary>
        /// 新增/更新 发外厂商品质 项目列表
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject UpdateProjectList(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();

            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);

            try
            {
                string Data = ReqObj.Data.ToString();
                var requst = Newtonsoft.Json.JsonConvert.DeserializeObject<UpdateProjectListReq>(Data);
                #region 逻辑
                DB.Open();
                DB.BeginTransaction();
                DateTime currDate = DateTime.Now;
                string userCode = SJeMES_Framework_NETCore.Web.System.GetUserCodeByToken(ReqObj.UserToken);
                if (requst.project_datas.Count > 0)
                {
                    var existIdsList = requst.project_datas.Where(x => x.ID.HasValue).Select(x => x.ID);
                    if (existIdsList.Count() > 0)
                    {
                        string existIds = string.Join(',', existIdsList);
                        //先删除请求里不存在的ID数据
                        string delSql = $@"DELETE FROM BDM_OUT_QUALITY_LIST_M WHERE ID NOT IN({existIds})";
                        DB.ExecuteNonQuery(delSql);
                    }
                    else
                    {
                        string delSql = $@"DELETE FROM BDM_OUT_QUALITY_LIST_M";
                        DB.ExecuteNonQuery(delSql);
                    }
                    foreach (var item in requst.project_datas)
                    {
               
                        
                        if (!item.ID.HasValue)
                        {//新增
                            Dictionary<string, object> insert_dic = new Dictionary<string, object>();

                            insert_dic.Add("PROJECT", item.PROJECT);
                            insert_dic.Add("SCORE", item.SCORE);
                            insert_dic.Add("REAL_SCORE", item.REAL_SCORE);
                            insert_dic.Add("PROBLEM_POINT", item.PROBLEM_POINT);
                            insert_dic.Add("REMARK", item.REMARK);

                            insert_dic.Add("CREATEBY", userCode);
                            insert_dic.Add("CREATEDATE", currDate.ToString("yyyy-MM-dd"));
                            insert_dic.Add("CREATETIME", currDate.ToString("HH:mm:ss"));

                            string sql = SJeMES_Framework_NETCore.Common.StringHelper.GetInsertSqlByDictionary("oracle", "BDM_OUT_QUALITY_LIST_M", insert_dic);
                            DB.ExecuteNonQuery(sql, insert_dic);
                        }
                        else
                        {//修改
                            Dictionary<string, object> update_val_dic = new Dictionary<string, object>();
                            update_val_dic.Add("PROJECT", item.PROJECT);
                            update_val_dic.Add("SCORE", item.SCORE);
                            update_val_dic.Add("REAL_SCORE", item.REAL_SCORE);
                            update_val_dic.Add("PROBLEM_POINT", item.PROBLEM_POINT);
                            update_val_dic.Add("REMARK", item.REMARK);

                            update_val_dic.Add("MODIFYBY", userCode);
                            update_val_dic.Add("MODIFYDATE", currDate.ToString("yyyy-MM-dd"));
                            update_val_dic.Add("MODIFYTIME", currDate.ToString("HH:mm:ss"));

                            string whereSql = $@"ID=@ID";
                            string sql = SJeMES_Framework_NETCore.Common.StringHelper.GetUpdateSqlByDictionary("BDM_OUT_QUALITY_LIST_M", whereSql, update_val_dic);
                            update_val_dic.Add("ID", item.ID);

                            DB.ExecuteNonQuery(sql, update_val_dic);
                        }
                    }
                }
                else
                {
                    //项目为空时，全部删除
                    string delSql = $@"DELETE FROM BDM_OUT_QUALITY_LIST_M";
                    DB.ExecuteNonQuery(delSql);
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
        /// 获取所有 发外厂商品质 项目列表
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetAllProjectList(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();

            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);

            try
            {
                string Data = ReqObj.Data.ToString();
                var requst = Newtonsoft.Json.JsonConvert.DeserializeObject<UpdateProjectListReq>(Data);

                #region 逻辑
                string sql = $@"
                                SELECT
	                                ID,
	                                PROJECT,
	                                SCORE,
	                                REAL_SCORE,
	                                PROBLEM_POINT,
	                                REMARK
                                FROM
	                                BDM_OUT_QUALITY_LIST_M
                                ";
                DataTable dt = DB.GetDataTable(sql+ "order by ID desc");
                var res = dt.ToDataList<GetAllProjectListResDto>();
                foreach (var item in res)
                {
                    item.IMG_LIST = new List<string>();
                }
                #endregion
                ret.RetData1 = res;
                ret.IsSuccess = true;
            }
            catch (Exception ex)
            {
                ret.IsSuccess = false;
            }

            return ret;

        }

        /// <summary>
        /// 获取 项目历史记录
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetAllProjectListLog(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();

            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);

            try
            {
                string Data = ReqObj.Data.ToString();
                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                string SUPPLIERS_NAME = jarr.ContainsKey("SUPPLIERS_NAME") ? jarr["SUPPLIERS_NAME"].ToString() : "";//厂商
                string CREATEDATE = jarr.ContainsKey("CREATEDATE") ? jarr["CREATEDATE"].ToString() : "";//创建日期

                string whereSql = " Where 1=1 ";
                if (!string.IsNullOrEmpty(SUPPLIERS_NAME))
                    whereSql += $@"and SUPPLIERS_NAME like '%{SUPPLIERS_NAME}%' ";
                if (!string.IsNullOrEmpty(CREATEDATE))
                    whereSql += $@"and CREATEDATE='{CREATEDATE}' ";

                #region 逻辑
                string sql = $@"SELECT SUPPLIERS_NAME,CREATEDATE,REAL_SCORE,GUID FROM QCM_OUT_QUALITY_LIST_LOG_M";
                DataTable dt = DB.GetDataTable(sql + whereSql+ "order by id desc");
                #endregion
                ret.RetData1 =dt;
                ret.IsSuccess = true;
            }
            catch (Exception ex)
            {
                ret.IsSuccess = false;
            }

            return ret;
        }

        /// <summary>
        /// 获取 项目历史记录 详情
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetAllProjectListLogDetails(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();

            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);

            try
            {
                string Data = ReqObj.Data.ToString();
                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                string GUID = jarr.ContainsKey("GUID") ? jarr["GUID"].ToString() : "";//关联键

                string whereSql = " Where 1=1 ";
                if (!string.IsNullOrEmpty(GUID))
                    //whereSql += $@"and GUID='{GUID}' ";
                    whereSql += $@"and a.GUID='{GUID}' ";

                #region 逻辑
                //string sql = $@"SELECT PROJECT,SCORE,REAL_SCORE,PROBLEM_POINT,REMARK,GUID_IMG FROM QCM_OUT_QUALITY_LIST_LOG_D ";
                string sql = $@"SELECT a.PROJECT,a.SCORE,a.REAL_SCORE,a.PROBLEM_POINT,a.REMARK,b.IMG_URL,b.IMG_NAME,b.GUID_IMG,b.GUID FROM QCM_OUT_QUALITY_LIST_LOG_D a LEFT JOIN QCM_OUT_QUALITY_LIST_LOG_IMAGEURL b on a.GUID_IMG=b.GUID_IMG and a.GUID=b.GUID";
                DataTable dt = DB.GetDataTable(sql + whereSql+ "order by a.id desc");
                #endregion
                ret.RetData1 = dt;
                ret.IsSuccess = true;
            }
            catch (Exception ex)
            {
                ret.IsSuccess = false;
                ret.ErrMsg = ex.Message;
            }

            return ret;
        }
        /// <summary>
        /// 获取 项目历史记录 详情D版，PC接口
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetAllProjectListLogDetailsList(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();

            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);

            try
            {
                string Data = ReqObj.Data.ToString();
                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                string GUID = jarr.ContainsKey("GUID") ? jarr["GUID"].ToString() : "";//关联键

                string whereSql = " Where 1=1 ";
                if (!string.IsNullOrEmpty(GUID))
                    //whereSql += $@"and GUID='{GUID}' ";
                    whereSql += $@"and a.GUID='{GUID}' ";

                #region 逻辑
             
                string sql = $@"SELECT
	a.SUPPLIERS_NAME,
	a.CREATEDATE,
	b.PROJECT,
	b.SCORE,
	b.REAL_SCORE,
	b.PROBLEM_POINT,
	b.REMARK,
	c.IMG_URL,
	c.IMG_NAME,
	c.GUID_IMG,
	c.GUID 
FROM
	QCM_OUT_QUALITY_LIST_LOG_M a 
	LEFT JOIN QCM_OUT_QUALITY_LIST_LOG_D b
	ON A.GUID=B.GUID
	LEFT JOIN QCM_OUT_QUALITY_LIST_LOG_IMAGEURL c ON b.GUID_IMG = c.GUID_IMG 
	AND b.GUID = c.GUID";
                DataTable dt = DB.GetDataTable(sql + whereSql + "order by a.id desc");
                #endregion
                ret.RetData1 = dt;
                ret.IsSuccess = true;
            }
            catch (Exception ex)
            {
                ret.IsSuccess = false;
                ret.ErrMsg = ex.Message;
            }

            return ret;
        }

        /// <summary>
        /// 发外厂商品质体系项目日志详情图片
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetAllProjectListIMG(object OBJ)
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
                string sql = $@"select IMG_NAME,IMG_URL from QCM_OUT_QUALITY_LIST_LOG_IMAGEURL where GUID_IMG='{GUID_IMG}'";
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
        /***********************************************网页end************************************************************/

        /***********************************************PDA start************************************************************/

        /// <summary>
        /// 获取厂商信息
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetAllSuppliers(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();

            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);

            try
            {
                string Data = ReqObj.Data.ToString();
                var requst = Newtonsoft.Json.JsonConvert.DeserializeObject<UpdateProjectListReq>(Data);

                #region 逻辑
                string sql = $@"SELECT SUPPLIERS_CODE,SUPPLIERS_NAME FROM BASE003M";
                DataTable dt = DB.GetDataTable(sql);
                #endregion
                ret.RetData1 = dt;
                ret.IsSuccess = true;
            }
            catch (Exception ex)
            {
                ret.IsSuccess = false;
            }

            return ret;

        }

        /// <summary>
        /// 保存 项目 记录
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject SaveProjectListLog(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();

            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);

            try
            {
                string Data = ReqObj.Data.ToString();
                var requst = Newtonsoft.Json.JsonConvert.DeserializeObject<SaveProjectListLogRequestDto>(Data);

                #region 逻辑
                DB.Open();
                DB.BeginTransaction();
                DateTime currDate = DateTime.Now;
                string userCode = SJeMES_Framework_NETCore.Web.System.GetUserCodeByToken(ReqObj.UserToken);

                string guid = Guid.NewGuid().ToString("N");
                //表头
                Dictionary<string, object> insert_m_dic = new Dictionary<string, object>();
                insert_m_dic.Add("SUPPLIERS_CODE", requst.SUPPLIERS_CODE);
                insert_m_dic.Add("SUPPLIERS_NAME", requst.SUPPLIERS_NAME);
                insert_m_dic.Add("REAL_SCORE", requst.Details.Sum(x => x.REAL_SCORE));
                insert_m_dic.Add("GUID", guid);

                insert_m_dic.Add("CREATEBY", userCode);
                insert_m_dic.Add("CREATEDATE", currDate.ToString("yyyy-MM-dd"));
                insert_m_dic.Add("CREATETIME", currDate.ToString("HH:mm:ss"));

                string sql = SJeMES_Framework_NETCore.Common.StringHelper.GetInsertSqlByDictionary("oracle", "QCM_OUT_QUALITY_LIST_LOG_M", insert_m_dic);
                DB.ExecuteNonQuery(sql, insert_m_dic);

                //表身
                foreach (var item in requst.Details)
                {
                    string guid_img = Guid.NewGuid().ToString("N");
                    Dictionary<string, object> insert_d_dic = new Dictionary<string, object>();
                    insert_d_dic.Add("PROJECT", item.PROJECT);
                    insert_d_dic.Add("SCORE", item.SCORE);
                    insert_d_dic.Add("REAL_SCORE", item.REAL_SCORE);
                    insert_d_dic.Add("PROBLEM_POINT", item.PROBLEM_POINT);
                    insert_d_dic.Add("REMARK", item.REMARK);
                    insert_d_dic.Add("GUID", guid);
                    insert_d_dic.Add("GUID_IMG", guid_img);

                    insert_d_dic.Add("CREATEBY", userCode);
                    insert_d_dic.Add("CREATEDATE", currDate.ToString("yyyy-MM-dd"));
                    insert_d_dic.Add("CREATETIME", currDate.ToString("HH:mm:ss"));

                    string sql_d = SJeMES_Framework_NETCore.Common.StringHelper.GetInsertSqlByDictionary("oracle", "QCM_OUT_QUALITY_LIST_LOG_D", insert_d_dic);
                    DB.ExecuteNonQuery(sql_d, insert_d_dic);

                    // 保存图片
                    foreach (string imgPath in item.IMG_LIST)
                    {
                        string fileName = imgPath.Split('/').LastOrDefault();
                        if (string.IsNullOrEmpty(fileName))
                            fileName = "";
                        DB.ExecuteNonQuery($@"insert into QCM_OUT_QUALITY_LIST_LOG_IMAGEURL (img_name,img_url,guid,guid_img,createby,createdate,createtime) 
                    values('{fileName}','{imgPath}','{guid}','{guid_img}','{userCode}','{currDate:yyyy-MM-dd}','{currDate:HH:mm:ss}')");
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

        /***********************************************PDA end************************************************************/
    }

    #region Dto
    public class UpdateProjectListReq
    {
        /// <summary>
        /// 项目集合
        /// </summary>
        public List<BDM_OUT_QUALITY_LIST_M_ModityDto> project_datas { get; set; }
    }
    public class BDM_OUT_QUALITY_LIST_M_ModityDto
    {
        public int? ID { get; set; }
        /// <summary>
        /// 项目
        /// </summary>
        public string PROJECT { get; set; }
        /// <summary>
        /// 配分
        /// </summary>
        public int SCORE { get; set; }
        /// <summary>
        /// 实际得分
        /// </summary>
        public int REAL_SCORE { get; set; }
        /// <summary>
        /// 问题得分
        /// </summary>
        public string PROBLEM_POINT { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string REMARK { get; set; }
    }

    public class SaveProjectListLogRequestDto
    {
        /// <summary>
        /// 厂商代号
        /// </summary>
        public string SUPPLIERS_CODE { get; set; }
        /// <summary>
        /// 厂商名称
        /// </summary>
        public string SUPPLIERS_NAME { get; set; }
        public List<SaveProjectListLogDetailsRequestDto> Details { get; set; }
    }
    public class SaveProjectListLogDetailsRequestDto
    {
        /// <summary>
        /// 项目
        /// </summary>
        public string PROJECT { get; set; }
        /// <summary>
        /// 配分
        /// </summary>
        public int SCORE { get; set; }
        /// <summary>
        /// 实际得分
        /// </summary>
        public int REAL_SCORE { get; set; }
        /// <summary>
        /// 问题得分
        /// </summary>
        public string PROBLEM_POINT { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string REMARK { get; set; }
        /// <summary>
        /// 图片集合
        /// </summary>
        public List<string> IMG_LIST { get; set; }
    }
    public class GetAllProjectListResDto
    {
        public string ID { get; set; }
        /// <summary>
        /// 项目
        /// </summary>
        public string PROJECT { get; set; }
        /// <summary>
        /// 配分
        /// </summary>
        public int? SCORE { get; set; }
        /// <summary>
        /// 实际得分
        /// </summary>
        public int? REAL_SCORE { get; set; }
        /// <summary>
        /// 问题点
        /// </summary>
        public string PROBLEM_POINT { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string REMARK { get; set; }
        /// <summary>
        /// 图片集合
        /// </summary>
        public List<string> IMG_LIST { get; set; }
    }



    #endregion

}
