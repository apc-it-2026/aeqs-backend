using SJ_QCMAPI.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace SJeMES_IQC
{
    public class IQC_Customer_Complaint
    {
        /// <summary>
        /// 客户投诉主页查询
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetCustomer_Complaint_Main(object OBJ)
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
                string datestart = jarr.ContainsKey("datestart") ? jarr["datestart"].ToString() : "";//查询条件 日期开始
                string dateend = jarr.ContainsKey("dateend") ? jarr["dateend"].ToString() : "";//查询条件 日期结束
                string shoe_name = jarr.ContainsKey("shoe_name") ? jarr["shoe_name"].ToString() : "";//查询条件 鞋型名称
                string prod_no = jarr.ContainsKey("prod_no") ? jarr["prod_no"].ToString() : "";//查询条件 art
                string PO_ORDER = jarr.ContainsKey("PO_ORDER") ? jarr["PO_ORDER"].ToString() : "";//查询条件 po
                string COUNTRY_REGION = jarr.ContainsKey("COUNTRY_REGION") ? jarr["COUNTRY_REGION"].ToString() : "";//查询条件 国家区域 
                string DEFECT_CONTENT = jarr.ContainsKey("DEFECT_CONTENT") ? jarr["DEFECT_CONTENT"].ToString() : "";//查询条件 问题点
                string DEVELOP_SEASON = jarr.ContainsKey("DEVELOP_SEASON") ? jarr["DEVELOP_SEASON"].ToString() : "";//查询条件 开发季度
                string Category = jarr.ContainsKey("Category") ? jarr["Category"].ToString() : "";//查询条件 Category
                string STATUS = jarr.ContainsKey("STATUS") ? jarr["STATUS"].ToString() : "";//查询条件 状态
                string processing_results_status = jarr.ContainsKey("processing_results_status") ? jarr["processing_results_status"].ToString() : "";//查询条件 处理结果
                string COMPLAINT_NO = jarr.ContainsKey("COMPLAINT_NO") ? jarr["COMPLAINT_NO"].ToString() : "";//查询条件 投诉编号
                string pageSize = jarr.ContainsKey("pageSize") ? jarr["pageSize"].ToString() : "";
                string pageIndex = jarr.ContainsKey("pageIndex") ? jarr["pageIndex"].ToString() : "";

                string where = string.Empty;
                if (!string.IsNullOrWhiteSpace(datestart) && !string.IsNullOrWhiteSpace(dateend))
                {
                    where += $@" and (m.COMPLAINT_DATE between to_date(@datestart,'yyyy-mm-dd,hh24:mi:ss') and to_date(@dateend,'yyyy-mm-dd,hh24:mi:ss'))";
                }
                if (!string.IsNullOrWhiteSpace(shoe_name))
                {
                    where += $@" and r.name_t like @shoe_name";
                }
                if (!string.IsNullOrWhiteSpace(prod_no))
                {
                    where += $@" and t.prod_no like @prod_no";
                }
                if (!string.IsNullOrWhiteSpace(PO_ORDER))
                {
                    where += $@" and m.PO_ORDER like @PO_ORDER";
                }
                if (!string.IsNullOrWhiteSpace(COUNTRY_REGION))
                {
                    where += $@" and m.COUNTRY_REGION like @COUNTRY_REGION";
                }
                if (!string.IsNullOrWhiteSpace(DEFECT_CONTENT))
                {
                    where += $@" and m.DEFECT_CONTENT like @DEFECT_CONTENT";
                }
                if (!string.IsNullOrWhiteSpace(DEVELOP_SEASON))
                {
                    where += $@" and p.DEVELOP_SEASON like @DEVELOP_SEASON";
                }
                if (!string.IsNullOrWhiteSpace(Category))
                {
                    where += $@" and bb.name_t like @Category";
                }
                if (!string.IsNullOrWhiteSpace(STATUS))
                {
                    where += $@" and m.STATUS = @STATUS";
                }
                if (!string.IsNullOrWhiteSpace(processing_results_status))
                {
                    where += $@" and m.processing_results_status = @processing_results_status";
                }
                if (!string.IsNullOrWhiteSpace(COMPLAINT_NO))
                {
                    where += $@" and m.COMPLAINT_NO like @COMPLAINT_NO";
                }

                string sql = string.Empty; 
                sql = $@"select 
                        MAX(m.id) as mid,
                        MAX(m.FOB) as FOB,
                        m.COMPLAINT_NO,
                        MAX(TO_CHAR( m.COMPLAINT_DATE,'yyyy-mm-dd')) as COMPLAINT_DATE,
                        MAX(m.COUNTRY_REGION) as COUNTRY_REGION,
                        MAX(m.PO_ORDER) as PO_ORDER,
                        MAX(t.SE_QTY) as ts_posl,
                        MAX(m.DEFECT_CONTENT) as DEFECT_CONTENT,
                        MAX(m.NG_QTY) as NG_QTY,
                        MAX(m.COMPLAINT_MONEY) as COMPLAINT_MONEY,
                        MAX(m.STATUS) as STATUS,
                        MAX(m.processing_results_status) as processing_results_status,
                        MAX(p.DEVELOP_SEASON) as DEVELOP_SEASON,
                        MAX(bb.name_t) as Category,
                        MAX(p.user_section) as user_section,
                        MAX(p.PRODUCT_MONTH) as PRODUCT_MONTH,
                        MAX(t.prod_no) as prod_no,
                        MAX(p.name_t) as prod_name,
                        MAX(t.shoe_no) as shoe_no,
                        MAX(r.name_t) as shoe_name,
                        MAX(p.COLOR_WAY) as Material_Way
                        from 
                        QCM_CUSTOMER_COMPLAINT_M m
                        LEFT JOIN BDM_SE_ORDER_MASTER b on m.PO_ORDER=b.mer_po
                        LEFT JOIN BDM_SE_ORDER_ITEM t on b.se_id=t.se_id
                        LEFT JOIN BDM_RD_PROD p on t.prod_no =p.prod_no
                        LEFT JOIN BDM_RD_STYLE r on t.shoe_no=r.shoe_no
                        LEFT JOIN bdm_cd_code bb ON r.style_seq=bb.code_no
                        where 1=1 {where} 
                        GROUP BY m.COMPLAINT_NO
                        order by max(m.id) desc";
                Dictionary<string, object> paramTestDic = new Dictionary<string, object>();
                paramTestDic.Add("datestart", $@"{datestart}");
                paramTestDic.Add("dateend", $@"{dateend}");
                paramTestDic.Add("shoe_name", $@"%{shoe_name}%");
                paramTestDic.Add("prod_no", $@"%{prod_no}%");
                paramTestDic.Add("PO_ORDER", $@"%{PO_ORDER}%");
                paramTestDic.Add("COUNTRY_REGION", $@"%{COUNTRY_REGION}%");
                paramTestDic.Add("DEFECT_CONTENT", $@"%{DEFECT_CONTENT}%");
                paramTestDic.Add("DEVELOP_SEASON", $@"%{DEVELOP_SEASON}%");
                paramTestDic.Add("Category", $@"%{Category}%");
                paramTestDic.Add("STATUS", $@"{STATUS}");
                paramTestDic.Add("processing_results_status", $@"{processing_results_status}");
                paramTestDic.Add("COMPLAINT_NO", $@"%{COMPLAINT_NO}%");
                DataTable dt = CommonBASE.GetPageDataTable(DB, sql, int.Parse(pageIndex), int.Parse(pageSize), "", paramTestDic);
                int rowCount = CommonBASE.GetPageDataTableCount(DB, sql, paramTestDic);



                Dictionary<string, object> dic = new Dictionary<string, object>();
                dic.Add("Data", dt);
                dic.Add("rowCount", rowCount);

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
        /// 客户投诉根据po查询数据
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetCustomer_Complaint_Edit_PO(object OBJ)
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
                string PO = jarr.ContainsKey("PO") ? jarr["PO"].ToString() : "";//查询条件 PO号
                //转译
                string sql = $@"select
                                p.DEVELOP_SEASON,
                                p.PRODUCT_MONTH,
                                p.COLOR_WAY as Material_Way,
                                p.prod_no,
                                r.name_t as shoe_name,
                                '' as cx,
                                t.SE_QTY
                                from
                                BDM_SE_ORDER_MASTER m
                                LEFT JOIN BDM_SE_ORDER_ITEM t on m.se_id=t.se_id
                                LEFT JOIN BDM_RD_PROD p on t.prod_no =p.prod_no
                                LEFT JOIN BDM_RD_STYLE r on t.shoe_no=r.shoe_no
                                where m.mer_po=@mer_po";
                Dictionary<string, object> dicWhere = new Dictionary<string, object>();
                dicWhere.Add("mer_po", $@"{PO}");
                DataTable dt = DB.GetDataTable(sql, dicWhere);
                Dictionary<string, object> poDicWhere = new Dictionary<string, object>();
                poDicWhere.Add("mer_po", $@"{PO}");
                sql = $@"select distinct from_line from mms_finishedtrackin_list where po = @mer_po";
                //                        sql = $@"SELECT DISTINCT
                //	d.DEPARTMENT_NAME AS DEPARTMENT_NAME
                //FROM
                //    BDM_SE_ORDER_MASTER A
                //    INNER JOIN mes010m b ON a.MER_PO = b.udf03
                //    LEFT JOIN mes010a1 c ON b.production_order = c.production_order
                //    AND b.org = c.org
                //    LEFT JOIN base005m d ON d.DEP_SAP = c.work_center
                //    AND d.factory_sap = b.org
                //WHERE
                //    c.procedure_no = 'L'
                //    AND a.MER_PO = '{PO_ORDER}'";
                DataTable poDt = DB.GetDataTable(sql, poDicWhere);
                string DEPARTMENT_NAME = string.Empty;
                foreach (DataRow item in poDt.Rows)
                {
                    DEPARTMENT_NAME += item["from_line"].ToString() + ";";
                }
                if (dt.Rows.Count > 0)
                    dt.Rows[0]["cx"] = DEPARTMENT_NAME.TrimEnd(';');
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

        /// <summary>
        /// 客户投诉编辑页面查询图片
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject Getimage_guid(object OBJ)
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
                string image_guid = jarr.ContainsKey("image_guid") ? jarr["image_guid"].ToString() : "";//查询条件 图片guid
                //string pageSize = jarr.ContainsKey("pageSize") ? jarr["pageSize"].ToString() : "";
                //string pageIndex = jarr.ContainsKey("pageIndex") ? jarr["pageIndex"].ToString() : "";

                string where = string.Empty;
                string sql = $@"SELECT
	                            file_name,
	                            file_url,
	                            'QCM_CUSTOMER_COMPLAINT_M_F' AS tablename,
	                            f.id,
	                            GUID 
                            FROM
	                            BDM_UPLOAD_FILE_ITEM t 
	                            LEFT JOIN QCM_CUSTOMER_COMPLAINT_M_F f on f.file_guid= t.guid
                                where  t.guid in('{image_guid.Replace(",", "','")}')";
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

        /// <summary>
        /// 客户投诉编辑
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject EditCustomer_Complaint_Edit(object OBJ)
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
                string mid = jarr.ContainsKey("mid") ? jarr["mid"].ToString() : "";//查询条件 主表id
                string COMPLAINT_NO = jarr.ContainsKey("COMPLAINT_NO") ? jarr["COMPLAINT_NO"].ToString() : "";//查询条件 投诉编号
                string COMPLAINT_DATE = jarr.ContainsKey("COMPLAINT_DATE") ? jarr["COMPLAINT_DATE"].ToString() : "";//查询条件 投诉日期
                string COUNTRY_REGION = jarr.ContainsKey("COUNTRY_REGION") ? jarr["COUNTRY_REGION"].ToString() : "";//查询条件 国家区域
                string PO_ORDER = jarr.ContainsKey("PO_ORDER") ? jarr["PO_ORDER"].ToString() : "";//查询条件 投诉PO单号
                string NG_QTY = jarr.ContainsKey("NG_QTY") ? jarr["NG_QTY"].ToString() : "";//查询条件 不良数量
                string COMPLAINT_MONEY = jarr.ContainsKey("COMPLAINT_MONEY") ? jarr["COMPLAINT_MONEY"].ToString() : "";//查询条件 投诉金额
                string DEFECT_CONTENT = jarr.ContainsKey("DEFECT_CONTENT") ? jarr["DEFECT_CONTENT"].ToString() : "";//查询条件 问题点
                string FOB = jarr.ContainsKey("FOB") ? jarr["FOB"].ToString() : "";//查询条件 FOB
                string imglist = jarr.ContainsKey("imglist") ? jarr["imglist"].ToString() : "";//查询条件 图片集
                string user = SJeMES_Framework_NETCore.Web.System.GetUserCodeByToken(ReqObj.UserToken);//获取的登陆人信息
                string date = DateTime.Now.ToString("yyyy-MM-dd");//日期
                string time = DateTime.Now.ToString("HH:mm:ss");//时间
                //string pageSize = jarr.ContainsKey("pageSize") ? jarr["pageSize"].ToString() : "";
                //string pageIndex = jarr.ContainsKey("pageIndex") ? jarr["pageIndex"].ToString() : "";

                //int count = DB.GetInt32($@"select count(1) from BDM_SE_ORDER_MASTER where mer_po='{PO_ORDER}'");
                //if (count <= 0)
                //{
                //    ret.IsSuccess = false;
                //    ret.ErrMsg = "未找到PO数据!";
                //    return ret;
                //}

                string sql = string.Empty;
                string sqlimg = string.Empty;
                if (string.IsNullOrWhiteSpace(mid))
                {
                    int countno = DB.GetInt32($@"select count(1) from QCM_CUSTOMER_COMPLAINT_M where COMPLAINT_NO='{COMPLAINT_NO}'");
                    if (countno > 0)
                    {
                        ret.IsSuccess = false;
                       // ret.ErrMsg = "投诉编号不能重复!";
                        ret.ErrMsg = "Complaint number cannot be repeated!";
                        return ret;
                    }
                    sql = $@"insert into QCM_CUSTOMER_COMPLAINT_M (COMPLAINT_NO,COMPLAINT_DATE,COUNTRY_REGION,PO_ORDER,NG_QTY,COMPLAINT_MONEY,
                            DEFECT_CONTENT,FOB,CREATEBY,CREATEDATE,CREATETIME) 
                            values('{COMPLAINT_NO}',to_date('{COMPLAINT_DATE}','yyyy-mm-dd hh24:mi:ss'),'{COUNTRY_REGION}','{PO_ORDER}','{NG_QTY}','{COMPLAINT_MONEY}','{DEFECT_CONTENT}','{FOB}','{user}',
                            '{date}','{time}')";
                    DB.ExecuteNonQuery(sql);

                    string[] imgs = imglist.Split(',');
                    for (int i = 0; i < imgs.Length; i++)
                    {
                        if (!string.IsNullOrWhiteSpace(imgs[i]))
                            sqlimg += $@"insert into QCM_CUSTOMER_COMPLAINT_M_F (COMPLAINT_NO,file_type,file_guid,CREATEBY,CREATEDATE,CREATETIME) 
                                    values('{COMPLAINT_NO}','0','{imgs[i]}','{user}','{date}','{time}');";
                    }
                    if (!string.IsNullOrWhiteSpace(sqlimg))
                        DB.ExecuteNonQuery($@"BEGIN {sqlimg} END;");
                    sqlimg = string.Empty;
                }
                else
                {
                    sql = $@"update QCM_CUSTOMER_COMPLAINT_M set COMPLAINT_DATE=to_date('{COMPLAINT_DATE}','yyyy-mm-dd hh24:mi:ss'),COUNTRY_REGION='{COUNTRY_REGION}',
                            PO_ORDER='{PO_ORDER}',NG_QTY='{NG_QTY}',COMPLAINT_MONEY='{COMPLAINT_MONEY}',DEFECT_CONTENT='{DEFECT_CONTENT}',FOB='{FOB}',MODIFYBY='{user}',
                            MODIFYDATE='{date}',MODIFYTIME='{time}' where id='{mid}'";
                    DB.ExecuteNonQuery(sql);

                    DB.ExecuteNonQuery($@"delete from QCM_CUSTOMER_COMPLAINT_M_F where COMPLAINT_NO='{COMPLAINT_NO}' and file_type='0'");
                    string[] imgs = imglist.Split(',');
                    for (int i = 0; i < imgs.Length; i++)
                    {
                        if (!string.IsNullOrWhiteSpace(imgs[i]))
                            sqlimg += $@"insert into QCM_CUSTOMER_COMPLAINT_M_F (COMPLAINT_NO,file_type,file_guid,CREATEBY,CREATEDATE,CREATETIME) 
                                    values('{COMPLAINT_NO}','0','{imgs[i]}','{user}','{date}','{time}');";
                    }
                    if (!string.IsNullOrWhiteSpace(sqlimg))
                        DB.ExecuteNonQuery($@"BEGIN {sqlimg} END;");
                    sqlimg = string.Empty;
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

        /// <summary>
        /// 客户投诉编辑修改时查询
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetCustomer_Complaint_Edit(object OBJ)
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
                string mid = jarr.ContainsKey("mid") ? jarr["mid"].ToString() : "";//查询条件 主表id

                string COMPLAINT_NO = DB.GetString($@"select COMPLAINT_NO from QCM_CUSTOMER_COMPLAINT_M where id='{mid}'");

                //转译
                string sql = $@"select
                                q.COMPLAINT_NO,
                                q.COMPLAINT_DATE,
                                q.COUNTRY_REGION,
                                q.PO_ORDER,
                                q.NG_QTY,
                                q.COMPLAINT_MONEY,
                                q.DEFECT_CONTENT,
                                p.DEVELOP_SEASON,
                                p.PRODUCT_MONTH,
                                p.COLOR_WAY as Material_Way,
                                p.prod_no,
                                q.FOB,
                                r.name_t as shoe_name,
                                '' as cx,
                                t.SE_QTY,
                                '' as imglist
                                from
                                QCM_CUSTOMER_COMPLAINT_M q
                                LEFT JOIN BDM_SE_ORDER_MASTER m on q.PO_ORDER=m.mer_po
                                LEFT JOIN BDM_SE_ORDER_ITEM t on m.se_id=t.se_id
                                LEFT JOIN BDM_RD_PROD p on t.prod_no =p.prod_no
                                LEFT JOIN BDM_RD_STYLE r on t.shoe_no=r.shoe_no
                                where q.id=@mid";
                Dictionary<string, object> dicWhere = new Dictionary<string, object>();
                dicWhere.Add("mid", $@"{mid}");
                DataTable dt = DB.GetDataTable(sql, dicWhere);
                foreach (DataRow item in dt.Rows)
                {
                    DataTable dtimgs = DB.GetDataTable($@"select file_guid from QCM_CUSTOMER_COMPLAINT_M_F where file_type='0' and COMPLAINT_NO='{COMPLAINT_NO}' ");
                    for (int i = 0; i < dtimgs.Rows.Count; i++)
                    {
                        item["imglist"] += dtimgs.Rows[i]["file_guid"].ToString() + ',';
                    }
                    item["imglist"] = item["imglist"].ToString().TrimEnd(',');
                }

                if (dt.Rows.Count > 0)
                {
                    var PO_ORDER = dt.Rows[0]["PO_ORDER"].ToString();
                    if (!string.IsNullOrEmpty(PO_ORDER))
                    {
                        Dictionary<string, object> poDicWhere = new Dictionary<string, object>();
                        poDicWhere.Add("mer_po", $@"{PO_ORDER}");
                        sql = $@"select distinct from_line from mms_finishedtrackin_list where po = @mer_po";
                        //                        sql = $@"SELECT DISTINCT
                        //	d.DEPARTMENT_NAME AS DEPARTMENT_NAME
                        //FROM
                        //    BDM_SE_ORDER_MASTER A
                        //    INNER JOIN mes010m b ON a.MER_PO = b.udf03
                        //    LEFT JOIN mes010a1 c ON b.production_order = c.production_order
                        //    AND b.org = c.org
                        //    LEFT JOIN base005m d ON d.DEP_SAP = c.work_center
                        //    AND d.factory_sap = b.org
                        //WHERE
                        //    c.procedure_no = 'L'
                        //    AND a.MER_PO = '{PO_ORDER}'";
                        DataTable poDt = DB.GetDataTable(sql, poDicWhere);
                        string DEPARTMENT_NAME = string.Empty;
                        foreach (DataRow item in poDt.Rows)
                        {
                            DEPARTMENT_NAME += item["from_line"].ToString() + ";";
                        }
                        if (dt.Rows.Count > 0)
                            dt.Rows[0]["cx"] = DEPARTMENT_NAME.TrimEnd(';');
                    }
                }

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

        /// <summary>
        /// 客户投诉删除
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject DeleteCustomer_Complaint_Main(object OBJ)
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
                string mid = jarr.ContainsKey("mid") ? jarr["mid"].ToString() : "";//查询条件 主表id
                //string user = SJeMES_Framework_NETCore.Web.System.GetUserCodeByToken(ReqObj.UserToken);//获取的登陆人信息
                //string date = DateTime.Now.ToString("yyyy-MM-dd");//日期
                //string time = DateTime.Now.ToString("HH:mm:ss");//时间
                //string pageSize = jarr.ContainsKey("pageSize") ? jarr["pageSize"].ToString() : "";
                //string pageIndex = jarr.ContainsKey("pageIndex") ? jarr["pageIndex"].ToString() : "";

                string COMPLAINT_NO = DB.GetString($@"select COMPLAINT_NO from QCM_CUSTOMER_COMPLAINT_M where id='{mid}'");

                DB.ExecuteNonQuery($@"delete from QCM_CUSTOMER_COMPLAINT_M where COMPLAINT_NO='{COMPLAINT_NO}'");
                DB.ExecuteNonQuery($@"delete from QCM_CUSTOMER_COMPLAINT_M_F where COMPLAINT_NO='{COMPLAINT_NO}'");
                DB.ExecuteNonQuery($@"delete from QCM_CUSTOMER_COMPLAINT_M_C where COMPLAINT_NO='{COMPLAINT_NO}'");

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
        /// 客户投诉处理时查询
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetCustomer_Complaint_Dispose(object OBJ)
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
                string COMPLAINT_NO = jarr.ContainsKey("COMPLAINT_NO") ? jarr["COMPLAINT_NO"].ToString() : "";//查询条件 投诉编号

                string sql = string.Empty;

                //转译

                #region 投诉信息
                sql = $@"select
                                m.COMPLAINT_NO,
                                m.COMPLAINT_DATE,
                                m.PO_ORDER,
                                m.COUNTRY_REGION,
                                m.NG_QTY,
                                m.COMPLAINT_MONEY,
                                m.DEFECT_CONTENT,
                                m.analysis,
                                m.FOB,
                                m.liability_determination,
                                m.improvement_measures,
                                m.processing_results,
                                m.processing_results_status,
                                '' as imglist,
                                '' as fxfile,
                                '' as zrpbfile,
                                '' as gscsfile,
                                i.SE_QTY,
                                s.SE_YEAR,
                                '' as cx
                                from
                                QCM_CUSTOMER_COMPLAINT_M m
                                LEFT JOIN BDM_SE_ORDER_MASTER s on m.PO_ORDER=s.mer_po
                                LEFT JOIN BDM_SE_ORDER_ITEM i on s.se_id=i.se_id
                                where m.COMPLAINT_NO=@COMPLAINT_NO";
                Dictionary<string, object> dicWhere = new Dictionary<string, object>();
                dicWhere.Add("COMPLAINT_NO", $@"{COMPLAINT_NO}");
                DataTable dtTous = DB.GetDataTable(sql, dicWhere);
                foreach (DataRow item in dtTous.Rows)
                {
                    DataTable dtimgs = DB.GetDataTable($@"select file_guid from QCM_CUSTOMER_COMPLAINT_M_F where file_type='0' and COMPLAINT_NO='{COMPLAINT_NO}' ");
                    for (int i = 0; i < dtimgs.Rows.Count; i++)
                    {
                        item["imglist"] += dtimgs.Rows[i]["file_guid"].ToString() + ',';
                    }
                    item["imglist"] = item["imglist"].ToString().TrimEnd(',');

                    DataTable fxfile = DB.GetDataTable($@"select file_guid from QCM_CUSTOMER_COMPLAINT_M_F where file_type='1' and COMPLAINT_NO='{COMPLAINT_NO}' ");
                    for (int i = 0; i < fxfile.Rows.Count; i++)
                    {
                        item["fxfile"] += fxfile.Rows[i]["file_guid"].ToString() + ',';
                    }
                    item["fxfile"] = item["fxfile"].ToString().TrimEnd(',');

                    DataTable zrpbfile = DB.GetDataTable($@"select file_guid from QCM_CUSTOMER_COMPLAINT_M_F where file_type='2' and COMPLAINT_NO='{COMPLAINT_NO}' ");
                    for (int i = 0; i < zrpbfile.Rows.Count; i++)
                    {
                        item["zrpbfile"] += zrpbfile.Rows[i]["file_guid"].ToString() + ',';
                    }
                    item["zrpbfile"] = item["zrpbfile"].ToString().TrimEnd(',');

                    DataTable gscsfile = DB.GetDataTable($@"select file_guid from QCM_CUSTOMER_COMPLAINT_M_F where file_type='3' and COMPLAINT_NO='{COMPLAINT_NO}' ");
                    for (int i = 0; i < gscsfile.Rows.Count; i++)
                    {
                        item["gscsfile"] += gscsfile.Rows[i]["file_guid"].ToString() + ',';
                    }
                    item["gscsfile"] = item["gscsfile"].ToString().TrimEnd(',');
                }
                if (dtTous.Rows.Count > 0)
                {
                    var PO_ORDER = dtTous.Rows[0]["PO_ORDER"].ToString();
                    if (!string.IsNullOrEmpty(PO_ORDER))
                    {
                        Dictionary<string, object> poDicWhere = new Dictionary<string, object>();
                        poDicWhere.Add("mer_po", $@"{PO_ORDER}");
                        sql = $@"select distinct from_line from mms_finishedtrackin_list where po = @mer_po";
//                        sql = $@"SELECT DISTINCT
//	d.DEPARTMENT_NAME AS DEPARTMENT_NAME
//FROM
//    BDM_SE_ORDER_MASTER A
//    INNER JOIN mes010m b ON a.MER_PO = b.udf03
//    LEFT JOIN mes010a1 c ON b.production_order = c.production_order
//    AND b.org = c.org
//    LEFT JOIN base005m d ON d.DEP_SAP = c.work_center
//    AND d.factory_sap = b.org
//WHERE
//    c.procedure_no = 'L'
//    AND a.MER_PO = '{PO_ORDER}'";
                        DataTable poDt = DB.GetDataTable(sql, poDicWhere);
                        string DEPARTMENT_NAME = string.Empty;
                        foreach (DataRow item in poDt.Rows)
                        {
                            DEPARTMENT_NAME += item["from_line"].ToString() + ";";
                        }
                        if (dtTous.Rows.Count > 0)
                            dtTous.Rows[0]["cx"] = DEPARTMENT_NAME.TrimEnd(';');
                    }
                }
                #endregion

                #region 鞋型信息
                sql = $@"SELECT
	                            (
                                select {CommonBASE.GetGroupConcatByOracleVersion(DB, "DISTINCT g.PROD_NO", "g.PROD_NO")}
                                from bdm_rd_prod g where g.SHOE_NO=b.SHOE_NO
	                            ) as PROD_NO,
	                            b.SHOE_NO,
	                            MAX (b.DEVELOP_SEASON) AS DEVELOP_SEASON,
                                MAX(T.file_url) AS file_url,
	                            MAX(bb.name_t) as rule_no,
								MAX(b.user_section) as user_section,
                                MAX(e.qa_principal) as qa_principal,
						        MAX(d.NAME_T) as name_t,
                                MAX(b.cwa_date) as cwa_date,
                                MAX(b.COLOR_WAY) as Material_Way
                            FROM
                                QCM_CUSTOMER_COMPLAINT_M m
                                LEFT JOIN BDM_SE_ORDER_MASTER se on m.PO_ORDER=se.mer_po
                                LEFT JOIN BDM_SE_ORDER_ITEM it on se.se_id=it.se_id
		                        LEFT JOIN bdm_rd_prod b on it.prod_no=b.prod_no
                                LEFT JOIN BDM_RD_STYLE d on b.SHOE_NO=d.SHOE_NO
                                LEFT JOIN qcm_shoes_qa_record_m q ON b.shoe_no = q.shoes_code
                                LEFT JOIN BDM_UPLOAD_FILE_ITEM T ON q.image_guid = T .guid
                                LEFT JOIN qcm_dqa_mag_m f ON b.SHOE_NO = f.shoes_code  AND f.isdelete='0' 
                                LEFT JOIN bdm_rd_style aa ON b.shoe_no=aa.shoe_no
                                LEFT JOIN bdm_cd_code bb ON aa.style_seq=bb.code_no
                                LEFT JOIN bdm_shoe_extend_m e on e.shoe_no=b.SHOE_NO
                                WHERE
	                            1 = 1  AND m.COMPLAINT_NO=@COMPLAINT_NO
                            GROUP BY
	                            b.SHOE_NO";
                Dictionary<string, object> dicxx = new Dictionary<string, object>();
                dicxx.Add("COMPLAINT_NO", $@"{COMPLAINT_NO}");
                DataTable dtXiex = DB.GetDataTable(sql, dicxx);
                #endregion

                #region 品质成本
                sql = $@"select 
						qa_cost_cate_no,
						qa_cost_cate_name,
						ref_unit_price,
						qa_cost_cate_u,
						act_unit_price,
						quantity
						from
						QCM_CUSTOMER_COMPLAINT_M_C where COMPLAINT_NO=@COMPLAINT_NO";
                Dictionary<string, object> dicpz = new Dictionary<string, object>();
                dicpz.Add("COMPLAINT_NO", $@"{COMPLAINT_NO}");
                DataTable dtPinz = DB.GetDataTable(sql, dicpz);
                #endregion


                Dictionary<string, object> dic = new Dictionary<string, object>();
                dic.Add("Data1", dtTous);//投诉信息
                dic.Add("Data2", dtXiex);//鞋型信息
                dic.Add("Data3", dtPinz);//品质成本

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
        /// 客户投诉处理页查询文件
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetCustomer_Complaint_DisposeFile(object OBJ)
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
                string SHOE_NO = jarr.ContainsKey("SHOE_NO") ? jarr["SHOE_NO"].ToString() : "";//查询条件 鞋型

                string where = string.Empty;
                string sql = $@"select f.file_url,f.file_name,'qcm_shoes_qa_record_file_m' as tablename,m.id from BDM_UPLOAD_FILE_ITEM f INNER JOIN qcm_shoes_qa_record_file_m m on f.guid=m.file_id where m.shoes_code=@SHOE_NO";
                Dictionary<string, object> paramTestDic = new Dictionary<string, object>();
                paramTestDic.Add("SHOE_NO", $@"{SHOE_NO}");
                DataTable dt = DB.GetDataTable(sql, paramTestDic);

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

        /// <summary>
        /// 客户投诉处理页面保存(分析,责任判定,改善措施)
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject EditCustomer_Complaint_Dispose(object OBJ)
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
                string COMPLAINT_NO = jarr.ContainsKey("COMPLAINT_NO") ? jarr["COMPLAINT_NO"].ToString() : "";//查询条件 投诉编号
                string file_type = jarr.ContainsKey("file_type") ? jarr["file_type"].ToString() : "";//查询条件 文件类型
                string ali_remarks = jarr.ContainsKey("ali_remarks") ? jarr["ali_remarks"].ToString() : "";//查询条件 分析/责任判定/改善措施的信息
                string ali_img = jarr.ContainsKey("ali_img") ? jarr["ali_img"].ToString() : "";//查询条件 图片guid集
                string user = SJeMES_Framework_NETCore.Web.System.GetUserCodeByToken(ReqObj.UserToken);//获取的登陆人信息
                string date = DateTime.Now.ToString("yyyy-MM-dd");//日期
                string time = DateTime.Now.ToString("HH:mm:ss");//时间
                //string pageSize = jarr.ContainsKey("pageSize") ? jarr["pageSize"].ToString() : "";
                //string pageIndex = jarr.ContainsKey("pageIndex") ? jarr["pageIndex"].ToString() : "";
                string sql = string.Empty;
                string sqlimg = string.Empty;
                switch (file_type)
                {
                    case "1"://分析
                        sql = $@"update QCM_CUSTOMER_COMPLAINT_M set analysis='{ali_remarks}',MODIFYBY='{user}',MODIFYDATE='{date}',MODIFYTIME='{time}'
                                where COMPLAINT_NO='{COMPLAINT_NO}'";
                        DB.ExecuteNonQuery(sql);
                        string[] fx_imgs = ali_img.Split(',');
                        DB.ExecuteNonQuery($@"delete from QCM_CUSTOMER_COMPLAINT_M_F where COMPLAINT_NO='{COMPLAINT_NO}' and file_type='1'");
                        for (int i = 0; i < fx_imgs.Length; i++)
                        {
                            if (!string.IsNullOrWhiteSpace(fx_imgs[i]))
                                sqlimg += $@"insert into QCM_CUSTOMER_COMPLAINT_M_F (COMPLAINT_NO,file_type,file_guid,createby,createdate,createtime) 
                                        values('{COMPLAINT_NO}','1','{fx_imgs[i]}','{user}','{date}','{time}');";
                        }
                        if (!string.IsNullOrWhiteSpace(sqlimg))
                            DB.ExecuteNonQuery($@"BEGIN {sqlimg} END;");
                        sqlimg = string.Empty;
                        break;
                    case "2"://责任判定
                        sql = $@"update QCM_CUSTOMER_COMPLAINT_M set liability_determination='{ali_remarks}',MODIFYBY='{user}',MODIFYDATE='{date}',MODIFYTIME='{time}' 
                                where COMPLAINT_NO='{COMPLAINT_NO}'";
                        DB.ExecuteNonQuery(sql);
                        string[] zrpd_imgs = ali_img.Split(',');
                        DB.ExecuteNonQuery($@"delete from QCM_CUSTOMER_COMPLAINT_M_F where COMPLAINT_NO='{COMPLAINT_NO}' and file_type='2'");
                        for (int i = 0; i < zrpd_imgs.Length; i++)
                        {
                            if (!string.IsNullOrWhiteSpace(zrpd_imgs[i]))
                                sqlimg += $@"insert into QCM_CUSTOMER_COMPLAINT_M_F (COMPLAINT_NO,file_type,file_guid,createby,createdate,createtime) 
                                        values('{COMPLAINT_NO}','2','{zrpd_imgs[i]}','{user}','{date}','{time}');";
                        }
                        if (!string.IsNullOrWhiteSpace(sqlimg))
                            DB.ExecuteNonQuery($@"BEGIN {sqlimg} END;");
                        sqlimg = string.Empty;
                        break;
                    case "3"://责任判定
                        sql = $@"update QCM_CUSTOMER_COMPLAINT_M set improvement_measures='{ali_remarks}',MODIFYBY='{user}',MODIFYDATE='{date}',MODIFYTIME='{time}' 
                                where COMPLAINT_NO='{COMPLAINT_NO}'";
                        DB.ExecuteNonQuery(sql);
                        string[] gscs_imgs = ali_img.Split(',');
                        DB.ExecuteNonQuery($@"delete from QCM_CUSTOMER_COMPLAINT_M_F where COMPLAINT_NO='{COMPLAINT_NO}' and file_type='3'");
                        for (int i = 0; i < gscs_imgs.Length; i++)
                        {
                            if (!string.IsNullOrWhiteSpace(gscs_imgs[i]))
                                sqlimg += $@"insert into QCM_CUSTOMER_COMPLAINT_M_F (COMPLAINT_NO,file_type,file_guid,createby,createdate,createtime) 
                                        values('{COMPLAINT_NO}','3','{gscs_imgs[i]}','{user}','{date}','{time}');";
                        }
                        if (!string.IsNullOrWhiteSpace(sqlimg))
                            DB.ExecuteNonQuery($@"BEGIN {sqlimg} END;");
                        sqlimg = string.Empty;
                        break;
                    default:
                        break;
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

        /// <summary>
        /// 客户投诉处理页面改善结果保存
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject EditCustomer_Complaint_Dispose_gsjg(object OBJ)
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
                string COMPLAINT_NO = jarr.ContainsKey("COMPLAINT_NO") ? jarr["COMPLAINT_NO"].ToString() : "";//查询条件 投诉编号
                string processing_results = jarr.ContainsKey("processing_results") ? jarr["processing_results"].ToString() : "";//查询条件 处理结果
                string processing_results_status = jarr.ContainsKey("processing_results_status") ? jarr["processing_results_status"].ToString() : "";//查询条件 处理结果状态 
                string user = SJeMES_Framework_NETCore.Web.System.GetUserCodeByToken(ReqObj.UserToken);//获取的登陆人信息
                string date = DateTime.Now.ToString("yyyy-MM-dd");//日期
                string time = DateTime.Now.ToString("HH:mm:ss");//时间
                //string pageSize = jarr.ContainsKey("pageSize") ? jarr["pageSize"].ToString() : "";
                //string pageIndex = jarr.ContainsKey("pageIndex") ? jarr["pageIndex"].ToString() : "";
                string sql = string.Empty;

                sql = $@"update QCM_CUSTOMER_COMPLAINT_M set processing_results='{processing_results}',processing_results_status='{processing_results_status}',
                         MODIFYBY='{user}',MODIFYDATE='{date}',MODIFYTIME='{time}' where COMPLAINT_NO='{COMPLAINT_NO}'";
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
        /// 客户投诉处理页面查询异常成品类别
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetCustomer_Complaint_Dispose_qa_cost_cate(object OBJ)
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
                string sql = $@"select qa_cost_cate_no as code,qa_cost_cate_name as value from bdm_qa_cost_cate_m";
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

        /// <summary>
        /// 客户投诉处理页面根据异常成品类别编号查询
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetCustomer_Complaint_Dispose_qa_cost_cate_no(object OBJ)
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
                string qa_cost_cate_no = jarr.ContainsKey("qa_cost_cate_no") ? jarr["qa_cost_cate_no"].ToString() : "";//查询条件 异常成品类别编号
                //转译
                string sql = $@"select qa_cost_cate_no,qa_cost_cate_name,unit_price,qa_cost_cate_u from bdm_qa_cost_cate_m where qa_cost_cate_no='{qa_cost_cate_no}'";
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

        /// <summary>
        /// 客户投诉处理页面品质成本保存
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject EditCustomer_Complaint_Dispose_pzcb(object OBJ)
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
                string COMPLAINT_NO = jarr.ContainsKey("COMPLAINT_NO") ? jarr["COMPLAINT_NO"].ToString() : "";//查询条件 投诉编号
                DataTable CUSTOMER_COMPLAINT_M_C = jarr.ContainsKey("CUSTOMER_COMPLAINT_M_C") ? Newtonsoft.Json.JsonConvert.DeserializeObject<DataTable>(jarr["CUSTOMER_COMPLAINT_M_C"].ToString()) : null;//查询条件 客户投诉——品质成本类别
                string user = SJeMES_Framework_NETCore.Web.System.GetUserCodeByToken(ReqObj.UserToken);//获取的登陆人信息
                string date = DateTime.Now.ToString("yyyy-MM-dd");//日期
                string time = DateTime.Now.ToString("HH:mm:ss");//时间
                //string pageSize = jarr.ContainsKey("pageSize") ? jarr["pageSize"].ToString() : "";
                //string pageIndex = jarr.ContainsKey("pageIndex") ? jarr["pageIndex"].ToString() : "";
                string sql = string.Empty;

                DB.ExecuteNonQuery($@"delete from QCM_CUSTOMER_COMPLAINT_M_C where COMPLAINT_NO='{COMPLAINT_NO}'");
                foreach (DataRow item in CUSTOMER_COMPLAINT_M_C.Rows)
                {
                    if (!string.IsNullOrWhiteSpace(item["异常成本类别编号"].ToString()))
                        sql += $@"insert into QCM_CUSTOMER_COMPLAINT_M_C (COMPLAINT_NO,qa_cost_cate_no,qa_cost_cate_name,ref_unit_price,qa_cost_cate_u,
                                act_unit_price,quantity,createby,createdate,createtime) 
                              values('{COMPLAINT_NO}','{item["异常成本类别编号"]}','{item["异常成本类别"]}','{item["参考单价"]}','{item["单位"]}',
                                '{item["实际单价"]}','{item["数量"]}','{user}','{date}','{time}');";
                }
                if (!string.IsNullOrWhiteSpace(sql))
                    DB.ExecuteNonQuery($@"BEGIN {sql} END;");

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
        /// 客户投诉处理页面结案
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject EditCustomer_Complaint_Dispose_ja(object OBJ)
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
                string COMPLAINT_NO = jarr.ContainsKey("COMPLAINT_NO") ? jarr["COMPLAINT_NO"].ToString() : "";//查询条件 投诉编号
                string user = SJeMES_Framework_NETCore.Web.System.GetUserCodeByToken(ReqObj.UserToken);//获取的登陆人信息
                string date = DateTime.Now.ToString("yyyy-MM-dd");//日期
                string time = DateTime.Now.ToString("HH:mm:ss");//时间
                //string pageSize = jarr.ContainsKey("pageSize") ? jarr["pageSize"].ToString() : "";
                //string pageIndex = jarr.ContainsKey("pageIndex") ? jarr["pageIndex"].ToString() : "";
                string sql = string.Empty;

                sql = $@"update QCM_CUSTOMER_COMPLAINT_M set STATUS='1',MODIFYBY='{user}',MODIFYDATE='{date}',MODIFYTIME='{time}' where COMPLAINT_NO='{COMPLAINT_NO}'";
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
        /// 客户投诉主页导出
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetCustomer_Complaint_Main_dc(object OBJ)
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
                string datestart = jarr.ContainsKey("datestart") ? jarr["datestart"].ToString() : "";//查询条件 日期开始
                string dateend = jarr.ContainsKey("dateend") ? jarr["dateend"].ToString() : "";//查询条件 日期结束
                string shoe_name = jarr.ContainsKey("shoe_name") ? jarr["shoe_name"].ToString() : "";//查询条件 鞋型名称
                string prod_no = jarr.ContainsKey("prod_no") ? jarr["prod_no"].ToString() : "";//查询条件 art
                string PO_ORDER = jarr.ContainsKey("PO_ORDER") ? jarr["PO_ORDER"].ToString() : "";//查询条件 po
                string COUNTRY_REGION = jarr.ContainsKey("COUNTRY_REGION") ? jarr["COUNTRY_REGION"].ToString() : "";//查询条件 国家区域 
                string DEFECT_CONTENT = jarr.ContainsKey("DEFECT_CONTENT") ? jarr["DEFECT_CONTENT"].ToString() : "";//查询条件 问题点
                string DEVELOP_SEASON = jarr.ContainsKey("DEVELOP_SEASON") ? jarr["DEVELOP_SEASON"].ToString() : "";//查询条件 开发季度
                string Category = jarr.ContainsKey("Category") ? jarr["Category"].ToString() : "";//查询条件 Category
                string STATUS = jarr.ContainsKey("STATUS") ? jarr["STATUS"].ToString() : "";//查询条件 状态
                string processing_results_status = jarr.ContainsKey("processing_results_status") ? jarr["processing_results_status"].ToString() : "";//查询条件 处理结果
                string COMPLAINT_NO = jarr.ContainsKey("COMPLAINT_NO") ? jarr["COMPLAINT_NO"].ToString() : "";//查询条件 投诉编号

                string where = string.Empty;
                if (!string.IsNullOrWhiteSpace(datestart) && !string.IsNullOrWhiteSpace(dateend))
                {
                    where += $@" and (m.COMPLAINT_DATE between to_date(@datestart,'yyyy-mm-dd') and to_date(@dateend,'yyyy-mm-dd'))";
                }
                if (!string.IsNullOrWhiteSpace(shoe_name))
                {
                    where += $@" and r.name_t like @shoe_name";
                }
                if (!string.IsNullOrWhiteSpace(prod_no))
                {
                    where += $@" and t.prod_no like @prod_no";
                }
                if (!string.IsNullOrWhiteSpace(PO_ORDER))
                {
                    where += $@" and m.PO_ORDER like @PO_ORDER";
                }
                if (!string.IsNullOrWhiteSpace(COUNTRY_REGION))
                {
                    where += $@" and m.COUNTRY_REGION like @COUNTRY_REGION";
                }
                if (!string.IsNullOrWhiteSpace(DEFECT_CONTENT))
                {
                    where += $@" and m.DEFECT_CONTENT like @DEFECT_CONTENT";
                }
                if (!string.IsNullOrWhiteSpace(DEVELOP_SEASON))
                {
                    where += $@" and p.DEVELOP_SEASON like @DEVELOP_SEASON";
                }
                if (!string.IsNullOrWhiteSpace(Category))
                {
                    where += $@" and bb.name_t like @Category";
                }
                if (!string.IsNullOrWhiteSpace(STATUS))
                {
                    where += $@" and m.STATUS = @STATUS";
                }
                if (!string.IsNullOrWhiteSpace(processing_results_status))
                {
                    where += $@" and m.processing_results_status = @processing_results_status";
                }
                if (!string.IsNullOrWhiteSpace(COMPLAINT_NO))
                {
                    where += $@" and m.COMPLAINT_NO like @COMPLAINT_NO";
                }

                string sql = string.Empty;
                sql = $@"select 
                        m.COMPLAINT_NO,
                        MAX(TO_CHAR( m.COMPLAINT_DATE,'yyyy-mm-dd')) COMPLAINT_DATE,
                        MAX(m.COUNTRY_REGION) as COUNTRY_REGION,
                        MAX(m.PO_ORDER) as PO_ORDER,
                        MAX(t.SE_QTY) as ts_posl,
                        MAX(m.DEFECT_CONTENT) as DEFECT_CONTENT,
                        MAX(m.NG_QTY) as NG_QTY,
                        MAX(m.COMPLAINT_MONEY) as COMPLAINT_MONEY,
                        MAX(m.STATUS) as STATUS,
                        MAX(m.FOB) as FOB,
                        MAX(m.processing_results_status) as processing_results_status,
                        MAX(p.DEVELOP_SEASON) as DEVELOP_SEASON,
                        MAX(bb.name_t) as Category,
                        MAX(p.user_section) as user_section,
                        MAX(p.PRODUCT_MONTH) as PRODUCT_MONTH,
                        MAX(t.prod_no) as prod_no,
                        MAX(r.name_t) as shoe_name,
                        MAX(p.COLOR_WAY) as Material_Way
                        from 
                        QCM_CUSTOMER_COMPLAINT_M m
                        LEFT JOIN BDM_SE_ORDER_MASTER b on m.PO_ORDER=b.mer_po
                        LEFT JOIN BDM_SE_ORDER_ITEM t on b.se_id=t.se_id
                        LEFT JOIN BDM_RD_PROD p on t.prod_no =p.prod_no
                        LEFT JOIN BDM_RD_STYLE r on t.shoe_no=r.shoe_no
                        LEFT JOIN bdm_cd_code bb ON r.style_seq=bb.code_no
                        where 1=1 {where} 
                        GROUP BY m.COMPLAINT_NO
                        order by max(m.id) desc";
                Dictionary<string, object> paramTestDic = new Dictionary<string, object>();
                paramTestDic.Add("datestart", $@"{datestart}");
                paramTestDic.Add("dateend", $@"{dateend}");
                paramTestDic.Add("shoe_name", $@"%{shoe_name}%");
                paramTestDic.Add("prod_no", $@"%{prod_no}%");
                paramTestDic.Add("PO_ORDER", $@"%{PO_ORDER}%");
                paramTestDic.Add("COUNTRY_REGION", $@"%{COUNTRY_REGION}%");
                paramTestDic.Add("DEFECT_CONTENT", $@"%{DEFECT_CONTENT}%");
                paramTestDic.Add("DEVELOP_SEASON", $@"%{DEVELOP_SEASON}%");
                paramTestDic.Add("Category", $@"%{Category}%");
                paramTestDic.Add("STATUS", $@"{STATUS}");
                paramTestDic.Add("processing_results_status", $@"{processing_results_status}");
                paramTestDic.Add("COMPLAINT_NO", $@"%{COMPLAINT_NO}%");
                DataTable dt = DB.GetDataTable(sql, paramTestDic);



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

        /// <summary>
        /// 客户投诉处理页查询实验室测试报告
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetEx_LookResult(object OBJ)
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
                string ART = jarr.ContainsKey("ART") ? jarr["ART"].ToString() : "";//查询条件 ART

                string sql = string.Empty;
                sql = $@"SELECT * FROM(
                        SELECT
	                        task_no
                        FROM
	                        qcm_ex_task_list_m 
                        WHERE
	                        test_type = '0' 
	                        AND art_no = '{ART}' 
	                        ORDER BY
	                        id DESC
	                        )
	                        WHERE  ROWNUM = 1  ";

                string task_no = DB.GetString(sql);

                Dictionary<string, object> dic = new Dictionary<string, object>();
                dic.Add("task_no", task_no);

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
