using SJ_QCMAPI.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace SJ_QCMAPI
{
    public class PilotRun
    {
        /// <summary>
        /// 量试主页查询
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetPilotRun_Main(object OBJ)
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
                string shoes_name = jarr.ContainsKey("shoes_name") ? jarr["shoes_name"].ToString() : "";//查询条件 鞋型
                string prod_name = jarr.ContainsKey("prod_name") ? jarr["prod_name"].ToString() : "";//查询条件 art
                string pageSize = jarr.ContainsKey("pageSize") ? jarr["pageSize"].ToString() : "";
                string pageIndex = jarr.ContainsKey("pageIndex") ? jarr["pageIndex"].ToString() : "";
                //转译
                string where = string.Empty;
                if (!string.IsNullOrWhiteSpace(shoes_name))
                {
                    where += $@" and shoes_name like '%{shoes_name}%'";
                }
                if (!string.IsNullOrWhiteSpace(prod_name))
                {
                    where += $@" and prod_no like '%{prod_name}%'";
                }
                string sql = $@"select * from(
                                SELECT
                                    m.id,
	                                m.shoes_code,
	                                m.prod_no,
	                                r.name_t as prod_name,
	                                s.name_t as shoes_name
                                FROM
	                                qcm_ls_list_m m
	                                LEFT JOIN BDM_RD_PROD r on m.PROD_NO=r.PROD_NO
	                                LEFT JOIN BDM_RD_STYLE s on m.shoes_code=s.shoe_no
                                    where m.isdelete='0')t where 1=1 {where} ORDER BY id desc";
                DataTable dt = CommonBASE.GetPageDataTable(DB, sql, int.Parse(pageIndex), int.Parse(pageSize));
                int rowCount = CommonBASE.GetPageDataTableCount(DB, sql);

                Dictionary<string, object> dic = new Dictionary<string, object>();
                dic.Add("Data", dt);
                dic.Add("rowCount", rowCount);

                ret.RetData1 = dic;
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
        /// 新增查询art
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetPilotRun_Insert_art(object OBJ)
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
                string keyword = jarr.ContainsKey("keyword") ? jarr["keyword"].ToString() : "";//查询条件
                string pageRow = jarr.ContainsKey("pageRow") ? jarr["pageRow"].ToString() : "";
                string page = jarr.ContainsKey("page") ? jarr["page"].ToString() : "";
                //转译
                string where = string.Empty;
                if (!string.IsNullOrWhiteSpace(keyword))
                {
                    where += $@" and (prod_no like '%{keyword}%' or name_t like '%{keyword}%')";
                }
                string sql = $@"select prod_no as value,name_t as label from BDM_RD_PROD where 1=1 {where}";
                DataTable dt = CommonBASE.GetPageDataTable(DB, sql, int.Parse(page), int.Parse(pageRow));
                int rowCount = CommonBASE.GetPageDataTableCount(DB, sql);

                Dictionary<string, object> dic = new Dictionary<string, object>();
                dic.Add("Data", dt);
                dic.Add("rowCount", rowCount);

                ret.RetData1 = dic;
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
        /// 新增时根据art查询信息
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetPilotRun_art_Detail(object OBJ)
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
                string prod_no = jarr.ContainsKey("prod_no") ? jarr["prod_no"].ToString() : "";//查询条件 art
                //转译

                string sql = $@"SELECT
                                r.prod_no,
                                r.name_t as prod_name,
                                r.shoe_no,
                                d.name_t as shoe_name,
                                r.develop_season,
                                r.user_section,
                                
                                c.name_t as rule_no,
                                r.user_in_shoecharge,
                                r.test_level,
                                r.user_technical,
                                r.develop_type,
                                m.qa_principal,
                                r.COL1
                                FROM
                                BDM_RD_PROD r
                                LEFT JOIN BDM_RD_STYLE d on r.SHOE_NO=d.SHOE_NO
                                LEFT JOIN bdm_shoe_extend_m m on r.SHOE_NO=m.SHOE_NO
                                LEFT JOIN bdm_rd_style s on r.shoe_no=s.shoe_no
                                LEFT JOIN bdm_cd_code c on s.style_seq=c.code_no
                                where prod_no='{prod_no}' and rownum<2";
                DataTable dt = DB.GetDataTable(sql);
                dt.Columns.Add("file_list", typeof(object));
                DataTable fl = new DataTable();
                fl.Columns.Add("file_name");
                fl.Columns.Add("file_url");
                foreach (DataRow item in dt.Rows)
                {
                    item["file_list"] = fl;
                    sql = $@"select id from qcm_dqa_mag_d where shoes_code='{item["shoe_no"]}'";
                    DataTable dtdid = DB.GetDataTable(sql);
                    DataRow[] drdid = dtdid.Select();
                    string[] dids = drdid.Select(x => x["id"].ToString()).ToArray();
                    string where = string.Empty;
                    if (dids.Length > 0)
                    {
                        where = $@" and f.d_id in ({string.Join(",", dids.Select(x => $@"'{x}'"))})";

                        sql = $@"SELECT
	                            t.file_url,
	                            t.file_name
                            FROM
	                            qcm_dqa_mag_d_f f
	                            INNER JOIN BDM_UPLOAD_FILE_ITEM t ON  f.file_id = t.guid
                            where 1=1 {where}";
                        fl = DB.GetDataTable(sql);
                        if (fl.Rows.Count > 0)
                        {
                            item["file_list"] = fl;
                        }
                    }
                }
                Dictionary<string, object> dic = new Dictionary<string, object>();
                for (int i = 0; i < dt.Columns.Count; i++)
                {
                    dic.Add(dt.Columns[i].ColumnName, dt.Rows[0][dt.Columns[i].ColumnName]);
                }
                ret.RetData1 = dic;
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
        /// 量试主页新增
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject InsertPilotRun_Main(object OBJ)
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
                string shoes_code = jarr.ContainsKey("shoes_code") ? jarr["shoes_code"].ToString() : "";//查询条件 鞋型
                string prod_no = jarr.ContainsKey("prod_no") ? jarr["prod_no"].ToString() : "";//查询条件 art
                string user = SJeMES_Framework_NETCore.Web.System.GetUserCodeByToken(ReqObj.UserToken);//获取的登陆人信息
                string date = DateTime.Now.ToString("yyyy-MM-dd");//日期
                string time = DateTime.Now.ToString("HH:mm:ss");//时间

                string sql = string.Empty;
                int count = DB.GetInt32($@"select count(1) from qcm_ls_list_m where prod_no='{prod_no}'");
                if (count>0)
                {
                    ret.IsSuccess = false;
                    ret.ErrMsg = "该ART编号已创建!";
                    return ret;
                }
                sql = $@"insert into qcm_ls_list_m (shoes_code,prod_no,createby,createdate,createtime) 
                        values('{shoes_code}','{prod_no}','{user}','{date}','{time}') ";

                DB.ExecuteNonQuery(sql);
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

        /// <summary>
        /// 量试管理查看附件
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        //public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetPilotRun_Edit_File(object OBJ)
        //{
        //    SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
        //    SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
        //    SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase();
        //    try
        //    {
        //        DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);
        //        string Data = ReqObj.Data.ToString();
        //        DB.Open();
        //        DB.BeginTransaction();
        //        var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
        //        //转译
        //        string shoes_code = jarr.ContainsKey("shoes_code") ? jarr["shoes_code"].ToString() : "";//查询条件 鞋型
        //        string user = SJeMES_Framework_NETCore.Web.System.GetUserCodeByToken(ReqObj.UserToken);//获取的登陆人信息
        //        string date = DateTime.Now.ToString("yyyy-MM-dd");//日期
        //        string time = DateTime.Now.ToString("HH:mm:ss");//时间

        //        string sql = string.Empty;
        //        sql = $@"select id from qcm_dqa_mag_d where shoes_code='{shoes_code}'";
        //        DataTable dtdid = DB.GetDataTable(sql);
        //        DataRow[] drdid = dtdid.Select();
        //        string[] dids = drdid.Select(x => x["id"].ToString()).ToArray();
        //        string where = string.Empty;
        //        if (dids.Length > 0)
        //        {
        //            where = $@" and f.d_id in ({string.Join(",", dids.Select(x => $@"'{x}'"))})";
        //        }
        //        else
        //        {
        //            where = $@" and 1=2";
        //        }
        //        sql = $@"SELECT
        //                     t.file_url,
        //                     t.file_name
        //                    FROM
        //                     qcm_dqa_mag_d_f f
        //                     INNER JOIN BDM_UPLOAD_FILE_ITEM t ON  f.file_id = t.guid
        //                    where 1=1 {where}";

        //        DataTable dt = DB.GetDataTable(sql);

        //        Dictionary<string, object> dic = new Dictionary<string, object>();
        //        dic.Add("Data", dt);

        //        ret.RetData1 = dic;
        //        ret.IsSuccess = true;

        //    }
        //    catch (Exception ex)
        //    {
        //        DB.Rollback();
        //        ret.IsSuccess = false;
        //        ret.ErrMsg = ex.Message;
        //    }
        //    finally
        //    {
        //        DB.Close();
        //    }
        //    return ret;
        //}

        /// <summary>
        /// 量试管理查询工段
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetPilotRun_Edit_Workshop(object OBJ)
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
                string keyword = jarr.ContainsKey("keyword") ? jarr["keyword"].ToString() : "";//查询条件
                string pageRow = jarr.ContainsKey("pageRow") ? jarr["pageRow"].ToString() : "";
                string page = jarr.ContainsKey("page") ? jarr["page"].ToString() : "";
                //转译
                string where = string.Empty;
                if (!string.IsNullOrWhiteSpace(keyword))
                {
                    where += $@" and (workshop_section_no like '%{keyword}%' or workshop_section_name like '%{keyword}%')";
                }
                string sql = $@"select workshop_section_no as value,workshop_section_name as label from bdm_workshop_section_m where 1=1 {where}";
                DataTable dt = CommonBASE.GetPageDataTable(DB, sql, int.Parse(page), int.Parse(pageRow));
                int rowCount = CommonBASE.GetPageDataTableCount(DB, sql);

                Dictionary<string, object> dic = new Dictionary<string, object>();
                dic.Add("Data", dt);
                dic.Add("rowCount", rowCount);

                ret.RetData1 = dic;
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
        /// 量试管理查询
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetPilotRun_Edit(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase();
            SJeMES_Framework_NETCore.DBHelper.DataBase DBServer = new SJeMES_Framework_NETCore.DBHelper.DataBase();
            try
            {
                DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);
                DBServer = new SJeMES_Framework_NETCore.DBHelper.DataBase(string.Empty);
                string Data = ReqObj.Data.ToString();
                DB.Open();
                DB.BeginTransaction();
                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                string CompanyCode = jarr.ContainsKey("CompanyCode") ? jarr["CompanyCode"].ToString() : "";//查询条件 代号【C端跳转使用】
                string shoes_code = jarr.ContainsKey("shoes_code") ? jarr["shoes_code"].ToString() : "";//查询条件 鞋型
                string workshop_section_no = jarr.ContainsKey("workshop_section_no") ? jarr["workshop_section_no"].ToString() : "";//查询条件 工段编号
                string pageRow = jarr.ContainsKey("pageRow") ? jarr["pageRow"].ToString() : "";
                string page = jarr.ContainsKey("page") ? jarr["page"].ToString() : "";
                //转译
                string where = string.Empty;
                if (!string.IsNullOrWhiteSpace(shoes_code))
                {
                    where += $@" and shoes_code='{shoes_code}'";
                }
                if (!string.IsNullOrWhiteSpace(workshop_section_no))
                {
                    where += $@" and workshop_section_no='{workshop_section_no}'";
                }
                string sql = $@"SELECT
	                                * 
                                FROM
	                                (
	                                SELECT
		                                d.id,
		                                d.shoes_code,
		                                d.choice_no,
		                                d.choice_name,
		                                d.inspection_code,
		                                '' AS inspection_name,
		                                s.enum_value2,
		                                s.enum_value,
		                                d.standard_value,
		                                d.unit,
		                                d.remark,
		                                d.other_measures,
		                                d.workshop_section_name,
		                                d.workshop_section_no,
		                                t.file_url as url,
		                                'true' AS ifls 
	                                FROM
		                                qcm_ls_list_d d
		                                LEFT JOIN SYS001M s ON d.inspection_type = s.enum_code and s.enum_type='enum_inspection_type'
		                                LEFT JOIN BDM_UPLOAD_FILE_ITEM t on d.image_guid=t.guid
		                                AND s.enum_type = 'enum_inspection_type'
                                        where d.isdelete='0'
                                    UNION
	                                SELECT
		                                d.id,
		                                d.shoes_code,
		                                d.choice_no,
		                                d.choice_name,
		                                d.inspection_code,
		                                '' AS inspection_name,
		                                s.enum_value2,
		                                s.enum_value,
		                                d.standard_value,
		                                d.unit,
		                                d.remark,
		                                d.other_measures,
		                                m.workshop_section_name,
		                                m.workshop_section_no,
		                                t.file_url as url,
		                                'false' AS ifls 
	                                FROM
		                                qcm_dqa_mag_d d
		                                LEFT JOIN SYS001M s ON d.inspection_type = s.enum_code and s.enum_type='enum_inspection_type'
		                                AND s.enum_type = 'enum_inspection_type'
		                                LEFT JOIN qcm_dqa_mag_m m ON d.m_id = m.id AND m.isdelete = '0' 
		                                LEFT JOIN BDM_UPLOAD_FILE_ITEM t on d.image_guid=t.guid
	                                WHERE
	                                d.isdelete = '0' 
	                                ) t where 1=1 {where}";
                DataTable dt = CommonBASE.GetPageDataTable(DB, sql, int.Parse(page), int.Parse(pageRow));
                foreach (DataRow item in dt.Rows)
                {
                    if (!string.IsNullOrWhiteSpace(item["enum_value2"].ToString()) && item["enum_value2"] != null)
                    {
                        string inspection_name = DB.GetString($@"select inspection_name from {item["enum_value2"]} where inspection_code='{item["inspection_code"]}'");
                        item["inspection_name"] = inspection_name;
                    }
                }
                dt.Columns.Add("check_qty");//检验总数
                dt.Columns.Add("qty");//合格数量
                dt.Columns.Add("bad_descr");//不良问题描述
                dt.Columns.Add("check_res");//检验结果
                dt.Columns.Add("img_list2", typeof(object));//图片集合
                foreach (DataRow item in dt.Rows)
                {
                    DataTable im = new DataTable();
                    im.Columns.Add("guid");
                    im.Columns.Add("url");
                    item["img_list2"] = im;
                    if (item["ifls"].ToString() == "true")
                    {
                        sql = $@"select id,check_qty,qty,bad_descr,check_res from qcm_ls_list_d_d where isdelete='0' and m_id='{item["id"]}' order by id asc";
                        DataTable lsdt = DB.GetDataTable(sql);
                        if (lsdt.Rows.Count > 0)
                        {
                            foreach (DataRow lsdtit in lsdt.Rows)
                            {
                                item["check_qty"] = lsdtit["check_qty"];
                                item["qty"] = lsdtit["qty"];
                                item["bad_descr"] = lsdtit["bad_descr"];
                                item["check_res"] = lsdtit["check_res"];
                                sql = $@"SELECT 
                                    t.guid,
                                    t.file_url as url
                                    FROM
                                    qcm_ls_list_d_f q
                                    LEFT JOIN BDM_UPLOAD_FILE_ITEM t on q.file_guid=t.guid
                                    where q.union_id='{lsdtit["id"]}'";
                                im = DB.GetDataTable(sql);
                                if (im.Rows.Count > 0)
                                {
                                    item["img_list2"] = im;
                                }
                            }
                        }
                    }
                    else if (item["ifls"].ToString() == "false")
                    {
                        sql = $@"select id,check_qty,qty,bad_descr,check_res from qcm_dqa_mag_d_d where isdelete='0' and m_id='{item["id"]}' order by id asc";
                        DataTable dqadt = DB.GetDataTable(sql);
                        if (dqadt.Rows.Count > 0)
                        {
                            foreach (DataRow dqait in dqadt.Rows)
                            {
                                item["check_qty"] = dqait["check_qty"];
                                item["qty"] = dqait["qty"];
                                item["bad_descr"] = dqait["bad_descr"];
                                item["check_res"] = dqait["check_res"];
                                sql = $@"SELECT 
                                    t.guid,
                                    t.file_url as url
                                    FROM
                                    qcm_dqa_mag_d_d_f q
                                    LEFT JOIN BDM_UPLOAD_FILE_ITEM t on q.file_guid=t.guid
                                    where q.union_id='{dqait["id"]}'";
                                im = DB.GetDataTable(sql);
                                if (im.Rows.Count > 0)
                                {
                                    item["img_list2"] = im;
                                }
                            }
                        }
                    }
                }
                dt.Columns.Add("file_list", typeof(object));
                foreach (DataRow item in dt.Rows)
                {
                    DataTable fl = new DataTable();
                    fl.Columns.Add("file_name");
                    fl.Columns.Add("file_url");
                    item["file_list"] = fl;
                    if (item["ifls"].ToString() == "false")
                    {
                        sql = $@"SELECT 
                        t.file_name,
                        t.file_url
                        FROM
                        qcm_dqa_mag_d_f q
                        LEFT JOIN BDM_UPLOAD_FILE_ITEM t on q.file_id=t.guid
                        where q.ISDELETE='0' and q.d_id='{item["id"]}'";
                        fl = DB.GetDataTable(sql);
                        if (fl.Rows.Count > 0)
                        {
                            item["file_list"] = fl;
                        }
                    }
                }
                int rowCount = CommonBASE.GetPageDataTableCount(DB, sql);

                string url = DBServer.GetString($@"SELECT uploadurl from SYSORG01M where org = '{CompanyCode}'");

                Dictionary<string, object> dic = new Dictionary<string, object>();
                dic.Add("Data", dt);
                dic.Add("rowCount", rowCount);
                dic.Add("url", url);

                ret.RetData1 = dic;
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
        /// 量试管理查询材料/工序
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject Getchoice(object OBJ)
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
                string keyword = jarr.ContainsKey("keyword") ? jarr["keyword"].ToString() : "";//查询条件
                string workshop_section_no = jarr.ContainsKey("workshop_section_no") ? jarr["workshop_section_no"].ToString() : "";//查询条件 工段编号
                string pageRow = jarr.ContainsKey("pageRow") ? jarr["pageRow"].ToString() : "";
                string page = jarr.ContainsKey("page") ? jarr["page"].ToString() : "";

                string where = string.Empty;
                if (string.IsNullOrWhiteSpace(workshop_section_no))
                {
                    ret.IsSuccess = false;
                    ret.ErrMsg = "请先选择工段!";
                    return ret;
                }
                if (!string.IsNullOrEmpty(keyword))
                {
                    where = $@" and t.choice_no like '%{keyword}%' or t.choice_name like '%{keyword}%'";
                }

                string ds = DB.GetString($@"select data_source from bdm_workshop_section_m where workshop_section_no='{workshop_section_no}'");

                string sql = string.Empty;
                if (!string.IsNullOrEmpty(ds))
                {
                    if (ds == "0")
                    {
                        sql = $@"select choice_no,choice_name from( select item_no as choice_no,NAME_T as choice_name from bdm_rd_item)t where 1=1 {where}";
                    }
                    else
                    {
                        sql = $@"select choice_no,choice_name from(select procedure_no as choice_no,procedure_name as choice_name from base025m)t where 1=1 {where}";
                    }
                }
                else
                {
                    sql = $@"
SELECT choice_no,choice_name FROM (
select choice_no,choice_name from(select procedure_no as choice_no,procedure_name as choice_name from base025m)t 
UNION ALL
select choice_no,choice_name from( select item_no as choice_no,NAME_T as choice_name from bdm_rd_item)t 
) tab
WHERE 1=1 and choice_no like '%{keyword}%' OR choice_name like '%{keyword}%' ";
                }

                DataTable dt = CommonBASE.GetPageDataTable(DB, sql, int.Parse(page), int.Parse(pageRow));
                int rowCount = CommonBASE.GetPageDataTableCount(DB, sql);

                Dictionary<string, object> dic = new Dictionary<string, object>();
                dic.Add("Data", dt);
                dic.Add("rowCount", rowCount);

                ret.RetData1 = dic;
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
        /// 量试管理查询检测项
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject Getinspection(object OBJ)
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
                string workshop_section_no = jarr.ContainsKey("workshop_section_no") ? jarr["workshop_section_no"].ToString() : "";//查询条件 工段编号
                string keyword = jarr.ContainsKey("keyword") ? jarr["keyword"].ToString() : "";//查询条件
                string pageRow = jarr.ContainsKey("pageRow") ? jarr["pageRow"].ToString() : "";
                string page = jarr.ContainsKey("page") ? jarr["page"].ToString() : "";

                string where = string.Empty;
                if (!string.IsNullOrWhiteSpace(keyword))
                {
                    where = $@" and (inspection_code like '%{keyword}%' or inspection_name like '%{keyword}%')";
                }
                string bname = $@"SELECT
                                s.enum_code,
	                            s.enum_value2 
                            FROM
	                            bdm_workshop_section_m m
	                            INNER JOIN bdm_workshop_section_d d ON m.id = d.m_id
	                            LEFT JOIN sys001m s ON d.inspection_type = s.enum_code 
                            WHERE
	                            s.enum_type = 'enum_inspection_type' 
	                            AND m.workshop_section_no = '{workshop_section_no}'
                            order by d.id asc";
                DataTable tablename = DB.GetDataTable(bname);
                string sql = string.Empty;
                if (tablename.Rows.Count > 0)
                {
                    sql = $@"select '{tablename.Rows[0]["enum_code"]}' as inspection_type,inspection_code,inspection_name from {tablename.Rows[0]["enum_value2"]} where 1=1 {where}";
                }

                DataTable dt = CommonBASE.GetPageDataTable(DB, sql, int.Parse(page), int.Parse(pageRow));
                int rowCount = CommonBASE.GetPageDataTableCount(DB, sql);

                Dictionary<string, object> dic = new Dictionary<string, object>();
                dic.Add("Data", dt);
                dic.Add("rowCount", rowCount);

                ret.RetData1 = dic;
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
        /// 量试管理新增
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject InsertPilotRun_Edit(object OBJ)
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
                string ifls = jarr.ContainsKey("ifls") ? jarr["ifls"].ToString() : "";//查询条件 是否量试
                string mid = jarr.ContainsKey("mid") ? jarr["mid"].ToString() : "";//查询条件 量试清单外面列表id
                string id = jarr.ContainsKey("id") ? jarr["id"].ToString() : "";//查询条件 量试或dqa的id
                string shoes_code = jarr.ContainsKey("shoes_code") ? jarr["shoes_code"].ToString() : "";//查询条件 鞋型
                Dictionary<string, object> datahead = jarr.ContainsKey("datahead") ? Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(jarr["datahead"].ToString()) : null;//查询条件 头数据
                Dictionary<string, object> databody = jarr.ContainsKey("databody") ? Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(jarr["databody"].ToString()) : null;//查询条件 身数据
                DataTable dataguid = jarr.ContainsKey("dataguid") ? Newtonsoft.Json.JsonConvert.DeserializeObject<DataTable>(jarr["dataguid"].ToString()) : null;//查询条件 身图片guid数据
                string user = SJeMES_Framework_NETCore.Web.System.GetUserCodeByToken(ReqObj.UserToken);//获取的登陆人信息
                string date = DateTime.Now.ToString("yyyy-MM-dd");//日期
                string time = DateTime.Now.ToString("HH:mm:ss");//时间

                string sql = string.Empty;
                if (ifls == "true")
                {
                    if (string.IsNullOrWhiteSpace(id))
                    {
                        if (datahead.Count > 0)
                        {
                            sql = $@"insert into qcm_ls_list_d (shoes_code,m_id,image_guid,choice_no,choice_name,inspection_code,inspection_type, 
                                    other_measures,workshop_section_no,workshop_section_name,remark,createby,createdate,createtime) 
                                    values('{shoes_code}','{mid}','{datahead["image_guid"]}','{datahead["choice_no"]}','{datahead["choice_name"]}','{datahead["inspection_code"]}',
                                    '{datahead["inspection_type"]}','{datahead["other_measures"]}',
                                    '{datahead["workshop_section_no"]}','{datahead["workshop_section_name"]}','{datahead["remark"]}','{user}','{date}','{time}')";
                            DB.ExecuteNonQuery(sql);
                        }
                        string did = SJeMES_Framework_NETCore.DBHelper.DataBase.GetCurId(DB, "qcm_ls_list_d");
                        if (databody.Count > 0)
                        {
                            sql = $@"insert into qcm_ls_list_d_d (m_id,check_qty,qty,bad_descr,check_res,createby,createdate,createtime) 
                                    values('{did}','{databody["check_qty"]}','{databody["qty"]}','{databody["bad_descr"]}','{databody["check_res"]}','{user}','{date}','{time}')";
                            DB.ExecuteNonQuery(sql);

                            #region 更新首次检验
                            DataTable curr_qcm_ls_list_d_dt = DB.GetDataTablebyline($@"
SELECT 
	SHOES_CODE,
	INSPECTION_CODE,
	INSPECTION_TYPE,
	WORKSHOP_SECTION_NO
FROM qcm_ls_list_d 
WHERE id={did}
");
                            if (curr_qcm_ls_list_d_dt.Rows.Count > 0)
                            {
                                string SHOES_CODE = curr_qcm_ls_list_d_dt.Rows[0]["SHOES_CODE"].ToString();
                                string INSPECTION_CODE = curr_qcm_ls_list_d_dt.Rows[0]["INSPECTION_CODE"].ToString();
                                string INSPECTION_TYPE = curr_qcm_ls_list_d_dt.Rows[0]["INSPECTION_TYPE"].ToString();
                                string WORKSHOP_SECTION_NO = curr_qcm_ls_list_d_dt.Rows[0]["WORKSHOP_SECTION_NO"].ToString();

                                DataTable QCM_DQA_MAG_M_dt = DB.GetDataTablebyline($@"
SELECT 
	d.ID,
	d.F_INSP_RES
FROM 
QCM_DQA_MAG_M m
INNER JOIN QCM_DQA_MAG_D d ON d.M_ID=m.ID 
WHERE d.SHOES_CODE='{SHOES_CODE}' 
AND d.INSPECTION_CODE='{INSPECTION_CODE}'
 AND d.INSPECTION_TYPE='{INSPECTION_TYPE}' 
AND m.WORKSHOP_SECTION_NO='{WORKSHOP_SECTION_NO}'
");
                                if (QCM_DQA_MAG_M_dt.Rows.Count > 0)
                                {
                                    string check_res_res = "Fail";
                                    if (databody["check_res"].ToString() == "0")
                                    {
                                        check_res_res = "Pass";
                                    }
                                    string QCM_DQA_MAG_M_ID = QCM_DQA_MAG_M_dt.Rows[0]["ID"].ToString();
                                    string QCM_DQA_MAG_M_F_INSP_RES = QCM_DQA_MAG_M_dt.Rows[0]["F_INSP_RES"].ToString();
                                    if (string.IsNullOrWhiteSpace(QCM_DQA_MAG_M_F_INSP_RES))
                                    {
                                        string dp_name = DB.GetStringline($@"
SELECT
	a.DEPARTMENT_NAME
FROM
	HR001M m
INNER JOIN BASE005M a ON a.DEPARTMENT_CODE=m.STAFF_DEPARTMENT
WHERE m.STAFF_NO='{user}'
");
                                        string QCM_DQA_MAG_M_sql = $@"UPDATE QCM_DQA_MAG_D SET F_INSP_DEP='{dp_name}',F_INSP_DATE='{date}',F_INSP_RES='{check_res_res}' WHERE ID={QCM_DQA_MAG_M_ID}";
                                        DB.ExecuteNonQuery(QCM_DQA_MAG_M_sql);
                                    }
                                }
                            }
                            #endregion
                        }
                        string ddid = SJeMES_Framework_NETCore.DBHelper.DataBase.GetCurId(DB, "qcm_ls_list_d_d");
                        string ssqq = string.Empty;
                        if (dataguid.Rows.Count > 0)
                        {
                            foreach (DataRow item in dataguid.Rows)
                            {
                                ssqq += $@"insert into qcm_ls_list_d_f (union_id,file_guid,createby,createdate,createtime) 
                                    values('{ddid}','{item["guid"]}','{user}','{date}','{time}');";
                            }
                            DB.ExecuteNonQuery($@"BEGIN {ssqq} END;");
                        }
                    }
                    else
                    {
                        if (databody.Count > 0)
                        {
                            sql = $@"insert into qcm_ls_list_d_d (m_id,check_qty,qty,bad_descr,check_res,createby,createdate,createtime) 
                                    values('{id}','{databody["check_qty"]}','{databody["qty"]}','{databody["bad_descr"]}','{databody["check_res"]}','{user}','{date}','{time}')";
                            DB.ExecuteNonQuery(sql);

                            #region 更新首次检验
                            DataTable curr_qcm_ls_list_d_dt = DB.GetDataTablebyline($@"
SELECT 
	SHOES_CODE,
	INSPECTION_CODE,
	INSPECTION_TYPE,
	WORKSHOP_SECTION_NO
FROM qcm_ls_list_d 
WHERE id={id}
");
                            if (curr_qcm_ls_list_d_dt.Rows.Count > 0)
                            {
                                string SHOES_CODE = curr_qcm_ls_list_d_dt.Rows[0]["SHOES_CODE"].ToString();
                                string INSPECTION_CODE = curr_qcm_ls_list_d_dt.Rows[0]["INSPECTION_CODE"].ToString();
                                string INSPECTION_TYPE = curr_qcm_ls_list_d_dt.Rows[0]["INSPECTION_TYPE"].ToString();
                                string WORKSHOP_SECTION_NO = curr_qcm_ls_list_d_dt.Rows[0]["WORKSHOP_SECTION_NO"].ToString();

                                DataTable QCM_DQA_MAG_M_dt = DB.GetDataTablebyline($@"
SELECT 
	d.ID,
	d.F_INSP_RES
FROM 
QCM_DQA_MAG_M m
INNER JOIN QCM_DQA_MAG_D d ON d.M_ID=m.ID 
WHERE d.SHOES_CODE='{SHOES_CODE}' 
AND d.INSPECTION_CODE='{INSPECTION_CODE}'
 AND d.INSPECTION_TYPE='{INSPECTION_TYPE}' 
AND m.WORKSHOP_SECTION_NO='{WORKSHOP_SECTION_NO}'
");
                                if (QCM_DQA_MAG_M_dt.Rows.Count > 0)
                                {
                                    string check_res_res = "Fail";
                                    if (databody["check_res"].ToString() == "0")
                                    {
                                        check_res_res = "Pass";
                                    }
                                    string QCM_DQA_MAG_M_ID = QCM_DQA_MAG_M_dt.Rows[0]["ID"].ToString();
                                    string QCM_DQA_MAG_M_F_INSP_RES = QCM_DQA_MAG_M_dt.Rows[0]["F_INSP_RES"].ToString();
                                    if (string.IsNullOrWhiteSpace(QCM_DQA_MAG_M_F_INSP_RES))
                                    {
                                        string dp_name = DB.GetStringline($@"
SELECT
	a.DEPARTMENT_NAME
FROM
	HR001M m
INNER JOIN BASE005M a ON a.DEPARTMENT_CODE=m.STAFF_DEPARTMENT
WHERE m.STAFF_NO='{user}'
");
                                        string QCM_DQA_MAG_M_sql = $@"UPDATE QCM_DQA_MAG_D SET F_INSP_DEP='{dp_name}',F_INSP_DATE='{date}',F_INSP_RES='{check_res_res}' WHERE ID={QCM_DQA_MAG_M_ID}";
                                        DB.ExecuteNonQuery(QCM_DQA_MAG_M_sql);
                                    }
                                }
                            }
                            #endregion
                        }
                        string ddid = SJeMES_Framework_NETCore.DBHelper.DataBase.GetCurId(DB, "qcm_ls_list_d_d");
                        string ssqq = string.Empty;
                        if (dataguid.Rows.Count > 0)
                        {
                            foreach (DataRow item in dataguid.Rows)
                            {
                                ssqq += $@"insert into qcm_ls_list_d_f (union_id,file_guid,createby,createdate,createtime) 
                                    values('{ddid}','{item["guid"]}','{user}','{date}','{time}');";
                            }
                            DB.ExecuteNonQuery($@"BEGIN {ssqq} END;");
                        }
                    }
                }
                else if (ifls == "false")
                {
                    if (databody.Count > 0)
                    {
                        sql = $@"insert into qcm_dqa_mag_d_d (m_id,check_qty,qty,bad_descr,check_res,createby,createdate,createtime) 
                                    values('{id}','{databody["check_qty"]}','{databody["qty"]}','{databody["bad_descr"]}','{databody["check_res"]}','{user}','{date}','{time}')";
                        DB.ExecuteNonQuery(sql);

                        #region 更新首次检验

                        DataTable QCM_DQA_MAG_M_dt = DB.GetDataTablebyline($@"
SELECT 
	d.ID,
	d.F_INSP_RES
FROM 
QCM_DQA_MAG_M m
INNER JOIN QCM_DQA_MAG_D d ON d.M_ID=m.ID 
WHERE d.ID={id}
");
                        if (QCM_DQA_MAG_M_dt.Rows.Count > 0)
                        {
                            string check_res_res = "Fail";
                            if (databody["check_res"].ToString() == "0")
                            {
                                check_res_res = "Pass";
                            }
                            string QCM_DQA_MAG_M_ID = QCM_DQA_MAG_M_dt.Rows[0]["ID"].ToString();
                            string QCM_DQA_MAG_M_F_INSP_RES = QCM_DQA_MAG_M_dt.Rows[0]["F_INSP_RES"].ToString();
                            if (string.IsNullOrWhiteSpace(QCM_DQA_MAG_M_F_INSP_RES))
                            {
                                string dp_name = DB.GetStringline($@"
SELECT
	a.DEPARTMENT_NAME
FROM
	HR001M m
INNER JOIN BASE005M a ON a.DEPARTMENT_CODE=m.STAFF_DEPARTMENT
WHERE m.STAFF_NO='{user}'
");
                                string QCM_DQA_MAG_M_sql = $@"UPDATE QCM_DQA_MAG_D SET F_INSP_DEP='{dp_name}',F_INSP_DATE='{date}',F_INSP_RES='{check_res_res}' WHERE ID={QCM_DQA_MAG_M_ID}";
                                DB.ExecuteNonQuery(QCM_DQA_MAG_M_sql);
                            }
                        }
                        #endregion
                    }
                    string ddid = SJeMES_Framework_NETCore.DBHelper.DataBase.GetCurId(DB, "qcm_dqa_mag_d_d");
                    string ssqq = string.Empty;
                    if (dataguid.Rows.Count > 0)
                    {
                        foreach (DataRow item in dataguid.Rows)
                        {
                            ssqq += $@"insert into qcm_dqa_mag_d_d_f (union_id,file_guid,createby,createdate,createtime) 
                                    values('{ddid}','{item["guid"]}','{user}','{date}','{time}');";
                        }
                        DB.ExecuteNonQuery($@"BEGIN {ssqq} END;");
                    }
                }
                //sql = $@"";

                //DB.ExecuteNonQuery(sql);
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

        /// <summary>
        /// 量试管理删除
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject DeletePilotRun_Edit(object OBJ)
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
                string lsid = jarr.ContainsKey("lsid") ? jarr["lsid"].ToString() : "";//量试详情表id
                string user = SJeMES_Framework_NETCore.Web.System.GetUserCodeByToken(ReqObj.UserToken);//获取的登陆人信息
                string date = DateTime.Now.ToString("yyyy-MM-dd");//日期
                string time = DateTime.Now.ToString("HH:mm:ss");//时间

                string sql = string.Empty;
                sql = $@"update qcm_ls_list_d set isdelete='1' where id='{lsid}'";
                DB.ExecuteNonQuery(sql);

                sql = $@"update qcm_ls_list_d_d set isdelete='1' where m_id='{lsid}'";
                DB.ExecuteNonQuery(sql);

                sql = $@"select id from qcm_ls_list_d_d where isdelete='0' and m_id='{lsid}'";
                DataTable dtddid = DB.GetDataTable(sql);
                sql = "";
                if (dtddid.Rows.Count > 0)
                {
                    foreach (DataRow item in dtddid.Rows)
                    {
                        sql += $@"update qcm_ls_list_d_f set isdelete='1' where union_id='{item["id"]}';";
                    }
                    DB.ExecuteNonQuery($@"BEGIN {sql} END;");
                }
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

        ///// <summary>
        ///// 量试管理查询dqa附件
        ///// </summary>
        ///// <param name="OBJ"></param>
        ///// <returns></returns>
        //public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetPilotRun_Edit_dqa(object OBJ)
        //{
        //    SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
        //    SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
        //    SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase();
        //    try
        //    {
        //        DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);
        //        string Data = ReqObj.Data.ToString();
        //        DB.Open();
        //        DB.BeginTransaction();
        //        var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
        //        //转译
        //        string dqaid = jarr.ContainsKey("dqaid") ? jarr["dqaid"].ToString() : "";//查询条件 dqa详情id

        //        string sql = string.Empty;
        //        sql = $@"SELECT 
        //                t.file_name,
        //                t.file_url
        //                FROM
        //                qcm_dqa_mag_d_f q
        //                LEFT JOIN BDM_UPLOAD_FILE_ITEM t on q.file_id=t.guid
        //                where q.ISDELETE='0' and q.d_id='{dqaid}'";

        //        DataTable dt = DB.GetDataTable(sql);
        //        Dictionary<string, object> dic = new Dictionary<string, object>();
        //        dic.Add("Data", dt);

        //        ret.RetData1 = dic;
        //        ret.IsSuccess = true;

        //    }
        //    catch (Exception ex)
        //    {
        //        DB.Rollback();
        //        ret.IsSuccess = false;
        //        ret.ErrMsg = ex.Message;
        //    }
        //    finally
        //    {
        //        DB.Close();
        //    }
        //    return ret;
        //}

        /// <summary>
        /// 量试管理查看历史
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetPilotRun_Edit_History(object OBJ)
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
                string ifls = jarr.ContainsKey("ifls") ? jarr["ifls"].ToString() : "";//查询条件 是否量试
                string id = jarr.ContainsKey("id") ? jarr["id"].ToString() : "";//查询条件 dqa或量试详情id
                string keycode = jarr.ContainsKey("keycode") ? jarr["keycode"].ToString() : "";//查询条件
                string pageRow = jarr.ContainsKey("pageRow") ? jarr["pageRow"].ToString() : "";
                string page = jarr.ContainsKey("page") ? jarr["page"].ToString() : "";
                //转译
                string where = string.Empty;
                if (!string.IsNullOrWhiteSpace(keycode))
                {
                    where = $@" and (check_qty like '%{keycode}%' or qty like '%{keycode}%' or bad_descr like '%{keycode}%' or check_res like '%{keycode}%')";
                }
                string sql = string.Empty;
                if (ifls == "true")
                {
                    sql = $@"select id,check_qty,qty,bad_descr,check_res from qcm_ls_list_d_d where isdelete='0' and m_id='{id}' {where} ORDER BY id desc";
                }
                else if (ifls == "false")
                {
                    sql = $@"select id,check_qty,qty,bad_descr,check_res from qcm_dqa_mag_d_d where isdelete='0' and m_id='{id}' {where} ORDER BY id desc";
                }

                DataTable dt = CommonBASE.GetPageDataTable(DB, sql, int.Parse(page), int.Parse(pageRow));
                dt.Columns.Add("file_list", typeof(object));
                foreach (DataRow item in dt.Rows)
                {
                    DataTable fl = new DataTable();
                    fl.Columns.Add("file_name");
                    fl.Columns.Add("file_url");
                    item["file_list"] = fl;
                    if (ifls == "true")
                    {
                        sql = $@"SELECT 
                            t.file_name,
                            t.file_url
                            FROM
                            qcm_ls_list_d_f f
                            LEFT JOIN BDM_UPLOAD_FILE_ITEM t on f.file_guid=t.guid
                            WHERE f.union_id='{item["id"]}'";
                        fl = DB.GetDataTable(sql);
                        if (fl.Rows.Count > 0)
                        {
                            item["file_list"] = fl;
                        }
                    }
                    else if (ifls == "false")
                    {
                        sql = $@"SELECT 
                            t.file_name,
                            t.file_url
                            FROM
                            qcm_dqa_mag_d_d_f f
                            LEFT JOIN BDM_UPLOAD_FILE_ITEM t on f.file_guid=t.guid
                            WHERE f.union_id='{item["id"]}'";
                        fl = DB.GetDataTable(sql);
                        if (fl.Rows.Count > 0)
                        {
                            item["file_list"] = fl;
                        }
                    }
                }
                int rowCount = CommonBASE.GetPageDataTableCount(DB, sql);

                Dictionary<string, object> dic = new Dictionary<string, object>();
                dic.Add("Data", dt);
                dic.Add("rowCount", rowCount);

                ret.RetData1 = dic;
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

        ///// <summary>
        ///// 量试管理查看历史中查看图片
        ///// </summary>
        ///// <param name="OBJ"></param>
        ///// <returns></returns>
        //public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetPilotRun_Edit_History_img(object OBJ)
        //{
        //    SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
        //    SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
        //    SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase();
        //    try
        //    {
        //        DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);
        //        string Data = ReqObj.Data.ToString();
        //        DB.Open();
        //        DB.BeginTransaction();
        //        var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
        //        string ifls = jarr.ContainsKey("ifls") ? jarr["ifls"].ToString() : "";//查询条件 是否量试
        //        string id = jarr.ContainsKey("id") ? jarr["id"].ToString() : "";//查询条件 dqa或量试详情提交id
        //                                                                        //转译
        //        string sql = string.Empty;
        //        if (ifls == "true")
        //        {
        //            sql = $@"SELECT 
        //                    t.file_name,
        //                    t.file_url
        //                    FROM
        //                    qcm_ls_list_d_f f
        //                    LEFT JOIN BDM_UPLOAD_FILE_ITEM t on f.file_guid=t.guid
        //                    WHERE f.union_id='{id}'";
        //        }
        //        else if (ifls == "false")
        //        {
        //            sql = $@"SELECT 
        //                    t.file_name,
        //                    t.file_url
        //                    FROM
        //                    qcm_dqa_mag_d_d_f f
        //                    LEFT JOIN BDM_UPLOAD_FILE_ITEM t on f.file_guid=t.guid
        //                    WHERE f.union_id='{id}'";
        //        }

        //        DataTable dt = DB.GetDataTable(sql);

        //        Dictionary<string, object> dic = new Dictionary<string, object>();
        //        dic.Add("Data", dt);

        //        ret.RetData1 = dic;
        //        ret.IsSuccess = true;

        //    }
        //    catch (Exception ex)
        //    {
        //        DB.Rollback();
        //        ret.IsSuccess = false;
        //        ret.ErrMsg = ex.Message;
        //    }
        //    finally
        //    {
        //        DB.Close();
        //    }
        //    return ret;
        //}
    }
}
