using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace QF_Timer_Client
{
    public partial class Form2 : Form
    {
        Form1 form1;
        public Form2(Form1 _form1)
        {
            form1 = _form1;
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (textBox1.Text != "" && textBox2.Text != "")
            {
                var config = new ConfigInfo { Server = textBox1.Text, ServerPort = Convert.ToInt32(textBox2.Text) };
                File.WriteAllText("config_client.json",JsonConvert.SerializeObject(config));
                form1.InitConfig();
                this.Close();
            }
            else
            {
                MessageBox.Show("请填写完整");
            }


        }

        private void Form2_Load(object sender, EventArgs e)
        {
            var config = JsonConvert.DeserializeObject<ConfigInfo>(File.ReadAllText("config_client.json"));
            if (config==null)
            {
                MessageBox.Show("读取配置文件错误");
            }
            else
            {
                textBox1.Text = config.Server;
                textBox2.Text = config.ServerPort + "";
            }
            
        }
    }
}
