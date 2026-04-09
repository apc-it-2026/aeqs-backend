using SJ_QCMAPI.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace SJ_QCMAPI
{
    public class QAShoeShapeTable
    {
        /// <summary>
        /// QA鞋型管理首页视图显示
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GET_ShoeShapeTable_List(object OBJ)
        {

            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase();
            try
            {
                DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);
                string Data = ReqObj.Data.ToString();
                //DB.Open();
                //DB.BeginTransaction();
                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);

                string develop_season = jarr.ContainsKey("develop_season") ? jarr["develop_season"].ToString() : "";
                string shoe_no = jarr.ContainsKey("shoe_no") ? jarr["shoe_no"].ToString() : "";
                string prod_no = jarr.ContainsKey("prod_no") ? jarr["prod_no"].ToString() : "";
                string develop_type = jarr.ContainsKey("develop_type") ? jarr["develop_type"].ToString() : "";

                string pageSize = jarr.ContainsKey("pageSize") ? jarr["pageSize"].ToString() : "";
                string pageIndex = jarr.ContainsKey("pageIndex") ? jarr["pageIndex"].ToString() : "";
                string CreactUserId = SJeMES_Framework_NETCore.Web.System.GetUserCodeByToken(ReqObj.UserToken);//查询登录人UserID
                string Day = DateTime.Now.ToString("yyyy-MM-dd");
                string Time = DateTime.Now.ToString("HH:mm:ss");

                string wheres = string.Empty;


                if (!string.IsNullOrEmpty(develop_season))
                    wheres += $" and a.develop_season like '%{develop_season }%'";
                if (!string.IsNullOrEmpty(shoe_no))
                    wheres += $"and a.shoe_no like '%{shoe_no}%'";
                if (!string.IsNullOrEmpty(prod_no))
                    wheres += $"and a.prod_no like '%{prod_no}%'";
                if (!string.IsNullOrEmpty(develop_type))
                    wheres += $"and a.develop_type like '%{develop_type}%'";

                //转译
                string sql = $@"SELECT
	a.DEVELOP_SEASON,
  a.SHOE_NO,
	a.DEVELOP_TYPE,
	({CommonBASE.GetGroupConcatByOracleVersion(DB, "DISTINCT PROD_NO", "PROD_NO")} PROD_NO,
  (case when	(select count(1) from qcm_qa_shoeshape_file h where h.SHOE_NO =a.SHOE_NO and h.DEVELOP_SEASON = a.DEVELOP_SEASON and file_type='1'  )>0 then '1' else '0' end) Limitedrelease,
  (case when	(select count(1) from qcm_qa_shoeshape_file h where h.SHOE_NO =a.SHOE_NO and h.DEVELOP_SEASON = a.DEVELOP_SEASON and file_type='2'  )>0 then '2' else '0' end) Disclimer,
	(case when	(select count(1) from qcm_qa_shoeshape_file h where h.SHOE_NO =a.SHOE_NO and h.DEVELOP_SEASON = a.DEVELOP_SEASON and file_type='3'  )>0 then '3' else '0' end) Visualstandard,
	(case when	(select count(1) from qcm_qa_shoeshape_file h where h.SHOE_NO =a.SHOE_NO and h.DEVELOP_SEASON = a.DEVELOP_SEASON and file_type='4'  )>0 then '4' else '0' end) Other,
	(select IMG_URL from QCM_QA_SHOESHAPE_IMAGE where DEVELOP_SEASON=a.DEVELOP_SEASON and SHOE_NO=a.SHOE_NO and rownum=1 and IMG_URL is not null ) img_url
FROM
	BDM_RD_PROD a
where 1=1 {wheres}
GROUP BY
	a.DEVELOP_SEASON,
	a.SHOE_NO,
	a.DEVELOP_TYPE";

                DataTable dt = CommonBASE.GetPageDataTable(DB, sql, int.Parse(pageIndex), int.Parse(pageSize));
                int rowCount = CommonBASE.GetPageDataTableCount(DB, sql);
                Dictionary<string, object> dic = new Dictionary<string, object>();
                dic.Add("Data", dt);
                dic.Add("rowCount", rowCount);
                ret.IsSuccess = true;
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

        /// <summary>
        /// QA鞋型管理阶段样品品质表头查询赋值
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GET_ShoeShapeHeader(object OBJ)
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

                string user = SJeMES_Framework_NETCore.Web.System.GetUserCodeByToken(ReqObj.UserToken);//获取的登陆人信息
                string date = DateTime.Now.ToString("yyyy-MM-dd");//日期
                string time = DateTime.Now.ToString("HH-mm-ss");//时间

                string develop_season = jarr.ContainsKey("develop_season") ? jarr["develop_season"].ToString() : "";//季度
                string shoe_no = jarr.ContainsKey("shoe_no") ? jarr["shoe_no"].ToString() : "";//鞋型


                //判断qcm_qa_shoeshape_m表中是否有数据没有就加一条
                string count = DB.GetString($@"select count(1) from qcm_qa_shoeshape_m where develop_season='{develop_season}' and shoe_no='{shoe_no}'");
                if (count=="0")
                {
                    DB.ExecuteNonQuery($@"insert into qcm_qa_shoeshape_m (develop_season,shoe_no,createby,createdate,createtime) values('{develop_season}','{shoe_no}','{user}','{date}','{time}')");
                }

                //根据鞋型跟季度查询数据
                string sql = $@"select * from bdm_rd_prod where SHOE_NO='{shoe_no}' and  DEVELOP_SEASON='{develop_season}'";
                DataTable dt = DB.GetDataTable(sql);


                DB.Commit();
                Dictionary<string, object> dic = new Dictionary<string, object>();
                dic.Add("Data",dt);
                ret.IsSuccess = true;
                ret.RetData = Newtonsoft.Json.JsonConvert.SerializeObject(dic);
            }
            catch (Exception ex)
            {
                //DB.Rollback();
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
        /// 历史各阶段样品品质状况
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GET_ShoeShapecenterView(object OBJ)
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
                string develop_season = jarr.ContainsKey("develop_season") ? jarr["develop_season"].ToString() : "";//鞋型
                string shoe_no = jarr.ContainsKey("shoe_no") ? jarr["shoe_no"].ToString() : "";//鞋号
                string sql = $@"select develop_season,shoe_no,check_date,dpstage_code,dpstage_name,qty  from qcm_qa_shoeshape_d where develop_season='{develop_season}' and shoe_no='{shoe_no}'order by id desc";
                string sql2 = $@"select*from QCM_QA_SHOESHAPE_ITEM a  left join qcm_qa_shoeshape_d b on a.develop_season=b.develop_season and a.shoe_no=b.shoe_no and a.check_date=b.check_date and a.dpstage_code=b.dpstage_code order by a.id desc";
                string sql3 = $@"select*from qcm_qa_shoeshape_image where develop_season='{develop_season}' and shoe_no='{shoe_no}' order by id desc";
                DataTable dt = DB.GetDataTable(sql);
                DataTable dt2 = DB.GetDataTable(sql2);
                DataTable dt3 = DB.GetDataTable(sql3);
                Dictionary<string, object> dic = new Dictionary<string, object>();
                dic.Add("Data", dt);
                dic.Add("Data1", dt2);
                dic.Add("Data2", dt3);
                ret.IsSuccess = true;
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
        /// <summary>
        /// QA鞋型文件上传查看
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GET_qcm_qa_shoeshape_file_View(object OBJ)
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

                string develop_season = jarr.ContainsKey("develop_season") ? jarr["develop_season"].ToString() : "";//鞋型
                string shoe_no = jarr.ContainsKey("shoe_no") ? jarr["shoe_no"].ToString() : "";//鞋号

                //根据问题分类代号查询问题追踪点
                string sql = $@"select*from qcm_qa_shoeshape_file where DEVELOP_SEASON='{develop_season}' and SHOE_NO='{shoe_no}'";
                DataTable dt = DB.GetDataTable(sql);

                Dictionary<string, object> dic = new Dictionary<string, object>();
                dic.Add("Data", dt);
                ret.IsSuccess = true;
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
        /// <summary>
        /// 重要问题视图展示
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        /// <summary>
        /// 查询新增中的阶段
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GET_ShoeShape(object OBJ)
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

                string develop_season = jarr.ContainsKey("develop_season") ? jarr["develop_season"].ToString() : "";//季度
                string shoe_no = jarr.ContainsKey("shoe_no") ? jarr["shoe_no"].ToString() : "";//鞋型

                //根据鞋型跟季度查询数据
                string sql = $@"select dpstage_code,dpstage_name from bdm_dpstage_m where dpstage_code NOT IN (select dpstage_code from qcm_qa_shoeshape_item where develop_season='{develop_season}' and shoe_no='{shoe_no}')";
                DataTable dt = DB.GetDataTable(sql);

                Dictionary<string, object> dic = new Dictionary<string, object>();
                dic.Add("Data", dt);
                ret.IsSuccess = true;
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
        /// <summary>
        /// 文件查看视图
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetFileView(object OBJ)
        {

            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase();
            try
            {
                DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);
                string Data = ReqObj.Data.ToString();
                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                string develop_season = jarr.ContainsKey("develop_season") ? jarr["develop_season"].ToString() : "";//鞋型
                string shoe_no = jarr.ContainsKey("shoe_no") ? jarr["shoe_no"].ToString() : "";//鞋号
                string sql = $@"select * from QCM_QA_SHOESHAPE_FILE where develop_season='{develop_season}' and shoe_no='{shoe_no}'";
                DataTable dt = DB.GetDataTable(sql);
                Dictionary<string, object> dic = new Dictionary<string, object>();
                dic.Add("Data", dt);
                //dic.Add("Data1", dt2);
                //dic.Add("Data2", dt3);
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
        /// QA问题点文件查看视图
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetFileViewS(object OBJ)
        {

            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase();
            try
            {
                DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);
                string Data = ReqObj.Data.ToString();
                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);

                string develop_season = jarr.ContainsKey("develop_season") ? jarr["develop_season"].ToString() : "";//季度
                string shoe_no = jarr.ContainsKey("shoe_no") ? jarr["shoe_no"].ToString() : "";//鞋型

                string sql = $@"select * from QCM_QA_SHOESHAPE_IMAGE where develop_season='{develop_season}' and shoe_no='{shoe_no}'";
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
        /// <summary>
        /// 查询重要问题分类
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GET_Problemcategory(object OBJ)
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

                //根据鞋型跟季度查询数据
                string sql = $@"select problemcategory_no,problemcategory_name from bdm_qa_problemcategory_m";
                DataTable dt = DB.GetDataTable(sql);

                Dictionary<string, object> dic = new Dictionary<string, object>();
                dic.Add("Data", dt);
                ret.IsSuccess = true;
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

        /// <summary>
        /// 查询重要问题追踪点
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GET_ProblemcategoryD(object OBJ)
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
                string problemcategory_no = jarr.ContainsKey("problemcategory_no") ? jarr["problemcategory_no"].ToString() : "";//问题分类代号

                //根据问题分类代号查询问题追踪点
                string sql = $@"select problem_no,problem_name from bdm_qa_problemcategory_d where problemcategory_no='{problemcategory_no}'";
                DataTable dt = DB.GetDataTable(sql);

                Dictionary<string, object> dic = new Dictionary<string, object>();
                dic.Add("Data", dt);
                ret.IsSuccess = true;
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

        /// <summary>
        /// 查询最后一次新增的数据
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GET_LastShoeshape_Item(object OBJ)
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
                string develop_season = jarr.ContainsKey("develop_season") ? jarr["develop_season"].ToString() : "";//季度
                string shoe_no = jarr.ContainsKey("shoe_no") ? jarr["shoe_no"].ToString() : "";//鞋型

                //查询QA鞋型品质信息最后一次添加的数据
                string where = string.Empty;
                if (!string.IsNullOrEmpty(develop_season))
                {
                    where += $" and develop_season='{develop_season}'";
                }
                if (!string.IsNullOrEmpty(shoe_no))
                {
                    where += $" and shoe_no='{shoe_no}'";
                }
                string dpstage_code = $@"select dpstage_code from qcm_qa_shoeshape_d where 1=1 {where} order by id desc";
                DataTable dc = DB.GetDataTable(dpstage_code);
                DataTable dt = new DataTable();
                string sql = string.Empty;
                if (dc.Rows.Count>0)
                {
                    if (!string.IsNullOrEmpty(dc.Rows[0]["dpstage_code"].ToString()))
                    {
                        where += $" and dpstage_code='{dc.Rows[0]["dpstage_code"].ToString()}'";
                    }
                    sql = $@"select * from qcm_qa_shoeshape_item where 1=1 {where}";
                    dt = DB.GetDataTable(sql);
                }


                Dictionary<string, object> dic = new Dictionary<string, object>();
                dic.Add("Data", dt);
                ret.IsSuccess = true;
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

        /// <summary>
        /// 新增修改阶段样品品质状况
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject InsertShoeshape_Item(object OBJ)
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
                string user = SJeMES_Framework_NETCore.Web.System.GetUserCodeByToken(ReqObj.UserToken);//获取的登陆人信息
                string date = DateTime.Now.ToString("yyyy-MM-dd");//日期
                string time = DateTime.Now.ToString("HH-mm-ss");//时间
                string develop_season = jarr.ContainsKey("develop_season") ? jarr["develop_season"].ToString() : "";//季度
                string shoe_no = jarr.ContainsKey("shoe_no") ? jarr["shoe_no"].ToString() : "";//鞋型
                string check_date = jarr.ContainsKey("check_date") ? jarr["check_date"].ToString() : "";//日期
                string dpstage_code = jarr.ContainsKey("dpstage_code") ? jarr["dpstage_code"].ToString() : "";//阶段代号
                string qty = jarr.ContainsKey("qty") ? jarr["qty"].ToString() : "";//生产总数
                DataTable qcm_qa_shoeshape_item = Newtonsoft.Json.JsonConvert.DeserializeObject<DataTable>(jarr["qcm_qa_shoeshape_item"].ToString());//QA鞋型品质问题点表
                string dpstage_name = DB.GetString($@"select dpstage_name from bdm_dpstage_m where dpstage_code='{dpstage_code}'");//阶段名称

                //新增QA鞋型品质信息
                DB.ExecuteNonQuery($@"insert into qcm_qa_shoeshape_d (develop_season,shoe_no,check_date,dpstage_code,dpstage_name,qty,createby,createdate,createtime) 
                                      values('{develop_season}','{shoe_no}','{check_date}','{dpstage_code}','{dpstage_name}','{qty}','{user}','{date}','{time}')");

                //新增QA鞋型品质问题点
                if (qcm_qa_shoeshape_item.Rows.Count>0)
                {
                    foreach (DataRow item in qcm_qa_shoeshape_item.Rows)
                    {
                        string problemcategory_name = DB.GetString($@"select problemcategory_name from bdm_qa_problemcategory_m where problemcategory_no='{item["problemcategory_no"]}'");
                        string problem_name = DB.GetString($@"select problem_name from bdm_qa_problemcategory_d where problemcategory_no='{item["problemcategory_no"]}' and problem_no='{item["problem_no"]}'");

                        DB.ExecuteNonQuery($@"insert into qcm_qa_shoeshape_item (develop_season,shoe_no,check_date,dpstage_code,problemcategory_no,problemcategory_name,
                                              problem_no,problem_name,ng_qty,ng_rate,respon_people,improvement_measures,createby,createdate,createtime) 
                                              values('{develop_season}','{shoe_no}','{check_date}','{dpstage_code}','{item["problemcategory_no"]}','{problemcategory_name}',
                                              '{item["problem_no"]}','{problem_name}','{item["ng_qty"]}','{item["ng_rate"]}','{item["respon_people"]}','{item["improvement_measures"]}','{user}','{date}','{time}')");
                    }
                }

                DB.Commit();
              
                ret.IsSuccess = true;
                ret.RetData = "";
            }
            catch (Exception ex)
            {
                //DB.Rollback();
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
        /// 查看上传QA鞋型品质问题点图片
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GET_Qcm_qa_shoeshape_image(object OBJ)
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
                string develop_season = jarr.ContainsKey("develop_season") ? jarr["develop_season"].ToString() : "";//季度
                string shoe_no = jarr.ContainsKey("shoe_no") ? jarr["shoe_no"].ToString() : "";//鞋型
                string dpstage_code = jarr.ContainsKey("dpstage_code") ? jarr["dpstage_code"].ToString() : "";//阶段代号
                string problemcategory_no = jarr.ContainsKey("problemcategory_no") ? jarr["problemcategory_no"].ToString() : "";//问题分类代号
                string problem_no = jarr.ContainsKey("problem_no") ? jarr["problem_no"].ToString() : "";//问题追踪点代号

                //查询上传到QA鞋型品质问题点的图片
                string sql = $@"select img_name,img_url from qcm_qa_shoeshape_image where develop_season='{develop_season}' and shoe_no='{shoe_no}'
                                and dpstage_code='{dpstage_code}' and problemcategory_no='{problemcategory_no}' and problem_no='{problem_no}'";
                DataTable dt = DB.GetDataTable(sql);

                Dictionary<string, object> dic = new Dictionary<string, object>();
                dic.Add("Data", dt);
                ret.IsSuccess = true;
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

        /// <summary>
        /// 修改QA鞋型管理
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject UpdateQcm_qa_shoeshape_m(object OBJ)
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
                string user = SJeMES_Framework_NETCore.Web.System.GetUserCodeByToken(ReqObj.UserToken);//获取的登陆人信息
                string date = DateTime.Now.ToString("yyyy-MM-dd");//日期
                string time = DateTime.Now.ToString("HH-mm-ss");//时间
                string develop_season = jarr.ContainsKey("develop_season") ? jarr["develop_season"].ToString() : "";//季度
                string shoe_no = jarr.ContainsKey("shoe_no") ? jarr["shoe_no"].ToString() : "";//鞋型
                string tryon_result= jarr.ContainsKey("tryon_result") ? jarr["tryon_result"].ToString() : "";//试穿结果
                string fgt_result = jarr.ContainsKey("fgt_result") ? jarr["fgt_result"].ToString() : "";//FGT结果
                string cma_result = jarr.ContainsKey("cma_result") ? jarr["cma_result"].ToString() : "";//CMA结果

                string sql = $@"update qcm_qa_shoeshape_m set tryon_result='{tryon_result}',fgt_result='{fgt_result}',cma_result='{cma_result}',modifyby='{user}',modifydate='{date}',modifytime='{time}' 
                                where develop_season='{develop_season}' and shoe_no='{shoe_no}'";
                DB.ExecuteNonQuery(sql);
                DB.Commit();
                DB.Close();                ret.IsSuccess = true;
                ret.RetData = "";
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
        /// 保存文件(url,name)
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject SaveFileImg(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
            SJeMES_Framework_NETCore.DBHelper.DataBase SysDB = new SJeMES_Framework_NETCore.DBHelper.DataBase(string.Empty);
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);

            try
            {
                string Data = ReqObj.Data.ToString();
                #region 接口参数

                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                string FILE_NAME = jarr.ContainsKey("FILE_NAME") ? jarr["FILE_NAME"].ToString() : "";//图片名称
                string FILE_URL = jarr.ContainsKey("FILE_URL") ? jarr["FILE_URL"].ToString() : "";//图片路径
                string guid = jarr.ContainsKey("GUID") ? jarr["GUID"].ToString() : "";//GUID
                string file_type = jarr.ContainsKey("TYPE") ? jarr["TYPE"].ToString() : "";// 文件类型
                string develop_season = jarr.ContainsKey("DEVELOP_SEASON") ? jarr["DEVELOP_SEASON"].ToString() : "";
                string shoe_no = jarr.ContainsKey("SHOE_NO") ? jarr["SHOE_NO"].ToString() : "";


                #endregion

                #region 逻辑
                DB.Open();
                DB.BeginTransaction();
                var userDIC = SysDB.GetDictionary($"select CompanyCode,UserCode from [dbo].[usertoken] where UserToken='{ReqObj.UserToken}'");
                string createby = userDIC["UserCode"].ToString();
                string createdate = DateTime.Now.ToString("yyyy-MM-dd");
                string createtime = DateTime.Now.ToString("HH:mm:ss");
                //DB.ExecuteNonQueryOffline($@"INSERT INTO QCM_CUSTOMER_COMPLAINT_FILE (COMPLAINT_NO,IMG_NAME,IMG_URL,GUID,TYPE,CREATEBY,CREATEDATE,CREATETIME)VALUES('{COMPLAINT_NO}','{IMG_NAME}','{IMG_URL}','{GUID}','{TYPE}','{createby}','{createdate}','{createtime}')");

                ;

                DB.ExecuteNonQueryOffline($@"
                                insert into qcm_qa_shoeshape_file(develop_season,shoe_no,file_type,file_name,file_url,guid,CREATEBY,CREATEDATE,CREATETIME)
                                VALUES('{develop_season}','{shoe_no}','{file_type}','{FILE_NAME}','{FILE_URL}','{guid}','{createby}','{createdate}','{createtime}')");

                DB.Commit();
                ret.ErrMsg = "保存成功";
                ret.IsSuccess = true;

                //if (DB.ExecuteNonQueryOffline($@"
                //                insert into qcm_qa_shoeshape_file(develop_season,shoe_no,file_type,file_name,file_url,guid,CREATEBY,CREATEDATE,CREATETIME)
                //                VALUES('{develop_season}','{shoe_no}','{file_type}','{FILE_NAME}','{FILE_URL}','{guid}','{createby}','{createdate}','{createtime}')") > 0)
                //{
                //    DB.Commit();
                //    ret.ErrMsg = "保存成功";
                //    ret.IsSuccess = true;
                //}
                //else
                //{
                //    ret.ErrMsg = "保存失败，原因：" + ex.Message;
                //    ret.IsSuccess = false;
                //}
                #endregion

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
    }
}
