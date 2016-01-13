using Dapper;
using ILvYou.Repository;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;

namespace ILvYou.DBAccess
{
    public delegate string ProcessProc(string sql);

    public class DefaultDataAccess : IDataAccess
    {
        #region ctor

        public DefaultDataAccess(string connectionString)
        {
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new ArgumentNullException("connectionString");
            }

            this.ConnectionString = connectionString;

        }
        #endregion

        #region private fields
        private string ConnectionString = string.Empty;
        #endregion private fields

        #region public method
        /// <summary>
        /// 定义一个Connection
        /// </summary>
        /// <returns>IDbConnection</returns>
        public IDbConnection DbService()
        {
            return new Npgsql.NpgsqlConnection(this.ConnectionString);
        }

        /// <summary>
        /// 关闭数据连接
        /// </summary>
        /// <param name="con">数据连接IDbConnection</param>
        /// <returns>成功返回true，失败返回false</returns>
        public bool CloseConnection(IDbConnection con)
        {
            bool result = false;
            try
            {
                con.Close();
                result = true;
            }
            catch (Exception ex)
            {
                result = false;
                Log.Error("CloseConnection error", ex);
                throw ex;
            }
            return result;
        }

        /// <summary>
        /// 执行一个sql文件，返回Ilist<T>
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="sql">sql文件地址</param>
        /// <param name="param">查询参数</param>
        /// <param name="processor">委托</param>
        /// <returns></returns>
        public IList<T> ExecuteIList<T>(string sql, object param, ProcessProc r)
        {
            return ExecuteIListdDlegate<T>(sql, param, r);
        }

