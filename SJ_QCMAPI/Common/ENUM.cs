using System;
using System.Collections.Generic;
using System.Text;

namespace SJ_QCMAPI.Common
{

    /// <summary>
    /// /*系统枚举*/
    /// </summary>
    public class ENUM
    {
    }

    /// <summary>
    /// 是/否
    /// </summary>
    public enum enum_whether
    {
        /// <summary>
        /// 是
        /// </summary>
        TRUE,
        /// <summary>
        /// 否
        /// </summary>
        FALSE
    }

    /// <summary>
    /// 引用等级
    /// </summary>
    public enum enum_ref_level
    {
        /// <summary>
        /// ART
        /// </summary>
        ART,
        /// <summary>
        /// ART
        /// </summary>
        MW
    }

    /// <summary>
    /// 公式类型
    /// </summary>
    public class enum_formula_type
    {
        /// <summary>
        /// 通用
        /// </summary>
        public const string enum_formula_type_0 = "0";

        /// <summary>
        /// 自定义
        /// </summary>
        public const string enum_formula_type_1 = "1";

    }

    /// <summary>
    /// 测试项目 — 检测标准类型
    /// </summary>
    public class enum_testitem_type
    {
        /// <summary>
        /// 固定值
        /// </summary>
        public const string enum_testitem_type_1 = "1";
        /// <summary>
        /// 上下限
        /// </summary>
        public const string enum_testitem_type_2 = "2";
        /// <summary>
        /// 误差值
        /// </summary>
        public const string enum_testitem_type_3 = "3";
    }

    /// <summary>
    /// 测试项目判断标准
    /// </summary>
    public class enum_judge_symbol
    {
        /// <summary>
        /// 大于(>)
        /// </summary>
        public const string enum_judge_symbol_1 = ">";

        /// <summary>
        /// 大于等于(>=)
        /// </summary>
        public const string enum_judge_symbol_2 = ">=";

        /// <summary>
        /// 小于(<)
        /// </summary>
        public const string enum_judge_symbol_3 = "<";

        /// <summary>
        /// 小于等于(<=)
        /// </summary>
        public const string enum_judge_symbol_4 = "<=";

        /// <summary>
        /// 等于(=)
        /// </summary>
        public const string enum_judge_symbol_5 = "=";

    }

    public class enum_judgment_criteria
    {
        /// <summary>
        /// 大于(>)
        /// </summary>
        public const string enum_judgment_criteria_1 = "1";

        /// <summary>
        /// 小于(<)
        /// </summary>
        public const string enum_judgment_criteria_2 = "2";

        /// <summary>
        /// 大于等于(>=)
        /// </summary>
        public const string enum_judgment_criteria_3 = "3";

        /// <summary>
        /// 小于等于(<=)
        /// </summary>
        public const string enum_judgment_criteria_4 = "4";

        /// <summary>
        /// 等于(=)
        /// </summary>
        public const string enum_judgment_criteria_5 = "5";
        /// <summary>
        /// 等于(=)
        /// </summary>
        public const string enum_judgment_criteria_6 = "6";

    }

    /// <summary>
    /// 通用公式
    /// </summary>
    public class enum_general_formula
    {
        /// <summary>
        /// 平均值
        /// </summary>
        public const string enum_general_formula_0 = "0";

        /// <summary>
        /// 最大值
        /// </summary>
        public const string enum_general_formula_1 = "1";

        /// <summary>
        /// 最小值
        /// </summary>
        public const string enum_general_formula_2 = "2";

        /// <summary>
        /// 极差值
        /// </summary>
        public const string enum_general_formula_3 = "3";

    }

    /// <summary>
    /// 检测项目分类
    /// </summary>
    public class enum_testitem_category
    {
        /// <summary>
        /// 测试项目
        /// </summary>
        public const string enum_testitem_category_1 = "1";

        /// <summary>
        /// 外观检测项目
        /// </summary>
        public const string enum_testitem_category_2 = "2";

        /// <summary>
        /// 试穿检测项目
        /// </summary>
        public const string enum_testitem_category_3 = "3";


    }

