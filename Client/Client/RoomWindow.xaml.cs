using Client.CheckinReference;
using Client.LoginReference;
using System;
using System.ServiceModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;


namespace Client
{
    //大厅界面逻辑
    public partial class RoomWindow : Window, ICheckinServerCallback
    {
        private LoginServiceClient loginclient;
        private CheckinServerClient Checkinclient;
        private User item;//每一个id所属，item可以控制该id下的所有窗口
        public LoginReference.User us;//用户的所有信息
        public static bool music_play = true;
        public static MediaPlayer player = new MediaPlayer();

        //进入大厅初始化
        public RoomWindow(LoginReference.User ustmp)
        {
            InitializeComponent();
            us = ustmp;
            item = CC.GetUser(us.Acount);
            Checkinclient = new CheckinServerClient(new InstanceContext(this));
            loginclient = new LoginServiceClient();
            Checkinclient.Login(us.Name);
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            player.Open(new Uri("bgm.mp3", UriKind.Relative));
            player.Play();
        }

        //刷新房间信息
        public void refreshRoomInfo(int[] roomPlayerNum, bool[] isStart)
        {
            room1player.Text = roomPlayerNum[0].ToString() + "/8";
            room2player.Text = roomPlayerNum[1].ToString() + "/8";
            room3player.Text = roomPlayerNum[2].ToString() + "/8";
            room4player.Text = roomPlayerNum[3].ToString() + "/8";
            room1info.Text = isStart[0] ? "游戏中" : "可加入";
            room2info.Text = isStart[1] ? "游戏中" : "可加入";
            room3info.Text = isStart[2] ? "游戏中" : "可加入";
            room4info.Text = isStart[3] ? "游戏中" : "可加入";
        }

        //进入房间
        private void room_Click(object sender, RoutedEventArgs e)
        {
            //获取点击房间号
            Button bt = e.Source as Button;
            int idx = (int)((bt.Name)[4]) - 48;

            //判断是否可以进入房间
            string isCanEnter = Checkinclient.Checkin(us.Name, idx);
            if (isCanEnter!="OK")
            {
                MessageBox.Show(isCanEnter);
                return;
            }

            //隐藏大厅进入对应房间
            item.RoomWindow.Hide();
            MainWindow mw = new MainWindow(us);
            mw.roomId = idx;
            mw.us = us;
            item.MainWindow = mw;
            item.MainWindow.Show();

            //回调进入房间
            item.MainWindow.EnterRoom(us.Name, idx);

        }

        //回车发送聊天信息
        private void SendBox_KeyUp(object sender, KeyEventArgs e)
        {
            if(e.Key==Key.Enter)
            {
                if(SendBox.Text != "")
                {
                    Checkinclient.Talk(us.Name, this.SendBox.Text);
                    this.SendBox.Text = "";
                }

            }
        }

        //点击发送聊天信息
        private void SendBnt_Click(object sender, RoutedEventArgs e)
        {
            if (SendBox.Text != "")
            {
                Checkinclient.Talk(us.Name, this.SendBox.Text);
                this.SendBox.Text = "";
            }
        }

        //背景音乐控制
        private void music_Click(object sender, RoutedEventArgs e)
        {
            if (music_play)
            {
                music_play = false;
                player.Stop();
            }
            else
            {
                music_play = true;
                player.Play();
            }
        }

        //点击游戏内叉号退出游戏
        private void exitBnt_Click(object sender, RoutedEventArgs e)
        {
            Checkinclient.Logout(us.Name);
            this.Close();
        }

        //点击右上角关闭按钮退出游戏
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Checkinclient.Logout(us.Name);
        }


        #region 聊天室的回调函数实现
        //进入游戏
        public void ShowLogin(string loginUserName)
        {
            this.PlayerInfo.Text += "[" + loginUserName + "]" + "进入大厅" + '\n';
            scrollviewer.ScrollToBottom();
        }

        //退出游戏
        public void ShowLogout(string userName)
        {
            this.PlayerInfo.Text += "[" + userName + "]" + "退出大厅" + '\n';
            scrollviewer.ScrollToBottom();
        }

        //进入房间
        public void ShowCheckin(string userName, int roomnumber)
        {
            this.PlayerInfo.Text += "[" + userName + "]" + "进入了"+roomnumber+"号房间" + '\n';
            scrollviewer.ScrollToBottom();
        }

        //聊天内容
        public void ShowTalk(string userName, string message)
        {
            this.PlayerInfo.Text += "[" + userName + "]说：" + message + '\n';
            scrollviewer.ScrollToBottom();
        }

        #endregion


    }
}
