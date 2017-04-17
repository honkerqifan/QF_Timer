using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Windows.Forms;

namespace QF_Timer_Client
{
    public partial class Form1 : Form
    {
        UdpClient udpClient;
        ConfigInfo config;
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            InitConfig();
            
        }

        //初始化界面
        public void InitConfig()
        {
            config = JsonConvert.DeserializeObject<ConfigInfo>(File.ReadAllText("config_client.json"));
            if (config == null)
            {
                throw new Exception("config_client.json 配置文件初始化失败！");
            }
            richTextBox1.AppendText(DateTime.Now.ToString() + "  " + "初始化配置成功！\r\n");
            try
            {
                if (udpClient!=null)
                {
                    udpClient.Close();
                }
                udpClient = new UdpClient(0);
                udpClient.Connect(IPAddress.Parse(config.Server), config.ServerPort);
                richTextBox1.AppendText(DateTime.Now.ToString() + "  " + "绑定端口成功！\r\n");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                Environment.Exit(0);
            }

        }

        private void button1_Click(object sender, EventArgs e)
        {
            Byte[] sendBytes = Encoding.ASCII.GetBytes("start");
            udpClient.Send(sendBytes, sendBytes.Length);
            richTextBox1.AppendText(DateTime.Now.ToString() + "  " + "发送【开始计时】\r\n");
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Byte[] sendBytes = Encoding.ASCII.GetBytes("stop");
            udpClient.Send(sendBytes, sendBytes.Length);
            richTextBox1.AppendText(DateTime.Now.ToString()+"  "+"发送【停止计时】\r\n");
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            udpClient.Close();
        }

        private void 服务端设置ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            
        }

        private void 控制端设置ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new Form2(this).ShowDialog();
        }
    }
}
