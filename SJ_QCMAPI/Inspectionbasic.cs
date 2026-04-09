
using SJ_QCMAPI.Common;
using SJeMES_Framework_NETCore.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace SJ_QCMAPI
{
    public class Inspectionbasic
    {
        /// <summary>
        /// 添加表头，表身数据
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject BDM_RD_ITEM_Select_item_Add(object OBJ)
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
                //string material_nos = jarr.ContainsKey("material_nos") ? jarr["material_nos"].ToString() : "";//物料条码
                string material_no = jarr.ContainsKey("material_no") ? jarr["material_no"].ToString() : "";//物料条码
                string material_name = jarr.ContainsKey("material_name") ? jarr["material_name"].ToString() : "";//物料名称
                string category_no = jarr.ContainsKey("category_no") ? jarr["category_no"].ToString() : "";//样式种类
                string general_testtype_no = jarr.ContainsKey("general_testtype_no") ? jarr["general_testtype_no"].ToString() : "";//通用检测标准
                string department_no = jarr.ContainsKey("department_no") ? jarr["department_no"].ToString() : "";//阶段
                string UserId = jarr.ContainsKey("Id") ? jarr["Id"].ToString() : "";//账号
                string UserName = jarr.ContainsKey("Name") ? jarr["Name"].ToString() : "";//名称
                string Branch = jarr.ContainsKey("Branch") ? jarr["Branch"].ToString() : "";//部门
                string art_code = jarr.ContainsKey("art_code") ? jarr["art_code"].ToString() : "";//ART选择
                string productionline_no = jarr.ContainsKey("productionline_no") ? jarr["productionline_no"].ToString() : "";//产线
                string productionline_name = jarr.ContainsKey("productionline_name") ? jarr["productionline_name"].ToString() : "";//
                string plantarea_no = jarr.ContainsKey("plantarea_no") ? jarr["plantarea_no"].ToString() : "";//厂区*/
                string plantarea_name = jarr.ContainsKey("plantarea_name") ? jarr["plantarea_name"].ToString() : "";//厂区*/
                string category_name = jarr.ContainsKey("category_name") ? jarr["category_name"].ToString() : "";//厂名
                string CreactUserId = SJeMES_Framework_NETCore.Web.System.GetUserCodeByToken(ReqObj.UserToken);//查询登录人UserID

                string cbo_document_type = jarr.ContainsKey("cbo_document_type") ? jarr["cbo_document_type"].ToString() : "";//来源单号类型
                string txt_sjno = jarr.ContainsKey("txt_sjno") ? jarr["txt_sjno"].ToString() : "";//单号
                //送检单号
                string inspection_no = "QA" + DateTime.Now.ToString("yyyyMMdd");//QA20211016  00001
                int status = 1;
                string sql = $@"select max(inspection_no) from qcm_inspection_laboratory_m where inspection_no like '{inspection_no}%'";
                string max_inspection_no = DB.GetString(sql);
                //查询送检单号有没有相同的
                if (!string.IsNullOrEmpty(max_inspection_no))
                {
                    string seq = max_inspection_no.Replace(inspection_no, "");//00002
                    int int_seq = Convert.ToInt32(seq)+1;//3   00111

                    inspection_no = inspection_no + int_seq.ToString().PadLeft(5,'0');
                    //throw new Exception("送检单号：【" + inspection_no + "】重复，请检查!");
                }
                else
                {
                    inspection_no = inspection_no + "00001";
                }
                string sql1 = $@"select*from qcm_inspection_laboratory_m where material_no like '{material_no.Trim()}' and status!=2";
                DataTable dt1 = DB.GetDataTable(sql1);
                if (cbo_document_type == "0")//PO单号
                {
                    string sqq= $@"select MER_PO from BDM_SE_ORDER_MASTER where  MER_PO='{txt_sjno.Trim()}'";
                    DataTable dt = DB.GetDataTable(sqq);
                    if (dt.Rows.Count == 0)
                    {
                        throw new Exception("PO单号：【" + txt_sjno.Trim() + "】不存在，请重新输入!");
                    }
                }
                else if (cbo_document_type == "1")//收料单号
                {
                    string sqw = $@"select CHK_NO from wms_rcpt_m where CHK_NO='{txt_sjno.Trim()}'";
                    DataTable dt = DB.GetDataTable(sqw);
                    if (dt.Rows.Count == 0)
                    {
                        throw new Exception("收料单号：【" + txt_sjno.Trim() + "】不存在，请重新输入!");
                    }
                }
                /*if (dt1.Rows.Count==0)
                {*/
                    //表头添加数据
                    string sql2 = $@"insert into qcm_inspection_laboratory_m(document_type,doc_no,inspection_no,staff_no,staff_name,department_no,department_name,material_no,material_name,general_testtype_no,general_testtype_name,category_no,category_name,art_code,plantarea_no,plantarea_name,productionline_no,productionline_name,inspection_date,inspection_time,inspection_enddate,inspection_endtime,status,remarks,createby,createdate,createtime) values('{cbo_document_type}','{txt_sjno}','{inspection_no}','{UserId}','{UserName}','{department_no}','{Branch}','{material_no}','{material_name}','{general_testtype_no}','','{category_no}','{category_name}','{art_code}','{plantarea_no}','{plantarea_name}','{productionline_no}','{productionline_name}','{DateTime.Now.ToString("yyyy-MM-dd")}','{DateTime.Now.ToString("HH:mm:ss")}','','','{status}','','{CreactUserId}','{DateTime.Now.ToString("yyyy-MM-dd")}','{DateTime.Now.ToString("HH:mm:ss")}')";
              

                //表身添加数据
                var data11 = jarr.ContainsKey("data11") ? jarr["data11"].ToString() : "";
                    List<BdnListClass> list = Newtonsoft.Json.JsonConvert.DeserializeObject<List<BdnListClass>>(data11);
                    string sql3 = string.Empty;
                    foreach (var item in list)
                    {
                        sql3+= $@"into qcm_inspection_laboratory_d(inspection_no,testitem_code,testitem_name,testtype_no,testtype_name,sample_num,reference_level,check_type,t_check_item,t_check_value,d_check_item,d_check_value,currency_formula,custom_formula,unit,art_remarks,result_value,check_result,test_remarks,createby,createdate,createtime) values('{inspection_no}','{item.testitem_code}','{item.testitem_name}','{item.testtype_no}','{item.testtype_name}','{item.sample_num}','{item.reference_level}','{item.check_type}','{item.t_check_item}','{item.t_check_value}','{item.d_check_item}','{item.d_check_value}','{item.currency_formula}','{item.custom_formula}','{item.unit}','{item.art_remarks}','{item.result_value}','{item.check_result}','{item.test_remarks}','{CreactUserId}','{DateTime.Now.ToString("yyyy-MM-dd")}','{DateTime.Now.ToString("HH:mm:ss")}')";
                     }
                     string sql33 = $@"insert ALL {sql3} SELECT * FROM dual";
                     DB.ExecuteNonQuery(sql33);
                     DB.ExecuteNonQuery(sql2);
                     DB.Commit();
                    ret.IsSuccess = true;
                    ret.RetData = inspection_no;
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
