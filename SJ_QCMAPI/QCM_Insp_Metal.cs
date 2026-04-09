using SJ_QCMAPI.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Net;
using System.Net.Mail;
using System.Text;

namespace SJ_QCMAPI
{
    public class QCM_Insp_Metal
    {
        /// <summary>
        /// 查询金属检测主页
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetMetalCheckList(object OBJ)
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
                string[] date = jarr.ContainsKey("date") ? Newtonsoft.Json.JsonConvert.DeserializeObject<string[]>(jarr["date"].ToString()) : null;//日期范围
                string keyword = jarr.ContainsKey("keyword") ? jarr["keyword"].ToString() : "";//搜索关键词 可模糊搜索 po号 ART 鞋型 码数
                string type = jarr.ContainsKey("type") ? jarr["type"].ToString() : "";//类型 2未选择 1人工创建 0机器创建
                string line = jarr.ContainsKey("line") ? jarr["line"].ToString() : "";//产线代号
                string task_no = jarr.ContainsKey("task_no") ? jarr["task_no"].ToString() : "";//任务号 当此字段有值时 忽略其他的搜索筛选条件
                string pageRow = jarr.ContainsKey("pageRow") ? jarr["pageRow"].ToString() : "";
                string page = jarr.ContainsKey("page") ? jarr["page"].ToString() : "";

                string where = string.Empty;
                if (!string.IsNullOrWhiteSpace(task_no))
                {
                    where += $@" and CONCAT(CONCAT(m.task_no,''), id) like '%{task_no}%'";
                }
                else
                {
                    if (date.Length > 1)
                    {
                        where += $@" and (to_date(CONCAT(CONCAT(m.CREATEDATE,' '), m.CREATETIME),'yyyy-mm-dd,hh24:mi:ss') between to_date('{date[0]}','yyyy-mm-dd,hh24:mi:ss') and to_date('{date[1]}','yyyy-mm-dd,hh24:mi:ss'))";
                    }
                    if (!string.IsNullOrWhiteSpace(keyword))
                    {
                        where += $@" and (m.po_no like '%{keyword}%' or m.art_no like '%{keyword}%' or m.shoe_no like '%{keyword}%' or m.shoe_size like '%{keyword}%')";
                    }
                    if (!string.IsNullOrWhiteSpace(type) && type != "2")
                    {
                        where += $@" and m.generate_type ='{type}'";
                    }
                    if (!string.IsNullOrWhiteSpace(line))
                    {
                        where += $@" and m.pro_line_no like '%{line}%'";
                    }
                }

                string sql = $@"
                                SELECT
                                    CONCAT( CONCAT( m.task_no, '' ), id ) AS task_no,
	                                CONCAT( CONCAT( m.CREATEDATE, ' ' ), m.CREATETIME ) AS dete,
	                                m.shoe_no AS shoe_type,
	                                r.name_t AS shoe_name,
	                                m.pro_line_name AS line_name,
	                                m.po_no AS po,
	                                m.shoe_size AS shoe_num,
	                                m.art_no AS art,
	                                m.closing_status,
	                                CASE closing_status
	                                WHEN '0' THEN
		                                'Open_Case'
			                                WHEN '1' THEN
		                                'Closed'
                                END as  status,
	                                CASE processing_res
	                                WHEN '0' THEN
		                                'FAIL'
		                                WHEN '1' THEN
		                                'PASS'
		                                ELSE ''
                                END AS result
                                FROM
	                                qcm_insp_metal_m m
                                    LEFT JOIN bdm_rd_style r on m.shoe_no=r.shoe_no
                                    where 1=1 {where}";
                DataTable dt = CommonBASE.GetPageDataTable(DB, sql, int.Parse(page), int.Parse(pageRow));
                dt.Columns.Add("del_show",typeof(bool));
                foreach (DataRow item in dt.Rows)
                {
                    if (item["closing_status"].ToString()=="1")
                    {
                        item["del_show"] = false;
                    }
                    else
                    {
                        item["del_show"] = true;
                    }
                }

