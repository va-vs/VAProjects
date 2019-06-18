using System;
using System.Data;

namespace FSCWebParts
{
    public static class DataTableReader
    {
        #region DataReader 扩展
        public static T SafeRead<T>(this IDataReader reader, string fieldName, T defaultValue)
        {
            try
            {
                object obj = reader[fieldName];
                if (obj == null || obj == DBNull.Value)
                {
                    return defaultValue;
                }

                return (T)Convert.ChangeType(obj, defaultValue.GetType());
            }
            catch
            {
                return defaultValue;
            }
        }

        /// <summary>
        /// 获取字符串类型数据
        /// </summary>
        /// <param name="dr">数据行</param>
        /// <param name="name">字段名称</param>
        /// <returns></returns>
        public static string GetString(this IDataReader dr, string name)
        {

            if (dr[name] != DBNull.Value && dr[name] != null)
            {
                return dr[name].ToString();
            }

            return String.Empty;
        }
        /// <summary>
        /// 获取非空类型数据
        /// </summary>
        /// <param name="con"></param>
        /// <param name="name"></param>
        /// <param name="dr">todo: describe dr parameter on GetDateTime</param>
        /// <returns></returns>
        public static DateTime GetDateTime(this IDataReader dr, string name)
        {
            DateTime result = DateTime.Now;
            if (dr[name] != DBNull.Value && dr[name] != null)
            {
                if (!DateTime.TryParse(dr[name].ToString(), out result))
                {
                    throw new Exception("日期格式数据转换失败");
                }
            }
            return result;
        }
        /// <summary>
        /// 获取可空类型日期数据
        /// </summary>
        /// <param name="dr"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static DateTime? GetNullDateTime(this IDataReader dr, string name)
        {

            DateTime? result = null;
            DateTime time = DateTime.Now;
            if (dr[name] != DBNull.Value && dr[name] != null)
            {
                if (!DateTime.TryParse(dr[name].ToString(), out time))
                {
                    throw new Exception("日期格式数据转换失败");
                }

                result = time;
            }
            return result;
        }

        /// <summary>
        /// 获取guid类型数据
        /// </summary>
        /// <param name="dr"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static Guid GetGuid(this IDataReader dr, string name)
        {
            Guid guid = Guid.Empty;
            if (dr[name] != DBNull.Value && dr[name] != null)
            {
                if (Guid.TryParse(dr[name].ToString(), out guid))
                {
                    throw new Exception("guid类型数据转换失败");
                }
            }
            return guid;
        }
        /// <summary>
        /// 获取整形数据
        /// </summary>
        /// <param name="dr"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static int GetInt32(this IDataReader dr, string name)
        {
            int result = 0;

            if (dr[name] != DBNull.Value && dr[name] != null)
            {
                if (!int.TryParse(dr[name].ToString(), out result))
                {
                    throw new Exception("整形转换失败");
                }
            }
            return result;
        }

        /// <summary>
        /// 获取双精度类型数据
        /// </summary>
        /// <param name="dr"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static double GetDouble(this IDataReader dr, string name)
        {
            double result = 0.00;
            if (dr[name] != DBNull.Value && dr[name] != null)
            {
                if (!double.TryParse(dr[name].ToString(), out result))
                {
                    throw new Exception("双精度类型转换失败");
                }
            }
            return result;
        }
        /// <summary>
        /// 获取单精度类型数据
        /// </summary>
        /// <param name="dr"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static float GetSingle(this IDataReader dr, string name)
        {
            float result = 0.00f;
            if (dr[name] != DBNull.Value && dr[name] != null)
            {
                if (!float.TryParse(dr[name].ToString(), out result))
                {
                    throw new Exception("单精度类型转换失败");
                }
            }

            return result;
        }

        /// <summary>
        /// 获取decimal类型数据
        /// </summary>
        /// <param name="dr"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static decimal GetDecimal(this IDataReader dr, string name)
        {
            decimal result = 0.00m;
            if (dr[name] != DBNull.Value && dr[name] != null)
            {
                if (!decimal.TryParse(dr[name].ToString(), out result))
                {
                    throw new Exception("Decimal类型转换失败");
                }
            }
            return result;
        }

        /// <summary>
        /// 获取int16类型数据
        /// </summary>
        /// <param name="dr"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static Int16 GetInt16(this IDataReader dr, string name)
        {
            short result = 0;
            if (dr[name] != DBNull.Value && dr[name] != null)
            {
                if (!short.TryParse(dr[name].ToString(), out result))
                {
                    throw new Exception("短整形转换失败");
                }
            }
            return result;
        }

        /// <summary>
        /// 获取Byte类型数据
        /// </summary>
        ///  <param name="dr"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static byte GetByte(this IDataReader dr, string name)
        {
            byte result = 0;
            if (dr[name] != DBNull.Value && dr[name] != null)
            {
                if (!byte.TryParse(dr[name].ToString(), out result))
                {
                    throw new Exception("Byte类型转换失败");
                }
            }
            return result;
        }

        /// <summary>
        /// 获取bool类型数据如果传值是1或者是返回true;
        /// </summary>
        /// <param name="con"></param>
        /// <param name="name"></param>
        /// <param name="dr">todo: describe dr parameter on GetBool</param>
        /// <returns></returns>
        public static bool GetBool(this IDataReader dr, string name)
        {
            if (dr[name] != DBNull.Value && dr[name] != null)
            {
                return dr[name].ToString() == "1" || dr[name].ToString() == "是" || dr[name].ToString().ToLower() == "true";
            }
            return false;
        }
        #endregion DataReader 扩展