    /// <summary>
    /// QA鞋型管理文件类型
    /// </summary>
    public class enum_qa_file_type
    {
        /// <summary>
        /// 文件类型Limited release
        /// </summary>
        public const string enum_qa_file_type_0 = "1";

        /// <summary>
        /// 文件类型Disclimer
        /// </summary>
        public const string enum_qa_file_type_1 = "2";
        /// <summary>
        /// 文件类型Visual standard
        /// </summary>
        public const string enum_qa_file_type_2 = "3";
        /// <summary>
        /// 文件类型Other
        /// </summary>
        public const string enum_qa_file_type_3 = "4";
    }

    /// <summary>
    /// 单据类型
    /// </summary>
    public class enum_document_type
    {
        /// <summary>
        /// PO单号
        /// </summary>
        public const string enum_document_type_0 = "0";

        /// <summary>
        /// 收料单号
        /// </summary>
        public const string enum_document_type_1 = "1";
    }


    /// <summary>
    /// 上传图片/文件 路径枚举
    /// </summary>
    public enum enum_filepath1
    {
        /// <summary>
        /// 实验室送测提交图片
        /// </summary>
        [Remark("实验室送测提交图片")]
        [TableName("QCM_INSPECTION_LABORATORY_IMAGEURL")]
        [ImagePath(@"/pictrue/InspectionLaboratory")]
        enum_filepath_1 = 1,

        /// <summary>
        /// ART图片上传
        /// </summary>
        [Remark("ART图片上传")]
        [ImagePath(@"/pictrue/ArtImage")]
        enum_filepath_2 = 2,

        /// <summary>
        /// ART定制检验项目文件路径
        /// </summary>
        [Remark("ART定制检验项目文件路径")]
        [ImagePath(@"/file/ARTCustomQualityFile")]
        enum_filepath_3 = 3,

        /// <summary>
        /// QA鞋型问题点图片上传路径
        /// </summary>
        [Remark("QA鞋型问题点图片上传路径")]
        [ImagePath(@"/pictrue/QAShoeShapeImage")]
        enum_filepath_4 = 4,

        /// <summary>
        /// QA鞋型 文件上传 四种类型（Limited release/Disclimer/Visual standard/Other）
        /// </summary>
        [Remark("QA鞋型 文件上传 四种类型（Limited release/Disclimer/Visual standard/Other）")]
        [ImagePath(@"/file/QAShoeShapeFile")]
        enum_filepath_5 = 5,

        /// <summary>
        /// 金属检验上传图片
        /// </summary>
        [Remark("金属检验上传图片")]
        [TableName("qcm_inspection_metal_image")]
        [ImagePath(@"/pictrue/InspectionMetal")]
        enum_filepath_6 = 6,

        /// <summary>
        /// 色卡检验项目图片
        /// </summary>
        [Remark("色卡检验项目图片")]
        [TableName("QCM_SPOTCHECK_TASK_M")]
        [ImagePath(@"/pictrue/ColorCard")]
        enum_filepath_7 = 7,

        /// <summary>
        /// 鞋面品质审核图片上传路径
        /// </summary>
        [Remark("鞋面品质审核图片上传路径")]
        [TableName("QCM_VAMP_QUALITY_IMAGEURL")]
        [ImagePath(@"/pictrue/VampQuality")]
        enum_filepath_8 = 8,

        /// <summary>
        /// 发外厂商品质体系图片上传路径
        /// </summary>
        [Remark("发外厂商品质体系图片上传路径")]
        [TableName("QCM_OUT_QUALITY_LIST_LOG_IMAGEURL")]
        [ImagePath(@"/pictrue/OutQuantityLog")]
        enum_filepath_9 = 9,

        /// <summary>
        /// 平板抽检图片上传路径
        /// </summary>
        [Remark("平板抽检图片上传路径")]
        [TableName("QCM_SPOTCHECK_TASK_IMAGE")]
        [ImagePath(@"/pictrue/SpotcheckTask")]
        enum_filepath_10 = 10,

