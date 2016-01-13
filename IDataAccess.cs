using System;
using System.Collections.Generic;
using System.Data;

namespace ILvYou.DBAccess
{
    public interface IDataAccess
    {
        /// <summary>
        /// 定义一个Connection
        /// </summary>
        /// <returns></returns>
        IDbConnection DbService();

        /// <summary>
        /// 关闭数据连接
        /// </summary>
        /// <param name="con">数据连接IDbConnection</param>
        /// <returns>成功返回true，失败返回false</returns>
        bool CloseConnection(IDbConnection con);

        /// <summary>
        /// 执行一个sql文件，返回Ilist<T>
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="sql">sql文件地址</param>
        /// <param name="param">sql参数</param>
        /// <param name="hasHransaction">是否需要事物</param>
        /// <returns></returns>
        IList<T> ExecuteIList<T>(string sql, object param, bool hasHransaction = false);
        T ExecuteScalar<T>(string sql, object param, bool hasHransaction = false, CommandType commandType=CommandType.Text);
        /// <summary>
        /// 执行一个sql文件，返回Ilist<T>
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="sql">sql文件地址</param>
        /// <param name="param">sql参数</param>
        /// <param name="transaction">外部事物</param>
        /// <param name="con">数据连接IDbConnection</param>
        /// <returns></returns>
        IList<T> ExecuteIList<T>(string sql, object param, IDbTransaction transaction, IDbConnection con);

        /// <summary>
        /// 执行一个存储过程，返回Ilist<T>
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="sql">存储过程名</param>
        /// <param name="param">sql参数</param>
        /// <param name="hasHransaction">是否需要事物</param>
        /// <returns>Ilist<T></returns>
        IList<T> ExecuteIListBySP<T>(string sql, object param, bool hasHransaction = false);

        /// <summary>
        /// 执行一个存储过程，返回Ilist<T>
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="sql">存储过程名</param>
        /// <param name="param">sql参数</param>
        /// <param name="transaction">外部事物</param>
        /// <param name="con">数据连接IDbConnection</param>
        /// <returns></returns>
        IList<T> ExecuteIListBySP<T>(string sql, object param, IDbTransaction transaction, IDbConnection con);

        /// <summary>
        /// 执行一个sql文件，返回一个object
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="sql">sql文件地址</param>
        /// <param name="param">sql参数</param>
        /// <param name="hasHransaction">是否需要事物</param>
        /// <returns></returns>
        object ExecuteObj(string sql, object param, bool hasHransaction = false);

        /// <summary>
        /// 执行一个sql文件，返回一个object
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="sql">sql文件地址</param>
        /// <param name="param">sql参数</param>
        /// <param name="transaction">外部事物</param>
        /// <param name="con">数据连接IDbConnection</param>
        /// <returns></returns>
        object ExecuteObj(string sql, object param, IDbTransaction transaction, IDbConnection con);

        /// <summary>
        /// 执行一个存储过程，返回object
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="sql">存储过程名</param>
        /// <param name="param">sql参数</param>
        /// <param name="hasHransaction">是否需要事物</param>
        /// <returns></returns>
        object ExecuteObjBySP(string sql, object param, bool hasHransaction = false);

        /// <summary>
        /// 执行一个存储过程，返回object
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="sql">存储过程名</param>
        /// <param name="param">sql参数</param>
        /// <param name="transaction">外部事物</param>
        /// <param name="con">数据连接IDbConnection</param>
        /// <returns></returns>
        object ExecuteObjBySP(string sql, object param, IDbTransaction transaction, IDbConnection con);

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
        IList<TReturn> ExecuteIList<TFirst, TSecond, TReturn>(string sql, Func<TFirst, TSecond, TReturn> map, object param, string splitOn, bool hasHransaction = false);

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
        IList<TReturn> ExecuteIList<TFirst, TSecond, TReturn>(string sql, Func<TFirst, TSecond, TReturn> map, object param, string splitOn, IDbTransaction transaction, IDbConnection con);

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
        IList<TReturn> ExecuteIListBySP<TFirst, TSecond, TReturn>(string sql, Func<TFirst, TSecond, TReturn> map, object param, string splitOn, bool hasHransaction = false);

        /// <summary>
        /// 存储过程执行一个连接查询，返回一个IList<TReturn>
        /// </summary>
        /// <typeparam name="TFirst">第一个连接对象类型</typeparam>
        /// <typeparam name="TSecond">第二个连接对象类型</typeparam>
        /// <typeparam name="TReturn">返回对象的类型</typeparam>
        /// <param name="sql">存储过程名称</param>
        /// <param name="map">俩个表连接的方法</param>
        /// <param name="param">sql参数</param>
        /// <param name="splitOn">从哪个列起开始读取第二个对象的列名</param>
        /// <param name="transaction">外部事物</param>
        /// <param name="con">数据连接IDbConnection</param>
        /// <returns></returns>
        IList<TReturn> ExecuteIListBySP<TFirst, TSecond, TReturn>(string sql, Func<TFirst, TSecond, TReturn> map, object param, string splitOn, IDbTransaction transaction, IDbConnection con);

        /// <summary>
        /// 执行sql文件，返回受影响行数
        /// </summary>
        /// <param name="sql">sql文件地址</param>
        /// <param name="param">sql参数</param>
        /// <param name="hasHransaction">是否需要事物</param>
        /// <returns></returns>
        int ExecuteNonQuery(string sql, object param, bool hasHransaction = false);

        /// <summary>
        /// 执行sql文件，返回受影响行数
        /// </summary>
        /// <param name="sql">sql文件地址</param>
        /// <param name="param">sql参数</param>
        /// <param name="transaction">外部事物</param>
        /// <param name="con">数据连接IDbConnection</param>
        /// <returns></returns>
        int ExecuteNonQuery(string sql, object param, IDbTransaction transaction, IDbConnection con);

        /// <summary>
        /// 执行存储过程，返回受影响行数
        /// </summary>
        /// <param name="sql">存储过程名称</param>
        /// <param name="param">sql参数</param>
        /// <param name="hasHransaction">是否需要事物</param>
        /// <returns></returns>
        int ExecuteNonQueryBySP(string sql, object param, bool hasHransaction = false);

        /// <summary>
        /// 执行存储过程，返回受影响行数
        /// </summary>
        /// <param name="sql">存储过程名称</param>
        /// <param name="param">sql参数</param>
        /// <param name="transaction">外部事物</param>
        /// <param name="con">数据连接IDbConnection</param>
        /// <returns></returns>
        int ExecuteNonQueryBySP(string sql, object param, IDbTransaction transaction, IDbConnection con);
    }
}
