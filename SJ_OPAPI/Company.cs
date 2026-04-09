using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace SJ_OPAPI
{
    public class Company
    {
        /// <summary>
        /// 添加企业信息接口
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static GDSJFramework_NETCore.WebAPI.ResultObject AddCompany(object OBJ)
        {
            GDSJFramework_NETCore.WebAPI.RequestObject ReqObj = (GDSJFramework_NETCore.WebAPI.RequestObject)OBJ;
            GDSJFramework_NETCore.WebAPI.ResultObject ret = new GDSJFramework_NETCore.WebAPI.ResultObject();

            string Data = string.Empty;
            string UserToken = string.Empty;
            string guid = string.Empty;
            GDSJFramework_NETCore.DBHelper.DataBase DB = new GDSJFramework_NETCore.DBHelper.DataBase();
            try
            {

                Data = ReqObj.Data.ToString();
                UserToken = ReqObj.UserToken.ToString();
                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                DB = new GDSJFramework_NETCore.DBHelper.DataBase(string.Empty);


                #region 接口参数
                string CompanyCode = jarr["CompanyCode"].ToString();
                string CompanyName = jarr["CompanyName"].ToString();
                string CompanyType = jarr["CompanyType"].ToString();
                string LegalPerson = jarr["LegalPerson"].ToString();
                string Location = jarr["Location"].ToString();
                string Address = jarr["Address"].ToString();
                string Contacts = jarr["Contacts"].ToString();
                string ContactNumber = jarr["ContactNumber"].ToString();
                string Mail = jarr["Mail"].ToString();
                string WebAddress = jarr["WebAddress"].ToString();
                string EnterpriseDesc = jarr["EnterpriseDesc"].ToString();
                //string WeChat = jarr["WeChat"].ToString();
                //string CapitalAccount = jarr["CapitalAccount"].ToString();
                string BusinessLicense = jarr["BusinessLicense"].ToString();
                string status = jarr["status"].ToString();
                #endregion
                #region 逻辑
                if (!string.IsNullOrEmpty(CompanyCode))
                {
                    string Company = DB.GetString("select CompanyCode from CompanyInfo where CompanyCode='" + CompanyCode + "'");
                    if (string.IsNullOrEmpty(Company))
                    {
                        string UserCode = DB.GetString("select UserCode from UserToken where UserToken='"+ UserToken + "'");
                        string sql = "insert into CompanyInfo(CompanyCode,CompanyName,CompanyType,LegalPerson,Location,Address,Contacts,ContactNumber,Mail,WebAddress,EnterpriseDesc," +
                            "BusinessLicense,status,createby,createdate,createtime) " +
                            "values('" + CompanyCode + "','" + CompanyName + "','" + CompanyType + @"',
                     '" + LegalPerson + "','" + Location + "','" + Address + "','" + Contacts + "','" + ContactNumber + "','" + Mail + "','" + WebAddress + "','" + EnterpriseDesc + "'," +
                     "'" + BusinessLicense + "','" + status + "','"+ UserCode + "','"+DateTime.Now.ToString("yyyy-MM-dd")+"','"+ DateTime.Now.ToString("HH:mm:ss") + "')";
                        DB.ExecuteNonQueryOffline(sql);
                        ret.IsSuccess = true;
                        ret.ErrMsg = "保存成功！";
                    }
                    else
                    {
                        ret.IsSuccess = false;
                        ret.ErrMsg = "该公司已存在！";
                    }

                }
                else
                {
                    ret.IsSuccess = false;
                    ret.ErrMsg = "公司代号不能为空！";
                }

                #endregion

            }
            catch (Exception ex)
            {
                ret.IsSuccess = false;
                ret.ErrMsg = "00000:" + ex.Message;
            }
            return ret;
        }

        /// <summary>
        /// 修改企业信息接口
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static GDSJFramework_NETCore.WebAPI.ResultObject ModifyCompany(object OBJ)
        {
            GDSJFramework_NETCore.WebAPI.RequestObject ReqObj = (GDSJFramework_NETCore.WebAPI.RequestObject)OBJ;
            GDSJFramework_NETCore.WebAPI.ResultObject ret = new GDSJFramework_NETCore.WebAPI.ResultObject();

            string Data = string.Empty;
            string UserToken = string.Empty;
            string guid = string.Empty;
            GDSJFramework_NETCore.DBHelper.DataBase DB = new GDSJFramework_NETCore.DBHelper.DataBase();
            try
            {

                Data = ReqObj.Data.ToString();

                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                DB = new GDSJFramework_NETCore.DBHelper.DataBase(string.Empty);


                #region 接口参数
                string CompanyCode = jarr["CompanyCode"].ToString();
                string CompanyName = jarr["CompanyName"].ToString();
                string CompanyType = jarr["CompanyType"].ToString();
                string LegalPerson = jarr["LegalPerson"].ToString();
                string Location = jarr["Location"].ToString();
                string Address = jarr["Address"].ToString();
                string Contacts = jarr["Contacts"].ToString();
                string ContactNumber = jarr["ContactNumber"].ToString();
                string Mail = jarr["Mail"].ToString();
                string WebAddress = jarr["WebAddress"].ToString();
                string EnterpriseDesc = jarr["EnterpriseDesc"].ToString();
                //string WeChat = jarr["WeChat"].ToString();
                //string CapitalAccount = jarr["CapitalAccount"].ToString();
                string BusinessLicense = jarr["BusinessLicense"].ToString();
                string status = jarr["status"].ToString();
                #endregion
                #region 逻辑
                if (!string.IsNullOrEmpty(CompanyCode))
                {
                    string Company = DB.GetString("select CompanyCode from CompanyInfo where CompanyCode='" + CompanyCode + "'");
                    if (!string.IsNullOrEmpty(Company))
                    {
                        string UserCode = DB.GetString("select UserCode from UserToken where UserToken='" + UserToken + "'");
                        string sql = "update CompanyInfo set CompanyCode='" + CompanyCode + "',CompanyName='" + CompanyName + "',CompanyType='" + CompanyType + "',LegalPerson='" + LegalPerson + "'," +
                            "Location='" + Location + "',Address='" + Address + "',Contacts='" + Contacts + "',ContactNumber='" + ContactNumber + "',Mail='" + Mail + "',WebAddress='" + WebAddress + "'" +
                            ",EnterpriseDesc='" + EnterpriseDesc + "',BusinessLicense='" + BusinessLicense + "'," +
                            "status='" + status + "',modifyby='"+ UserCode + "',modifydate='"+DateTime.Now.ToString("yyyy-MM-dd")+ "',modifytime='"+DateTime.Now.ToString("HH:mm:ss")+"' where CompanyCode='" + CompanyCode + "'";
                        DB.ExecuteNonQueryOffline(sql);
                        ret.IsSuccess = true;
                        ret.ErrMsg = "修改成功！";
                    }
                    else
                    {
                        ret.IsSuccess = false;
                        ret.ErrMsg = "查无此数据！";
                    }

                }
                else
                {
                    ret.IsSuccess = false;
                    ret.ErrMsg = "公司代号不能为空！";
                }

                #endregion

            }
            catch (Exception ex)
            {
                ret.IsSuccess = false;
                ret.ErrMsg = "00000:" + ex.Message;
            }
            return ret;
        }

        /// <summary>
        /// 查询企业信息接口
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static GDSJFramework_NETCore.WebAPI.ResultObject QueryCompany(object OBJ)
        {
            GDSJFramework_NETCore.WebAPI.RequestObject ReqObj = (GDSJFramework_NETCore.WebAPI.RequestObject)OBJ;
            GDSJFramework_NETCore.WebAPI.ResultObject ret = new GDSJFramework_NETCore.WebAPI.ResultObject();

            string Data = string.Empty;

            string guid = string.Empty;
            GDSJFramework_NETCore.DBHelper.DataBase DB = new GDSJFramework_NETCore.DBHelper.DataBase();
            try
            {

                Data = ReqObj.Data.ToString();

                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                DB = new GDSJFramework_NETCore.DBHelper.DataBase(string.Empty);


                #region 接口参数
                string Where = jarr["Where"].ToString();
                string OrderBy = jarr["OrderBy"].ToString();
                string PageRow = jarr["PageRow"].ToString();
                string Page = jarr["Page"].ToString();
                #endregion
                #region 逻辑
                int total2 = (int.Parse(Page) - 1) * int.Parse(PageRow);
                int total = DB.GetInt32("select count(*) from CompanyInfo where 1=1 " + Where + "");
                string sql = @"select * from (
select M.*,@n:= @n + 1 as RN from CompanyInfo M,(select @n:= 0) d) tab where  RN > " + total2 + " " + Where + " " + OrderBy + " limit " + PageRow + "";
                DataTable dt = DB.GetDataTable(sql);
                string json = GDSJFramework_NETCore.Common.JsonHelper.GetJsonByDataTable(dt);
                Dictionary<string, object> p = new Dictionary<string, object>();
                p.Add("data", json);
                p.Add("total", total);
                ret.IsSuccess = true;
                ret.RetData = Newtonsoft.Json.JsonConvert.SerializeObject(p);
                #endregion

            }
            catch (Exception ex)
            {
                ret.IsSuccess = false;
                ret.ErrMsg = "00000:" + ex.Message;
            }
            return ret;
        }

        /// <summary>
        /// 删除企业信息接口
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static GDSJFramework_NETCore.WebAPI.ResultObject DeleteCompany(object OBJ)
        {
            GDSJFramework_NETCore.WebAPI.RequestObject ReqObj = (GDSJFramework_NETCore.WebAPI.RequestObject)OBJ;
            GDSJFramework_NETCore.WebAPI.ResultObject ret = new GDSJFramework_NETCore.WebAPI.ResultObject();

            string Data = string.Empty;

            string guid = string.Empty;
            GDSJFramework_NETCore.DBHelper.DataBase DB = new GDSJFramework_NETCore.DBHelper.DataBase();
            try
            {

                Data = ReqObj.Data.ToString();

                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                DB = new GDSJFramework_NETCore.DBHelper.DataBase(string.Empty);


                #region 接口参数
                string CompanyCode = jarr["CompanyCode"].ToString();
                #endregion
                #region 逻辑
                if (!string.IsNullOrEmpty(CompanyCode))
                {
                    string code = DB.GetString("select CompanyCode from CompanyInfo where CompanyCode='" + CompanyCode + "'");
                    if (!string.IsNullOrEmpty(code))
                    {
                        string sql = "delete from CompanyInfo where  CompanyCode='" + CompanyCode + "'";
                        ret.IsSuccess = true;
                        ret.ErrMsg = "删除成功！";
                    }
                    else
                    {
                        ret.IsSuccess = false;
                        ret.ErrMsg = "查无此数据！";
                    }
                }
                else
                {
                    ret.IsSuccess = false;
                    ret.ErrMsg = "传入参数不能为空！";
                }
                #endregion

            }
            catch (Exception ex)
            {
                ret.IsSuccess = false;
                ret.ErrMsg = "00000:" + ex.Message;
            }
            return ret;
        }


    }

}