        /// <summary>
        /// ART文件绑定
        /// </summary>
        [Remark("ART文件绑定")]
        [ImagePath(@"/pictrue/ARTFile")]
        enum_filepath_11 = 11,

        /// <summary>
        /// 重检报告
        /// </summary>
        [Remark("发外厂商品质体系图片上传路径")]
        [TableName("QCM_REINSPECTION_REPORT_IMAGEURL")]
        [ImagePath(@"/pictrue/ReinspectionReport")]
        enum_filepath_12 = 12,

        /// <summary>
        /// 量产试作图片
        /// </summary>
        [Remark("量产试作图片")]
        [TableName("QCM_BATCH_PRODUCTION_IMAGEURL")]
        [ImagePath(@"/pictrue/BatchProduction")]
        enum_filepath_13 = 13,

        /// <summary>
        /// 异常呈报单
        /// </summary>
        [Remark("异常呈报单图片")]
        [TableName("QCM_ABNORMAL_REPORT_IMAGEURL")]
        [ImagePath(@"/pictrue/AbnormalReport")]
        enum_filepath_14 = 14,

        /// <summary>
        /// 不良退货图片
        /// </summary>
        [Remark("不良退货图片")]
        [TableName("qcm_bad_return_image")]
        [ImagePath(@"/pictrue/BadReturn")]
        enum_filepath_15 = 15,

        [Remark("客户投诉图片上传路径")]
        [TableName("QCM_CUSTOMER_COMPLAINT_FILE")]
        [ImagePath(@"/pictrue/CustomerComplaint")]
        enum_filepath_16 = 16,


        [Remark("客户投诉文件上传路径")]
        [TableName("QCM_CUSTOMER_COMPLAINT_FILE")]
        [ImagePath(@"/file/CustomerComplaint")]
        enum_filepath_17 = 17,

        [Remark("体系文件上传路径")]
        [TableName("")]
        [ImagePath(@"/file/SystemFileMaintenance")]
        enum_filepath_18 = 18,

    }

    /// <summary>
    /// 导入模板类型
    /// </summary>
    public enum enum_import_type
    {
        /// <summary>
        /// 量产试作导入模板
        /// </summary>
        [Remark("量产试作导入模板")]
        enum_import_type_1 = 1,
        /// <summary>
        /// 客户投诉导入模板
        /// </summary>
        [Remark("客户投诉导入模板")]
        enum_import_type_2 = 2,
        /// <summary>
        /// 首件确认导入模板
        /// </summary>
        [Remark("首件确认导入模板")]
        enum_import_type_3 = 3,
        /// <summary>
        /// 发外厂商品质体系项目日志(重检报告)导入模板
        /// </summary>
        [Remark("发外厂商品质体系项目日志(重检报告)导入模板")]
        enum_import_type_4 = 4,
        /// <summary>
        /// 发外厂商色卡导入模板
        /// </summary>
        [Remark("发外厂商色卡导入模板")]
        enum_import_type_5 = 5,
        /// <summary>
        /// 抽检监督报表导入模板
        /// </summary>
        [Remark("抽检监督报表导入模板")]
        enum_import_type_6 = 6,
        /// <summary>
        /// 抽检监督报表导入模板
        /// </summary>
        [Remark("不良退货导入模板")]
        enum_import_type_7 = 7,
        /// <summary>
        /// 客户退货数据导入模板
        /// </summary>
        [Remark("中国区域客户退货数据导入模板")]
        enum_import_type_8 = 8,
        /// <summary>
        /// 确认鞋库位维护导入模板
        /// </summary>
        [Remark("确认鞋库位维护导入模板")]
        enum_import_type_9 = 9,
        /// <summary>
        /// 确认鞋仓库维护导入模板
        /// </summary>
        [Remark("确认鞋仓库维护导入模板")]
        enum_import_type_10 = 10,
        /// <summary>
        /// 出货仓库维护导入模板
        /// </summary>
        [Remark("出货仓库维护导入模板")]
        enum_import_type_11 = 11,
        /// <summary>
        /// 出货库位维护导入模板
        /// </summary>
        [Remark("出货库位维护导入模板")]
        enum_import_type_12 = 12,
        /// <summary>
        /// 
        /// </summary>
        [Remark("客户退货导入模板")]
        enum_import_type_13 = 13,
        /// <summary>
        /// 
        /// </summary>
        [Remark("客户退货导入模板NEW")]
        enum_import_type_14 = 14

    }

