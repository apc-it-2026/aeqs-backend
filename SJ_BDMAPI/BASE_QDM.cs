using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace SJ_BDMAPI
{
    public class BASE_QDM
    {
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetDataTable(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(string.Empty);
            try
            {
                string Data = ReqObj.Data.ToString();
                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                var companyCode = SJeMES_Framework_NETCore.Web.System.GetCompanyCodeByToken(ReqObj.UserToken);
                string sql = jarr.ContainsKey("sql") ? jarr["sql"].ToString() : "";

                var qdmDt = DB.GetDataTable($@"
SELECT
	qdmdbtype,
	qdmdbserver,
	qdmdbname,
	qdmdbuser,
	qdmdbpassword
FROM
	[dbo].[SYSORG01M]
WHERE
	UPPER (org) = '{companyCode.ToUpper()}';");
                if (qdmDt.Rows.Count > 0)
                {
                    var qdmRow = qdmDt.Rows[0];
                    SJeMES_Framework_NETCore.DBHelper.DataBase qdmDB = new SJeMES_Framework_NETCore.DBHelper.DataBase(qdmRow["qdmdbtype"].ToString(), qdmRow["qdmdbserver"].ToString(), qdmRow["qdmdbname"].ToString(), qdmRow["qdmdbuser"].ToString(), qdmRow["qdmdbpassword"].ToString(), "");
                    var res = qdmDB.GetDataTable(sql);

                    ret.RetData = Newtonsoft.Json.JsonConvert.SerializeObject(res);
                    ret.IsSuccess = true;
                }
                else
                {
                    ret.IsSuccess = false;
                    ret.ErrMsg = "qdm connection failed";
                }

            }
            catch (Exception ex)
            {
                ret.IsSuccess = false;
                ret.ErrMsg = ex.Message;
            }

            return ret;
        }

        /// <summary>
        /// 自动生成aql任务
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public SJeMES_Framework_NETCore.WebAPI.ResultObject AutoGenerateAqlTask(object OBJ)
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

                string sql = $@"
                                    SELECT
	                                    A.PO
                                    FROM
	                                    AQL_TRANSFER_RECORD A
                                    INNER JOIN BDM_SE_ORDER_MASTER B ON B.MER_PO=A.PO
                                    WHERE
	                                    A.IS_GENERATE IN ('0', '1')
                                    GROUP BY A.PO 
                                ";
                // 查询有 “未生成任务”，“未生产完任务”的po号
                int total = Convert.ToInt32(DB.GetStringline($@"SELECT count(1) FROM ({sql})"));
                int pageSize = 1000;//每页数量
                int pageCount = total % pageSize > 0 ? total / pageSize + 1 : total / pageSize; //总页数

                // 分页循环po生成任务
                for (int i = 1; i <= pageCount; i++)
                {
                    int calTotal = (i - 1) * pageSize;
                    string getPageSql = $@"
                                    select * from (
                                        select M.*,ROWNUM as RN from ({sql}) M
                                        where 1=1 
                                    ) tab
                                    where RN between {calTotal + 1} and {calTotal + Convert.ToInt32(pageSize)}";
                    // 获取分页po集合
                    DataTable poDt = DB.GetDataTablebyline(getPageSql);
                    if (poDt.Rows.Count > 0)
                    {
                        List<string> poList = new List<string>();
                        foreach (DataRow item in poDt.Rows)
                        {
                            poList.Add($@"'{item["PO"]}'");
                        }

                        string poSizeSql = $@"
                                            SELECT
	                                            M.MER_PO,
	                                            S.CR_SIZE,
	                                            SUM(S.SE_QTY) SE_QTY
                                            FROM
	                                            BDM_SE_ORDER_MASTER M
                                            INNER JOIN BDM_SE_ORDER_SIZE S ON S.ORG_ID=M.ORG_ID AND S.SE_ID=M.SE_ID 
                                            WHERE M.MER_PO IN({string.Join(",", poList)}) 
                                            GROUP BY M.MER_PO,S.CR_SIZE
                                            ";
                        // 获取分页po集合的size明细数据
                        DataTable sizeDt = DB.GetDataTablebyline(poSizeSql);
                        var sizeList = sizeDt.ToDataList<PoSizeDto>();
                        var groupSizeList = sizeList.GroupBy(x => x.MER_PO).ToList();
                        var poTotalCount = groupSizeList.Select(x => new { po = x.Key, qty = x.Sum(y => y.SE_QTY) }).ToList();

                        string poDetailSql = $@"SELECT * FROM AQL_TRANSFER_RECORD WHERE PO IN({string.Join(",", poList)})";
                        // 获取分页po集合的进出库存记录数据
                        DataTable poDetailDt = DB.GetDataTablebyline(poDetailSql);
                        List<AqlTransferRecordDto> poDetailList = poDetailDt.ToDataList<AqlTransferRecordDto>();

                        // 根据po 分组
                        var poDetailListGroupByPo = poDetailList.GroupBy(x => x.PO).ToList();
                        // 扣除每个po已使用的size数量!!!
                        foreach (var item in poDetailListGroupByPo)
                        {
                            var currPoSize = groupSizeList.FirstOrDefault(x => x.Key == item.Key);
                            if (currPoSize == null)
                                continue;
                            var currPoDetail = item.OrderBy(x => x.INSERT_TIME).ToList();
                            foreach (var currPoItem in currPoDetail)
                            {
                                if (currPoItem.IS_GENERATE == "0")
                                    break;
                                else if (currPoItem.IS_GENERATE == "2")
                                {
                                    var currSize = currPoSize.FirstOrDefault(x => x.CR_SIZE == currPoItem.SIZE_CODE);
                                    if (currSize == null)
                                        continue;
                                    if (currPoItem.WAREHOUSING_STATE == "0")
                                        currSize.SE_QTY = currSize.SE_QTY - currPoItem.QUANTITY;
                                    else if (currPoItem.WAREHOUSING_STATE == "1")
                                        currSize.SE_QTY = currSize.SE_QTY + currPoItem.QUANTITY;
                                }
                                else if (currPoItem.IS_GENERATE == "1")
                                {
                                    var currSize = currPoSize.FirstOrDefault(x => x.CR_SIZE == currPoItem.SIZE_CODE);
                                    if (currSize == null)
                                        continue;
                                    if (currPoItem.WAREHOUSING_STATE == "0")
                                        currSize.SE_QTY = currSize.SE_QTY - currPoItem.USE_QUANTITY;
                                    else if (currPoItem.WAREHOUSING_STATE == "1")
                                        currSize.SE_QTY = currSize.SE_QTY + currPoItem.USE_QUANTITY;
                                }
                            }
                        }

                        string task_no = "";
                        // 开始生成po任务
                        foreach (var item in poDetailListGroupByPo)
                        {
                            var currPoSizeList = groupSizeList.FirstOrDefault(x => x.Key == item.Key);
                            if (currPoSizeList == null)
                                continue;
                            // po基本信息
                            var currPoSize = poTotalCount.FirstOrDefault(x => x.po == item.Key);
                            if (currPoSize == null)
                                continue;
                            decimal poTotalQty = currPoSize.qty;
                            int order_level = 0;//0：大制令 1：小制令
                            if (poTotalQty < 3200)
                                order_level = 1;
                            string po = item.Key;
                            string art_no = "";
                            if (item.Count() > 0)
                                art_no = item.Max(x => x.ART);
                            else
                                continue;

                            // po根据po，工厂，仓库 分组 来生成任务
                            var groupPoDetailList = item.GroupBy(x => new { x.PO, x.FACTORY, x.WAREHOUSE }).ToList();
                            Dictionary<string, List<GenerateAqlDto>> surplusDic = new Dictionary<string, List<GenerateAqlDto>>();
                            foreach (var groupByPFWItem in groupPoDetailList)
                            {
                                var currPFWDetailGroupBySize = groupByPFWItem.Where(x => x.IS_GENERATE == "0" || x.IS_GENERATE == "1").GroupBy(x => x.SIZE_CODE).ToList();

                                Dictionary<string, List<GenerateAqlDto>> sizesDetailDic = new Dictionary<string, List<GenerateAqlDto>>();
                                foreach (var sizeGroupItem in currPFWDetailGroupBySize)
                                {
                                    //获取此码数进仓的最后一条数据id
                                    var finalObj = sizeGroupItem.Where(x => x.WAREHOUSING_STATE == "0").OrderByDescending(x => x.INSERT_TIME).FirstOrDefault();
                                    if (finalObj != null)
                                    {
                                        decimal groupQty = 0;
                                        int finalID = finalObj.ID;
                                        //1.复制到另一个新集合
                                        List<GenerateAqlDto> groupPFWSize = new List<GenerateAqlDto>();
                                        foreach (var currPFWItem in sizeGroupItem.OrderBy(x => x.INSERT_TIME))
                                        {
                                            var add = new GenerateAqlDto()
                                            {
                                                ID = currPFWItem.ID,
                                                ART = currPFWItem.ART,
                                                SIZE_CODE = currPFWItem.SIZE_CODE,
                                                QUANTITY = currPFWItem.QUANTITY,
                                                IS_GENERATE = currPFWItem.IS_GENERATE,
                                                WAREHOUSING_STATE = currPFWItem.WAREHOUSING_STATE,
                                                TASK_NO = currPFWItem.TASK_NO,
                                                USE_QUANTITY = currPFWItem.USE_QUANTITY,
                                                INSERT_TIME = currPFWItem.INSERT_TIME
                                            };
                                            groupPFWSize.Add(add);
                                            if (add.WAREHOUSING_STATE == "0")
                                            {
                                                decimal aQty = add.QUANTITY - add.USE_QUANTITY;
                                                groupQty += aQty;
                                            }
                                            else if (add.WAREHOUSING_STATE == "1")
                                            {
                                                groupQty -= add.QUANTITY;
                                            }
                                            if (currPFWItem.ID == finalID)
                                                break;
                                        }
                                        if (groupPFWSize.Count > 0 && groupQty > 0)
                                        {
                                            sizesDetailDic.Add(sizeGroupItem.Key, groupPFWSize);
                                        }
                                    }
                                }
                                // 生成当前po，工厂，仓库分组的任务,并且返回剩余没生成的
                                var shenyuSizeDetail = GenerateAqlTask(DB, sizesDetailDic, task_no, po, order_level, art_no, poTotalQty, currPoSizeList);
                                var aShenyuSizeDetail = shenyuSizeDetail.Where(x => x.IS_GENERATE == "0" || x.IS_GENERATE == "1").OrderBy(x => x.INSERT_TIME).ToList();
                                if (aShenyuSizeDetail.Count > 0)
                                    surplusDic.Add($@"{groupByPFWItem.Key.PO}${groupByPFWItem.Key.FACTORY}${groupByPFWItem.Key.WAREHOUSE}", aShenyuSizeDetail);
                            }

                            //判断此po剩余size
                            if (surplusDic.Count() > 0)
                            {
                                Dictionary<string, decimal> surplusSizeDic = new Dictionary<string, decimal>();
                                foreach (var surplusItem in surplusDic)
                                {
                                    foreach (var surplusSubItem in surplusItem.Value)
                                    {
                                        if (surplusSubItem.WAREHOUSING_STATE == "0")
                                        {
                                            var aQty = surplusSubItem.QUANTITY - surplusSubItem.USE_QUANTITY;
                                            if (surplusSizeDic.ContainsKey(surplusItem.Key))
                                            {
                                                surplusSizeDic[surplusSubItem.SIZE_CODE] += aQty;
                                            }
                                            else
                                            {
                                                surplusSizeDic.Add(surplusSubItem.SIZE_CODE, aQty);
                                            }
                                        }
                                        else if (surplusSubItem.WAREHOUSING_STATE == "1")
                                        {
                                            if (surplusSizeDic.ContainsKey(surplusItem.Key))
                                            {
                                                surplusSizeDic[surplusSubItem.SIZE_CODE] -= surplusSubItem.QUANTITY;
                                            }
                                            else
                                            {
                                                surplusSizeDic.Add(surplusSubItem.SIZE_CODE, -surplusSubItem.QUANTITY);
                                            }
                                        }
                                    }
                                }

                                bool isOk = true;
                                foreach (var surplusSize in surplusSizeDic)
                                {
                                    var findSize = currPoSizeList.FirstOrDefault(x => x.CR_SIZE == surplusSize.Key);
                                    if (findSize == null)
                                    {
                                        isOk = false;
                                        break;
                                    }
                                    else
                                    {
                                        if(findSize.SE_QTY!= surplusSize.Value)
                                        {
                                            isOk = false;
                                            break;
                                        }
                                    }
                                }

                                //满足剩余生成任务条件
                                if (isOk)
                                {
                                    string user = "auto_user";
                                    foreach (var surplusSize in surplusDic)
                                    {
                                        var currDate = DateTime.Now;
                                        string date = currDate.ToString("yyyy-MM-dd");
                                        string time = currDate.ToString("HH:mm:ss");

                                        string fSql = "";
                                        task_no = GetAutoGenerateAqlTaskNo(task_no, DB);
                                        Dictionary<string, decimal> keyValuePairs = new Dictionary<string, decimal>();
                                        // 更新出入记录状态，剩余数量
                                        for (int k = 0; k < surplusSize.Value.Count; k++)
                                        {
                                            var currGenerateAqlTaskListCanNew = surplusSize.Value[k];

                                            if (currGenerateAqlTaskListCanNew.WAREHOUSING_STATE == "0")
                                            {
                                                decimal thisUseQty = currGenerateAqlTaskListCanNew.USE_QUANTITY;
                                                currGenerateAqlTaskListCanNew.USE_QUANTITY = currGenerateAqlTaskListCanNew.QUANTITY;
                                                currGenerateAqlTaskListCanNew.IS_GENERATE = "2";
                                                if (keyValuePairs.ContainsKey(currGenerateAqlTaskListCanNew.SIZE_CODE))
                                                {
                                                    keyValuePairs.Add(currGenerateAqlTaskListCanNew.SIZE_CODE, currGenerateAqlTaskListCanNew.USE_QUANTITY - thisUseQty);
                                                }
                                                else
                                                {
                                                    keyValuePairs[currGenerateAqlTaskListCanNew.SIZE_CODE] += (currGenerateAqlTaskListCanNew.USE_QUANTITY - thisUseQty);
                                                }
                                            }
                                            else if (currGenerateAqlTaskListCanNew.WAREHOUSING_STATE == "1")
                                            {
                                                currGenerateAqlTaskListCanNew.IS_GENERATE = "2";
                                                if (keyValuePairs.ContainsKey(currGenerateAqlTaskListCanNew.SIZE_CODE))
                                                {
                                                    keyValuePairs.Add(currGenerateAqlTaskListCanNew.SIZE_CODE, -currGenerateAqlTaskListCanNew.QUANTITY);
                                                }
                                                else
                                                {
                                                    keyValuePairs[currGenerateAqlTaskListCanNew.SIZE_CODE] -= currGenerateAqlTaskListCanNew.QUANTITY;
                                                }
                                            }
                                            sql += $@"UPDATE AQL_TRANSFER_RECORD SET TASK_NO=TASK_NO||'{task_no}',IS_GENERATE='{currGenerateAqlTaskListCanNew.IS_GENERATE}',USE_QUANTITY='{currGenerateAqlTaskListCanNew.USE_QUANTITY}' WHERE ID={currGenerateAqlTaskListCanNew.ID};";
                                        }

                                        //初始化点箱数据
                                        foreach (var pointItem in keyValuePairs)
                                        {
                                            fSql += $@"insert into aql_cma_task_list_m_pb (task_no,cr_size,SE_QTY,createby,createdate,createtime) 
                            values('{task_no}','{pointItem.Key}','{pointItem.Value}','{user}','{date}','{time}');";
                                        }

                                        //任务表头生成
                                        fSql += $@"insert into aql_cma_task_list_m (task_no,po,art_no,po_num,lot_num,order_level,full_state,inspection_state,
                        task_type,createby,createdate,createtime) 
                        values('{task_no}','{po}','{art_no}','{po}','3200','{order_level}','1','0','0','{user}','{date}','{time}');";

                                        if (!string.IsNullOrEmpty(fSql))
                                        {
                                            string bSql = $@"
                                                        begin
                                                        {fSql}
                                                        end;
                                                        ";
                                            DB.ExecuteNonQuery(bSql);
                                        }
                                    }
                                }

                            }

                        }

                    }
                }


                DB.Commit();
                ret.IsSuccess = true;
                ret.RetData = "aql出货生成成功";

            }
            catch (Exception ex)
            {
                DB.Rollback();
                ret.ErrMsg = "Aql shipment generation failed, the reason：" + ex.Message;
                ret.IsSuccess = false;
            }
            finally
            {
                DB.Close();
            }
            return ret;
        }

        public string GetAutoGenerateAqlTaskNo(string task_no, SJeMES_Framework_NETCore.DBHelper.DataBase DB)
        {

            if (string.IsNullOrEmpty(task_no))
            {
                task_no = DB.GetStringline($@"SELECT task_no FROM(select task_no from aql_cma_task_list_m where task_no like '{"Z" + DateTime.Now.ToString("yyyyMMdd")}%'  order by id desc)t WHERE ROWNUM <= 1");
                if (string.IsNullOrWhiteSpace(task_no))
                {
                    task_no = "Z" + DateTime.Now.ToString("yyyyMMdd") + "1";
                }
                else
                {
                    task_no = $@"Z{DateTime.Now.ToString("yyyyMMdd")}" + (Convert.ToInt32(task_no.Replace($@"S{DateTime.Now.ToString("yyyyMMdd")}", "")) + 1);
                }
            }
            else
            {
                task_no = $@"Z{DateTime.Now.ToString("yyyyMMdd")}" + (Convert.ToInt32(task_no.Replace($@"S{DateTime.Now.ToString("yyyyMMdd")}", "")) + 1);
            }
            return task_no;
        }

        /// <summary>
        /// 生成aql cma任务，根据po、工厂、仓库 来生成
        /// </summary>
        /// <param name="DB"></param>
        /// <param name="sizesDetailDic"></param>
        /// <param name="task_no"></param>
        /// <param name="po"></param>
        /// <param name="order_level"></param>
        /// <param name="art_no"></param>
        /// <param name="po_num"></param>
        public List<GenerateAqlDto> GenerateAqlTask(SJeMES_Framework_NETCore.DBHelper.DataBase DB, Dictionary<string, List<GenerateAqlDto>> sizesDetailDic, string task_no, string po, int order_level, string art_no, decimal po_num,IGrouping<string ,PoSizeDto> poSizeDtos)
        {
            decimal generateQty = 3200;
            string user = "auto_user";

            List<GenerateAqlDto> generateAqlTaskList = new List<GenerateAqlDto>();
            Dictionary<string, List<GenerateAqlDto>> generateAqlTaskList2 = new Dictionary<string, List<GenerateAqlDto>>();
            //循环每个码数的明细进 generateAqlTaskList
            foreach (var sizeItem in sizesDetailDic)
            {
                foreach (var item in sizeItem.Value)
                {
                    var add = new GenerateAqlDto()
                    {
                        ID = item.ID,
                        SIZE_CODE = item.SIZE_CODE,
                        QUANTITY = item.QUANTITY,
                        IS_GENERATE = item.IS_GENERATE,
                        WAREHOUSING_STATE = item.WAREHOUSING_STATE,
                        TASK_NO = item.TASK_NO,
                        USE_QUANTITY = item.USE_QUANTITY,
                        INSERT_TIME = item.INSERT_TIME
                    };
                    generateAqlTaskList.Add(add);
                }
            }


            //生成任务
            List<GenerateAqlDto> generateAqlTaskListCan = generateAqlTaskList.Where(x => x.IS_GENERATE == "0" || x.IS_GENERATE == "1").OrderBy(x => x.INSERT_TIME).ToList();
            bool isContinue = generateAqlTaskListCan.Count() > 0 ? true : false;
            while (isContinue)
            {
                decimal leijiQty = 0;
                for (int i = 0; i < generateAqlTaskListCan.Count(); i++)
                {
                    var currObj = generateAqlTaskListCan[i];

                    if (currObj.WAREHOUSING_STATE == "0")
                    {
                        var aQty = currObj.QUANTITY - currObj.USE_QUANTITY;
                        leijiQty += aQty;
                    }
                    else if (currObj.WAREHOUSING_STATE == "1")
                    {
                        leijiQty -= currObj.QUANTITY;
                    }

                    //满足生成数量，开始生成任务
                    if (leijiQty >= generateQty)
                    {
                        task_no = GetAutoGenerateAqlTaskNo(task_no, DB);
                        var currDate = DateTime.Now;
                        string date = currDate.ToString("yyyy-MM-dd");
                        string time = currDate.ToString("HH:mm:ss");
                        decimal ggggQty = 0;//触发生成标记点
                        for (int j = 0; j < generateAqlTaskListCan.Count; j++)
                        {
                            var currGenerateAqlTaskListCan = generateAqlTaskListCan[j];
                            if (currGenerateAqlTaskListCan.WAREHOUSING_STATE == "0")
                            {
                                decimal aQty = currGenerateAqlTaskListCan.QUANTITY - currGenerateAqlTaskListCan.USE_QUANTITY;
                                ggggQty += aQty;
                            }
                            else if (currGenerateAqlTaskListCan.WAREHOUSING_STATE == "1")
                            {
                                ggggQty -= currGenerateAqlTaskListCan.QUANTITY;
                            }

                            if (ggggQty >= generateQty)
                            {
                                string sql = "";
                                task_no = GetAutoGenerateAqlTaskNo(task_no, DB);
                                decimal outQty = ggggQty - generateQty;//超出3200的数量
                                Dictionary<string, decimal> keyValuePairs = new Dictionary<string, decimal>();
                                // 更新出入记录状态，剩余数量
                                for (int k = j; k >= 0; k--)
                                {
                                    var currGenerateAqlTaskListCanNew = generateAqlTaskListCan[k];

                                    if (currGenerateAqlTaskListCanNew.WAREHOUSING_STATE == "0")
                                    {
                                        decimal thisUseQty = currGenerateAqlTaskListCanNew.USE_QUANTITY;//已使用数量
                                        if (k == j)
                                        {
                                            currGenerateAqlTaskListCanNew.USE_QUANTITY = currGenerateAqlTaskListCanNew.QUANTITY - outQty;
                                        }
                                        else
                                        {
                                            currGenerateAqlTaskListCanNew.USE_QUANTITY = currGenerateAqlTaskListCanNew.QUANTITY;
                                        }
                                        if (currGenerateAqlTaskListCanNew.USE_QUANTITY == currGenerateAqlTaskListCanNew.QUANTITY)
                                        {
                                            currGenerateAqlTaskListCanNew.IS_GENERATE = "2";
                                        }
                                        else
                                        {
                                            currGenerateAqlTaskListCanNew.IS_GENERATE = "1";
                                        }
                                        if (keyValuePairs.ContainsKey(currGenerateAqlTaskListCanNew.SIZE_CODE))
                                        {
                                            keyValuePairs.Add(currGenerateAqlTaskListCanNew.SIZE_CODE, currGenerateAqlTaskListCanNew.USE_QUANTITY - thisUseQty);
                                        }
                                        else
                                        {
                                            keyValuePairs[currGenerateAqlTaskListCanNew.SIZE_CODE] += (currGenerateAqlTaskListCanNew.USE_QUANTITY - thisUseQty);
                                        }
                                    }
                                    else if (currGenerateAqlTaskListCanNew.WAREHOUSING_STATE == "1")
                                    {
                                        currGenerateAqlTaskListCanNew.IS_GENERATE = "2";
                                        if (keyValuePairs.ContainsKey(currGenerateAqlTaskListCanNew.SIZE_CODE))
                                        {
                                            keyValuePairs.Add(currGenerateAqlTaskListCanNew.SIZE_CODE, -currGenerateAqlTaskListCanNew.QUANTITY);
                                        }
                                        else
                                        {
                                            keyValuePairs[currGenerateAqlTaskListCanNew.SIZE_CODE] -= currGenerateAqlTaskListCanNew.QUANTITY;
                                        }
                                    }
                                    sql += $@"UPDATE AQL_TRANSFER_RECORD SET TASK_NO=TASK_NO||'{task_no}',IS_GENERATE='{currGenerateAqlTaskListCanNew.IS_GENERATE}',USE_QUANTITY='{currGenerateAqlTaskListCanNew.USE_QUANTITY}' WHERE ID={currGenerateAqlTaskListCanNew.ID};";
                                }

                                //初始化点箱数据
                                foreach (var pointItem in keyValuePairs)
                                {
                                    sql += $@"insert into aql_cma_task_list_m_pb (task_no,cr_size,SE_QTY,createby,createdate,createtime) 
                            values('{task_no}','{pointItem.Key}','{pointItem.Value}','{user}','{date}','{time}');";
                                    var findSize = poSizeDtos.FirstOrDefault(x => x.CR_SIZE == pointItem.Key);
                                    findSize.SE_QTY -= pointItem.Value;
                                }

                                //任务表头生成
                                sql += $@"insert into aql_cma_task_list_m (task_no,po,art_no,po_num,lot_num,order_level,full_state,inspection_state,
                        task_type,createby,createdate,createtime) 
                        values('{task_no}','{po}','{art_no}','{po_num}','3200','{order_level}','1','0','0','{user}','{date}','{time}');";

                                if (!string.IsNullOrEmpty(sql))
                                {
                                    string bSql = $@"
                                                        begin
                                                        {sql}
                                                        end;
                                                        ";
                                    DB.ExecuteNonQuery(bSql);
                                }
                            }
                        }

                        generateAqlTaskListCan = generateAqlTaskListCan.Where(x => x.IS_GENERATE == "0" || x.IS_GENERATE == "1").OrderBy(x => x.INSERT_TIME).ToList();
                        if (generateAqlTaskListCan.Count() > 0)
                        {
                            isContinue = true;
                            break;
                        }
                        else
                        {
                            isContinue = false;
                            break;
                        }
                    }

                    // 正常走到最后一个循环也没生成任务，
                    if (generateAqlTaskListCan.Count() == i)
                    {
                        isContinue = false;
                    }
                }
            }

            return generateAqlTaskListCan;
        }


        public SJeMES_Framework_NETCore.WebAPI.ResultObject chaojiwudidashabi(object OBJ)
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


                //                DB.ExecuteNonQuery(bSql);
                DB.Commit();

                ret.IsSuccess = true;
                ret.RetData = "aql出货生成成功";

            }
            catch (Exception ex)
            {
                DB.Rollback();
                ret.ErrMsg = "Aql shipment generation failed, the reason：" + ex.Message;
                ret.IsSuccess = false;
            }
            finally
            {
                DB.Close();
            }
            return ret;
        }

    }

    #region 自动生成aql任务 dto
    public class AqlTransferRecordDto
    {
        public int ID { get; set; }
        public string PO { get; set; }
        public string ART { get; set; }
        /// <summary>
        /// 0：进仓；1：出仓；
        /// </summary>
        public string WAREHOUSING_STATE { get; set; }
        /// <summary>
        /// 码数
        /// </summary>
        public string SIZE_CODE { get; set; }
        /// <summary>
        /// 进/出数量
        /// </summary>
        public decimal QUANTITY { get; set; }
        /// <summary>
        /// 工厂
        /// </summary>
        public string FACTORY { get; set; }
        /// <summary>
        /// 仓库
        /// </summary>
        public string WAREHOUSE { get; set; }
        /// <summary>
        /// 0：未生成任务；1：生成部分任务；2：已完全生成任务
        /// </summary>
        public string IS_GENERATE { get; set; }
        /// <summary>
        /// 插入记录时间
        /// </summary>
        public string INSERT_TIME { get; set; }
        public string TASK_NO { get; set; }
        public decimal USE_QUANTITY { get; set; }
    }

    public class PoSizeDto
    {
        /// <summary>
        /// po
        /// </summary>
        public string MER_PO { get; set; }
        /// <summary>
        /// 码数
        /// </summary>
        public string CR_SIZE { get; set; }
        /// <summary>
        /// 码数 数量
        /// </summary>
        public decimal SE_QTY { get; set; }
    }


    public class GenerateAqlDto
    {
        public int ID { get; set; }
        public string PO { get; set; }
        public string ART { get; set; }
        /// <summary>
        /// 0：进仓；1：出仓；
        /// </summary>
        public string WAREHOUSING_STATE { get; set; }
        /// <summary>
        /// 码数
        /// </summary>
        public string SIZE_CODE { get; set; }
        /// <summary>
        /// 进/出数量
        /// </summary>
        public decimal QUANTITY { get; set; }
        /// <summary>
        /// 工厂
        /// </summary>
        public string FACTORY { get; set; }
        /// <summary>
        /// 仓库
        /// </summary>
        public string WAREHOUSE { get; set; }
        /// <summary>
        /// 0：未生成任务；1：生成部分任务；2：已完全生成任务
        /// </summary>
        public string IS_GENERATE { get; set; }
        /// <summary>
        /// 插入记录时间
        /// </summary>
        public string INSERT_TIME { get; set; }
        public string TASK_NO { get; set; }
        public decimal USE_QUANTITY { get; set; }
    }

    public class TaskGDto
    {
        public string PO { get; set; }
        public string FACTORY { get; set; }
        public string WAREHOUSE { get; set; }
        public List<GenerateAqlDto> Details { get; set; }
    }

    #endregion
}
