using System;
//using log4net;

//[assembly: log4net.Config.XmlConfigurator(ConfigFile = "Log.config", ConfigFileExtension = "ext", Watch = true)]
namespace ILvYou.DBAccess
{
    /// <summary>
    /// 日志类
    /// </summary>
    public class Log
    {
        //private static readonly ILog loginfo = LogManager.GetLogger("iNotes");

        #region public members
        public static void Error(string msg, Exception ex)
        {
            //loginfo.Error(msg, ex);
        }

        public static void Error(string msg)
        {
            //loginfo.Error(msg);
        }

        public static void Info(string msg, Exception ex)
        {
            //loginfo.Info(msg, ex);
        }

        public static void Info(string msg)
        {
            //loginfo.Info(msg);
        }

        public static void Debug(string msg, Exception ex)
        {
            //loginfo.Debug(msg, ex);
        }

        public static void Debug(string msg)
        {
            //loginfo.Debug(msg);
        }

        public static void Warn(string msg, Exception ex)
        {
            //loginfo.Warn(msg, ex);
        }

        public static void Warn(string msg)
        {
            //loginfo.Warn(msg);
        }
        #endregion
    }
}