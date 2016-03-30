using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
//using System.Threading.Tasks;
using System.Windows.Forms;

namespace SWXTools.Until
{
    class CSVFileHelper
    {

        /// <summary>
         /// 将DataTable中数据写入到CSV文件中
         /// </summary>
         /// <param name="dt">提供保存数据的DataTable</param>
         /// <param name="fileName">CSV的文件路径</param>
         public static void SaveCSV(DataTable dt, string fullPath)
         {
             FileInfo fi = new FileInfo(fullPath);
             if (!fi.Directory.Exists)
             {
                 fi.Directory.Create();
             }
             FileStream fs = new FileStream(fullPath, System.IO.FileMode.Create, System.IO.FileAccess.Write);
             //StreamWriter sw = new StreamWriter(fs, System.Text.Encoding.Default);
             StreamWriter sw = new StreamWriter(fs, System.Text.Encoding.UTF8);
             string data = "";
             //写出列名称
             for (int i = 0; i < dt.Columns.Count; i++)
             {
                 data += dt.Columns[i].ColumnName.ToString();
                 if (i < dt.Columns.Count - 1)
                 {
                     data += ",";
                 }
             }
             sw.WriteLine(data);
             //写出各行数据
             for (int i = 0; i < dt.Rows.Count; i++)
             {
                 data = "";
                 for (int j = 0; j < dt.Columns.Count; j++)
                 {
                     string str = dt.Rows[i][j].ToString();
                     str = str.Replace("\"", "\"\"");//替换英文冒号 英文冒号需要换成两个冒号
                     if (str.Contains(',') || str.Contains('"') 
                         || str.Contains('\r') || str.Contains('\n')) //含逗号 冒号 换行符的需要放到引号中
                     {
                         str = string.Format("\"{0}\"", str);
                     }
     
                     data += str;
                     if (j < dt.Columns.Count - 1)
                     {
                         data += ",";
                     }
                 }
                 sw.WriteLine(data);
             }
             sw.Close();
             fs.Close();
             DialogResult result = MessageBox.Show("CSV文件保存成功！");
             if (result == DialogResult.OK)
             {
                 //System.Diagnostics.Process.Start("explorer.exe", System.Data.Common.PATH_LANG);
             }
         }
        /// <summary>
        /// 将CSV文件的数据读取到DataTable中
        /// </summary>
        /// <param name="fileName">CSV文件路径</param>
        /// <returns>返回读取了CSV数据的DataTable</returns>
        public static DataTable OpenCSV(string filePath) 
        {
            DataTable dt = new DataTable();
            FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read);

            StreamReader sr = new StreamReader(fs, Encoding.UTF8);
            string strLine = "";
            string[] aryLine = null;
            string[] tableHead = null;
            int columnCount = 0;
            bool IsFirst = true;
            while ((strLine = sr.ReadLine()) != null)
            {
                if (IsFirst == true) 
                {
                    tableHead = strLine.Split(',');
                    IsFirst = false;
                    columnCount = tableHead.Length;

                    for (int i = 0; i < columnCount; i++) 
                    {
                        DataColumn dc = new DataColumn(tableHead[i]);
                        dt.Columns.Add(dc);
                    }
                }
                else 
                {
                    aryLine = strLine.Split(',');
                    DataRow dr = dt.NewRow();
                    for (int j = 0; j < columnCount; j++) 
                    {
                        dr[j] = aryLine[j];
                    }
                    dt.Rows.Add(dr);
                }
            }

            if (aryLine != null && aryLine.Length > 0)
                dt.DefaultView.Sort = tableHead[0] + " " + "asc";


            sr.Close();
            fs.Close();
            return dt;
        }
    }
}
