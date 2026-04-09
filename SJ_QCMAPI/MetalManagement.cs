using SJ_QCMAPI.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace SJ_QCMAPI
{
    public class MetalManagement
    {
        /// <summary>
        /// 金属检验操作
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject MetalOperation(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();

            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);

            try
            {
                string Data = ReqObj.Data.ToString();

                //打开事务
                DB.Open();
                DB.BeginTransaction();
                #region 逻辑

                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                //转译
                string isSelect= jarr.ContainsKey("isSelect") ? jarr["isSelect"].ToString() : "";//判断是查询还是提交
                string po_order = jarr.ContainsKey("PO") ? jarr["PO"].ToString() : "";//PO号
                string shoe_no = jarr.ContainsKey("shoe_no") ? jarr["shoe_no"].ToString() : "";//鞋型
                string prod_no = jarr.ContainsKey("prod_no") ? jarr["prod_no"].ToString() : "";//ART
                string code_number = jarr.ContainsKey("code_number") ? jarr["code_number"].ToString() : "";//码数
                string left_or_right = jarr.ContainsKey("left_or_right") ? jarr["left_or_right"].ToString() : "";//R/L
                string handle_way = jarr.ContainsKey("handle_way") ? jarr["handle_way"].ToString() : "";//处理方法
                string handle_result = jarr.ContainsKey("handle_result") ? jarr["handle_result"].ToString() : "";//处理结果

                string user = SJeMES_Framework_NETCore.Web.System.GetUserCodeByToken(ReqObj.UserToken);//获取的登陆人信息
                string date = DateTime.Now.ToString("yyyy-MM-dd");//日期
                string time = DateTime.Now.ToString("HH-mm-ss");//时间

                DataTable ImgList = Newtonsoft.Json.JsonConvert.DeserializeObject<DataTable>(jarr.ContainsKey("ImgList") ? jarr["ImgList"].ToString() : "");//图片集合


                Dictionary<string, object> ALLlist = new Dictionary<string, object>();//返回数据

                //检验单号M
                string inspect_no = DateTime.Now.ToString("yyyyMMdd");//QA20211016  00001
                int status = 1;
                string sql = $@"select max(inspect_no) from qcm_inspection_metal_m where inspect_no like '{inspect_no}%'";
                string max_inspection_no = DB.GetString(sql);
                //查询画皮单号有没有相同的
                if (!string.IsNullOrEmpty(max_inspection_no))
                {
                    string seq = max_inspection_no.Replace(inspect_no, "");//00002

                    int int_seq = Convert.ToInt32(seq) + 1;//3   00111

                    inspect_no = inspect_no + int_seq.ToString().PadLeft(5, '0');
                    //throw new Exception("检验单号：【" + inspect_no + "】重复，请检查!");
                }
                else
                {
                    inspect_no = inspect_no + "00001";
                }

                //判断PO号要是为空就弹出
                if (string.IsNullOrEmpty(po_order))
                {
                    ret.ErrMsg = "请输入PO号!";
                    ret.IsSuccess = false;
                    return ret;
                }

                string department_no = "空";//部门代号
                string department_name = "空";//部门名称
                string productionline_no = "空";//产线代号
                string productionline_name = "空";//产线名称
                if (isSelect=="select")//查询
                {
                    string PNO = DB.GetString($@"SELECT
	                                            m.PROD_NO      
                                            FROM
	                                            BDM_SE_ORDER_MASTER b
                                            INNER JOIN
                                            BDM_SE_ORDER_ITEM m
                                            on b.SE_ID=m.SE_ID        
                                            where b.MER_PO='{po_order}'");//ART
                    string SNO = DB.GetString($@"SELECT
	                                            m.SHOE_NO  
                                            FROM
	                                            BDM_SE_ORDER_MASTER b
                                            INNER JOIN
                                            BDM_SE_ORDER_ITEM m
                                            on b.SE_ID=m.SE_ID        
                                            where b.MER_PO='{po_order}'");//鞋型
                    if (string.IsNullOrEmpty(PNO)&&string.IsNullOrEmpty(SNO))
                    {
                        ret.IsSuccess = false;
                        ret.ErrMsg = "查无数据!";
                        return ret;
                    }
                    ALLlist.Add("SHOE_NO", SNO);
                    ALLlist.Add("PROD_NO", PNO);
                    ret.IsSuccess = true;
                    ret.RetData1 = ALLlist;
                    return ret;
                }
                else if (isSelect=="submit")//提交
                {
                    string PNO = DB.GetString($@"SELECT
	                                            m.PROD_NO      
                                            FROM
	                                            BDM_SE_ORDER_MASTER b
                                            INNER JOIN
                                            BDM_SE_ORDER_ITEM m
                                            on b.SE_ID=m.SE_ID        
                                            where b.MER_PO='{po_order}'");//ART
                    string SNO = DB.GetString($@"SELECT
	                                            m.SHOE_NO  
                                            FROM
	                                            BDM_SE_ORDER_MASTER b
                                            INNER JOIN
                                            BDM_SE_ORDER_ITEM m
                                            on b.SE_ID=m.SE_ID        
                                            where b.MER_PO='{po_order}'");//鞋型
                    if (PNO!= prod_no&& SNO!=shoe_no)
                    {
                        ret.IsSuccess = false;
                        ret.ErrMsg = "数据不一!";
                        return ret;
                    }
                    DB.ExecuteNonQuery($@"insert into qcm_inspection_metal_m (inspect_no,department_no,department_name,productionline_no,productionline_name,po_order,shoe_no,prod_no,code_number,left_or_right,handle_way,handle_result,createby,createdate,createtime) 
                                          values('{inspect_no}','{department_no}','{department_name}','{productionline_no}','{productionline_name}','{po_order}','{shoe_no}','{prod_no}','{code_number}','{left_or_right}','{handle_way}','{handle_result}','{user}','{date}','{time}')");
                    foreach (DataRow item in ImgList.Rows)
                    {
                        string guid = Guid.NewGuid().ToString("N");
                        DB.ExecuteNonQuery($@"insert into qcm_inspection_metal_image (inspect_no,po_order,img_name,img_url,guid,createby,createdate,createtime) 
                                              values('{inspect_no}','{po_order}','{item["img_name"]}','{item["img_url"]}','{guid}','{user}','{date}','{time}')");
                    }
                }
                else
                {
                    ret.ErrMsg = "请检查数据是否正确!";
                    ret.IsSuccess = false;
                    return ret;
                }

                #endregion
                //数据合并返回

                //ret.RetData1 = WWPlist;
                ret.IsSuccess = true;
                DB.Commit();//提交事务
            }
            catch (Exception ex)
            {
                /*DB.Rollback();*///回滚事务
                ret.IsSuccess = false;
                ret.ErrMsg = "00000:" + ex.Message;

            }
            finally
            {
                DB.Close();//关闭事务
            }
            return ret;

        }
        /// <summary>
        /// 金属检验首页数据展示
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject MetalManagementList(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase();
            try
            {
                DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);
                string Data = ReqObj.Data.ToString();

                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                string po_order = jarr.ContainsKey("po_order") ? jarr["po_order"].ToString() : "";
                string prod_no = jarr.ContainsKey("prod_no") ? jarr["prod_no"].ToString() : "";
                string left_or_right = jarr.ContainsKey("left_or_right") ? jarr["left_or_right"].ToString() : "";

                string pageSize = jarr.ContainsKey("pageSize") ? jarr["pageSize"].ToString() : "";
                string pageIndex = jarr.ContainsKey("pageIndex") ? jarr["pageIndex"].ToString() : "";
                var sql = string.Empty;

                string strwhere = string.Empty;
                if (!string.IsNullOrEmpty(po_order))
                {
                    strwhere += $@" and po_order like '%{po_order}%'";
                }
                if (!string.IsNullOrEmpty(prod_no))
                {
                    strwhere += $@" and prod_no like '%{prod_no}%'";
                }
                if (!string.IsNullOrEmpty(left_or_right))
                {
                    strwhere += $@" and left_or_right like '%{left_or_right}%'";
                }


                sql = $@"select*from  qcm_inspection_metal_m   where 1=1 {strwhere} order by id";
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
        /// 金属检验IMG图片展示
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject MetalManagement_imgList(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase();
            try
            {
                DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);
                string Data = ReqObj.Data.ToString();

                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);

                string inspect_no = jarr.ContainsKey("inspect_no") ? jarr["inspect_no"].ToString() : "";
                string po_order = jarr.ContainsKey("po_order") ? jarr["po_order"].ToString() : "";
                //转译
                //先找料品bom
                string sql = $@"select IMG_URL,IMG_NAME from  qcm_inspection_metal_image where inspect_no='{inspect_no}' and po_order='{po_order}'";
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
        /// 金属检验删除功能
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject MetalManagementDelete(object OBJ)
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
                string inspect_no = jarr.ContainsKey("inspect_no") ? jarr["inspect_no"].ToString() : "";

               
                string sql1 = $@"delete qcm_inspection_metal_m where inspect_no='{inspect_no}'";
                string sql2 = $@"delete qcm_inspection_metal_image where inspect_no='{inspect_no}'";

                DB.ExecuteNonQuery(sql1);
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
}
