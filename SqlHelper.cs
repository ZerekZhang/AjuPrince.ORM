using AjuPrince.ORM.Frameworker;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace AjuPrince.ORM
{
    /// <summary>
    /// 数据库访问帮助类
    /// </summary>
    public class SqlHelper
    {
        private static string ConnectionString = "连接字符串";

        /// <summary>
        /// 根据主键ID 查询
        /// </summary>
        /// <typeparam name="T">类型参数</typeparam>
        /// <param name="id">主键ID</param>
        /// <returns></returns>
        public async Task<T> FindByPKAsync<T>(int id)
        {
            var type = typeof(T);
            string sql = SqlBuilder<T>.GetSql(SqlType.Find);
            //参数化
            IEnumerable<SqlParameter> sqlParameters = new List<SqlParameter>
            {
                new SqlParameter($"@{type.GetPropertiesKey()}",id)
            };
            return await await ExecuteSql(sql, sqlParameters, async cmd =>
            {
                var reader = await cmd.ExecuteReaderAsync();
                if (await reader.ReadAsync())
                {
                    T t = (T)Activator.CreateInstance(type);
                    foreach (var prop in type.GetProperties())
                    {
                        prop.SetValue(t, reader[prop.GetMappingName()] is DBNull
                            ? null
                            : reader[prop.GetMappingName()]);
                    }
                    return t;
                }
                return default;
            });
        }

        /// <summary>
        /// Insert
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <returns></returns>
        public async Task<bool> InsertAsync<T>(T t) where T : class
        {
            var type = typeof(T);
            string sql = SqlBuilder<T>.GetSql(SqlType.Insert);
            //参数化
            IEnumerable<SqlParameter> sqlParameters = type.GetPropertiesWithoutKey()
                .Select(p => new SqlParameter($"@{p.GetMappingName()}", p.GetValue(t) ?? DBNull.Value));
            return await await ExecuteSql(sql, sqlParameters, async cmd =>
            {
                var i = await cmd.ExecuteNonQueryAsync();
                return i == 1;
            });
        }

        /// <summary>
        /// 更新
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t">需要更新的实体</param>
        /// <returns></returns>
        public async Task<bool> UpdateAsync<T>(T t) where T : class
        {
            var type = typeof(T);
            IEnumerable<SqlParameter> sqlParameters = type.GetProperties().Select(p => new SqlParameter($"@{p.GetMappingName()}", p.GetValue(t) ?? DBNull.Value));
            string sql = SqlBuilder<T>.GetSql(SqlType.Update);
            return await await ExecuteSql(sql, sqlParameters, async cmd =>
             {
                 var i = await cmd.ExecuteNonQueryAsync();
                 return i == 1;
             });
        }

        /// <summary>
        /// 执行Sql
        /// </summary>
        /// <param name=""></param>
        private async Task<T> ExecuteSql<T>(string sql, IEnumerable<SqlParameter> sqlParameters
            , Func<SqlCommand, T> func)
        {
            SqlTransaction sqlTransaction = null;
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                try
                {
                    sqlTransaction = connection.BeginTransaction();
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        await connection.OpenAsync();
                        command.Parameters.AddRange(sqlParameters.ToArray());
                        T t = func.Invoke(command);
                        sqlTransaction.Commit();
                        return t;
                    }
                }
                catch (Exception ex)
                {
                    if (sqlTransaction != null)
                        sqlTransaction.Rollback();
                    throw ex;
                }
            }
        }
    }
}
