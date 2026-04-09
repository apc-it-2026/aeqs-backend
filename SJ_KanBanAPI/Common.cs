using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace SJ_KanBanAPI
{
    /// <summary>
    /// 数据转换,取值
    /// </summary>
    internal class Common
    {
        /// <summary>
        /// 时间数组
        /// </summary>
        /// <param name="timeType">日期类型：【m】月，【d】日</param>
        /// <param name="startTime">开始时间</param>
        /// <param name="endTime">结束时间</param>
        /// <returns></returns>
        public static List<string> ReturnData(string timeType, string startTime, string endTime)
        {
            List<string> TimeList = new List<string>();
            switch (timeType.ToLower().Trim())
            {
                case "m":
                    startTime = Convert.ToDateTime(startTime).ToString("yyyy-MM");
                    endTime = Convert.ToDateTime(endTime).AddMonths(1).ToString("yyyy-MM");
                    while (startTime != endTime)
                    {
                        TimeList.Add(startTime);
                        startTime = Convert.ToDateTime(startTime).AddMonths(1).ToString("yyyy-MM");
                    }
                    break;
                case "d":
                    startTime = Convert.ToDateTime(startTime).ToString("yyyy-MM-dd");
                    endTime = Convert.ToDateTime(endTime).AddDays(1).ToString("yyyy-MM-dd");
                    while (startTime != endTime)
                    {
                        TimeList.Add(startTime);
                        startTime = Convert.ToDateTime(startTime).AddDays(1).ToString("yyyy-MM-dd");
                    }
                    break;
            }
            return TimeList;
        }

        /// <summary>
        /// 封装柱状图数据，仅适用与两个柱状数据一条折线数据，且必须保证匹配字段唯一
        /// </summary>
        /// <param name="DB"></param>
        /// <param name="TimeList">x轴数组</param>
        /// <param name="Sql">数据查询sql</param>
        /// <param name="DtSelectStr">Dt中匹配x轴字段</param>
        /// <param name="DataStr1">取值字段1</param>
        /// <param name="DataStr2">取值字段2</param>
        /// <param name="DataStr3">取值字段3,当此值为空时采取计算方式</param>
        /// <param name="DSName1">数据标题1</param>
        /// <param name="DSName2">数据标题2</param>
        /// <param name="DSName3">数据标题3</param>
        /// <returns></returns>
        public static Dictionary<string, object> RetunHisData(SJeMES_Framework_NETCore.DBHelper.DataBase DB, List<string> TimeList, string Sql, string DtSelectStr, string DataStr1, string DataStr2, string DataStr3, string DSName1, string DSName2, string DSName3)
        {
            Dictionary<string, object> HisDic = new Dictionary<string, object>();
            HisDic["x"] = TimeList;//x轴
            HisDic["bar"] = null;//柱图
            HisDic["line"] = null;//折线


            DataTable LabMonthDt = DB.GetDataTable(Sql);
            List<decimal> LabPassList = new List<decimal>();//PassList
            List<decimal> LabFailList = new List<decimal>();//FailList
            List<decimal> LabRateList = new List<decimal>();//RateList
            List<Dictionary<string, object>> LabBarDic = new List<Dictionary<string, object>>();
            List<Dictionary<string, object>> LabLinerDic = new List<Dictionary<string, object>>();
            foreach (var item in TimeList)
            {
                decimal PassNum = 0;
                decimal failNum = 0;
                decimal RateNum = 0;
                DataRow[] SelectRow = LabMonthDt.Select($@"{DtSelectStr}='{item}'");
                foreach (DataRow Ritem in SelectRow)
                {
                    PassNum += Convert.ToDecimal(string.IsNullOrEmpty(Ritem[DataStr1].ToString()) ? "0" : Ritem[DataStr1].ToString());
                    failNum += Convert.ToDecimal(string.IsNullOrEmpty(Ritem[DataStr2].ToString()) ? "0" : Ritem[DataStr2].ToString());
                    if (string.IsNullOrEmpty(DataStr3))
                    {
                        if (PassNum + failNum > 0)
                        {
                            RateNum += Math.Round(PassNum / (PassNum + failNum) * 100, 2);
                        }
                    }
                    else
                    {
                        RateNum += Convert.ToDecimal(string.IsNullOrEmpty(Ritem[DataStr3].ToString()) ? "0" : Ritem[DataStr3].ToString());
                    }

                }
                LabPassList.Add(PassNum);
                LabFailList.Add(failNum);
                LabRateList.Add(RateNum);
            }

            //柱状图数据
            Dictionary<string, object> LabPassDic = new Dictionary<string, object>();
            LabPassDic["name"] = DSName1;
            LabPassDic["data"] = LabPassList;
            LabBarDic.Add(LabPassDic);
            Dictionary<string, object> LabFailDic = new Dictionary<string, object>();
            LabFailDic["name"] = DSName2;
            LabFailDic["data"] = LabFailList;
            LabBarDic.Add(LabFailDic);
            HisDic["bar"] = LabBarDic;

            //折线数据
            Dictionary<string, object> RateDic = new Dictionary<string, object>();
            RateDic["name"] = DSName3;
            RateDic["data"] = LabRateList;
            LabLinerDic.Add(RateDic);
            HisDic["line"] = LabLinerDic;//折线
            return HisDic;
        }
        
        /// <summary>
        /// 返回饼图数据
        /// </summary>
        /// <param name="DB"></param>
        /// <param name="SQL">sql</param>
        /// <param name="Type">数据处理方式【0】:sql处理【1】:程序处理 【2】:特殊处理返回前十数据</param>
        /// <param name="NameStr">name取值字段</param>
        /// <param name="ValueStr">value取值字段</param>
        /// <param name="RankingStr">程序序号处理字段</param>
        /// <returns></returns>
        public static List<Dictionary<string, object>> ReturnCakeData(SJeMES_Framework_NETCore.DBHelper.DataBase DB, string SQL, int Type, string NameStr, string ValueStr, string RankingStr,string ui_lan_type="en",string moudle_code = "")
        {
            List<Dictionary<string, object>> CakeDic = new List<Dictionary<string, object>>();
            DataTable CakeDt = DB.GetDataTable(SQL);
            DataView DV = new DataView(CakeDt);
            DV.Sort = $@"{ValueStr} desc";
            CakeDt = DV.ToTable();
            decimal otherNum = 0;
            List<string> list = new List<string>() { };
            foreach (DataRow item in CakeDt.Rows)
            {
                list.Add(item[NameStr].ToString() );
            }
            var LanDic = Common.GetLanguagebyKanBan(ui_lan_type, moudle_code, list);
            foreach (DataRow item in CakeDt.Rows)
            {
                Dictionary<string, object> Dic = new Dictionary<string, object>();
                if (Type>0)
                {
                    int num = 5;
                    if (Type==2)
                    {
                        num = 10;
                    }
                    if (Convert.ToDecimal(item[RankingStr].ToString()) <= num)
                    {
                        decimal Num = Convert.ToDecimal(string.IsNullOrEmpty(item[ValueStr].ToString()) ? "0" : item[ValueStr].ToString());
                        if (Num>0)
                        {
                            Dic["name"] = item[NameStr];
                            Dic["value"] = Num;
                            CakeDic.Add(Dic);
                        }

                    }
                    else if (Type==2)
                    {
                        break;
                    }
                    else
                    {
                        otherNum += Convert.ToDecimal(string.IsNullOrEmpty(item[ValueStr].ToString()) ? "0" : item[ValueStr].ToString());
                    }
                }
                else
                {
                    decimal Num = Convert.ToDecimal(string.IsNullOrEmpty(item[ValueStr].ToString()) ? "0" : item[ValueStr].ToString());
                    if (Num>0)
                    {
                        Dic["name"] = item[NameStr];
                        Dic["value"] = Num;
                        CakeDic.Add(Dic);
                    }
                    
                }

            }
            if (otherNum > 0)
            {
                Dictionary<string, object> Dic = new Dictionary<string, object>();
                //Dic["name"] = "其他";
                Dic["name"] = "Other";
                Dic["value"] = otherNum;
                CakeDic.Add(Dic);
            }
            return CakeDic;
        }


        public static Dictionary<string, object> RetunHisDatagraph(SJeMES_Framework_NETCore.DBHelper.DataBase DB, List<string> TimeList, string Sql, string DtSelectStr, string DataStr1, string DataStr2, string DataStr3, string DSName1, string DSName2, string DSName3)
        {
            Dictionary<string, object> HisDic = new Dictionary<string, object>();
            HisDic["y"] = TimeList;//x轴
            HisDic["bar"] = null;//柱图
            HisDic["line"] = null;//折线


            DataTable LabMonthDt = DB.GetDataTable(Sql);
            List<decimal> LabPassList = new List<decimal>();//PassList
            List<decimal> LabFailList = new List<decimal>();//FailList
            List<decimal> LabRateList = new List<decimal>();//RateList
            List<Dictionary<string, object>> LabBarDic = new List<Dictionary<string, object>>();
            List<Dictionary<string, object>> LabLinerDic = new List<Dictionary<string, object>>();

            foreach (var item in TimeList)
            {
                decimal PassNum = 0;
                decimal failNum = 0;
                decimal RateNum = 0;
                DataRow[] SelectRow = LabMonthDt.Select($@"{DtSelectStr}='{item}'");

                foreach (DataRow Ritem in SelectRow)
                {
                    PassNum += Convert.ToDecimal(string.IsNullOrEmpty(Ritem[DataStr1].ToString()) ? "0" : Ritem[DataStr1].ToString());
                    failNum += Convert.ToDecimal(string.IsNullOrEmpty(Ritem[DataStr2].ToString()) ? "0" : Ritem[DataStr2].ToString());
                    RateNum += Convert.ToDecimal(string.IsNullOrEmpty(Ritem[DataStr3].ToString()) ? "0" : Ritem[DataStr3].ToString());

                    //if (string.IsNullOrEmpty(DataStr3))
                    //{
                    //    if (PassNum + failNum > 0)
                    //    {
                    //        RateNum += Math.Round(PassNum / (PassNum + failNum) * 100, 2);
                    //    }
                    //}
                    //else
                    //{
                    //    RateNum += Convert.ToDecimal(string.IsNullOrEmpty(Ritem[DataStr3].ToString()) ? "0" : Ritem[DataStr3].ToString());
                    //}

                }

                LabPassList.Add(PassNum);
                LabFailList.Add(failNum);
                LabRateList.Add(RateNum);
            }

            //柱状图数据
            Dictionary<string, object> LabPassDic = new Dictionary<string, object>();
            LabPassDic["name"] = DSName1;
            LabPassDic["data"] = LabPassList;
            LabBarDic.Add(LabPassDic);
            Dictionary<string, object> LabFailDic = new Dictionary<string, object>();
            LabFailDic["name"] = DSName2;
            LabFailDic["data"] = LabFailList;
            LabBarDic.Add(LabFailDic);
            //Dictionary<string, object> RateDic = new Dictionary<string, object>();
            //RateDic["name"] = DSName3;
            //RateDic["data"] = LabRateList;
            //LabBarDic.Add(RateDic);
            HisDic["bar"] = LabBarDic;

            //折线数据
            Dictionary<string, object> RateDic = new Dictionary<string, object>();
            RateDic["name"] = DSName3;
            RateDic["data"] = LabRateList;
            LabLinerDic.Add(RateDic);
            HisDic["line"] = LabLinerDic;
            return HisDic;
        }
        /// <summary>
        /// 返回 倒序/顺序 前几得数据
        /// </summary>
        /// <param name="Dt"></param>
        /// <param name="RowName">排序字段名</param>
        /// <param name="OrderType">排序方式</param>
        /// <param name="Num">返回数据行数</param>
        /// <returns></returns>
        public static DataTable ReturnTopNumTable(DataTable Dt,string RowName,string OrderType,int Num)
        {
            DataTable RetDt = Dt.Clone();
            DataView DV = new DataView(Dt);
            DV.Sort = $@"{RowName} {OrderType}";
            Dt = DV.ToTable();

            int index = 0;
            foreach (DataRow item in Dt.Rows)
            {
                RetDt.Rows.Add(item.ItemArray);
                index++;
                if (index>=Num)
                {
                    break;
                }
            }
            return RetDt;
        }


        /// <summary>
        /// 返回 倒序/顺序数据
        /// </summary>
        /// <param name="Dt"></param>
        /// <param name="RowName">排序字段名</param>
        /// <param name="OrderType">排序方式</param>
        /// <param name="Num">返回数据行数</param>
        /// <returns></returns>
        public static DataTable ReturnOrderByTable(DataTable Dt, string RowName, string OrderType)
        {
            DataTable RetDt = Dt.Clone();
            DataView DV = new DataView(Dt);
            DV.Sort = $@"{RowName} {OrderType}";
            RetDt = DV.ToTable();
            return RetDt;
        }
        /// <summary>
        /// 返回 倒序/顺序 前几得数据
        /// </summary>
        /// <param name="Dt"></param>
        /// <param name="RowName">排序字段名</param>
        /// <param name="OrderType">排序方式</param>
        /// <param name="Num">返回数据行数</param>
        /// <returns></returns>
        public static DataTable ReturnTopRowNumTable(DataTable Dt, string RowName, string OrderType, int Num)
        {
            DataTable RetDt = Dt.Clone();
            DataView DV = new DataView(Dt);
            DV.Sort = $@"{RowName} {OrderType}";
            Dt = DV.ToTable();

            int index = 0;
            foreach (DataRow item in Dt.Rows)
            {
                RetDt.Rows.Add(item.ItemArray);
                index++;
                if (index >= Num)
                {
                    break;
                }
            }
            return RetDt;
        }

        public static Dictionary<string,Object> ReturnLineData(DataTable Dt,string YRowName,string DataRowName)
        {
            Dictionary<string, object> RetDic = new Dictionary<string, object>();
            RetDic["y"] = TranData(YRowName,Dt);
            RetDic["data"] = TranData(DataRowName, Dt);
            return RetDic;
        }
        
        /// <summary>
        /// 取一个列的所有数据
        /// </summary>
        /// <param name="RowName">列名</param>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static List<string> TranData(string RowName, DataTable dt)
        {
            List<string> RetList = new List<string>();
            foreach (DataRow item in dt.Rows)
            {
                RetList.Add(item[RowName].ToString());
            }
            return RetList;
        }
        /// <summary>
        /// 取一个列的所有数据(去除重复数据)
        /// </summary>
        /// <param name="RowName">列名</param>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static List<string> DistantTranData(string RowName, DataTable dt)
        {
            List<string> RetList = new List<string>();
            foreach (DataRow item in dt.Rows)
            {
                if (!RetList.Contains(item[RowName].ToString()))
                {
                    RetList.Add(item[RowName].ToString());
                }
            }
            return RetList;
        }

        /// <summary>
        /// 根据数据条件查询
        /// </summary>
        /// <param name="Dt"></param>
        /// <param name="RowNameOrderBYList">查询条件 ： 字段 排序方式【desc/asc】</param>
        /// <returns></returns>
        public static DataTable ReturnDTOrderByRows(DataTable Dt,List<string> RowNameOrderBYList,int NUM,bool IsExport = false)
        {
            DataTable ReturnDt = new DataTable();
            DataView DV = new DataView(Dt);
            DV.Sort = $@"{string.Join(",", RowNameOrderBYList)}";
            Dt = DV.ToTable();
            int index = 0;
            ReturnDt=Dt.Clone();
            foreach (DataRow item in Dt.Rows)
            {
                ReturnDt.Rows.Add(item.ItemArray);
                index++;
                if (index >= NUM)
                {
                    //IsExport为true，则为导出 不限制
                    if (!IsExport)
                        break;
                }
            }
            return ReturnDt;
        }

        /// <summary>
        /// 看板多语言
        /// </summary>
        /// <param name="ui_lan_type">语言</param>
        /// <param name="moudle_code">模块代号</param>
        /// <param name="KeyWords">翻译字段</param>
        /// <returns></returns>
        public static Dictionary<string, object> GetLanguagebyKanBan(string ui_lan_type, string moudle_code, List<string> KeyWords)
        {

            string guid = string.Empty;
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(string.Empty);
            Dictionary<string, object> res = new Dictionary<string, object>();
            DB.Open();
            DB.BeginTransaction();
            try
            {
                if (!string.IsNullOrEmpty(ui_lan_type) && !string.IsNullOrEmpty(moudle_code) && KeyWords.Count > 0)
                {




                    #region 逻辑

                    string isconfig = DB.GetString($@"SELECT 1 FROM [dbo].[SJQDMS_UILAN_APP_D_D] 
WHERE ui_lan_type ='{ui_lan_type}' and moudle_code = '{moudle_code}'
and filed_code in({string.Join(',', KeyWords.Select(x => $"'{x}'"))})");

                    if (string.IsNullOrEmpty(isconfig))
                    {
                        foreach (var item in KeyWords)
                        {
                            Dictionary<string, object> SJQDMS_UILAN_APP_D_D_DIC = new Dictionary<string, object>();
                            SJQDMS_UILAN_APP_D_D_DIC.Add("ui_lan_type", ui_lan_type);
                            SJQDMS_UILAN_APP_D_D_DIC.Add("moudle_code", moudle_code);
                            SJQDMS_UILAN_APP_D_D_DIC.Add("filed_code", item);
                            SJQDMS_UILAN_APP_D_D_DIC.Add("filed_name", item);

                            DB.ExecuteNonQuery($@"
INSERT INTO SJQDMS_UILAN_APP_D_D (ui_lan_type,moudle_code,filed_code,filed_name)
VALUES(@ui_lan_type,@moudle_code,@filed_code,@filed_name)
", SJQDMS_UILAN_APP_D_D_DIC);
                        }
                        DB.Commit();
                    }


                    if (KeyWords.Count > 0)
                    {
                        string sql = $@"SELECT filed_code,filed_name FROM [dbo].[SJQDMS_UILAN_APP_D_D] 
WHERE ui_lan_type ='{ui_lan_type}' and moudle_code = '{moudle_code}'
and filed_code in({string.Join(',', KeyWords.Select(x => $"'{x}'"))})";

                        res = SJ_BASEAPI.BASE.GetDic(DB, sql);

                    }

                    #endregion
                }

            }
            catch (Exception ex)
            {
                DB.Rollback();
                throw new Exception("000:" + ex.Message);
            }
            finally
            {
                DB.Close();
            }
            return res;

        }
    }
}
