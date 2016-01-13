using System;
using System.Configuration;
using System.Web.Configuration;

namespace ILvYou.DBAccess
{
    public static class DBManager
    {
        #region APIs
        public static IDataAccess BCDB_BPP
        {
            get
            {
                ConnectParameter cp;
                if (s_BCDB_BPPFactory == null)
                {
                    cp = new ConnectParameter()
                    {
                        ConnectionString = WebConfigurationManager.ConnectionStrings["BCDB_BPP"].ConnectionString
                    };
                    s_BCDB_BPPFactory = CreateDataAccess(cp);
                }
                return s_BCDB_BPPFactory;
            }
        }
        public static IDataAccess BCDB_BPPProduct
        {
            get
            {
                ConnectParameter cp;
                if (s_BCDB_BPPProduct == null)
                {
                    cp = new ConnectParameter()
                    {
                        ConnectionString = WebConfigurationManager.ConnectionStrings["BCDB_BPPProduct"].ConnectionString
                    };
                    s_BCDB_BPPProduct = CreateDataAccess(cp);
                }
                return s_BCDB_BPPProduct;
            }
        }
        public static IDataAccess BPP_Base
        {
            get
            {
                ConnectParameter cp;
                if (s_BPP_Base == null)
                {
                    cp = new ConnectParameter()
                    {
                        ConnectionString = WebConfigurationManager.ConnectionStrings["BPP_Base"].ConnectionString
                    };
                    s_BPP_Base = CreateDataAccess(cp);
                }
                return s_BPP_Base;
            }
        }
        public static IDataAccess LHF_Order
        {
            get
            {
                ConnectParameter cp;
                if (s_LHF_Order == null)
                {
                    cp = new ConnectParameter()
                    {
                        ConnectionString = WebConfigurationManager.ConnectionStrings["LHF_Order"].ConnectionString
                    };
                    s_LHF_Order = CreateDataAccess(cp);
                }
                return s_LHF_Order;
            }
        }


        #endregion APIs

        #region private methods

        private static IDataAccess CreateDataAccess(ConnectParameter cp)
        {
            return new DBAccessProxy(AccessFactory.Create(cp));
        }

        #endregion private methods

        #region private fields
        [ThreadStatic]
        private static IDataAccess s_BCDB_BPPFactory;
        [ThreadStatic]
        private static IDataAccess s_BCDB_BPPProduct;
        [ThreadStatic]
        private static IDataAccess s_BPP_Base;
        [ThreadStatic]
        private static IDataAccess s_LHF_Order;
        #endregion private fields
    }
}
