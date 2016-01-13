using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace ILvYou.DBAccess
{
    /// <summary>
    /// Sql工具类
    /// </summary>
    public class SqlUtility
    {
        public static string LoadSqlFromFile(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                throw new ArgumentNullException("path");
            }

            string result;

            if (!TryGetFromCache(path, out result))
            {
                result = null;
                if (File.Exists(path))
                {
                    result = File.ReadAllText(path);
                    StoreToCache(path, result);
                }
            }

            return result;
        }

        public static string BaseDirectory()
        {
            return AppDomain.CurrentDomain.BaseDirectory;
        }

        #region private methods

        private static bool TryGetFromCache(string path, out string content)
        {
            string key;

            //Debug.Assert(!string.IsNullOrEmpty(path));

            key = GetKey(path);
            lock (s_locker)
            {
                return s_cache.TryGetValue(key, out content);
            }
        }

        private static void StoreToCache(string path, string content)
        {
            string key;

            //Debug.Assert(!string.IsNullOrEmpty(path));

            key = GetKey(path);
            lock (s_locker)
            {
                s_cache[key] = content;
            }
        }

        private static string GetKey(string path)
        {
            //Debug.Assert(!string.IsNullOrEmpty(path));
            return Path.GetFullPath(path).ToLower();
        }

        #endregion

        #region data members

        private static readonly Dictionary<string, string> s_cache = new Dictionary<string, string>();
        private static readonly object s_locker = new object();

        #endregion
    }
}