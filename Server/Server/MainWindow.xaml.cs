using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;


namespace Server
{
    public partial class MainWindow : Window
    {

        //该窗口是用于手动控制“你画我猜”应用程序的服务端，仅用于启动以及停止监听

        private ServiceHost host1;
        private ServiceHost host2;
        private ServiceHost host3;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void insertQues(string filePath)
        {
            string[] lines = File.ReadAllLines(filePath, Encoding.UTF8);
            MyDbEntities myDbEntities = new MyDbEntities();

            int id = 20;

            foreach (string line in lines)
            {
                ++id;
                Questions q = new Questions();
                q.Question = line;
                q.Id = id;
                q.Tip = string.Format("{0}个字", line.Count());
                myDbEntities.Questions.Add(q);
                
            }
            myDbEntities.SaveChanges();
        }

        //启动服务
        private void btnStart_Click(object sender, RoutedEventArgs e)
        {
            MyDbEntities myDbEntities = new MyDbEntities();

            //string filePath = @"D:\QQ\QQchatFile\1946146453\FileRecv\[中文] 啥都有（245个词）.txt";
            //insertQues(filePath);
            

            ChangeState(btnStart, false, btnStop, true);
            host1 = new ServiceHost(typeof(Service));
            host1.Open();
            textBlock1.Text += "####################################\n";
            textBlock1.Text += "本机服务已启动，监听的Uri为：\n";
            foreach (var v in host1.Description.Endpoints)
            {
                textBlock1.Text += v.ListenUri.ToString() + "\n";
                scrollviewer.ScrollToBottom();
            }

            host2 = new ServiceHost(typeof(LoginService));
            host2.Open();
            //textBlock1.Text += "本机服务已启动，监听的Uri为：\n";
            foreach (var v in host2.Description.Endpoints)
            {
                textBlock1.Text += v.ListenUri.ToString() + "\n";
                scrollviewer.ScrollToBottom();
            }

            host3 = new ServiceHost(typeof(CheckinServer));
            host3.Open();
            //textBlock1.Text += "本机服务已启动，监听的Uri为：\n";
            foreach (var v in host3.Description.Endpoints)
            {
                textBlock1.Text += v.ListenUri.ToString() + "\n";
                textBlock1.Text += "####################################\n";
                scrollviewer.ScrollToBottom();
            }


        }

        //关闭服务
        private void btnStop_Click(object sender, RoutedEventArgs e)
        {
            host1.Close();
            host2.Close();
            host3.Close();
            textBlock1.Text += "本机服务已关闭\n";
            scrollviewer.ScrollToBottom();
            ChangeState(btnStart, true, btnStop, false);
        }

        //改变按钮状态
        private static void ChangeState(Button btnStart, bool isStart, Button btnStop, bool isStop)
        {
            btnStart.IsEnabled = isStart;
            btnStop.IsEnabled = isStop;
        }

        //窗体关闭事件，防止窗体关闭但是服务未关闭
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (host1 != null)
            {
                if (host1.State == CommunicationState.Opened)
                {
                    host1.Close();
                }
            }

            if (host2 != null)
            {
                if (host2.State == CommunicationState.Opened)
                {
                    host2.Close();
                }
            }

            if (host3 != null)
            {
                if (host3.State == CommunicationState.Opened)
                {
                    host3.Close();
                }
            }
        }
    }
}
