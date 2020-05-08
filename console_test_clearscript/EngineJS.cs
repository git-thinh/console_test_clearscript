using Microsoft.ClearScript.V8;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;

namespace console_test_clearscript
{
    public class _CONFIG
    {
        public static string PATH_ROOT = @"E:\test\console_test_clearscript\console_test_clearscript\bin\Debug";
    }

    public class EngineJS
    {
        static V8ScriptEngine m_engine;
        public static void _init()
        {
            m_engine = new V8ScriptEngine();
            m_engine.AddHostObject("___api", new apiJS());
        }

        public static string Execute(string file___api, Dictionary<string, object> parameters = null, Dictionary<string, object> request = null)
        {
            if (request == null) request = new Dictionary<string, object>();
            if (parameters == null) parameters = new Dictionary<string, object>();

            oResult r = new oResult()
            {
                ok = false,
                name = file___api,
                input = parameters,
                request = request
            };

            string file_main = Path.Combine(_CONFIG.PATH_ROOT, "_api\\main.js");
            if (File.Exists(file_main) == false)
            {
                r.error = "ERROR[clsEngineJS.Execute] Cannot found file: main.js ";
                return JsonConvert.SerializeObject(r);
            }

            string file_api = Path.Combine(_CONFIG.PATH_ROOT, "_api\\" + file___api + ".js");
            if (File.Exists(file_api) == false)
            {
                r.error = "ERROR[clsEngineJS.Execute] Cannot found file: " + file_api;
                return JsonConvert.SerializeObject(r);
            }

            try
            {
                string js_main = File.ReadAllText(file_main);
                string js_api = File.ReadAllText(file_api);

                js_main = js_main.Replace("[FILE___API]", file___api)
                    .Replace("[TEXT_REQUEST]", JsonConvert.SerializeObject(request))
                    .Replace("[TEXT_PARAMETER]", JsonConvert.SerializeObject(parameters))
                    .Replace("[EXECUTE_TEXT_JS]", js_api);

                var toReturn = m_engine.Evaluate(js_main);
                if (toReturn is Microsoft.ClearScript.Undefined)
                {
                    r.ok = true;
                }
                else
                {
                    string json = toReturn.ToString();
                    return json;
                }

            }
            catch (Exception e)
            {
                r.error = "ERROR_THROW: " + e.Message;
            }
            return JsonConvert.SerializeObject(r);
        }
    }

    public class apiJS
    {
        public void log(string file___api, string text)
        {
        }
        
        public string js_call(string file___api, string api_name, string paramenter = null, string request = null)
        {
            oResult r = new oResult() { ok = false };

            Dictionary<string, object> para = null;
            Dictionary<string, object> req = null;

            if (string.IsNullOrWhiteSpace(paramenter)) para = new Dictionary<string, object>();
            if (string.IsNullOrWhiteSpace(request)) req = new Dictionary<string, object>();

            try
            {
                if (!string.IsNullOrWhiteSpace(paramenter))
                    para = JsonConvert.DeserializeObject<Dictionary<string, object>>(paramenter);
            }
            catch (Exception ex)
            {
                r.error = "Convert Paramenter to type Dictionary<string, object> has error: " + ex.Message;
                return JsonConvert.SerializeObject(r);
            }

            try
            {
                if (!string.IsNullOrWhiteSpace(request))
                    req = JsonConvert.DeserializeObject<Dictionary<string, object>>(request);
            }
            catch (Exception ex)
            {
                r.error = "Convert Request to type Dictionary<string, object> has error: " + ex.Message;
                return JsonConvert.SerializeObject(r);
            }

            bool exist = false;
            switch (api_name)
            {
                case "notify_user":
                    exist = true;
                    break;
                case "notify_broadcast":
                    exist = true;
                    break;
                case "db_execute":
                    exist = true;
                    r = db_execute(para, req);
                    break;
                case "file_read":
                    exist = true;
                    string file = para.getValue("file");
                    if (File.Exists(file))
                    {
                        r.ok = true;
                        r.data = File.ReadAllText(file);
                    }
                    else {
                        r.error = "Cannot found file: " + file;
                    }
                    break;
            }

            if (exist == false)
                r.error = "Cannot found API " + api_name;

            return JsonConvert.SerializeObject(r);
        }


        #region [ db_execute ]

        public oResult db_rollback(Dictionary<string, object> paramenter = null, Dictionary<string, object> request = null) {
            return null;
        }

        public oResult db_execute(Dictionary<string, object> paramenter = null, Dictionary<string, object> request = null)
        {
            if (paramenter == null) paramenter = new Dictionary<string, object>();
            if (request == null) request = new Dictionary<string, object>();
            oResult r = new oResult() { ok = false, request = request, input = paramenter };
            try
            {
                if (paramenter.Count == 0
                    || !paramenter.ContainsKey("connect_string")
                    || !paramenter.ContainsKey("script_command"))
                {
                    r.error = "Paramenter missing connect_string or script_command";
                    return r;
                }

                var connect_string = paramenter.getValue("connect_string");
                var script_command = paramenter.getValue("script_command");

                using (SqlConnection connection = new SqlConnection(connect_string))
                {
                    SqlCommand cmd = new SqlCommand(script_command, connection);

                    foreach (var kv in paramenter)
                        cmd.Parameters.AddWithValue("@" + kv.Key, kv.Value);

                    foreach (var kv in request)
                        cmd.Parameters.AddWithValue("@" + kv.Key, kv.Value);

                    SqlDataAdapter dataAdapt = new SqlDataAdapter();
                    dataAdapt.SelectCommand = cmd;
                    DataTable dataTable = new DataTable();
                    dataAdapt.Fill(dataTable);

                    r.ok = true;
                    r.data = dataTable;
                }
            }
            catch (Exception e)
            {
                r.error = "ERROR_THROW: API[db_execute] " + e.Message;
            }
            return r;
        }

        #endregion
    }
}