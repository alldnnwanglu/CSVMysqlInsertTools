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
    class ChannelReportEntity 
    {
        public int id;
        public DateTime day;
        public string app_id;
        public string app_name;
        public int channel_id;
        public string channel_name;
        public string server;
        public int init_count;
        public int pay_count;
        public int total_fee;
        public float per_fee;
        public float arup;
        public DateTime createTime;
        public DateTime updateTime;

        public string getSqlString() 
        {
            if (id == 0)
                return ToInsterSqlString();
            else
                return ToUpdateSqlString();
        }
        
        private string ToInsterSqlString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("insert into bossSystem.t_channel_report_daily(");
            sb.Append("day,app_id,app_name,channel_id,channel_name,server,init_count,pay_count,total_fee,per_fee,arpu,create_time,update_time) ");
            sb.Append("values (");
            sb.Append("'").Append(day).Append("',");
            sb.Append("'").Append(app_id).Append("'").Append(",");
            sb.Append("'").Append(app_name).Append("',");
            sb.Append(channel_id).Append(",");
            sb.Append("'").Append(channel_name).Append("',");
            sb.Append("'").Append(server).Append("',");
            sb.Append(init_count).Append(",");
            sb.Append(pay_count).Append(",");
            sb.Append(total_fee).Append(",");
            sb.Append(per_fee).Append(",");
            sb.Append(arup).Append(","); 
            sb.Append("'").Append(createTime).Append("',");
            sb.Append("'").Append(updateTime).Append("');");
            return sb.ToString();
        }

        private string ToUpdateSqlString() 
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("update bossSystem.t_channel_report_daily");
            sb.Append(" set day='").Append(day).Append("',");
            sb.Append("app_id='").Append(app_id).Append("',");
            sb.Append("app_name='").Append( app_name).Append("',");
            sb.Append("channel_id=").Append(channel_id).Append(",");
            sb.Append("channel_name='").Append(channel_name).Append("',");
            sb.Append("server='").Append(server).Append("',");
            sb.Append("init_count=").Append(init_count).Append(",");
            sb.Append("pay_count=").Append(pay_count).Append(",");
            sb.Append("total_fee=").Append(total_fee).Append(",");
            sb.Append("per_fee=").Append(per_fee).Append(",");
            sb.Append("arpu=").Append(arup).Append(",");
            sb.Append("create_time='").Append(createTime).Append("',");
            sb.Append("update_time='").Append(updateTime).Append("'");
            sb.Append(" where id=").Append(id).Append(";");
            return sb.ToString();
        }
    }


    class ChannelReportDaily
    {
        private string sql = "";
        private DataTable _dt;
        private Dictionary<string, string> _dirApps;
        private Dictionary<string, int> _dirChannels;

        public ChannelReportDaily( DataTable dt , Dictionary<string , string> apps , Dictionary<string , int> channels ) 
        {
            this._dt = dt;
            this._dirApps = apps;
            this._dirChannels = channels;
        }

        public string doQury() 
        {
            StringBuilder sb = new StringBuilder();
            bool skipHead = true;
            // create ChannelReportEntity 
            ChannelReportEntity ent = new ChannelReportEntity();
            
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

                ent.id = getInt(dr, "id");
                ent.day = getDateTime(dr);
                ent.app_id = getString(dr, "app_id");
                ent.app_name = getAppName(ent.app_id, _dirApps);
                ent.channel_name = getString(dr, "channel_id");
                ent.channel_id = getChannelId(ent.channel_name, _dirChannels);
                ent.server = dr["server"].ToString();
                ent.init_count = getInt(dr, "init_count");
                ent.pay_count = getInt(dr, "pay_count");
                ent.total_fee = getInt(dr, "total_fee");
                ent.per_fee = getInt(dr, "per_fee");
                ent.arup = getFloat(dr, "arpu");
                ent.createTime = DateTime.Now;
                ent.updateTime = DateTime.Now;
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
            string dayStr = dr["day"].ToString();
            DateTime dt = DateTime.Now;
            if (DateTime.TryParse(dayStr, out dt))
                return dt;
            return DateTime.Now;
        }

        public string getAppName( string app_id,Dictionary<string,string> apps )
        {
            string name;
            if (apps.TryGetValue(app_id, out name))
                return name;
                //return Encoding.UTF8.GetString(Encoding.Default.GetBytes(name));
            return null;
        }

        public int getChannelId( string channel_name , Dictionary<string,int> channels )
        {
            int id;
            if (channels.TryGetValue(channel_name, out id))
                return id;
               // return Encoding.UTF8.GetString(Encoding.Default.GetBytes(name));
            return -1;
        }
    }
}
