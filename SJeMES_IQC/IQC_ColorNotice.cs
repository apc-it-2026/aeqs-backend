using Newtonsoft.Json;
using SJ_QCMAPI.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace SJeMES_IQC
{
    class IQC_ColorNotice
    {
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject ColorNoticeUpload(object OBJ)
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
                string file_name = jarr.ContainsKey("file_name") ? jarr["file_name"].ToString() : "";//查询条件 生产厂商编号
                string file_url = jarr.ContainsKey("file_url") ? jarr["file_url"].ToString() : "";//查询条件 采购单号
                string file_guid = jarr.ContainsKey("file_guid") ? jarr["file_guid"].ToString() : "";//查询条件 采购单号
                string mcode = jarr.ContainsKey("mcode") ? jarr["mcode"].ToString() : "";//查询条件 采购单号
                string user = SJeMES_Framework_NETCore.Web.System.GetUserCodeByToken(ReqObj.UserToken);//获取的登陆人信息
                string date = DateTime.Now.ToString("yyyy-MM-dd");//日期
                string time = DateTime.Now.ToString("HH:mm:ss");//时间 
                string sql = string.Empty;
                
                    sql = $@"insert into BDM_COLOR_NOTICE (id,material_code,file_name,file_url,file_upload_time,createby,createdate,createtime,file_guid) 
                             values( bdm_color_notice_seq.nextval,'{mcode}','{file_name}','{file_url}',sysdate,'{user}','{date}','{time}','{file_guid}')";
                
                DB.ExecuteNonQuery(sql);
                ret.IsSuccess = true;
                ret.ErrMsg = "Uploaded Successfully";
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

        public static SJeMES_Framework_NETCore.WebAPI.ResultObject Get_ColorNotice_file(object OBJ)
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
                string material_code = jarr.ContainsKey("material_code") ? jarr["material_code"].ToString() : "";//查询条件 生产厂商编号 
                string sql = string.Empty;
                string wheresql=string.Empty;

                Dictionary<string, object> paramTestDic = new Dictionary<string, object>();
                string where = string.Empty;
                if (!string.IsNullOrWhiteSpace(material_code))
                {
                    where += $@"and material_code like '%{material_code}%'"; 
                } 
                sql = $@"
SELECT
*
FROM
	BDM_COLOR_NOTICE   
where 1=1 {where} 
order by file_upload_time desc";

                DataTable dt = DB.GetDataTable(sql); 
                Dictionary<string, object> dic = new Dictionary<string, object>();
                dic.Add("data", dt); 
                ret.IsSuccess = true;
                ret.RetData = JsonConvert.SerializeObject(dic);   
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

        public static SJeMES_Framework_NETCore.WebAPI.ResultObject Get_Material_Details(object OBJ)
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
                string Mat_Code = jarr.ContainsKey("Mat_Code") ? jarr["Mat_Code"].ToString() : "";//查询条件 生产厂商编号
                string pageSize = jarr.ContainsKey("pageSize") ? jarr["pageSize"].ToString() : "";
                string pageIndex = jarr.ContainsKey("pageIndex") ? jarr["pageIndex"].ToString() : "";
                string where = string.Empty;
                if (!string.IsNullOrWhiteSpace(Mat_Code))
                {
                    where += $@" and a.item_no like @Mat_Code ";
                }
                string sql = $@"select distinct a.item_no,b.name_t from wms_rcpt_d a left join bdm_rd_item b on a.item_no=b.item_no where 1=1
                                and a.insert_date between add_months(to_char(sysdate),-6) and sysdate{where}";
                Dictionary<string, object> paramTestDic = new Dictionary<string, object>();
                paramTestDic.Add("Mat_Code", $@"%{Mat_Code}%");
                DataTable dt = CommonBASE.GetPageDataTable(DB, sql, int.Parse(pageIndex), int.Parse(pageSize), "", paramTestDic);
                int rowCount = CommonBASE.GetPageDataTableCount(DB, sql, paramTestDic);
                Dictionary<string, object> dic = new Dictionary<string, object>();
                dic.Add("Data", dt);
                dic.Add("rowCount", rowCount);
                ret.RetData = Newtonsoft.Json.JsonConvert.SerializeObject(dic);
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

        public static SJeMES_Framework_NETCore.WebAPI.ResultObject DeleteById(object OBJ)
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
                string id= jarr.ContainsKey("id") ? jarr["id"].ToString() : "";//查询条件 生产厂商编号
                string file_name = jarr.ContainsKey("file_name") ? jarr["file_name"].ToString() : ""; 

                string sql = $@"delete BDM_COLOR_NOTICE where id='{id}'";
                DB.ExecuteNonQuery(sql);
                ret.IsSuccess = true;
                ret.ErrMsg = "Deleted Successfully";
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


        public static SJeMES_Framework_NETCore.WebAPI.ResultObject Get_ColorNotice_byART(object OBJ)
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
                string Input_Val = jarr.ContainsKey("Input_Val") ? jarr["Input_Val"].ToString() : "";//查询条件 生产厂商编号 
                string start_date = jarr.ContainsKey("start_date") ? jarr["start_date"].ToString() : ""; 
                string end_date = jarr.ContainsKey("end_date") ? jarr["end_date"].ToString() : ""; 
                string sql = string.Empty;
                string wheresql = string.Empty;

                Dictionary<string, object> paramTestDic = new Dictionary<string, object>();
                string where = string.Empty;
                if (!string.IsNullOrWhiteSpace(Input_Val))
                {
                    where += $@"and Substr(a.file_name,0,instr(a.file_name,'&')-1) like '%{Input_Val}%' or Substr(a.file_name,instr(a.file_name,'&')+1,10) like '%{Input_Val}%'";
                }
                if (!string.IsNullOrWhiteSpace(start_date) && !string.IsNullOrWhiteSpace(end_date))
                {
                    where += $@"and a.createdate between to_char(to_date('{start_date}','yyyy/MM/dd'),'yyyy-MM-dd') and to_char(to_date('{end_date}','yyyy/MM/dd'),'yyyy-MM-dd')";
                }
                
                sql = $@"
SELECT
*
FROM
	BDM_COLOR_NOTICE a  
where 1=1 {where} 
order by file_upload_time desc";

                DataTable dt = DB.GetDataTable(sql);
                Dictionary<string, object> dic = new Dictionary<string, object>();
                dic.Add("data", dt);
                ret.IsSuccess = true;
                ret.RetData = JsonConvert.SerializeObject(dic);
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

    }
}