                ret.RetData1 = dt;
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
        /// 金属检测主页删除
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject DelMetalCheck(object OBJ)
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
                string task_no = jarr.ContainsKey("task_no") ? jarr["task_no"].ToString() : "";//任务编码
                string user = SJeMES_Framework_NETCore.Web.System.GetUserCodeByToken(ReqObj.UserToken);//获取的登陆人信息
                string date = DateTime.Now.ToString("yyyy-MM-dd");//日期
                string time = DateTime.Now.ToString("HH:mm:ss");//时间

                string sql = string.Empty;
                sql = $@"delete from qcm_insp_metal_m where CONCAT(CONCAT(task_no,''), id)='{task_no}'";
                DB.ExecuteNonQuery(sql);

                string mid = DB.GetString($@"select id from qcm_insp_metal_m where CONCAT(CONCAT(task_no,''), id)='{task_no}'");

                sql = $@"delete from qcm_insp_metal_f where m_id='{mid}'";
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
        /// 查询金属检测po信息
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject getPoInfo(object OBJ)
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
                string po = jarr.ContainsKey("po") ? jarr["po"].ToString() : "";//po信息

                string sql = $@"SELECT
	                            d.PROD_NO AS art,
	                            r.SHOE_NO AS shoe_type,
                                MAX(b.name_t) as shoe_name,
	                            MAX( f.FILE_URL ) AS url,
	                            MAX( f.GUID ) AS guid 
                                FROM
	                            BDM_SE_ORDER_ITEM d
					            INNER JOIN BDM_SE_ORDER_MASTER s on d.SE_ID=s.SE_ID
	                            INNER JOIN bdm_rd_prod r ON d.PROD_NO = r.PROD_NO
	                            LEFT JOIN qcm_shoes_qa_record_m q ON r.SHOE_NO = q.shoes_code
	                            LEFT JOIN BDM_UPLOAD_FILE_ITEM f ON q.IMAGE_GUID = f.GUID 
                                LEFT JOIN BDM_RD_STYLE b on r.shoe_no=b.shoe_no
															WHERE
	                            s.MER_PO = '{po}' 
                            GROUP BY
	                            d.PROD_NO,
	                            r.SHOE_NO";
                DataTable dt = DB.GetDataTable(sql);

                ret.RetData1 = dt;
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
        /// 查询金属检测供应商/部门信息
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject getSupplierList(object OBJ)
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
                string keyword = jarr.ContainsKey("keyword") ? jarr["keyword"].ToString() : "";//搜索关键字
                string page = jarr.ContainsKey("page") ? jarr["page"].ToString() : "";////页数 当前页数没有数据时RetData1传空数组
                string pageRow = jarr.ContainsKey("pageRow") ? jarr["pageRow"].ToString() : "";//页尺寸
                string where = string.Empty;
                if (!string.IsNullOrWhiteSpace(keyword))
                {
                    where = $@" and (value like '%{keyword}%' or label like '%{keyword}%')";
                }

                string sql = $@"SELECT
	                                * 
                                FROM
	                                (
	                                SELECT
		                                SUPPLIERS_CODE AS value,
		                                SUPPLIERS_NAME AS label,
		                                '0' AS responsible_type 
	                                FROM
		                                base003m 
	                                WHERE
		                                SUPPLIERS_CODE IS NOT NULL UNION
	                                SELECT
		                                DEPARTMENT_CODE AS value,
		                                DEPARTMENT_NAME AS label,
		                                '1' AS responsible_type 
	                                FROM
		                                base005m 
	                                WHERE
	                                DEPARTMENT_CODE IS NOT NULL 
	                                )t where 1=1 {where}";
                DataTable dt = CommonBASE.GetPageDataTable(DB, sql, int.Parse(page), int.Parse(pageRow));

