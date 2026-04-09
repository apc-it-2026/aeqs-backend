using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Text;

namespace SJ_QCMAPI.Common
{
   public static class EnumExtension
    {
        /// <summary>
        /// 获取枚举的备注信息
        /// </summary>
        /// <param name="em"></param>
        /// <returns></returns>
        public static string GetRemark(this Enum value)
        {
            FieldInfo fi = value.GetType().GetField(value.ToString());
            if (fi == null)
            {
                return value.ToString();
            }
            object[] attributes = fi.GetCustomAttributes(typeof(RemarkAttribute), false);
            if (attributes.Length > 0)
            {
                return ((RemarkAttribute)attributes[0]).Remark;
            }
            else
            {
                return value.ToString();
            }
        }

        /// <summary>
        /// 获取图片的保存路径
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string GetImagePath(this Enum value)
        {
            FieldInfo fi = value.GetType().GetField(value.ToString());
            object[] attributes = fi.GetCustomAttributes(typeof(ImagePathAttribute), false);
            if (attributes.Length > 0)
            {
                return ((ImagePathAttribute)attributes[0]).Path;
            }
            else
            {
                return value.ToString();
            }
        }

        /// <summary>
        /// 获取图片保存的表名
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string GetTableName(this Enum value)
        {
            FieldInfo fi = value.GetType().GetField(value.ToString());
            object[] attributes = fi.GetCustomAttributes(typeof(TableNameAttribute), false);
            if (attributes.Length > 0)
            {
                return ((TableNameAttribute)attributes[0]).TableName;
            }
            else
            {
                return value.ToString();
            }
        }
    }
}
