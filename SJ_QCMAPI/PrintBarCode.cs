using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace SJ_QCMAPI
{
    public class PrintBarCode
    {
        /// <summary>
        /// 获取条码枚举值
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetBarCodeEnum(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();

            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);

            try
            {
                string Data = ReqObj.Data.ToString();

                #region 逻辑

                var sql = $@"SELECT ENUM_CODE FROM SYS001M WHERE ENUM_TYPE =  'enum_barcode_print_type'";

                DataTable dt = DB.GetDataTable(sql);

                foreach (DataRow item in dt.Rows)
                {
                    switch (item["ENUM_CODE"].ToString())
                    {
                        case "0":
                            item["ENUM_CODE"] = SJ_QCMAPI.Common.enum_barcode_print_type.enum_barcode_print_type_0;
                            break;
                        case "1":
                            item["ENUM_CODE"] = SJ_QCMAPI.Common.enum_barcode_print_type.enum_barcode_print_type_1;
                            break;
                        case "2":
                            item["ENUM_CODE"] = SJ_QCMAPI.Common.enum_barcode_print_type.enum_barcode_print_type_2;
                            break;
                        case "3":
                            item["ENUM_CODE"] = SJ_QCMAPI.Common.enum_barcode_print_type.enum_barcode_print_type_3;
                            break;
                        case "4":
                            item["ENUM_CODE"] = SJ_QCMAPI.Common.enum_barcode_print_type.enum_barcode_print_type_4;
                            break;
                        case "5":
                            item["ENUM_CODE"] = SJ_QCMAPI.Common.enum_barcode_print_type.enum_barcode_print_type_5;
                            break;
                        case "6":
                            item["ENUM_CODE"] = SJ_QCMAPI.Common.enum_barcode_print_type.enum_barcode_print_type_6;
                            break;
                        case "7":
                            item["ENUM_CODE"] = SJ_QCMAPI.Common.enum_barcode_print_type.enum_barcode_print_type_7;
                            break;
                        default:
                            break;
                    }
                }
                Dictionary<string, object> dic = new Dictionary<string, object>();
                dic.Add("data", dt);
                #endregion
                ret.RetData = Newtonsoft.Json.JsonConvert.SerializeObject(dic);
                ret.IsSuccess = true;
            }
            catch (Exception ex)
            {
                ret.IsSuccess = false;
                ret.ErrMsg = "00000:" + ex.Message;

            }

            return ret;

        }

        /// <summary>
        /// 查询打印信息
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetPrintDetailPrint(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);
            //SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase();

            try
            {
                string Data = ReqObj.Data.ToString();
                DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);


                #region 接口参数

                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                string Type = jarr.ContainsKey("Type") ? jarr["Type"].ToString() : "";//条码类型
                string RecordStr = jarr.ContainsKey("RecordStr") ? jarr["RecordStr"].ToString() : "";//代号
                #endregion

                #region 逻辑

                string sql = string.Empty;
                List<string> RecordStrlist = new List<string>();
                if (!string.IsNullOrEmpty(RecordStr))
                {

                    RecordStrlist = RecordStr.Split(',').Select(x => $"'{x}'").ToList();
                    //sql += $" and production_order  IN ({string.Join(',', RecordStrlist)})";
                    //sql += $"{string.Join(',', RecordStrlist)}";
                    
                }
                DataTable dlt = new DataTable();
                dlt.Columns.Add("二维码", typeof(string));
                dlt.Columns.Add("名称", typeof(string));
                if (Type == "设备条码")
                {
                    

                    for (int i = 0; i < 1; i++)
                    {
                        DataRow dr = dlt.NewRow();
                        dr["二维码"] = RecordStr;
                        dr["名称"] = RecordStr;
                        dlt.Rows.Add(dr);
                    }
                }
                else
                {
                    sql = GetSql(Type, string.Join(',', RecordStrlist));
                    DataTable dt1 = DB.GetDataTable(sql);
                    DataRow newRow = dlt.NewRow();
                    
                    if (dt1.Rows.Count > 0)
                    {
                        foreach (DataRow lk in dt1.Rows)
                        {
                            newRow["二维码"] = lk["code"].ToString();
                            newRow["名称"] = lk["name"].ToString();
                            dlt.Rows.Add(newRow.ItemArray);
                        }
                    }
                }

                

                ret.IsSuccess = true;
                ret.RetData = Newtonsoft.Json.JsonConvert.SerializeObject(dlt);


                #endregion

            }
            catch (Exception ex)
            {
                ret.IsSuccess = false;
                ret.ErrMsg = "00000:" + ex.Message;

            }
            return ret;

        }


        public static string GetSql(string Type,string WHERE)
        {
            string sql = string.Empty;
            switch (Type)
            {
                case "材料条码":
                    sql = $@"SELECT ITEM_NO AS code,NAME_S AS name FROM BDM_RD_ITEM WHERE ITEM_NO IN({WHERE})";//料品信息表
                    break;
                case "容器条码":
                    //sql = $@"SELECT CONTAINER_NO AS code,CONTAINER_NAME AS name FROM QCM_CHEMICAL_CONTAINER_M WHERE CONTAINER_NO IN({WHERE})";//化学品容器管理
                    sql = $@"SELECT CONTAINER_NO code,CONTAINER_NAME name FROM BDM_CONTAINERINFORMATION_M where CONTAINER_NO in ({WHERE})";
                    break;
                case "人员条码":
                    sql = $@"SELECT STAFF_NO AS code,STAFF_NAME AS name FROM HR001M WHERE STAFF_NO IN({WHERE})";//HR001M
                    break;
                case "检验工具条码":
                    sql = $@"SELECT INSPECT_TOOL_CODE AS code,INSPECT_TOOL_NAME AS name FROM BDM_INSPECT_TOOL_M WHERE INSPECT_TOOL_CODE IN({WHERE}) ";//检验工具检验单主表
                    break;                                                                 
                case "设备条码":
                    sql = "";
                    break;
                case "产线条码":
                    sql = $@"SELECT PRODUCTIONLINE_NO AS code,PRODUCTIONLINE_NAME AS name FROM BDM_QUALITY_DEPARTMENT_D WHERE PRODUCTIONLINE_NO IN({WHERE})";//部门产线
                    break;
                case "库位条码":
                    sql = $@"SELECT LOCATION_NO AS code,LOCATION_NAME AS name FROM BDM_LABORATORYSAMPLE_LOCATION WHERE LOCATION_NO IN({WHERE}) "; //实验室样品库位
                    break;
                case "样品条码":
                    sql = $@"SELECT ITEM_NO AS code,ITEM_NAME AS name FROM QCM_LABORATORYSAMPLE_STORAGE_M WHERE ITEM_NO IN({WHERE})";//实验室样品存放管理
                    break;
                default:
                    break;
            }
            return sql;
        }

        public static bool WriteTxt(DataTable dt, string ModelName, string Path, int printQty)
        {
            try
            {
                string Data = string.Empty;

                string FilePath = Path + @"\" + ModelName + ".txt";

                foreach (DataColumn dc in dt.Columns)
                {
                    Data += dc.ColumnName + "￥";
                }

                Data = Data.Remove(Data.Length - 1) + "\r\n";

                foreach (DataRow dr in dt.Rows)
                {
                    for (int i = 0; i < printQty; i++)
                    {
                        foreach (DataColumn dc in dt.Columns)
                        {
                            Data += dr[dc].ToString() + "￥";
                        }

                        Data = Data.Remove(Data.Length - 1) + "\r\n";
                    }

                }
                WriteText(Data, FilePath);
                return true;
            }
            catch (Exception ex)
            {

                return false;
            }

        }

        private static void WriteText(string str, string FilePath)
        {
            string fileName = FilePath;

            if (File.Exists(fileName))
            {
                File.Delete(fileName);
            }
            System.IO.File.AppendAllText(fileName, str, Encoding.UTF8);
        }

        public static void p_OutputDataReceived(Object sender, DataReceivedEventArgs e)
        {
            //这里是正常的输出
            Console.WriteLine(e.Data);

        }
        public static void p_ErrorDataReceived(Object sender, DataReceivedEventArgs e)
        {
            //这里得到的是错误信息
            Console.WriteLine(e.Data);

        }
    }
}