    public class enum_barcode_print_type
    {
        /// <summary>
        /// 材料条码
        /// </summary>
        public const string enum_barcode_print_type_0 = "材料条码";

        /// <summary>
        /// 容器条码
        /// </summary>
        public const string enum_barcode_print_type_1 = "容器条码";
        /// <summary>
        /// 人员条码
        /// </summary>
        public const string enum_barcode_print_type_2 = "人员条码";
        /// <summary>
        /// 检验工具条码
        /// </summary>
        public const string enum_barcode_print_type_3 = "检验工具条码";
        /// <summary>
        /// 设备条码
        /// </summary>
        public const string enum_barcode_print_type_4 = "设备条码";
        /// <summary>
        /// 产线条码
        /// </summary>
        public const string enum_barcode_print_type_5 = "产线条码";
        /// <summary>
        /// 库位条码
        /// </summary>
        public const string enum_barcode_print_type_6 = "库位条码";
        /// <summary>
        /// 样品条码
        /// </summary>
        public const string enum_barcode_print_type_7 = "样品条码";
    }

    /// <summary>
    /// 原材料画皮等级
    /// </summary>
    public class enum_paintedskin_level
    {
        /// <summary>
        /// 一级
        /// </summary>
        public const string enum_barcode_print_type_1 = "1";
        /// <summary>
        /// 二级
        /// </summary>
        public const string enum_barcode_print_type_2 = "2";
        /// <summary>
        /// 三级
        /// </summary>
        public const string enum_barcode_print_type_3 = "3";
        /// <summary>
        /// 四级
        /// </summary>
        public const string enum_barcode_print_type_4 = "4";
        /// <summary>
        /// 五级
        /// </summary>
        public const string enum_barcode_print_type_5 = "5";
        /// <summary>
        /// 六级
        /// </summary>
        public const string enum_barcode_print_type_6 = "6";
        /// <summary>
        /// 六级以下
        /// </summary>
        public const string enum_barcode_print_type_6B = "6B";
    }

    /// <summary>
    /// 品质问题等级
    /// </summary>
    public class enum_quality_problem_level
    {
        /// <summary>
        /// 0级
        /// </summary>
        public const string enum_barcode_print_type_0 = "0";
        /// <summary>
        /// 1级
        /// </summary>
        public const string enum_barcode_print_type_1 = "1";
        /// <summary>
        /// 2级
        /// </summary>
        public const string enum_barcode_print_type_2 = "2";
    }

    public class enum_aql_level
    {
        /// <summary>
        /// 1
        /// </summary>
        public const string enum_barcode_print_type_0 = "0.010";

        /// <summary>
        /// 2
        /// </summary>
        public const string enum_barcode_print_type_1 = "0.015";

        /// <summary>
        /// 3
        /// </summary>
        public const string enum_barcode_print_type_2 = "0.025";

        /// <summary>
        /// 4
        /// </summary>
        public const string enum_barcode_print_type_3 = "0.040";

        /// <summary>
        /// 5
        /// </summary>
        public const string enum_barcode_print_type_4 = "0.065";

        /// <summary>
        /// 6
        /// </summary>
        public const string enum_barcode_print_type_5 = "0.10";

        /// <summary>
        /// 7
        /// </summary>
        public const string enum_barcode_print_type_6 = "0.15";

        /// <summary>
        /// 8
        /// </summary>
        public const string enum_barcode_print_type_7 = "0.25";

        /// <summary>
        /// 9
        /// </summary>
        public const string enum_barcode_print_type_8 = "0.40";

