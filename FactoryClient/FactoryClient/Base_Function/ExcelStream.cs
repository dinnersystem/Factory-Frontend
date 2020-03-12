using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using Excel = Microsoft.Office.Interop.Excel;
using System.Runtime.InteropServices;
using System.Threading;
using NPOI.XSSF.UserModel;
using System.IO;
using NPOI.SS.UserModel;

namespace FactoryClient
{
    class ExcelStream
    {
        Excel.Worksheet Sheet;
        Excel.Workbook Book;
        public ExcelStream(string path ,bool reset = true)
        {
            Excel.Application _Excel;
            bool flag = false;
            foreach (var item in Process.GetProcesses())
            {
                if (item.ProcessName == "EXCEL")
                {
                    flag = true;
                    break;
                }
            }

            if (!flag)
            {
                _Excel = new Excel.Application();
            }
            else
            {
                object obj = Marshal.GetActiveObject("Excel.Application");//引用已在執行的Excel
                _Excel = obj as Excel.Application;
            }
            _Excel.Visible = false;//設false效能會比較好
            Book = _Excel.Workbooks.Open(path);
            Sheet = Book.Sheets[1];

            /* Reset the file */
            if(reset)
            {
                _Excel.ActiveSheet.ResetAllPageBreaks();
                Sheet.get_Range("A2:ZZ999", Type.Missing).Cells.Value2 = "";
            }
        }
        ~ExcelStream() { try { Close(); } catch (Exception e) { } }

        public void Close()
        {
            Book.Save();
            Process[] procs = Process.GetProcessesByName("excel");
            foreach (Process pro in procs)
            {
                pro.Kill();//沒有更好的方法,只有殺掉進程
            }
            Thread.Sleep(1000);
        }

        public void Write(int x, int y, object value) { Sheet.Cells[x, y] = value; }

        public void PageBreak(int rows)
        {
            if (rows == 1) return;
            Sheet.HPageBreaks.Add(Sheet.Range["A" + rows.ToString()]);
        }

        public List<List<string>> GetRow()
        {
            List<List<string>> ret = new List<List<string>>();
            List<string> row = new List<string>();
            for (int i = 1; ; i++)
            {
                if (((Excel.Range)Sheet.Cells[i, 1]).Text == "") break;
                for (int j = 1; ; j++)
                {
                    string value = ((Excel.Range)Sheet.Cells[i, j]).Text;
                    if (value == "" && ((Excel.Range)Sheet.Cells[i, j + 1]).Text == "")
                    {
                        ret.Add(new List<string>(row));
                        row = new List<string>();
                        break;
                    }
                    row.Add(value);
                }
            }
            return ret;
        }
    }





    class ExcelStream
    {
        string path;
        public ExcelStream(string path, bool reset = true)
        {
            Excel.Application _Excel;
            bool flag = false;
            foreach (var item in Process.GetProcesses())
            {
                if (item.ProcessName == "EXCEL")
                {
                    flag = true;
                    break;
                }
            }

            if (!flag)
            {
                _Excel = new Excel.Application();
            }
            else
            {
                object obj = Marshal.GetActiveObject("Excel.Application");//引用已在執行的Excel
                _Excel = obj as Excel.Application;
            }
            _Excel.Visible = false;//設false效能會比較好
            Book = _Excel.Workbooks.Open(path);
            Sheet = Book.Sheets[1];

            /* Reset the file */
            if (reset)
            {
                _Excel.ActiveSheet.ResetAllPageBreaks();
                Sheet.get_Range("A2:ZZ999", Type.Missing).Cells.Value2 = "";
            }
        }
        ~ExcelStream() { try { Close(); } catch (Exception e) { } }

        public void Close()
        {
            Book.Save();
            Process[] procs = Process.GetProcessesByName("excel");
            foreach (Process pro in procs)
            {
                pro.Kill();//沒有更好的方法,只有殺掉進程
            }
            Thread.Sleep(1000);
        }

        public void Write(int x, int y, object value)
        {
            using (FileStream rstr = new FileStream(file, FileMode.Open, FileAccess.Read))
            {
                IWorkbook workbook = new XSSFWorkbook(rstr);
                ISheet sheet = workbook.GetSheetAt(0);

                using (FileStream wstr = new FileStream(file, FileMode.Open, FileAccess.ReadWrite))
                {
                    string secondValue = "changes";

                    row = sheet.GetRow(x); cell = row.GetCell(y); cell.SetCellValue(secondValue);
                    Debug.Print(cell.ToString());
                    workbook.Write(wstr);
                    wstr.Close();
                }
                rstr.Close();
            }
        }

        public void PageBreak(int rows)
        {
            if (rows == 1) return;
            Sheet.HPageBreaks.Add(Sheet.Range["A" + rows.ToString()]);
        }

        public List<List<string>> ReadExcel()
        {
            XSSFWorkbook workbook = new XSSFWorkbook(path);
            XSSFSheet sheet = (XSSFSheet)workbook.GetSheetAt(0);
            List<List<string>> ret = new List<List<string>>();
            for (int i = (sheet.FirstRowNum + 1); i < sheet.LastRowNum + 1; i++)
            {
                XSSFRow row = (XSSFRow)sheet.GetRow(i); List<string> row_ans = new List<string>();
                try
                {
                    for (int j = row.FirstCellNum; j < ((XSSFRow)sheet.GetRow(0)).LastCellNum; j++)
                        if (null != row.GetCell(j) && !string.IsNullOrEmpty(row.GetCell(j).ToString()) && !string.IsNullOrWhiteSpace(row.GetCell(j).ToString()))
                            row_ans.Add(row.GetCell(j).ToString());

                }
                catch (Exception Ex) { }
                finally { if (row_ans.Count != 0) ret.Add(row_ans); }
            }
            workbook.UnlockStructure(); workbook.UnlockRevision(); workbook.UnlockWindows();
            workbook = null; sheet = null;
            return ret;
        }
    }
}
