using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace QF_Timer
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
            if (textBox1.Text!="")
            {
                File.WriteAllText("config_server.json",textBox1.Text);
                MessageBox.Show("更新配置文件成功，重启本软件开始使用！", "注意", MessageBoxButtons.OK, MessageBoxIcon.Information);
                try
                {
                    form1.CloseApp();
                }
                catch (Exception)
                {
                    MessageBox.Show("请手动重启软件！", "注意", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
               
            }
            else
            {
                MessageBox.Show("请填写！");
            }
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            string str= File.ReadAllText("config_server.json");
            textBox1.Text = str;
        }

        private void button2_Click(object sender, EventArgs e)
        {
           
            if (this.Width==385)
            {
                this.Width = 797;
            }
            else
            {
                this.Width = 385;
            }
            
        }
    }
}