        /// <summary>
        /// 10
        /// </summary>
        public const string enum_barcode_print_type_9 = "0.65";

        /// <summary>
        /// 11
        /// </summary>
        public const string enum_barcode_print_type_10 = "1.0";

        /// <summary>
        /// 12
        /// </summary>
        public const string enum_barcode_print_type_11 = "1.5";

        /// <summary>
        /// 13
        /// </summary>
        public const string enum_barcode_print_type_12 = "2.5";

        /// <summary>
        /// 14
        /// </summary>
        public const string enum_barcode_print_type_13 = "4.0";

        /// <summary>
        /// 15
        /// </summary>
        public const string enum_barcode_print_type_14 = "6.5";

        /// <summary>
        /// 16
        /// </summary>
        public const string enum_barcode_print_type_15 = "10";

        /// <summary>
        /// 17
        /// </summary>
        public const string enum_barcode_print_type_16 = "15";

        /// <summary>
        /// 18
        /// </summary>
        public const string enum_barcode_print_type_17 = "25";

        /// <summary>
        /// 19
        /// </summary>
        public const string enum_barcode_print_type_18 = "40";

        /// <summary>
        /// 20
        /// </summary>
        public const string enum_barcode_print_type_19 = "65";

        /// <summary>
        /// 21
        /// </summary>
        public const string enum_barcode_print_type_20 = "100";

        /// <summary>
        /// 22
        /// </summary>
        public const string enum_barcode_print_type_21 = "150";

        /// <summary>
        /// 23
        /// </summary>
        public const string enum_barcode_print_type_22 = "250";

        /// <summary>
        /// 24
        /// </summary>
        public const string enum_barcode_print_type_23 = "400";

        /// <summary>
        /// 25
        /// </summary>
        public const string enum_barcode_print_type_24 = "650";

        /// <summary>
        /// 26
        /// </summary>
        public const string enum_barcode_print_type_25 = "1000";


    }
    /// <summary>
    /// 编码规则（初始规则）
    /// </summary>
    public class enum_initial_rule
    {
        /// <summary>
        /// 每日
        /// </summary>
        public const string enum_initial_date_0 = "0";
        /// <summary>
        /// 每月
        /// </summary>
        public const string enum_initial_date_1 = "1";
        /// <summary>
        /// 每年
        /// </summary>
        public const string enum_initial_date_2 = "2";
        /// <summary>
        /// 一直累加
        /// </summary>
        public const string enum_initial_date_3 = "3";
    }
    /// <summary>
    /// 编码规则明细（规则项）
    /// </summary>
    public class enum_rule_item
    {
        /// <summary>
        /// 固定字符
        /// </summary>
        public const string enum_rule_item_0 = "0";
        /// <summary>
        /// 4位年（yyyy）
        /// </summary>
        public const string enum_rule_item_1 = "1";
        /// <summary>
        /// 2位年（yy）
        /// </summary>
        public const string enum_rule_item_2 = "2";
        /// <summary>
        /// 月（MM）
        /// </summary>
        public const string enum_rule_item_3 = "3";
        /// <summary>
        /// 日（dd）
        /// </summary>
        public const string enum_rule_item_4 = "4";
        /// <summary>
        /// 流水号
        /// </summary>
        public const string enum_rule_item_5 = "5";
    }
    /// <summary>
    /// 编码规则明细（补位规则）
    /// </summary>
    public static class enum_complement
    {
        /// <summary>
        /// 不补位
        /// </summary>
        public const string enum_complement_0 = "不补位";
        /// <summary>
        /// 左补位
        /// </summary>
        public const string enum_complement_1 = "左补位";
        /// <summary>
        /// 右补位
        /// </summary>
        public const string enum_complement_2 = "右补位";
        /// <summary>
        /// 不补位
        /// </summary>
        public const string enum_complement_num_0 = "0";
        /// <summary>
        /// 左补位
        /// </summary>
        public const string enum_complement_num_1 = "1";
        /// <summary>
        /// 右补位
        /// </summary>
        public const string enum_complement_num_2 = "2";
    }
}
