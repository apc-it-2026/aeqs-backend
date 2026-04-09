using System;
using System.Collections.Generic;
using System.Text;

namespace SJ_AQLAPI.common
{
    public class Enum
    {
        public class Atype
        {
            //0：验货证明 1：正式订单 2：订单包装材料预算 3：特殊包装 4：样品单 5：A-01报告 6：FGT报告 7：拉力测试结果 8：CPSIA 9：vegan 10：客户国家特殊要求 11：FD/VS 12：MCS 13：SHAS 14：量产 15：仓库 16：CMA合格 17：UV-C处理 18：防霉包装纸 19：特殊的外观标准 20：工厂免责声明 21：FIT 22：防霉
            /// <summary>
            /// 验货证明
            /// </summary>
            public const string typekey = "0";
            /// <summary>
            /// 验货证明
            /// </summary>
            public const string typekeyname = "验货证明";
            /// <summary>
            /// 正式订单
            /// </summary>
            public const string typekey1 = "1";
            /// <summary>
            /// 正式订单
            /// </summary>
            public const string typekeyname1 = "正式订单";
            /// <summary>
            /// 订单包装材料预算
            /// </summary>
            public const string typekey2 = "2";
            /// <summary>
            /// 订单包装材料预算
            /// </summary>
            public const string typekeyname2 = "订单包装材料预算";
            /// <summary>
            /// 特殊包装
            /// </summary>
            public const string typekey3 = "3";
            /// <summary>
            /// 特殊包装
            /// </summary>
            public const string typekeyname3 = "特殊包装";
            /// <summary>
            /// 样品单
            /// </summary>
            public const string typekey4 = "4";
            /// <summary>
            /// 样品单
            /// </summary>
            public const string typekeyname4 = "样品单";
            /// <summary>
            /// A-01报告
            /// </summary>
            public const string typekey5 = "5";
            /// <summary>
            /// A-01报告
            /// </summary>
            public const string typekeyname5 = "A-01报告";
            /// <summary>
            /// FGT报告
            /// </summary>
            public const string typekey6 = "6";
            /// <summary>
            /// FGT报告
            /// </summary>
            public const string typekeyname6 = "FGT报告";
            /// <summary>
            /// 拉力测试结果
            /// </summary>
            public const string typekey7 = "7";
            /// <summary>
            /// 拉力测试结果
            /// </summary>
            public const string typekeyname7 = "拉力测试结果";
            /// <summary>
            /// CPSIA
            /// </summary>
            public const string typekey8 = "8";
            /// <summary>
            /// CPSIA
            /// </summary>
            public const string typekeyname8 = "CPSIA";
            /// <summary>
            /// vegan
            /// </summary>
            public const string typekey9 = "9";
            /// <summary>
            /// vegan
            /// </summary>
            public const string typekeyname9 = "vegan";
            /// <summary>
            /// 客户国家特殊要求
            /// </summary>
            public const string typekey10 = "10";
            /// <summary>
            /// 客户国家特殊要求
            /// </summary>
            public const string typekeyname10 = "客户国家特殊要求";
            /// <summary>
            /// FD/VS
            /// </summary>
            public const string typekey11 = "11";
            /// <summary>
            /// FD/VS
            /// </summary>
            public const string typekeyname11 = "FD/VS";
            /// <summary>
            /// MCS
            /// </summary>
            public const string typekey12 = "12";
            /// <summary>
            /// MCS
            /// </summary>
            public const string typekeyname12 = "MCS";
            /// <summary>
            /// SHAS
            /// </summary>
            public const string typekey13 = "13";
            /// <summary>
            /// SHAS
            /// </summary>
            public const string typekeyname13 = "SHAS";
            /// <summary>
            /// 量产
            /// </summary>
            public const string typekey14 = "14";
            /// <summary>
            /// 量产
            /// </summary>
            public const string typekeyname14 = "量产";
            /// <summary>
            /// 仓库
            /// </summary>
            public const string typekey15 = "15";
            /// <summary>
            /// 仓库
            /// </summary>
            public const string typekeyname15 = "仓库";
            /// <summary>
            /// CMA合格
            /// </summary>
            public const string typekey16 = "16";
            /// <summary>
            /// CMA合格
            /// </summary>
            public const string typekeyname16 = "CMA合格";
            /// <summary>
            /// UV-C处理
            /// </summary>
            public const string typekey17 = "17";
            /// <summary>
            /// UV-C处理
            /// </summary>
            public const string typekeyname17 = "UV-C处理";
            /// <summary>
            /// 防霉包装纸
            /// </summary>
            public const string typekey18 = "18";
            /// <summary>
            /// 防霉包装纸
            /// </summary>
            public const string typekeyname18 = "防霉包装纸";
            /// <summary>
            /// 特殊的外观标准
            /// </summary>
            public const string typekey19 = "19";
            /// <summary>
            /// 特殊的外观标准
            /// </summary>
            public const string typekeyname19 = "特殊的外观标准";
            /// <summary>
            /// 工厂免责声明
            /// </summary>
            public const string typekey20 = "20";
            /// <summary>
            /// 工厂免责声明
            /// </summary>
            public const string typekeyname20 = "工厂免责声明";
            /// <summary>
            /// FIT
            /// </summary>
            public const string typekey21 = "21";
            /// <summary>
            /// FIT
            /// </summary>
            public const string typekeyname21 = "FIT";
            /// <summary>
            /// 防霉
            /// </summary>
            public const string typekey22 = "22";
            /// <summary>
            /// 防霉
            /// </summary>
            public const string typekeyname22 = "防霉";

        }
        public class BotAtype
        {
            //0：基本 1：金属探测要求 2：FGT 3：防霉 4：特别管理 5：检查清单FIT 6：检查清单防霉 
            /// <summary>
            /// 基本
            /// </summary>
            public const string typekey = "0";
            /// <summary>
            /// 基本
            /// </summary>
            public const string typekeyname = "基本";
            /// <summary>
            /// 金属探测要求
            /// </summary>
            public const string typekey1 = "1";
            /// <summary>
            /// 金属探测要求
            /// </summary>
            public const string typekeyname1 = "金属探测要求";
            /// <summary>
            /// FGT
            /// </summary>
            public const string typekey2 = "2";
            /// <summary>
            /// FGT
            /// </summary>
            public const string typekeyname2 = "FGT";
            /// <summary>
            /// 防霉
            /// </summary>
            public const string typekey3 = "3";
            /// <summary>
            /// 防霉
            /// </summary>
            public const string typekeyname3 = "防霉";
            /// <summary>
            /// 特别管理 
            /// </summary>
            public const string typekey4 = "4";
            /// <summary>
            /// 特别管理 
            /// </summary>
            public const string typekeyname4 = "特别管理";
            /// <summary>
            /// 检查清单FIT
            /// </summary>
            public const string typekey5 = "5";
            /// <summary>
            /// 检查清单FIT
            /// </summary>
            public const string typekeyname5 = "检查清单FIT";
            /// <summary>
            /// 检查清单防霉 
            /// </summary>
            public const string typekey6 = "6";
            /// <summary>
            /// 检查清单防霉 
            /// </summary>
            public const string typekeyname6 = "检查清单防霉 ";

        }
    }
}
