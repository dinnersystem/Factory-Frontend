using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Linq;
using System.Configuration;
using System.Collections.Specialized;
using System.Web;

namespace FactoryClient
{
    public class Request
    {
        string cookieHeader;
        public string uname = "";
        public string user_id = "";
        public List<string> valid_opers = new List<string>();
        static string host = "https://ssh.dinnersystem.com/dinnersys_beta/backend/backend.php";

        public Request()
        {
            HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create(host);
            req.Method = "POST";
            WebResponse wr = req.GetResponse();
            cookieHeader = wr.Headers["Set-cookie"];
        }

        string Operate(Dictionary<string, string> data)
        {
            HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create(host);
            req.Headers.Add("Cookie", cookieHeader);
            req.Method = "POST";
            req.ContentType = "application/x-www-form-urlencoded";
            StringBuilder request_str = new StringBuilder();
            foreach(var pair in data) request_str.Append(pair.Key + "=" + pair.Value + "&");
            byte[] byteArray = Encoding.UTF8.GetBytes(request_str.ToString());
            using (Stream reqStream = req.GetRequestStream()) reqStream.Write(byteArray, 0, byteArray.Length - 1);
            WebResponse wr = req.GetResponse();
            StreamReader readStream = new StreamReader(wr.GetResponseStream(), Encoding.GetEncoding("utf-8"));
            string ret = readStream.ReadToEnd();
            wr.Close();
            return ret;
        }

        public JArray Get_Organization() { return JsonConvert.DeserializeObject<JArray>(Operate(new Dictionary<string, string> { { "cmd", "show_organization" } })); }

        public void Login(string id, string password, string org_id)
        {
            string response = Operate(new Dictionary<string, string> {
                { "cmd", "login" },{ "device_id", "factory_client_"+Environment.MachineName },{ "id", id },{ "password", password },{ "org_id", org_id }
            });
            JObject obj;
            try { obj = JsonConvert.DeserializeObject<JObject>(response); }
            catch (Exception) { throw new Exception(response); }

            bool able = false;
            foreach (JToken item in obj["valid_oper"])
            {
                able |= (item.ToString(Formatting.None) == "\"update_dish\"");
                able |= (item.ToString(Formatting.None) == "\"select_facto\"");
                able |= (item.ToString(Formatting.None) == "\"select_other\"");
                valid_opers.Add(item.ToString(Formatting.None));
            }
            user_id = obj["id"].ToString(); uname = obj["name"].ToString();
            if (!able) throw new Exception("Access denied");
        }

        public JArray Get_Order(string lower_bound, string upper_bound, bool model = false)
        {
            JArray array = JsonConvert.DeserializeObject<JArray>(Operate(new Dictionary<string, string> {
                { "cmd", "select_facto" },{ "esti_start", lower_bound },{ "esti_end", upper_bound },{ "history", "true" }
            }));
            JArray ret = new JArray();
            foreach (JToken order in array)
            {
                bool alive = false;
                foreach (JToken payment in order["money"]["payment"])
                    alive |= (payment["name"].ToString(Formatting.None) == "\"payment\"" && payment["paid"].ToString(Formatting.None) == "\"true\"");
                if (alive) ret.Add(order);
            }
            return ret;
        }

        public JArray Get_Dish()
        {
            string str = Operate(new Dictionary<string, string> { { "cmd", "show_dish" }, { "sortby", "dish_id" } });
            try { JArray array = JsonConvert.DeserializeObject<JArray>(str); return array; }
            catch (Exception e) { throw new Exception(e.ToString() + "\nCurrentJson:" + str); }
        }

        public void Update_Dish(List<Dictionary<string ,string> > suffix, UpdateProgress invoker)
        {
            int count = 0;
            foreach (var tmp in suffix)
            {
                tmp["cmd"] = "update_dish";
                string resp = Operate(tmp);
                if (resp != "Nothing to update." && resp != "Successfully updated food.") throw new Exception("Unable to update dish: " + resp);
                invoker((int)Math.Ceiling((double)count++ / suffix.Count * 100));
            }
        }

        public void Report_Error(string error)
        {
            string url = host + "/dinnersys_beta/backend/backend.php?cmd=error_report";
            var postData = new
            {
                user_name = uname,
                error_string = error,
                config = Properties.Settings.Default,
                location = AppDomain.CurrentDomain.BaseDirectory
            };
            url += "&data=" + Uri.EscapeUriString(JsonConvert.SerializeObject(postData));
            url = (url.Length >= 1750 ? url.Remove(1750) : url);

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Headers.Add("Cookie", cookieHeader);
            WebResponse resp = request.GetResponse();
            string s = new StreamReader(resp.GetResponseStream(), Encoding.GetEncoding("utf-8")).ReadToEnd();

        }
    }
}