using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
//using System.Threading.Tasks;

namespace SWXTools.Until
{
    class SqlHelper
    {
        static string mysqlcon = "database=xxx;Password=xxx;User ID=root;server=xxx;Charset=xxx";
        public static void GetDataTableBySql(string sql, out Dictionary<string,string> dir) 
        {
            MySqlConnection conn = new MySqlConnection(mysqlcon);
            dir = new Dictionary<string, string>();
            try
            {

                conn.Open();
                SqlHelper.insterData(conn, "set names gbk;");
                MySqlCommand com1 = new MySqlCommand(sql, conn);
                com1.ExecuteNonQuery();

                MySqlCommand com = new MySqlCommand(sql, conn);
                MySqlDataReader tReader = com.ExecuteReader();
                while (tReader.Read())
                {
                    if (tReader.HasRows)
                    {
                        dir.Add(tReader.GetString(0),
                         tReader.GetString(1));
                    }
                }
                conn.Close();//重要！要及时关闭
                tReader.Close();
            }
            catch
            {
                conn.Close();
            }
        }

        public static void GetDataTableBySql(string sql, out Dictionary<string, int> dir)
        {
            MySqlConnection conn = new MySqlConnection(mysqlcon);
            dir = new Dictionary<string, int>();
            try
            {

                conn.Open();
                SqlHelper.insterData(conn, "set names gbk;");
                MySqlCommand com1 = new MySqlCommand(sql, conn);
                com1.ExecuteNonQuery();

                MySqlCommand com = new MySqlCommand(sql, conn);
                MySqlDataReader tReader = com.ExecuteReader();
                while (tReader.Read())
                {
                    if (tReader.HasRows)
                    {
                        dir.Add(tReader.GetString(1),tReader.GetInt32(0));
                    }
                }
                conn.Close();//重要！要及时关闭
                tReader.Close();
            }
            catch
            {
                conn.Close();
            }
        }

        public static MySqlConnection OpenConnection()
        {
            MySqlConnection conn = new MySqlConnection(mysqlcon);
            conn.Open();
            return conn;
        }

        public static int insterData(MySqlConnection conn,string sql) 
        {
            try
            {
                MySqlCommand com = new MySqlCommand(sql, conn);
                return com.ExecuteNonQuery();
            }
            catch (Exception e) 
            {
                return -1;
            }
        }

        public static void closeConnection(MySqlConnection conn) 
        {
            conn.Close();
        }
    }
}
