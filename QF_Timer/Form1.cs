using Newtonsoft.Json;
using System;
using System.Drawing;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace QF_Timer
{
    public partial class Form1 : Form
    {
        Point CPoint;
        ConfigInfo config;
        delegate void WeiTuo(string str, int i = -1, int tag = 0);
        WeiTuo wt;
        Thread thread;
        UdpClient udpClient;
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            InitConfig();
            notifyIcon1.Visible = true;

            wt = new WeiTuo(Update);

            new Thread(new ThreadStart(ReceiveData)).Start();

        }
        //初始化界面
        private void InitConfig()
        {
            config = JsonConvert.DeserializeObject<ConfigInfo>(File.ReadAllText("config_server.json"));
            if (config == null)
            {
                throw new Exception("config_server.json 配置文件初始化失败！");
            }
            InitWidget();
        }

        private void InitWidget()
        {
            this.Width = config.Width;
            this.Height = config.Height;
            label1.Font = new Font(label1.Font.FontFamily, config.FontSize, FontStyle.Bold);
            this.BackColor = ColorTranslator.FromHtml(config.BackgroundColor);
            label1.ForeColor = ColorTranslator.FromHtml(config.FontColor);
            this.Opacity = config.Opacity;
            this.Left = config.Left;
            this.Top = config.Top;
            this.WindowState = FormWindowState.Normal;
        }
        /// <summary>
        /// 开始计时
        /// </summary>
        private void StartTimer()
        {
            try
            {
                if (thread!=null && thread.IsAlive)
                {
                    thread.Abort();
                }
                InitWidget();
                thread = new Thread(new ThreadStart(TimerManager));
                thread.IsBackground = true;
                thread.Start();
            }
            catch (Exception)
            {
                // MessageBox.Show(ex.Message);
            }

        }
        private void TimerManager()
        {
            int i = 0;
            int totalSec;//总秒数
            int tempSec = 0;//剩余秒数

            foreach (int item in config.TimerList)
            {
                totalSec = item * 60;
                tempSec = totalSec;
                for (int j = 0; j < totalSec; j++)
                {
                    TimeSpan ts = new TimeSpan(0, 0, tempSec);//剩余时间
                    //System.Diagnostics.Debug.WriteLine(tempSec + "...." + ts.Seconds);
                    if (tempSec <= 60 && tempSec >= 55)
                    {
                        if (tempSec == 60)
                        {
                            this.Invoke(wt, new object[] { "", i, 4 });//最后一分钟声音
                        }
                        this.Invoke(wt, new object[] { (ts.Hours > 0 ? ts.Hours.ToString("00") + ":" : "") + ts.Minutes.ToString("00") + ":" + ts.Seconds.ToString("00"), i, 1 });//最后一分钟，显示5秒
                    }
                    else if (tempSec <= 1)
                    {
                        this.Invoke(wt, new object[] { (ts.Hours > 0 ? ts.Hours.ToString("00") + ":" : "") + ts.Minutes.ToString("00") + ":" + ts.Seconds.ToString("00"), i, 2 });
                    }
                    else
                    {
                        this.Invoke(wt, new object[] { (ts.Hours > 0 ? ts.Hours.ToString("00") + ":" : "") + ts.Minutes.ToString("00") + ":" + ts.Seconds.ToString("00"), i, 0 });
                    }

                    tempSec--;//自减1
                    Thread.Sleep(1000);
                }

                i++;

            }
            //全部完成之后最大化
            this.Invoke(wt, new object[] { "", -1, 3 });

        }
        private void Update(string str, int i = -1, int tag = 0)
        {
            switch (tag)
            {
                case 0:
                    label1.Text = string.Format(config.TimerStrArr[i], str);
                    InitWidget();
                    break;
                case 1:
                    label1.Text = "最后一分钟 " + str;
                    this.Width = Screen.PrimaryScreen.WorkingArea.Width;
                    this.Left = 0;
                    this.Top = 0;
                    break;
                case 2:
                    //时间到
                    label1.Text = string.Format(config.TimerStrArr[i], str);
                    Player(1);
                    break;
                case 3:
                    this.WindowState = FormWindowState.Maximized;
                    label1.Text = "时间到";
                    label1.Font = new Font(label1.Font.FontFamily, config.LastFontSize, FontStyle.Bold);
                    this.BackColor = ColorTranslator.FromHtml(config.LastBackgroundColor);
                    label1.ForeColor = ColorTranslator.FromHtml(config.LastFontColor);
                    break;
                case 4:
                    //播放最后一分钟声音
                    Player(2);
                    break;
                default:
                    break;
            }

        }
        /// <summary>
        /// 停止计时
        /// </summary>
        private void StopTimer()
        {
            try
            {
                if (thread!=null && thread.IsAlive)
                {
                    thread.Abort();
                }
                UpdateStop();
            }
            catch (Exception)
            {

            }

        }

        private void UpdateStop()
        {
            label1.Text = "准备中...";
            InitWidget();
        }
        private void Player(int i)
        {
            string path1 = "shijiandao.wav";
            string path2 = "zui1.wav";
            System.Media.SoundPlayer player;
            switch (i)
            {
                case 1:
                    player = new System.Media.SoundPlayer(path1);
                    player.Play();//
                    break;
                case 2:
                    player = new System.Media.SoundPlayer(path2);
                    player.Play();//
                    break;
                default:
                    break;
            }
        }

        private void ReceiveData()
        {
            try
            {
                udpClient = new UdpClient(config.ServerPort);

                IPEndPoint remoteIpEndPoint = new IPEndPoint(IPAddress.Any, 0);
                while (true)
                {
                    Byte[] receiveBytes = udpClient.Receive(ref remoteIpEndPoint);
                    string returnData = Encoding.ASCII.GetString(receiveBytes);
                    switch (returnData)
                    {
                        case "start":
                            
                            StartTimer();
                            break;
                        case "stop":
                            StopTimer();
                            break;
                        default:
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                Environment.Exit(0);
            }
        }

        #region  利用窗体上的控件移动窗体
        /// <summary>
        /// 利用控件移动窗体
        /// </summary>
        /// <param Frm="Form">窗体</param>
        /// <param e="MouseEventArgs">控件的移动事件</param>
        public void FrmMove(Form Frm, MouseEventArgs e)  //Form或MouseEventArgs添加命名空间using System.Windows.Forms;
        {
            if (e.Button == MouseButtons.Left)
            {
                Point myPosittion = Control.MousePosition;//获取当前鼠标的屏幕坐标
                myPosittion.Offset(CPoint.X, CPoint.Y);//重载当前鼠标的位置
                Frm.DesktopLocation = myPosittion;//设置当前窗体在屏幕上的位置

            }
        }
        #endregion

        private void label1_MouseDown(object sender, MouseEventArgs e)
        {
            CPoint = new Point(-e.X, -e.Y);
        }

        private void label1_MouseMove(object sender, MouseEventArgs e)
        {
            FrmMove(this, e);
        }

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            udpClient.Close();
            Environment.Exit(0);
        }

        private void 设置ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new Form2(this).ShowDialog();
        }

        public void CloseApp() {
            if (thread.IsAlive)
            {
                thread.Abort();
            }
            if (udpClient!=null)
            {
                udpClient.Close();
            }

            Environment.Exit(0);
        }
       
    }
}
