using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace ILvYou.DBAccess
{
    public static class TypeUtility
    {
        public static string ToString(object obj)
        {
            if (obj == null)
                return "";

            return obj.ToString();
        }

        /// <summary>
        /// 获取对象属性值
        /// </summary>
        public static object GetPropertyValue(object obj, string propertyName)
        {
            object result;
            PropertyInfo p;

            //Debug.Assert(obj != null);
           
            p = obj.GetType().GetProperties().Where((x) => string.Equals(x.Name, propertyName)).FirstOrDefault();

            if (p == null)
            {
                return null;
            }

            result = p.GetGetMethod().Invoke(obj, /*parameters*/null);

            return result;
        }        
    }
}
