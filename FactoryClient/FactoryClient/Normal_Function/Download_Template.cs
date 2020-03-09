using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Net;

namespace FactoryClient.Normal_Function
{
    class Download_Template
    {
        const string template_location = "https://dinnersystem.com/dinnersys_beta/fclient/templates/";
        bool Exist_Local(string path) { return File.Exists(path);  }
        bool Exist_Remote(string path)
        {
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(path);
            request.Timeout = 1000;
            try { request.GetResponse(); } catch { return false; }
            return true;
        }
        void Download(string remote ,string local) { new WebClient().DownloadFile(remote, local); }
        public void Non_Override_Download(string path)
        {
            if (Exist_Local(path)) return;
            string name = Path.GetFileName(path);
            string remote = template_location + name;
            if (!Exist_Remote(remote)) throw new Exception("The template '" + name + "' does not exist in remote server.");
            Download(remote, path);
        }
    }
}
