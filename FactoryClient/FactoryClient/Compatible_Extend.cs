using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Reflection;
using System.Windows.Forms;

namespace Compatible_Extend
{
    class Task_Extend
    {
        public static void Run(Action o) { new Thread(new ThreadStart(o)).Start();  }
    }

    class WebUtility_Extend
    {
        public static string UrlEncode(string s) {
            string str = Uri.EscapeDataString(s);
            return Uri.EscapeDataString(s);
        }
    }
}
