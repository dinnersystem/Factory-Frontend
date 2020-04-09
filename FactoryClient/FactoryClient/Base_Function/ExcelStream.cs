using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Threading;
using System.IO;

namespace FactoryClient
{
    class ExcelStream
    {
        const string input_pos = "input.txt", output_pos = "output.txt", external_pos = "external.bat";
        StreamWriter writer = new StreamWriter(input_pos, false); StreamReader reader = new StreamReader(output_pos);
        string path;
        public ExcelStream(string path, bool reset = true)
        {
            /* Reset the file */
            if (reset) writer.WriteLine("RESET_PAGE_BREAK_AND_ALL_CELLS");
            this.path = path;
        }
        ~ExcelStream() { try { Flush(); reader.Dispose(); } catch (Exception e) { } }
        public string Flush()
        {
            writer.Flush(); writer.Dispose();
            ProcessStartInfo start = new ProcessStartInfo();
            start.FileName = external_pos; start.Arguments = path; start.UseShellExecute = false; Process.Start(start);
            string status = reader.ReadLine();
            if (status != "OK") throw new Exception(status);
            return reader.ReadToEnd();
        }
        public void Write(int x, int y, object value) { writer.WriteLine(String.Format("WRITE,{0},{1},{2}", x, y, value.ToString())); }
        public void PageBreak(int rows) { if (rows != 1) writer.WriteLine("PAGE_BREAK,{0}", rows); }
        public List<List<string>> GetRow()
        {
            writer.WriteLine("GET_DATA"); 
            string ans = Flush();
            List<string> lines = new List<string>(ans.Split(';'));
            List<List<string>> ret = new List<List<string>>();
            foreach (string line in lines) ret.Add(new List<string>(line.Split(',')));
            return ret;
        }
    }
}
