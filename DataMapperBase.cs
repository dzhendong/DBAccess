/**
 * -----创建--------------------
 * 作 者：BARRY
 * 日 期：2013-02-20 18:00
 * 描 述：数据适配器基类。
 * */

using Npgsql;
using System;
using System.Data;
using System.Web.Configuration;
using ILvYou.Common;

namespace ILvYou.DBAccess
{
	/// <summary>
	/// 数据适配器基类
	/// </summary>
	/// <typeparam name="T">所需的数据类型</typeparam>
	public abstract class DataMapperBase<T>
	{
		#region Protected Field
		protected NpgsqlConnection dbConnection = null;
		protected NpgsqlCommand command = null;
		protected NpgsqlTransaction npgTran = null;
		protected NpgsqlDataReader iReader = null;

		protected string plpgConnectString = String.Empty;
		#endregion

		#region 方法
		/// <summary>
		/// 从DataReader中填充领域模型
		/// </summary>
		/// <returns></returns>
		protected abstract T FillDataFromReader();

		/// <summary>
		/// 依据领域模型与数据行为，填充数据到DBCommand的Paras中
		/// </summary>
		/// <param name="item">领域模型</param>
		/// <param name="action">数据行为</param>
		protected abstract void FillParaFromItem(T item, DBActon action);
		#endregion

		#region 构造
		public DataMapperBase()
		{
			InitConnectionAndCommand ("BCDB"); 
		}

		public DataMapperBase(string connStringsKey)
		{
			InitConnectionAndCommand (connStringsKey);
		}

		/// <summary>
		/// Inits the connection and command.
		/// </summary>
		/// <param name="connStringsKey">Conn strings key.</param>
        public void InitConnectionAndCommand(string connStringsKey)
        {
            try
            {
                string _connString = WebConfigurationManager.ConnectionStrings[connStringsKey].ConnectionString;
                this.plpgConnectString = _connString;

                if (dbConnection != null && dbConnection.State == ConnectionState.Open)
                {
                    return;
                }

                dbConnection = new Npgsql.NpgsqlConnection(_connString);

                if (dbConnection.State == ConnectionState.Closed)
                {
                    dbConnection.Open();
                }

                command = dbConnection.CreateCommand();
                command.Connection = dbConnection;
            }
            catch (Exception e)
            {
                string _err_msg = string.Format("QD InitConnectionAndCommand open connecttion is error: ", e.Message);
                LogMessage.WriteLog("QD", _err_msg);
                CloseDBCommand();
                throw new Exception(_err_msg);
            }
            finally
            {
            }
        }
		#endregion

		#region 基类公用函数
		/// <summary>
		/// Inserts the para to db command.
		/// </summary>
		/// <param name="dbcommand">Dbcommand.</param>
		/// <param name="paraName">Para name.</param>
		/// <param name="dbType">Db type.</param>
		/// <param name="value">Value.</param>
		protected void InsertParaToDbCommand(NpgsqlCommand dbcommand, string paraName, System.Data.DbType dbType, object value)
        {
			NpgsqlParameter _para = dbcommand.CreateParameter ();
			_para.ParameterName = paraName;
            _para.DbType = dbType;
			_para.Value = value;
			dbcommand.Parameters.Add (_para);
		}

		/// <summary>
		/// 关闭DB COMMAND的DB链接
		/// </summary>
        protected void CloseDBCommand()
        {
            if (iReader != null)
            {
                iReader.Close();
            }

            if (command != null)
            {
                command.Dispose();
            }

            if (dbConnection != null)
            {
                dbConnection.Close();
                dbConnection.ClearPool();
            }
        }
		#endregion
	}

    /// <summary>
    /// 操作数据的行为枚举
    /// </summary>
    public enum DBActon
    {
        /// <summary>
        /// 新增
        /// </summary>
        New,

        /// <summary>
        /// 编辑
        /// </summary>
        Edit,

        /// <summary>
        /// 删除
        /// </summary>
        Delete
    }
}