                ret.RetData1 = dt;
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
        /// 金属检测新建
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject AddMetalCheck(object OBJ)
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
                string po = jarr.ContainsKey("po") ? jarr["po"].ToString() : "";//po号
                string shoe_no = jarr.ContainsKey("shoe_no") ? jarr["shoe_no"].ToString() : ""; //鞋型
                string art_no = jarr.ContainsKey("art_no") ? jarr["art_no"].ToString() : "";//art
                string line_no = jarr.ContainsKey("line_no") ? jarr["line_no"].ToString() : "";//产线代号
                string shoe_num = jarr.ContainsKey("shoe_num") ? jarr["shoe_num"].ToString() : "";//码数
                string remark = jarr.ContainsKey("remark") ? jarr["remark"].ToString() : "";//问题点说明
                string reason = jarr.ContainsKey("reason") ? jarr["reason"].ToString() : "";//原因分析
                string handle_way = jarr.ContainsKey("handle_way") ? jarr["handle_way"].ToString() : "";//处理方式
                string isFinish = jarr.ContainsKey("isFinish") ? jarr["isFinish"].ToString() : "";//是否已结案 1已结案 0未结案
                string handle_result = jarr.ContainsKey("handle_result") ? jarr["handle_result"].ToString() : "";//处理结果 0 FAIL    1 PASS
                string rl = jarr.ContainsKey("rl") ? jarr["rl"].ToString() : "";//处理结果 0 r 1 l
                string suppliertype = jarr.ContainsKey("suppliertype") ? jarr["suppliertype"].ToString() : "";//责任单位//0 供应商 1 部门
                string suppliervalue = jarr.ContainsKey("suppliervalue") ? jarr["suppliervalue"].ToString() : "";//处理结果 供应商or部门代号
                List<Dictionary<string,string>> imgs_wt= jarr.ContainsKey("imgs_wt") ? Newtonsoft.Json.JsonConvert.DeserializeObject<List<Dictionary<string, string>>>(jarr["imgs_wt"].ToString()) : null;//问题点录入图片
                List<Dictionary<string, string>> imgs_x = jarr.ContainsKey("imgs_x") ? Newtonsoft.Json.JsonConvert.DeserializeObject<List<Dictionary<string, string>>>(jarr["imgs_x"].ToString()) : null;//X光机复核反馈图片
                List<Dictionary<string, string>> imgs_end = jarr.ContainsKey("imgs_end") ? Newtonsoft.Json.JsonConvert.DeserializeObject<List<Dictionary<string, string>>>(jarr["imgs_end"].ToString()) : null;//最终处理结果图片

                string user = SJeMES_Framework_NETCore.Web.System.GetUserCodeByToken(ReqObj.UserToken);//获取的登陆人信息
                string date = DateTime.Now.ToString("yyyy-MM-dd");//日期
                string time = DateTime.Now.ToString("HH:mm:ss");//时间

                if (string.IsNullOrWhiteSpace(po)|| string.IsNullOrWhiteSpace(line_no) || string.IsNullOrWhiteSpace(shoe_num))
                {
                    ret.IsSuccess = false;
                    ret.ErrMsg = "Data entry header cannot be empty!";
                    return ret;
                }

                if (isFinish=="1")
                {
                    if (string.IsNullOrWhiteSpace(remark)|| string.IsNullOrWhiteSpace(reason) || string.IsNullOrWhiteSpace(handle_result))
                    {
                        ret.IsSuccess = false;
                        ret.ErrMsg = "When the case is closed, the problem point entry, cause analysis, and processing results cannot be empty!";
                        return ret;
                    }
                }

                string task_no = DateTime.Now.ToString("yyyyMMdd") + line_no;

