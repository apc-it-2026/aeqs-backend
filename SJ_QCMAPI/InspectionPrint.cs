using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;

namespace SJ_QCMAPI
{
    class InspectionPrint
    {
        /// <summary>
        /// 获取打印表头明细
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetPrintDetail(object OBJ)
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
                string INSPECTION_NO = jarr.ContainsKey("INSPECTION_NO") ? jarr["INSPECTION_NO"].ToString() : "";//送检单
                //INSPECTION_NO = "SYD202110141143";
                #endregion

                #region 逻辑

                var sql = $@"
SELECT
	QCM_INSPECTION_LABORATORY_M.INSPECTION_NO,
	QCM_INSPECTION_LABORATORY_M.INSPECTION_DATE,
	QCM_INSPECTION_LABORATORY_M.CATEGORY_NAME,
    QCM_INSPECTION_LABORATORY_M.GENERAL_TESTTYPE_NO ,-- 通用检测代号
	QCM_INSPECTION_LABORATORY_M.ART_CODE,
    QCM_INSPECTION_LABORATORY_M.DEPARTMENT_NAME ,-- 阶段
	QCM_INSPECTION_LABORATORY_M.PLANTAREA_NAME
FROM
	QCM_INSPECTION_LABORATORY_M
WHERE
	1 = 1
AND QCM_INSPECTION_LABORATORY_M.INSPECTION_NO = '{INSPECTION_NO}'";

                DataTable dt = DB.GetDataTable(sql);
                dt.Columns.Add("GENERAL_TESTTYPE_NAME", typeof(string));//检测类型名称

                foreach (DataRow item in dt.Rows)
                {
                    item["GENERAL_TESTTYPE_NAME"] = DB.GetString($@"SELECT GENERAL_TESTTYPE_NAME FROM BDM_GENERAL_TESTTYPE_M WHERE GENERAL_TESTTYPE_NO = '{item["GENERAL_TESTTYPE_NO"].ToString()}'");
                }


                ret.IsSuccess = true;
                ret.RetData = Newtonsoft.Json.JsonConvert.SerializeObject(dt);


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
        /// 获取打印表身明细
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetPrintDetailList(object OBJ)
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
                string INSPECTION_NO = jarr.ContainsKey("INSPECTION_NO") ? jarr["INSPECTION_NO"].ToString() : "";//送检单
                //INSPECTION_NO = "SYD202110141143";
                #endregion

                #region 逻辑

                var sql = $@"
SELECT
	INSPECTION_NO, -- 检验单号
	TESTITEM_CODE, -- 检测项编号
	TESTITEM_NAME, -- 检测项名称
	SAMPLE_NUM, -- 试样数量
	ID AS seq -- 序号
FROM
	QCM_INSPECTION_LABORATORY_D
WHERE
	1 = 1
AND INSPECTION_NO = '{INSPECTION_NO}'";

                DataTable dt = DB.GetDataTable(sql);

                ret.IsSuccess = true;
                ret.RetData = Newtonsoft.Json.JsonConvert.SerializeObject(dt);


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
                string INSPECTION_NO = jarr.ContainsKey("INSPECTION_NO") ? jarr["INSPECTION_NO"].ToString() : "";//送检单
                //INSPECTION_NO = "SYD202110141143";
                #endregion

                #region 逻辑

                var sql = $@"
SELECT
	INSPECTION_NO, -- 检验单号
	TESTITEM_CODE, -- 检测项编号
	TESTITEM_NAME, -- 检测项名称
	SAMPLE_NUM, -- 试样数量
	ID AS seq -- 序号
FROM
	QCM_INSPECTION_LABORATORY_D
WHERE
	1 = 1
AND INSPECTION_NO = '{INSPECTION_NO}'";

                //DataTable dt = DB.GetDataTable(sql);

                DataTable dt1 = DB.GetDataTable(sql);
                DataTable dlt = new DataTable();
                
                dlt.Columns.Add("序号", typeof(string));
                dlt.Columns.Add("检测单编号", typeof(string));
                dlt.Columns.Add("检测项编号", typeof(string));
                dlt.Columns.Add("检测项名称", typeof(string));
                dlt.Columns.Add("试样数量", typeof(string));
                dlt.Columns.Add("打印二维码", typeof(string));
                int index = 0;
                DataRow newRow = dlt.NewRow();
                int count = 0;
                foreach (DataRow item in dt1.Rows)
                {
                    count += Convert.ToInt32(item["SAMPLE_NUM"].ToString());
                }
                int id = 0;
                if (dt1.Rows.Count > 0)
                {
                    foreach (DataRow lk in dt1.Rows)
                    {
                        index = 0;
                        for (int i = 0; i < Convert.ToInt32(lk["SAMPLE_NUM"].ToString()) ; i++)
                        {
                            id = id + 1;
                            index = i + 1;
                            newRow["序号"] = id;//lk["SEQ"];
                            newRow["检测单编号"] = lk["INSPECTION_NO"].ToString();
                            newRow["检测项编号"] = lk["TESTITEM_CODE"].ToString() + "-" +  (i + 1);
                            newRow["检测项名称"] = lk["TESTITEM_NAME"].ToString();
                            newRow["试样数量"] = lk["SAMPLE_NUM"].ToString();
                            //newRow["打印二维码"] = lk["SEQ"].ToString() + "@" + lk["INSPECTION_NO"].ToString() + "@" + (lk["TESTITEM_CODE"].ToString() +i);
                            newRow["打印二维码"] = lk["INSPECTION_NO"].ToString() + "@" + (lk["TESTITEM_CODE"].ToString() +"@"+ index);
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
