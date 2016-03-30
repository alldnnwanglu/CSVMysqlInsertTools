using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
//using System.Threading.Tasks;
using SWXTools.Until;
using MySQLDriverCS;
using MySql.Data.MySqlClient;

namespace SwxMysqlTool.entity
{
    class KlwOrder 
    {
        public int id;
        public string tradeId;
        public string orderId;
        public string channel;
        public DateTime actionTime;
        public int price;

        public string getSqlString() 
        {
            return ToInsterSqlString();
        }
        
        private string ToInsterSqlString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("insert into test.t_klwOrder(");
            sb.Append("tradeId,orderId,channel,actionTime,price) ");
            sb.Append("values (");
            sb.Append("'").Append(tradeId).Append("',");
            sb.Append("'").Append(orderId).Append("',");
            sb.Append("'").Append(channel).Append("',");
            sb.Append("'").Append(actionTime).Append("',");
            sb.Append(price).Append(");");
            return sb.ToString();
        }
    }


    class KlwOrderTools
    {
        private string sql = "";
        private DataTable _dt;

        public KlwOrderTools(DataTable dt) 
        {
            this._dt = dt;
        }

        public string doQury() 
        {
            StringBuilder sb = new StringBuilder();
            bool skipHead = true;
            // create ChannelReportEntity 
            KlwOrder ent = new KlwOrder();
            
            MySqlConnection conn = null;
            try
            {
                 conn = SqlHelper.OpenConnection();
            }catch
            {
                if(conn!=null)
                    conn.Close();
                return "ERROR:505 打开连接失败,请检查网络!";
            }

            if(conn == null)
            {
                if(conn!=null)
                    conn.Close();
                return "ERROR:505 打开连接失败,请检查网络!";
            }else
            {
                sb.Append("INFO: 打开数据连接成功！").Append("\r\n");
                SqlHelper.insterData(conn, "set names gbk;");
                sb.Append("==================================================").Append("\r\n");
            }

            int index = 0 ,insert_index = 0 ,update_index =0;
            foreach (DataRow dr in _dt.Rows)
            {
                if (skipHead)
                {
                    skipHead = false;
                    continue;
                }

                ent.id = 0;
                ent.orderId = getString(dr, "orderId");
                ent.tradeId = getString(dr, "tradeId");
                ent.channel = getString(dr, "channel");
                ent.actionTime = getDateTime(dr);
                ent.price = getInt(dr, "price");
                index++;
                // 执行mysql
                int dx = SqlHelper.insterData(conn , ent.getSqlString());
                if (dx <= 0)
                    sb.Append("INFO: 第").Append(index).Append("行,导入数据失败").Append("\r\n");
                else
                    sb.Append("INFO: 第").Append(index).Append("行,导入数据成功").Append("\r\n");

                if (ent.id == 0)
                    insert_index++;
                else
                    update_index++;
            }

            sb.Append("====================").Append("inster total ").Append(insert_index).Append("====================").Append("\r\n");
            sb.Append("====================").Append("update total ").Append(update_index).Append("====================").Append("\r\n");

            try
            {
                SqlHelper.closeConnection(conn);
                sb.Append("INFO: 关闭数据库成功").Append("\r\n");
            }
            catch 
            {
                if(conn!=null)
                conn.Close(); 
            }
            return sb.ToString();
        }

        public int getInt(DataRow dr , string str) 
        {
            string idStr = dr[str].ToString();
            int id = 0;
            if (int.TryParse(idStr, out id))
                return id;
            return id;
        }

        public string getString(DataRow dr, string str) 
        {
            return dr[str].ToString();
        }

        public float getFloat(DataRow dr, string str) 
        {
            string idStr = dr[str].ToString();
            float id = 0.0f;
            if (float.TryParse(idStr, out id))
                return id;
            return id;
        }

        public DateTime getDateTime(DataRow dr) 
        {
            string dayStr = dr["actionTime"].ToString();
            DateTime dt = DateTime.Now;
            if (DateTime.TryParse(dayStr, out dt))
                return dt;
            return DateTime.Now;
        }
    }
}