                string sql = string.Empty;
                sql = $@"select*from (SELECT
	PRODUCTION_LINE_CODE label,
	PRODUCTION_LINE_NAME laname
FROM
	BDM_PRODUCTION_LINE_M
UNION 
SELECT
	DEPARTMENT_CODE label,
	DEPARTMENT_NAME laname
FROM
	base005m) where label='{line_no}'";
                DataTable dt = DB.GetDataTable(sql);
                string line_name = dt.Rows[0]["laname"].ToString();
                if (string.IsNullOrWhiteSpace(line_name))
                {
                    ret.IsSuccess = false;
                    ret.ErrMsg = "No such production line!";
                    return ret;
                }
                sql = $@"insert into qcm_insp_metal_m (task_no,po_no,art_no,shoe_no,shoe_size,generate_type,pro_line_no,pro_line_name,problem_point_desc,
                        cause_analysis,treatment_method,responsible_type,responsible_unit,processing_res,rl,closing_status,createby,createdate,createtime) 
                         values('{task_no}','{po}','{art_no}','{shoe_no}','{shoe_num}','0','{line_no}','{line_name}','{remark}','{reason}','{handle_way}','{suppliertype}',
                         '{suppliervalue}','{handle_result}','{rl}','{isFinish}','{user}','{date}','{time}')";

                int count = DB.ExecuteNonQuery(sql);

                string mid = SJeMES_Framework_NETCore.DBHelper.DataBase.GetCurId(DB, "qcm_insp_metal_m");

                if (imgs_wt.Count>0)
                {
                    for (int i = 0; i < imgs_wt.Count; i++)
                    {
                        sql = $@"insert into qcm_insp_metal_f (m_id,image_type,file_guid,createby,createdate,createtime) 
                                 values('{mid}','0','{imgs_wt[i]["guid"]}','{user}','{date}','{time}')";
                        DB.ExecuteNonQuery(sql);
                    }
                }

                if (imgs_x.Count > 0)
                {
                    for (int i = 0; i < imgs_x.Count; i++)
                    {
                        sql = $@"insert into qcm_insp_metal_f (m_id,image_type,file_guid,createby,createdate,createtime) 
                                 values('{mid}','1','{imgs_x[i]["guid"]}','{user}','{date}','{time}')";
                        DB.ExecuteNonQuery(sql);
                    }
                }

                if (imgs_end.Count > 0)
                {
                    for (int i = 0; i < imgs_end.Count; i++)
                    {
                        sql = $@"insert into qcm_insp_metal_f (m_id,image_type,file_guid,createby,createdate,createtime) 
                                 values('{mid}','2','{imgs_end[i]["guid"]}','{user}','{date}','{time}')";
                        DB.ExecuteNonQuery(sql);
                    }
                }
                if (count > 0)
                {
                    sql = $@"SELECT   d.cr_reqdate as shipment_date
                              FROM qcm_insp_metal_m m
                              LEFT JOIN BDM_SE_SHIPMENT_NOTIFICATION_M a
                              ON m.po_no = a.po_no
                              LEFT JOIN BDM_SE_ORDER_ITEM d
                              ON d.se_id = a.se_id
                              where m.createdate = to_char(sysdate, 'yyyy-MM-dd')";
                    string shipment_date = DB.GetString(sql);

                    string sql2 = $@"select a.to_mail,a.cc_mail from bdm_mail_list a where a.function_name='Metal_Detection'";
                    DataTable dt2 = DB.GetDataTable(sql2);

                    string mail = dt2.Rows[0]["to_mail"].ToString();
                    string ccMail = dt2.Rows[0]["cc_mail"].ToString(); 

                    //string mail = "it-software05@in.apachefootwear.com";
                    //string ccMail = "sailaja-v@in.apachefootwear.com";
                    //string ccMail2 = "prasanth-v@msin.apachefootwear.com"; 
                    //string bodyheading = "Vehicle Exited From Apache Footwear Private Limited Company";


                    using (MailMessage mm = new MailMessage())
                    {
                        mm.From = new MailAddress("IT-announcement@in.apachefootwear.com", "Metal Detection Alert");
                        mm.To.Add(mail);
                        mm.CC.Add(ccMail); 
                        mm.IsBodyHtml = true;
                        mm.Subject = "Metal Detection Alert";

                        string mailBody = $@"Dear <b>All</b><br /><br /> 
                                                 <b>This is a test Mail. Please ignore it</b> <br /><br />   

                                                  Best Regards!!<br /><br />
                                                  <b>APC IT</b><br /><br />
                                                  <u>Important Points From :</u><br />
                                                  1. This Mail is Auto Scheduled. Please Do Not Reply to this Email.<br />
                                                  2. If you would like to unsubscribe, please let the IT Department know.";

                        //string mailBody = $@"Dear <b>Line Incharge</b><br /><br /> 
                        //                         A metal peice was found on shoe of size <b>{shoe_num}</b> of article <b>{art_no}</b> and Po <b>{po}</b> produced from line <b>{line_name}</b> on {date} {time}<br /><br /> 
                        //                         Reason for detection was <b>{remark}</b>.
                        //                         Shipment date for the related PO is <b>{shipment_date}</b> <br /><br />
                        //                         Please Review it ASAP. Thanks.   <br /><br />   

                        //                          Best Regards!!<br /><br />
                        //                          <b>APC IT</b><br /><br />
                        //                          <u>Important Points From :</u><br />
                        //                          1. This Mail is Auto Scheduled. Please Do Not Reply to this Email.<br />
                        //                          2. If you would like to unsubscribe, please let the IT Department know.";

                        mm.Body = mailBody;
                        SmtpClient smtp = new SmtpClient();
                        smtp.Host = "10.3.0.254";
                        smtp.Port = 25;
                        smtp.Credentials = new NetworkCredential("IT-announcement@in.apachefootwear.com", "it-123456");
                        smtp.Send(mm);
                    }
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
        /// 金属检测编辑
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject EditMetalCheck(object OBJ)
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
                //string po = jarr.ContainsKey("po") ? jarr["po"].ToString() : "";//po号
                //string shoe_no = jarr.ContainsKey("shoe_no") ? jarr["shoe_no"].ToString() : ""; //鞋型
                //string art_no = jarr.ContainsKey("art_no") ? jarr["art_no"].ToString() : "";//art
                //string line_no = jarr.ContainsKey("line_no") ? jarr["line_no"].ToString() : "";//产线代号
                //string shoe_num = jarr.ContainsKey("shoe_num") ? jarr["shoe_num"].ToString() : "";//码数
                string task_no = jarr.ContainsKey("task_no") ? jarr["task_no"].ToString() : "";//任务代号
                string remark = jarr.ContainsKey("remark") ? jarr["remark"].ToString() : "";//问题点说明
                string reason = jarr.ContainsKey("reason") ? jarr["reason"].ToString() : "";//原因分析
                string handle_way = jarr.ContainsKey("handle_way") ? jarr["handle_way"].ToString() : "";//处理方式
                string isFinish = jarr.ContainsKey("isFinish") ? jarr["isFinish"].ToString() : "";//是否已结案 1已结案 0未结案
                string handle_result = jarr.ContainsKey("handle_result") ? jarr["handle_result"].ToString() : "";//处理结果 0 FAIL    1 PASS
                string rl = jarr.ContainsKey("rl") ? jarr["rl"].ToString() : "";//处理结果 0 r 1 l
                string suppliertype = jarr.ContainsKey("suppliertype") ? jarr["suppliertype"].ToString() : "";//责任单位//0 供应商 1 部门
                string suppliervalue = jarr.ContainsKey("suppliervalue") ? jarr["suppliervalue"].ToString() : "";//处理结果 供应商or部门代号
                List<Dictionary<string, string>> imgs_wt = jarr.ContainsKey("imgs_wt") ? Newtonsoft.Json.JsonConvert.DeserializeObject<List<Dictionary<string, string>>>(jarr["imgs_wt"].ToString()) : null;//问题点录入图片
                List<Dictionary<string, string>> imgs_x = jarr.ContainsKey("imgs_x") ? Newtonsoft.Json.JsonConvert.DeserializeObject<List<Dictionary<string, string>>>(jarr["imgs_x"].ToString()) : null;//X光机复核反馈图片
                List<Dictionary<string, string>> imgs_end = jarr.ContainsKey("imgs_end") ? Newtonsoft.Json.JsonConvert.DeserializeObject<List<Dictionary<string, string>>>(jarr["imgs_end"].ToString()) : null;//最终处理结果图片

                string user = SJeMES_Framework_NETCore.Web.System.GetUserCodeByToken(ReqObj.UserToken);//获取的登陆人信息
                string date = DateTime.Now.ToString("yyyy-MM-dd");//日期
                string time = DateTime.Now.ToString("HH:mm:ss");//时间

                if (isFinish == "1")
                {
                    if (string.IsNullOrWhiteSpace(remark) || string.IsNullOrWhiteSpace(reason) || string.IsNullOrWhiteSpace(handle_result))
                    {
                        ret.IsSuccess = false;
                        ret.ErrMsg = "When the case is closed, the problem point entry, cause analysis, and processing results cannot be empty!";
                        return ret;
                    }
                }

                string sql = string.Empty;
                sql = $@"update qcm_insp_metal_m set problem_point_desc='{remark}',cause_analysis='{reason}',treatment_method='{handle_way}',
                        responsible_type='{suppliertype}',responsible_unit='{suppliervalue}',processing_res='{handle_result}',rl='{rl}',
                        closing_status='{isFinish}',modifyby='{user}',modifydate='{date}',modifytime='{time}' where CONCAT(CONCAT(task_no,''), id)='{task_no}'";
                DB.ExecuteNonQuery(sql);

                int mid = DB.GetInt32($@"select id from qcm_insp_metal_m where CONCAT(CONCAT(task_no,''), id)='{task_no}'");

                if (imgs_wt.Count > 0)
                {
                    DB.ExecuteNonQuery($@"delete from qcm_insp_metal_f where image_type='0' and m_id='{mid}'");
                    for (int i = 0; i < imgs_wt.Count; i++)
                    {
                        sql = $@"insert into qcm_insp_metal_f (m_id,image_type,file_guid,createby,createdate,createtime) 
                                 values('{mid}','0','{imgs_wt[i]["guid"]}','{user}','{date}','{time}')";
                        DB.ExecuteNonQuery(sql);
                    }
                }

                if (imgs_x.Count > 0)
                {
                    DB.ExecuteNonQuery($@"delete from qcm_insp_metal_f where image_type='1' and m_id='{mid}'");
                    for (int i = 0; i < imgs_x.Count; i++)
                    {
                        sql = $@"insert into qcm_insp_metal_f (m_id,image_type,file_guid,createby,createdate,createtime) 
                                 values('{mid}','1','{imgs_x[i]["guid"]}','{user}','{date}','{time}')";
                        DB.ExecuteNonQuery(sql);
                    }
                }

                if (imgs_end.Count > 0)
                {
                    DB.ExecuteNonQuery($@"delete from qcm_insp_metal_f where image_type='2' and m_id='{mid}'");
                    for (int i = 0; i < imgs_end.Count; i++)
                    {
                        sql = $@"insert into qcm_insp_metal_f (m_id,image_type,file_guid,createby,createdate,createtime) 
                                 values('{mid}','2','{imgs_end[i]["guid"]}','{user}','{date}','{time}')";
                        DB.ExecuteNonQuery(sql);
                    }
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
        /// 查询金属检测任务详情
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetMetalCheckDetail(object OBJ)
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
                string task_no = jarr.ContainsKey("task_no") ? jarr["task_no"].ToString() : "";//任务代号
                string sql = string.Empty;
                sql = $@"SELECT
	                    po_no AS po,
	                    pro_line_no AS line_no,
                        pro_line_name as line_name,
	                    shoe_size AS shoe_num,
	                    problem_point_desc AS remark,
	                    cause_analysis AS reason,
	                    treatment_method AS handle_way,
	                    '' AS suppliervalue,
	                    '' AS supplierlabel,
	                    processing_res AS handle_result,
	                    rl,
	                    closing_status AS isFinish,
	                    responsible_type,
	                    responsible_unit,
	                    f.FILE_URL AS url,
	                    f.GUID AS guid,
                        X_RESULT
                    FROM
	                    qcm_insp_metal_m m
	                    LEFT JOIN qcm_dqa_mag_d q ON m.SHOE_NO = q.shoes_code
	                    LEFT JOIN BDM_UPLOAD_FILE_ITEM f ON q.IMAGE_GUID = f.GUID where CONCAT(CONCAT(m.task_no,''), m.id) ='{task_no}' ";
                DataTable dt = DB.GetDataTable(sql);
                foreach (DataRow item in dt.Rows)
                {
                    if (item["responsible_type"]!=null)
                    {
                        if (item["responsible_type"].ToString()=="0")
                        {
                            DataTable supplier = DB.GetDataTable($@"select SUPPLIERS_CODE,SUPPLIERS_NAME from base003m where SUPPLIERS_CODE='{item["responsible_unit"]}'");
                            item["suppliervalue"] = supplier.Rows[0]["SUPPLIERS_CODE"];
                            item["supplierlabel"] = supplier.Rows[0]["SUPPLIERS_NAME"];
                        }
                        if (item["responsible_type"].ToString() == "1")
                        {
                            DataTable supplier = DB.GetDataTable($@"select DEPARTMENT_CODE,DEPARTMENT_NAME from base005m where DEPARTMENT_CODE='{item["responsible_unit"]}'");
                            item["suppliervalue"] = supplier.Rows[0]["DEPARTMENT_CODE"];
                            item["supplierlabel"] = supplier.Rows[0]["DEPARTMENT_NAME"];
                        }
                    }
                }

                int mid = DB.GetInt32($@"select id from qcm_insp_metal_m where CONCAT(CONCAT(task_no,''), id)='{task_no}'");

                sql = $@"SELECT
	                    m.FILE_URL as url,
	                    m.GUID as guid
                    FROM
	                    qcm_insp_metal_f f
	                    LEFT JOIN BDM_UPLOAD_FILE_ITEM m on f.FILE_GUID=m.GUID
                        where image_type='0' and m.GUID is not null and m_id='{mid}'
	                    ";
                DataTable imgs_wt = DB.GetDataTable(sql);

                sql = $@"SELECT
	                    m.FILE_URL as url,
	                    m.GUID as guid
                    FROM
	                    qcm_insp_metal_f f
	                    LEFT JOIN BDM_UPLOAD_FILE_ITEM m on f.FILE_GUID=m.GUID 
                        where image_type='1' and m.GUID is not null and m_id='{mid}'
	                    ";
                DataTable imgs_x = DB.GetDataTable(sql);

                sql = $@"SELECT
	                    m.FILE_URL as url,
	                    m.GUID as guid
                    FROM
	                    qcm_insp_metal_f f
	                    LEFT JOIN BDM_UPLOAD_FILE_ITEM m on f.FILE_GUID=m.GUID
                        where image_type='2' and m.GUID is not null and m_id='{mid}'
	                    ";
                DataTable imgs_end = DB.GetDataTable(sql);

                Dictionary<string, object> dic = new Dictionary<string, object>();
                dic.Add("RetData1", dt);
                dic.Add("imgs_wt", imgs_wt);
                dic.Add("imgs_x", imgs_x);
                dic.Add("imgs_end", imgs_end);

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
        /// 查询金属检测po
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetMER_PO(object OBJ)
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
                string keyword = jarr.ContainsKey("keyword") ? jarr["keyword"].ToString() : "";//搜索关键字
                string page = jarr.ContainsKey("page") ? jarr["page"].ToString() : "";////页数 当前页数没有数据时RetData1传空数组
                string pageRow = jarr.ContainsKey("pageRow") ? jarr["pageRow"].ToString() : "";//页尺寸
                string where = string.Empty;
                if (!string.IsNullOrWhiteSpace(keyword))
                {
                    where = $@" and MER_PO like '%{keyword}%'";
                }

                string sql = $@"SELECT MER_PO as value FROM BDM_SE_ORDER_MASTER where 1=1 {where}";
                DataTable dt = CommonBASE.GetPageDataTable(DB, sql, int.Parse(page), int.Parse(pageRow));

                ret.RetData1 = dt;
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