        public T ExecuteScalar<T>(string sql, object param, bool hasHransaction = false, CommandType commandType = CommandType.Text)
        {
            T result;
            using (var con = DbService())
            {
                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                }
                try
                {
                    if (hasHransaction)
                    {
                        using (IDbTransaction trans = con.BeginTransaction(IsolationLevel.ReadCommitted))
                        {
                            if (commandType == CommandType.Text)
                            {
                                result = con.ExecuteScalar<T>(LoadSql(sql, param), param, trans, null, CommandType.Text);
                            }
                            else
                            {
                                result = con.ExecuteScalar<T>(sql, param, trans, null, CommandType.StoredProcedure);
                            }
                        }
                    }else
                    {
                        if (commandType == CommandType.Text)
                        {
                            result = con.ExecuteScalar<T>(LoadSql(sql, param), param, null, null, CommandType.Text);
                        }
                        else
                        {
                            result = con.ExecuteScalar<T>(sql, param, null, null, CommandType.StoredProcedure);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Log.Error(sql + " ExecuteNonQueryDefault error", ex);
                    throw ex;
                }
            }
            return result;
        }

        /// <summary>
        /// 执行一个sql文件，返回Ilist<T>
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="sql">sql文件地址</param>
        /// <param name="param"sql参数></param>
        /// <param name="hasHransaction">是否需要事物</param>
        /// <returns></returns>
        public IList<T> ExecuteIList<T>(string sql, object param, bool hasHransaction = false)
        {
            if (hasHransaction)
            {
                return ExecuteIListWithTran<T>(CommandType.Text, sql, param);
            }
            else
            {
                return ExecuteIListWithOutTran<T>(CommandType.Text, sql, param);
            }
        }

        /// <summary>
        /// 执行一个sql文件，返回Ilist<T>
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="sql">sql文件地址</param>
        /// <param name="param">sql参数</param>
        /// <param name="transaction">外部事物</param>
        /// <param name="con">数据连接IDbConnection</param>
        /// <returns></returns>
        public IList<T> ExecuteIList<T>(string sql, object param, IDbTransaction transaction, IDbConnection con)
        {
            return ExecuteIListDefault<T>(CommandType.Text, sql, param, transaction, con);
        }

        /// <summary>
        /// 执行一个存储过程，返回Ilist<T>
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="sql">存储过程名</param>
        /// <param name="param">sql参数</param>
        /// <param name="hasHransaction">是否需要事物</param>
        /// <returns></returns>
        public IList<T> ExecuteIListBySP<T>(string sql, object param, bool hasHransaction = false)
        {
            if (hasHransaction)
            {
                return ExecuteIListWithTran<T>(CommandType.StoredProcedure, sql, param);
            }
            else
            {
                return ExecuteIListWithOutTran<T>(CommandType.StoredProcedure, sql, param);
            }
        }

        /// <summary>
        /// 执行一个存储过程，返回Ilist<T>
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="sql">存储过程名</param>
        /// <param name="param">sql参数</param>
        /// <param name="transaction">外部事物</param>
        /// <param name="con">数据连接IDbConnection</param>
        /// <returns></returns>
        public IList<T> ExecuteIListBySP<T>(string sql, object param, IDbTransaction transaction, IDbConnection con)
        {
            return ExecuteIListDefault<T>(CommandType.StoredProcedure, sql, param, transaction, con);
        }

        /// <summary>
        /// 执行一个sql文件，返回一个object
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="sql">sql文件地址</param>
        /// <param name="param">sql参数</param>
        /// <param name="hasHransaction">是否需要事物</param>
        /// <returns></returns>
        public object ExecuteObj(string sql, object param, bool hasHransaction = false)
        {
            if (hasHransaction)
            {
                return ExecuteObjWithTran(CommandType.Text, sql, param);
            }
            else
            {
                return ExecuteObjWithOutTran(CommandType.Text, sql, param);
            }
        }

        /// <summary>
        /// 执行一个sql文件，返回一个object
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="sql">sql文件地址</param>
        /// <param name="param">sql参数</param>
        /// <param name="transaction">外部事物</param>
        /// <param name="con">数据连接IDbConnection</param>
        /// <returns></returns>
        public object ExecuteObj(string sql, object param, IDbTransaction transaction, IDbConnection con)
        {
            return ExecuteObjDefault(CommandType.Text, sql, param, transaction, con);
        }

        /// <summary>
        /// 执行一个存储过程，返回object
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="sql">存储过程名</param>
        /// <param name="param">sql参数</param>
        /// <param name="transaction">外部事物</param>
        /// <param name="con">数据连接IDbConnection</param>
        /// <returns></returns>
        public object ExecuteObjBySP(string sql, object param, bool hasHransaction = false)
        {
            if (hasHransaction)
            {
                return ExecuteObjWithTran(CommandType.StoredProcedure, sql, param);
            }
            else
            {
                return ExecuteObjWithOutTran(CommandType.StoredProcedure, sql, param);
            }
        }

        /// <summary>
        /// 执行一个存储过程，返回object
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="sql">存储过程名</param>
        /// <param name="param">sql参数</param>
        /// <param name="transaction">外部事物</param>
        /// <returns></returns>
        public object ExecuteObjBySP(string sql, object param, IDbTransaction transaction, IDbConnection con)
        {
            return ExecuteObjDefault(CommandType.StoredProcedure, sql, param, transaction, con);
        }

        /// <summary>
        /// sql文件方法执行一个连接查询，返回一个IList<TReturn>
        /// </summary>
        /// <typeparam name="TFirst">第一个连接对象类型</typeparam>
        /// <typeparam name="TSecond">第二个连接对象类型</typeparam>
        /// <typeparam name="TReturn">返回对象的类型</typeparam>
        /// <param name="sql">sql文件地址</param>
        /// <param name="map">俩个表连接的方法</param>
        /// <param name="param">sql参数</param>
        /// <param name="splitOn">来个表关联的字段id</param>
        /// <param name="hasHransaction">是否需要事物</param>
        /// <returns></returns>
        public IList<TReturn> ExecuteIList<TFirst, TSecond, TReturn>(string sql, Func<TFirst, TSecond, TReturn> map, object param, string splitOn, bool hasHransaction = false)
        {
            if (hasHransaction)
            {
                return ExecuteIListWithTran<TFirst, TSecond, TReturn>(CommandType.Text, sql, map, param, splitOn);
            }
            else
            {
                return ExecuteIListWithOutTran<TFirst, TSecond, TReturn>(CommandType.Text, sql, map, param, splitOn);
            }
        }

        /// <summary>
        /// 执行一个连接查询，返回一个IList<TReturn>
        /// </summary>
        /// <typeparam name="TFirst">第一个连接对象类型</typeparam>
        /// <typeparam name="TSecond">第二个连接对象类型</typeparam>
        /// <typeparam name="TReturn">返回对象的类型</typeparam>
        /// <param name="sql">sql文件地址</param>
        /// <param name="map">俩个表连接的方法</param>
        /// <param name="param">sql参数</param>
        /// <param name="splitOn">来个表关联的字段id</param>
        /// <param name="transaction">外部事物</param>
        /// <param name="con">数据连接IDbConnection</param>
        /// <returns></returns>
        public IList<TReturn> ExecuteIList<TFirst, TSecond, TReturn>(string sql, Func<TFirst, TSecond, TReturn> map, object param, string splitOn, IDbTransaction transaction, IDbConnection con)
        {
            return ExecuteIListDefault<TFirst, TSecond, TReturn>(CommandType.Text, sql, map, param, transaction, splitOn, con);
        }

        /// <summary>
        /// 存储过程执行一个连接查询，返回一个IList<TReturn>
        /// </summary>
        /// <typeparam name="TFirst">第一个连接对象类型</typeparam>
        /// <typeparam name="TSecond">第二个连接对象类型</typeparam>
        /// <typeparam name="TReturn">返回对象的类型</typeparam>
        /// <param name="sql">存储过程名称</param>
        /// <param name="map">俩个表连接的方法</param>
        /// <param name="param">sql参数</param>
        /// <param name="splitOn">来个表关联的字段id</param>
        /// <param name="hasHransaction">是否需要事物</param>
        /// <returns></returns>
        public IList<TReturn> ExecuteIListBySP<TFirst, TSecond, TReturn>(string sql, Func<TFirst, TSecond, TReturn> map, object param, string splitOn, bool hasHransaction = false)
        {
            if (hasHransaction)
            {
                return ExecuteIListWithTran<TFirst, TSecond, TReturn>(CommandType.StoredProcedure, sql, map, param, splitOn);
            }
            else
            {
                return ExecuteIListWithTran<TFirst, TSecond, TReturn>(CommandType.StoredProcedure, sql, map, param, splitOn);
            }
        }

        /// <summary>
        /// 存储过程执行一个连接查询，返回一个IList<TReturn>
        /// </summary>
        /// <typeparam name="TFirst">第一个连接对象类型</typeparam>
        /// <typeparam name="TSecond">第二个连接对象类型</typeparam>
        /// <typeparam name="TReturn">返回对象的类型</typeparam>
        /// <param name="sql">存储过程名称</param>
        /// <param name="map">俩个表连接的方法</param>
        /// <param name="param">sql参数</param>
        /// <param name="splitOn">来个表关联的字段id</param>
        /// <param name="transaction">外部事物</param>
        /// <param name="con">数据连接IDbConnection</param>
        /// <returns></returns>
        public IList<TReturn> ExecuteIListBySP<TFirst, TSecond, TReturn>(string sql, Func<TFirst, TSecond, TReturn> map, object param, string splitOn, IDbTransaction transaction, IDbConnection con)
        {
            return ExecuteIListDefault<TFirst, TSecond, TReturn>(CommandType.StoredProcedure, sql, map, param, transaction, splitOn, con);
        }

        /// <summary>
        /// 执行sql文件，返回受影响行数
        /// </summary>
        /// <param name="sql">sql文件地址</param>
        /// <param name="param">sql参数</param>
        /// <param name="hasHransaction">是否需要事物</param>
        /// <returns></returns>
        public int ExecuteNonQuery(string sql, object param, bool hasHransaction = false)
        {
            if (hasHransaction)
            {
                return ExecuteNonQueryWithTran(CommandType.Text, sql, param);
            }
            else
            {
                return ExecuteNonQueryWithOutTran(CommandType.Text, sql, param);
            }
        }

        /// <summary>
        /// 执行sql文件，返回受影响行数
        /// </summary>
        /// <param name="sql">sql文件地址</param>
        /// <param name="param">sql参数</param>
        /// <param name="transaction">外部事物</param>
        /// <param name="con">数据连接IDbConnection</param>
        /// <returns></returns>
        public int ExecuteNonQuery(string sql, object param, IDbTransaction transaction, IDbConnection con)
        {
            return ExecuteNonQueryDefault(CommandType.Text, sql, param, transaction, con);
        }

        /// <summary>
        /// 执行存储过程，返回受影响行数
        /// </summary>
        /// <param name="sql">存储过程名称</param>
        /// <param name="param">sql参数</param>
        /// <param name="hasHransaction">是否需要事物</param>
        /// <returns></returns>
        public int ExecuteNonQueryBySP(string sql, object param, bool hasHransaction = false)
        {
            if (hasHransaction)
            {
                return ExecuteNonQueryWithTran(CommandType.StoredProcedure, sql, param);
            }
            else
            {
                return ExecuteNonQueryWithOutTran(CommandType.StoredProcedure, sql, param);
            }
        }

        /// <summary>
        /// 执行存储过程，返回受影响行数
        /// </summary>
        /// <param name="sql">存储过程名称</param>
        /// <param name="param">sql参数</param>
        /// <param name="transaction">外部事物</param>
        /// <param name="con">数据连接IDbConnection</param>
        /// <returns></returns>
        public int ExecuteNonQueryBySP(string sql, object param, IDbTransaction transaction, IDbConnection con)
        {
            return ExecuteNonQueryDefault(CommandType.StoredProcedure, sql, param, transaction, con);
        }
        #endregion

        #region private method

        /// <summary>
        /// 加载SQL语句
        /// </summary>
        /// <param name="relativePath">路径</param>
        /// <returns></returns>
        private string LoadSql(string sql)
        {
            return LoadSql(sql, null);
        }

        private string LoadSql(string sql, object param)
        {
            if (string.IsNullOrEmpty(sql))
            {
                Log.Error(string.Format("{0} ScriptNotFound", sql), null);
                throw new Exception(string.Format("{0} PathIsEmpty", sql));
            }

            string result = string.Empty;
            if (param != null && !string.IsNullOrEmpty(sql))
            {
                result = ReplaceSql(sql, param);
            }

            return result;
        }

        private string LoadSql1(string relativePath)
        {
            return LoadSql1(relativePath, null);
        }

        private string LoadSql1(string relativePath, object param)
        {
            if (string.IsNullOrEmpty(relativePath))
            {
                Log.Error(string.Format("{0} ScriptNotFound", relativePath), null);
                throw new Exception(string.Format("{0} PathIsEmpty", relativePath));
            }

            string result;
            string path;

            path = Path.Combine(
                AppDomain.CurrentDomain.BaseDirectory,
                string.Format(@"SQLScripts\{0}.sql", relativePath)
                );

            path = path.Replace(@"\", "/");

            result = SqlUtility.LoadSqlFromFile(path);

            if (string.IsNullOrEmpty(result))
            {
                Log.Error(string.Format("{0} ScriptNotFound", path), null);
                throw new Exception(string.Format("{0} ScriptNotFound", path));
            }

            if (param != null && !string.IsNullOrEmpty(result))
            {
                result = ReplaceSql(result, param);
            }

            return result;
        }

        private string ReplaceSql(string sql, object param)
        {
            string result = XmlMapper.GetCommandSql(sql);
            object order = TypeUtility.GetPropertyValue(param, "Order");
            object group = TypeUtility.GetPropertyValue(param, "Group");

            if (order != null)
            {
                result = result.Replace("{{ORDER}}", TypeUtility.ToString(order));
            }
            if (group != null)
            {
                result = result.Replace("{{GROUP}}", TypeUtility.ToString(group));
            }
            return string.IsNullOrEmpty(result) ? sql : result;
        }

        private string FilterSql(string sql, object param)
        {
            return string.Empty;
        }
        #endregion

        #region private method
        private IList<T> ExecuteIListdDlegate<T>(string sql, object param, ProcessProc r)
        {
            IList<T> list = null;
            using (var con = DbService())
            {
                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                }

                try
                {
                    string text = r(LoadSql(sql, param));
                    list = con.Query<T>(text, param, null, true, null, CommandType.Text).ToList<T>();
                }
                catch (Exception ex)
                {
                    Log.Error(sql + "  ExecuteIListWithOutTran error", ex);
                    throw ex;
                }

            }
            return list;
        }

        private IList<T> ExecuteIListDefault<T>(CommandType commandType, string sql, object param, IDbTransaction trans, IDbConnection con)
        {
            IList<T> list = null;
            if (con.State == ConnectionState.Closed)
            {
                con.Open();
            }
            try
            {
                if (commandType == CommandType.Text)
                {
                    list = con.Query<T>(LoadSql(sql, param), param, trans, true, null, CommandType.Text).ToList<T>();
                }
                else
                {
                    list = con.Query<T>(sql, param, trans, true, null, CommandType.StoredProcedure).ToList<T>();
                }
            }
            catch (Exception ex)
            {
                Log.Error(sql + " ExecuteIListDefault  error", ex);
                throw ex;
            }
            return list;
        }

        private IList<T> ExecuteIListWithTran<T>(CommandType commandType, string sql, object param)
        {
            IList<T> list = null;
            using (var con = DbService())
            {
                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                }
                using (IDbTransaction trans = con.BeginTransaction(IsolationLevel.ReadCommitted))
                {
                    try
                    {
                        if (commandType == CommandType.Text)
                        {
                            list = con.Query<T>(LoadSql(sql, param), param, trans, true, null, CommandType.Text).ToList<T>();
                        }
                        else
                        {
                            list = con.Query<T>(sql, param, trans, true, null, CommandType.StoredProcedure).ToList<T>();
                        }
                        trans.Commit();
                    }
                    catch (Exception ex)
                    {
                        trans.Rollback();
                        Log.Error(sql + " ExecuteIListWithTran  error", ex);
                        throw ex;
                    }
                }

            }
            return list;
        }

