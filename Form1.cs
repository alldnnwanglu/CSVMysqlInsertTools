using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SWXTools.Until;
using SwxMysqlTool.entity;

namespace SWXTools
{
    public partial class SWXMysqlTools : Form
    {
        public SWXMysqlTools()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.openFileDialog1.Filter = "*.csv|*.csv";
            if (this.openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    string Name = this.openFileDialog1.FileName;
                    this.label2.Text = Name;
                }
                catch (Exception)
                {
                    MessageBox.Show("打开文件失败");
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Dictionary<string, string> appCache = new Dictionary<string, string>();
            Dictionary<string, int> channelCache = new Dictionary<string, int>();
            // 1. 读取 csv 文件
            string path = this.label2.Text;
            if (string.IsNullOrEmpty(path)) return;
            DataTable dt = CsvHelper.OpenCSV(path);


            // 2. 连接数据库 查找数据 建立数据缓存  userCache , chanelCache
            SqlHelper.GetDataTableBySql("select id as id,name from bossSystem.t_channel_info;", out channelCache);
            SqlHelper.GetDataTableBySql("select app_id,name from bossSystem.t_app_info;", out appCache);
            // 3. 通过缓冲建立 需要插入数据的缓存， 建立需要更新的缓存

            // 4. 开始导入数据到 mysql 中并生成结果报表
            ChannelReportDaily chanelReport = new ChannelReportDaily(dt, appCache, channelCache);
            textBox1.Text += chanelReport.doQury();

            textBox1.Text += "INFO: 完成！";
        }

        /*
        private void button3_Click(object sender, EventArgs e)
        {
            string path = this.label2.Text;
            if (string.IsNullOrEmpty(path)) return;
            DataTable dt = CsvHelper.OpenCSV(path);
            KlwOrderTools klwTool = new KlwOrderTools(dt);
            textBox1.Text += klwTool.doQury();
            textBox1.Text += "INFO: 完成！";
        }*/
    }
}
