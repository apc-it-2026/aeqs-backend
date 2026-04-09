using System;
using System.Collections.Generic;
using System.Text;

namespace SJ_QCMAPI.Common
{
    public class CustomAttribute: Attribute
    {
    }

    public class ImagePathAttribute : Attribute
    {
        /// <summary>
        /// 图片路径
        /// </summary>
        public string Path { get; set; }

        public ImagePathAttribute(string path)
        {
            this.Path = path;
        }
    }

    public class RemarkAttribute : Attribute
    {
        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }

        public RemarkAttribute(string remark)
        {
            this.Remark = remark;
        }
    }

    public class TableNameAttribute : Attribute
    {
        /// <summary>
        /// 图片保存表名
        /// </summary>
        public string TableName { get; set; }

        public TableNameAttribute(string tablename)
        {
            this.TableName = tablename;
        }
    }

}