        #region DataRow 扩展
        public static T SafeRead<T>(this DataRow dr, string fieldName, T defaultValue)
        {
            try
            {
                object obj = dr[fieldName];
                if (obj == null || obj == DBNull.Value)
                {
                    return defaultValue;
                }

                return (T)Convert.ChangeType(obj, defaultValue.GetType());
            }
            catch
            {
                return defaultValue;
            }
        }

        /// <summary>
        /// 获取字符串类型数据
        /// </summary>
        /// <param name="dr"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static string GetString(this DataRow dr, string name)
        {

            if (dr[name] != DBNull.Value && dr[name] != null)
            {
                return dr[name].ToString();
            }

            return String.Empty;
        }
        /// <summary>
        /// 获取非空类型数据
        /// </summary>
        /// <param name="con"></param>
        /// <param name="name"></param>
        /// <param name="dr">todo: describe dr parameter on GetDateTime</param>
        /// <returns></returns>
        public static DateTime GetDateTime(this DataRow dr, string name)
        {
            DateTime result = DateTime.Now;
            if (dr[name] != DBNull.Value && dr[name] != null)
            {
                if (!DateTime.TryParse(dr[name].ToString(), out result))
                {
                    throw new Exception("日期格式数据转换失败");
                }
            }
            return result;
        }
        /// <summary>
        /// 获取可空类型日期数据
        /// </summary>
        /// <param name="dr"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static DateTime? GetNullDateTime(this DataRow dr, string name)
        {

            DateTime? result = null;
            DateTime time = DateTime.Now;
            if (dr[name] != DBNull.Value && dr[name] != null)
            {
                if (!DateTime.TryParse(dr[name].ToString(), out time))
                {
                    throw new Exception("日期格式数据转换失败");
                }

                result = time;
            }
            return result;
        }

        /// <summary>
        /// 获取guid类型数据
        /// </summary>
        /// <param name="dr"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static Guid GetGuid(this DataRow dr, string name)
        {
            Guid guid = Guid.Empty;
            if (dr[name] != DBNull.Value && dr[name] != null)
            {
                if (Guid.TryParse(dr[name].ToString(), out guid))
                {
                    throw new Exception("guid类型数据转换失败");
                }
            }
            return guid;
        }
        /// <summary>
        /// 获取整形数据
        /// </summary>
        /// <param name="dr"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static int GetInt32(this DataRow dr, string name)
        {
            int result = 0;

            if (dr[name] != DBNull.Value && dr[name] != null)
            {
                if (!int.TryParse(dr[name].ToString(), out result))
                {
                    throw new Exception("整形转换失败");
                }
            }
            return result;
        }

        /// <summary>
        /// 获取双精度类型数据
        /// </summary>
        /// <param name="dr"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static double GetDouble(this DataRow dr, string name)
        {
            double result = 0.00;
            if (dr[name] != DBNull.Value && dr[name] != null)
            {
                if (!double.TryParse(dr[name].ToString(), out result))
                {
                    throw new Exception("双精度类型转换失败");
                }
            }
            return result;
        }
        /// <summary>
        /// 获取单精度类型数据
        /// </summary>
        /// <param name="dr"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static float GetSingle(this DataRow dr, string name)
        {
            float result = 0.00f;
            if (dr[name] != DBNull.Value && dr[name] != null)
            {
                if (!float.TryParse(dr[name].ToString(), out result))
                {
                    throw new Exception("单精度类型转换失败");
                }
            }

            return result;
        }

        /// <summary>
        /// 获取decimal类型数据
        /// </summary>
        /// <param name="dr"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static decimal GetDecimal(this DataRow dr, string name)
        {
            decimal result = 0.00m;
            if (dr[name] != DBNull.Value && dr[name] != null)
            {
                if (!decimal.TryParse(dr[name].ToString(), out result))
                {
                    throw new Exception("Decimal类型转换失败");
                }
            }
            return result;
        }

        /// <summary>
        /// 获取int16类型数据
        /// </summary>
        /// <param name="dr"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static Int16 GetInt16(this DataRow dr, string name)
        {
            short result = 0;
            if (dr[name] != DBNull.Value && dr[name] != null)
            {
                if (!short.TryParse(dr[name].ToString(), out result))
                {
                    throw new Exception("短整形转换失败");
                }
            }
            return result;
        }

        /// <summary>
        /// 获取Byte类型数据
        /// </summary>
        ///  <param name="dr"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static byte GetByte(this DataRow dr, string name)
        {
            byte result = 0;
            if (dr[name] != DBNull.Value && dr[name] != null)
            {
                if (!byte.TryParse(dr[name].ToString(), out result))
                {
                    throw new Exception("Byte类型转换失败");
                }
            }
            return result;
        }

        /// <summary>
        /// 获取bool类型数据如果传值是1或者是返回true;
        /// </summary>
        /// <param name="con"></param>
        /// <param name="name"></param>
        /// <param name="dr"></param>
        /// <returns></returns>
        public static bool GetBool(this DataRow dr, string name)
        {
            if (dr[name] != DBNull.Value && dr[name] != null)
            {
                return dr[name].ToString() == "1" || dr[name].ToString() == "是" || dr[name].ToString().ToLower() == "true";
            }
            return false;
        }
        #endregion
    }
}