using AjuPrince.ORM.Frameworker;
using System;
using System.Linq;

namespace AjuPrince.ORM
{
    /// <summary>
    /// SQL 生成器+缓存
    /// 泛型缓存 做 性能优化
    /// </summary>
    public class SqlBuilder<T>
    {
        private static string FindSql = string.Empty;

        private static string InserSql = string.Empty;

        private static string UpdateSql = string.Empty;
        static SqlBuilder()
        {
            var type = typeof(T);
            //insert
            string filedStr = string.Join(",", type.GetPropertiesWithoutKey().Select(p => $"[{p.GetMappingName()}]"));
            string valuesStr = string.Join(",", type.GetPropertiesWithoutKey().Select(p => $"@{p.GetMappingName()}"));
            InserSql = $@"insert into [{type.GetMappingName()}] ({filedStr}) values({valuesStr})";
            //select
            string selectStr = string.Join(",", type.GetProperties().Select(p => $"[{p.GetMappingName()}]"));
            FindSql = $"select {selectStr} form [{type.GetMappingName()}] where [{type.GetPropertiesKey()}]=@{type.GetPropertiesKey()}";
            //update 
            string columnStr = string.Join(",", type.GetPropertiesWithoutKey()
               .Select(p => $" [{p.GetMappingName()}]=@{p.GetMappingName()}"));
            string sql = $"update [{type.GetMappingName()}] set {columnStr} where [{type.GetPropertiesKey()}]=@{type.GetPropertiesKey()}";
            UpdateSql = sql;
        }

        public static string GetSql(SqlType sqlType)
        {
            switch (sqlType)
            {
                case SqlType.Find:
                    return FindSql;
                case SqlType.Insert:
                    return InserSql;
                case SqlType.Update:
                    return UpdateSql;
                default:
                    throw new Exception("unknown sqlType");
            }
        }
    }

    public enum SqlType
    {
        Find,
        Insert,
        Update
    }
}
