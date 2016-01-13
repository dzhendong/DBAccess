using System;
using System.Collections.Generic;
using System.Data;

namespace ILvYou.DBAccess
{
    public class DBAccessProxy : IDataAccess
    {
        #region ctor

        public DBAccessProxy(IDataAccess dataAccess)
        {
            if (dataAccess == null)
            {
                throw new ArgumentNullException("dataAccess");
            }

            _dataAccess = dataAccess;
        }

        #endregion ctor

        #region private fields

        private readonly IDataAccess _dataAccess;

        #endregion private fields

        #region public method
        public IDbConnection DbService()
        {
            return _dataAccess.DbService();
        }

        public bool CloseConnection(IDbConnection con)
        {
            return _dataAccess.CloseConnection(con);
        }

        public IList<T> ExecuteIList<T>(string sql, object param, bool hasHransaction = false)
        {
            return _dataAccess.ExecuteIList<T>(sql, param, hasHransaction);
        }

        public IList<T> ExecuteIList<T>(string sql, object param, IDbTransaction transaction, IDbConnection con)
        {
            return _dataAccess.ExecuteIList<T>(sql, param, transaction, con);
        }

        public IList<T> ExecuteIListBySP<T>(string sql, object param, bool hasHransaction = false)
        {
            return _dataAccess.ExecuteIListBySP<T>(sql, param, hasHransaction);
        }

        public IList<T> ExecuteIListBySP<T>(string sql, object param, IDbTransaction transaction, IDbConnection con)
        {
            return _dataAccess.ExecuteIListBySP<T>(sql, param, transaction, con);
        }

        public object ExecuteObj(string sql, object param, bool hasHransaction = false)
        {
            return _dataAccess.ExecuteObj(sql, param, hasHransaction);
        }

        public object ExecuteObj(string sql, object param, IDbTransaction transaction, IDbConnection con)
        {
            return _dataAccess.ExecuteObj(sql, param, transaction, con);
        }

        public object ExecuteObjBySP(string sql, object param, bool hasHransaction = false)
        {
            return _dataAccess.ExecuteObjBySP(sql, param, hasHransaction);
        }

        public object ExecuteObjBySP(string sql, object param, IDbTransaction transaction, IDbConnection con)
        {
            return _dataAccess.ExecuteObjBySP(sql, param, transaction, con);
        }

        public IList<TReturn> ExecuteIList<TFirst, TSecond, TReturn>(string sql, Func<TFirst, TSecond, TReturn> map, object param, string splitOn, bool hasHransaction = false)
        {
            return _dataAccess.ExecuteIList<TFirst, TSecond, TReturn>(sql, map, param, splitOn, hasHransaction);
        }

        public IList<TReturn> ExecuteIList<TFirst, TSecond, TReturn>(string sql, Func<TFirst, TSecond, TReturn> map, object param, string splitOn, IDbTransaction transaction, IDbConnection con)
        {
            return _dataAccess.ExecuteIList<TFirst, TSecond, TReturn>(sql, map, param, splitOn, transaction, con);
        }

        public IList<TReturn> ExecuteIListBySP<TFirst, TSecond, TReturn>(string sql, Func<TFirst, TSecond, TReturn> map, object param, string splitOn, bool hasHransaction = false)
        {
            return _dataAccess.ExecuteIListBySP<TFirst, TSecond, TReturn>(sql, map, param, splitOn, hasHransaction);
        }

        public IList<TReturn> ExecuteIListBySP<TFirst, TSecond, TReturn>(string sql, Func<TFirst, TSecond, TReturn> map, object param, string splitOn, IDbTransaction transaction, IDbConnection con)
        {
            return _dataAccess.ExecuteIListBySP<TFirst, TSecond, TReturn>(sql, map, param, splitOn, transaction, con);
        }

        public int ExecuteNonQuery(string sql, object param, bool hasHransaction = false)
        {
            return _dataAccess.ExecuteNonQuery(sql, param, hasHransaction);
        }

        public int ExecuteNonQuery(string sql, object param, IDbTransaction transaction, IDbConnection con)
        {
            return _dataAccess.ExecuteNonQuery(sql, param, transaction, con);
        }

        public int ExecuteNonQueryBySP(string sql, object param, bool hasHransaction = false)
        {
            return _dataAccess.ExecuteNonQueryBySP(sql, param, hasHransaction);
        }

        public int ExecuteNonQueryBySP(string sql, object param, IDbTransaction transaction, IDbConnection con)
        {
            return _dataAccess.ExecuteNonQueryBySP(sql, param, transaction, con);
        }
        public T ExecuteScalar<T>(string sql, object param, bool hasHransaction = false, CommandType commandType=CommandType.Text)
        {
            return _dataAccess.ExecuteScalar<T>(sql, param, hasHransaction, commandType);
        }
        #endregion
    }
}
