using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace System
{
    public static class ClassExt
    {
        public static string getValue(this Dictionary<string, object> dic, string key)
        {
            try
            {
                if (dic != null && dic.ContainsKey(key))
                {
                    var p = dic[key];
                    if (p == null) return string.Empty;
                    return p.ToString();
                }
            }
            catch { }
            return string.Empty;
        }

        public static T getValue<T>(this Dictionary<string, object> dic, string key)
        {
            try
            {
                if (dic != null && dic.ContainsKey(key))
                {
                    var p = (T)dic[key];
                    return p;
                }
            }
            catch { }

            T v = default(T);
            return v;
        }
         

    }
}