        private IList<T> ExecuteIListWithOutTran<T>(CommandType commandType, string sql, object param)
        {
            IList<T> list = null;
            using (var con = DbService())
            {
                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                }

                try
                {
                    if (commandType == CommandType.Text)
                    {
                        list = con.Query<T>(LoadSql(sql, param), param, null, true, null, CommandType.Text).ToList<T>();
                    }
                    else
                    {
                        list = con.Query<T>(sql, param, null, true, null, CommandType.StoredProcedure).ToList<T>();
                    }
                }
                catch (Exception ex)
                {
                    Log.Error(sql + "  ExecuteIListWithOutTran error", ex);
                    throw ex;
                }

            }
            return list;
        }

        private object ExecuteObjDefault(CommandType commandType, string sql, object param, IDbTransaction trans, IDbConnection con)
        {
            object obj = null;

            if (con.State == ConnectionState.Closed)
            {
                con.Open();
            }
            try
            {
                if (commandType == CommandType.Text)
                {
                    obj = con.Query(LoadSql(sql, param), param, trans, true, null, CommandType.Text).SingleOrDefault();
                }
                else
                {
                    obj = con.Query(sql, param, trans, true, null, CommandType.StoredProcedure).SingleOrDefault();
                }
            }
            catch (Exception ex)
            {
                Log.Error(sql + " ExecuteIListWithTran  error", ex);
                throw ex;
            }

            return obj;
        }

        private object ExecuteObjWithTran(CommandType commandType, string sql, object param)
        {
            object obj = null;
            using (var con = DbService())
            {
                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                }
                using (IDbTransaction trans = con.BeginTransaction(IsolationLevel.ReadCommitted))
                {
                    try
                    {
                        if (commandType == CommandType.Text)
                        {
                            obj = con.Query(LoadSql(sql, param), param, trans, true, null, CommandType.Text).SingleOrDefault();
                        }
                        else
                        {
                            obj = con.Query(sql, param, trans, true, null, CommandType.StoredProcedure).SingleOrDefault();
                        }

                        trans.Commit();
                    }
                    catch (Exception ex)
                    {
                        trans.Rollback();
                        Log.Error(sql + " ExecuteIListWithTran  error", ex);
                        throw ex;
                    }
                }
            }

            return obj;
        }

        private object ExecuteObjWithOutTran(CommandType commandType, string sql, object param)
        {
            object obj = null;

            using (var con = DbService())
            {
                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                }

                try
                {
                    if (commandType == CommandType.Text)
                    {
                        obj = con.Query(LoadSql(sql, param), param, null, true, null, CommandType.Text).SingleOrDefault();
                    }
                    else
                    {
                        obj = con.Query(sql, param, null, true, null, CommandType.StoredProcedure).SingleOrDefault();
                    }
                }
                catch (Exception ex)
                {
                    Log.Error(sql + " ExecuteIListWithOutTran error", ex);
                    throw ex;
                }
            }

            return obj;
        }

        private int ExecuteNonQueryDefault(CommandType commandType, string sql, object param, IDbTransaction transaction, IDbConnection con)
        {
            int result = 0;
            if (con.State == ConnectionState.Closed)
            {
                con.Open();
            }
            try
            {
                if (commandType == CommandType.Text)
                {
                    result = con.Execute(LoadSql(sql, param), param, transaction, null, CommandType.Text);
                }
                else
                {
                    result = con.Execute(sql, param, transaction, null, CommandType.StoredProcedure);
                }
            }
            catch (Exception ex)
            {
                Log.Error(sql + " ExecuteNonQueryDefault error", ex);
                throw ex;
            }
            return result;
        }

        private int ExecuteNonQueryWithTran(CommandType commandType, string sql, object param)
        {
            int result = 0;
            using (var con = DbService())
            {
                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                }
                using (IDbTransaction trans = con.BeginTransaction(IsolationLevel.ReadCommitted))
                {
                    try
                    {
                        if (commandType == CommandType.Text)
                        {
                            result = con.Execute(LoadSql(sql, param), param, trans, null, CommandType.Text);
                        }
                        else
                        {
                            result = con.Execute(sql, param, trans, null, CommandType.StoredProcedure);
                        }
                        trans.Commit();
                    }
                    catch (Exception ex)
                    {
                        trans.Rollback();
                        Log.Error(sql + " ExecuteNonQueryWithTran error", ex);
                        throw ex;
                    }
                }

            }
            return result;
        }

        private int ExecuteNonQueryWithOutTran(CommandType commandType, string sql, object param)
        {
            int result = 0;
            using (var con = DbService())
            {
                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                }
                try
                {
                    if (commandType == CommandType.Text)
                    {
                        result = con.Execute(LoadSql(sql, param), param, null, null, CommandType.Text);
                    }
                    else
                    {
                        result = con.Execute(sql, param, null, null, CommandType.StoredProcedure);
                    }
                }
                catch (Exception ex)
                {
                    Log.Error(sql + " ExecuteNonQueryWithOutTran error", ex);
                    throw ex;
                }

            }
            return result;
        }

        private IList<TReturn> ExecuteIListDefault<TFirst, TSecond, TReturn>(CommandType commandType, string sql, Func<TFirst, TSecond, TReturn> map, object param, IDbTransaction trans, string splitOn, IDbConnection con)
        {
            IList<TReturn> list = null;
            if (con.State == ConnectionState.Closed)
            {
                con.Open();
            }
            try
            {
                if (commandType == CommandType.Text)
                {
                    list = con.Query<TFirst, TSecond, TReturn>(LoadSql(sql, param), map, param, trans, true, splitOn, null, CommandType.Text).ToList<TReturn>();
                }
                else
                {
                    list = con.Query<TFirst, TSecond, TReturn>(sql, map, param, trans, true, splitOn, null, CommandType.StoredProcedure).ToList<TReturn>();
                }
            }
            catch (Exception ex)
            {
                Log.Error(sql + " ExecuteIListDefault error", ex);
                throw ex;
            }
            return list;

        }

        public IList<TReturn> ExecuteIListWithTran<TFirst, TSecond, TReturn>(CommandType commandType, string sql, Func<TFirst, TSecond, TReturn> map, object param, string splitOn)
        {
            IList<TReturn> list = null;
            using (var con = DbService())
            {
                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                }

                using (IDbTransaction trans = con.BeginTransaction(IsolationLevel.ReadCommitted))
                {
                    try
                    {
                        if (commandType == CommandType.Text)
                        {
                            list = con.Query<TFirst, TSecond, TReturn>(LoadSql(sql, param), map, param, trans, true, splitOn, null, CommandType.Text).ToList<TReturn>();
                        }
                        else
                        {
                            list = con.Query<TFirst, TSecond, TReturn>(sql, map, param, trans, true, splitOn, null, CommandType.StoredProcedure).ToList<TReturn>();
                        }
                        trans.Commit();
                    }
                    catch (Exception ex)
                    {
                        trans.Rollback();
                        Log.Error(sql + " ExecuteIListDefault error", ex);
                        throw ex;
                    }
                }
            }
            return list;
        }

        private IList<TReturn> ExecuteIListWithOutTran<TFirst, TSecond, TReturn>(CommandType commandType, string sql, Func<TFirst, TSecond, TReturn> map, object param, string splitOn)
        {
            IList<TReturn> list = null;
            using (var con = DbService())
            {
                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                }
                try
                {
                    if (commandType == CommandType.Text)
                    {
                        list = con.Query<TFirst, TSecond, TReturn>(LoadSql(sql, param), map, param, null, true, splitOn, null, CommandType.Text).ToList<TReturn>();
                    }
                    else
                    {
                        list = con.Query<TFirst, TSecond, TReturn>(sql, map, param, null, true, splitOn, null, CommandType.StoredProcedure).ToList<TReturn>();
                    }
                }
                catch (Exception ex)
                {
                    Log.Error(sql + " ExecuteIListDefault error", ex);
                    throw ex;
                }
            }
            return list;
        }
        #endregion
    }
}

