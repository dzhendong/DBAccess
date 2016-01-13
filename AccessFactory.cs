using System;
using System.Collections.Generic;
using System.Text;

namespace ILvYou.DBAccess
{
    public static class AccessFactory
    {
        public static IDataAccess Create(ConnectParameter cp)
        {
            if (string.IsNullOrEmpty(cp.ConnectionString))
            {
                throw new ArgumentNullException("cp.ConnectionString");
            }

            return new DefaultDataAccess(cp.ConnectionString);
        }
    }
    public struct ConnectParameter
    {
        public string ConnectionString { get; set; }
        public DatabaseType DatabaseType { get; set; }
    }

    public enum DatabaseType
    {
        SqlServer = 1,
        MySql = 2,
    }
}
