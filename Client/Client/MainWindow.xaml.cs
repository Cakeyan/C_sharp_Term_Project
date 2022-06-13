// zyx 1248

using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Ribbon;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Client.ServiceReference;
using Client.LoginReference;



namespace Client
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window, IServiceCallback
    {
        private ServiceClient client;
        private LoginServiceClient loginclient;
        public int roomId;//暂用
        public LoginReference.User us;//用户的所有信息
        private User item;//每一个id所属，item可以控制该id下的所有窗口
        private Userdata[] userdatas;
        public List<Image> Ready;
        public List<TextBlock> scores;

        //画板相关
        private DrawingAttributes inkDA;
        private Color currentColor;
        string TipCheck;
        private Color penColor = Colors.Black;
        bool ink_runtime_flag = false;

        //用于回退步骤
        private static Stack<string> ink_stack = new Stack<string>();


        //传参方式的变化
        public MainWindow(LoginReference.User ustmp)
        {
            InitializeComponent();
            us = ustmp;
            item = CC.GetUser(us.Acount);
            WindowStartupLocation = WindowStartupLocation.CenterScreen;


            inkcanvas.IsEnabled = false;
            clear.IsEnabled = false;
            undo.IsEnabled = false;
            random.IsEnabled = false;
            Ready = new List<Image>();
            Ready.Add(ready1);
            Ready.Add(ready2);
            Ready.Add(ready3);
            Ready.Add(ready4);
            Ready.Add(ready5);
            Ready.Add(ready6);
            Ready.Add(ready7);
            Ready.Add(ready8);
            for(int i = 0; i < 8; i++)
            {
                Ready[i].Visibility = Visibility.Collapsed;
            }

            scores = new List<TextBlock>();
            scores.Add(U1);
            scores.Add(U2);
            scores.Add(U3);
            scores.Add(U4);
            scores.Add(U5);
            scores.Add(U6);
            scores.Add(U7);
            scores.Add(U8);
        }




        /*----------------------------------------------------- 分割线  ----------------------------------------------------------------*/
        #region 客户端内的方法
        //画板相关+聊天室登录信息显示
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //初始化两个服务端接口
            client = new ServiceClient(new InstanceContext(this));
            loginclient = new LoginServiceClient();
            //显示登录
            client.Login(roomId,us.Name);
            this.textBoxUserName.Content = "当前用户："+us.Name;

            //初始化墨迹和画板
            currentColor = Colors.Black;
            inkDA = new DrawingAttributes()
            {
                Color = currentColor,
                Height = 5,
                Width = 5,
                FitToCurve = false
            };
            inkcanvas.DefaultDrawingAttributes = inkDA;
            inkcanvas.EditingMode = InkCanvasEditingMode.Ink;

            inkcanvas.AddHandler(InkCanvas.MouseDownEvent, new MouseButtonEventHandler(ink_MouseDown), true);
        }



        //用于绑定enter建
        private void SendBox_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if(SendBox.Text!="")
                {
                    client.Talk(us.Name, this.SendBox.Text);
                    this.SendBox.Text = "";
                }
            }
        }

        private void send_Click(object sender, RoutedEventArgs e)
        {
            if (SendBox.Text != "")
            {
                client.Talk(us.Name, this.SendBox.Text);
                this.SendBox.Text = "";
            }
        }
        private void exitBnt_Click(object sender, RoutedEventArgs e)
        {
            client.Logout(roomId,us.Name);
            this.Close();
            item.RoomWindow.Show();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            client.Logout(roomId,us.Name);
            item.RoomWindow.Show();
        }
        #endregion



        /*----------------------------------------------------- 分割线  ----------------------------------------------------------------*/
        #region 画板的回调函数实现
        /// <summary>
        /// 画板：将InkCanvas的墨迹转换为String
        /// </summary>
        private void ink_MouseUp(object sender, MouseButtonEventArgs e)
        {
            ink_runtime_flag = false;
            temp.X = 0;
            temp.Y = 0;
            StrokeCollection sc = inkcanvas.Strokes;
            string inkData = (new StrokeCollectionConverter()).ConvertToString(sc);
            ink_stack.Push(inkData);

            client.SendInk(roomId, inkData);
        }
        public void ShowInk(string inkData)
        {
            //删除原有的Strokes
            inkcanvas.Strokes.Clear();

            //将String转换为InkCanvas的墨迹
            Type tp = typeof(StrokeCollection);
            StrokeCollection sc =
                (StrokeCollection)(new StrokeCollectionConverter()).ConvertFrom(inkData);

            //新Strokes添加到InkCanvas中
            inkcanvas.Strokes = sc;
        }
        /// <summary>
        /// 画板：初始化画笔和画板
        /// </summary>
        private void InitColor()
        {
            inkDA.Color = currentColor;
            inkcanvas.DefaultDrawingAttributes = inkDA;
        }
        /// <summary>
        /// 画板：画板界面按钮
        /// </summary>
        private void Button_Checked(object sender, RoutedEventArgs e)
        {
            string name = (e.Source as Button).Name;
            switch (name)
            {
                case "delete":
                    inkcanvas.EditingMode = InkCanvasEditingMode.EraseByPoint;
                    break;
                case "pen":
                    inkcanvas.EditingMode = InkCanvasEditingMode.Ink;
                    currentColor = penColor;
                    InitColor();
                    break;
                case "clear":
                    inkcanvas.Strokes.Clear();
                    StrokeCollection sc = inkcanvas.Strokes;
                    string inkData = (new StrokeCollectionConverter()).ConvertToString(sc);
                    ink_stack.Push(inkData);
                    client.SendInk(roomId, inkData);
                    break;
                case "undo":
                    if(ink_stack.Count > 1)
                    {
                        ink_stack.Pop();
                        client.SendInk(roomId, ink_stack.Peek());
                    }
                    else if(ink_stack.Count == 1)
                    {
                        ink_stack.Pop();
                        inkcanvas.Strokes.Clear();
                        StrokeCollection strokes = inkcanvas.Strokes;
                        string InkData = (new StrokeCollectionConverter()).ConvertToString(strokes);
                        client.SendInk(roomId, InkData);
                    }
                    break;
            }
        }

        private void image_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Mouse.Capture(this);

            this.MouseMove += MainWindow_MouseMove;
            this.MouseUp += MainWindow_MouseUp;
        }

        private void MainWindow_MouseMove(object sender, MouseEventArgs e)
        {
            var pos = e.GetPosition(SampleImage);
            var img = SampleImage.Source as BitmapSource;

            if (pos.X > 0 && pos.Y > 0 && pos.X < img.PixelWidth && pos.Y < img.PixelHeight)
                SampleImageClick(img, pos);
        }

        protected void SampleImageClick(BitmapSource img, Point pos)
        {
            int stride = (int)img.Width * 4;
            int size = (int)img.Height * stride;
            byte[] pixels = new byte[(int)size];

            img.CopyPixels(pixels, stride, 0);

            // Get pixel
            var x = (int)pos.X;
            var y = (int)pos.Y;

            int index = y * stride + 4 * x;

            byte red = pixels[index];
            byte green = pixels[index + 1];
            byte blue = pixels[index + 2];
            byte alpha = pixels[index + 3];

            inkcanvas.EditingMode = InkCanvasEditingMode.Ink;
            currentColor = Color.FromArgb(alpha, blue, green, red);
            InitColor();

            penColor = currentColor;
        }

        private void MainWindow_MouseUp(object sender, MouseButtonEventArgs e)
        {
            Mouse.Capture(null);

            this.MouseMove -= MainWindow_MouseMove;
            this.MouseUp -= MainWindow_MouseUp;
        }

        private void slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            inkcanvas.DefaultDrawingAttributes.Width = slider.Value;
            inkcanvas.DefaultDrawingAttributes.Height = slider.Value;
        }

        private void ink_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (inkcanvas.EditingMode == InkCanvasEditingMode.Ink)
                ink_runtime_flag = true;
        }

        private void ink_MouseMove(object sender, MouseEventArgs e)
        {
            Point pos = e.GetPosition(inkcanvas);
            if (ink_runtime_flag)
                ink_runtime(pos);
        }

        private StylusPoint temp = new StylusPoint(0,0);
            
        private void ink_runtime(Point pos)
        {
            StylusPoint stylusPoint = new StylusPoint(pos.X, pos.Y);
            StylusPointCollection stylusPoints = new StylusPointCollection();
            if (temp.X == 0 && temp.Y == 0) temp = new StylusPoint(pos.X, pos.Y);
            stylusPoints.Add(temp);
            stylusPoints.Add(stylusPoint);
            temp.X = pos.X;
            temp.Y = pos.Y;
            Stroke stroke = new Stroke(stylusPoints, inkDA);

            StrokeCollection sc = inkcanvas.Strokes;
            sc.Add(stroke);
            string inkData = (new StrokeCollectionConverter()).ConvertToString(sc);

            client.SendInk(roomId, inkData);
        }

        #endregion

        /*----------------------------------------------------- 分割线  ----------------------------------------------------------------*/
        #region 聊天室的回调函数实现

        public void ShowLogin(string loginUserName)
        {
            this.ConversationBox.Text += "[" + loginUserName + "]" + "进入房间" + '\n';
            scrollviewer.ScrollToBottom();
        }

        /// <summary>其他用户退出</summary>
        public void ShowLogout(string userName)
        {
            this.ConversationBox.Text += "[" + userName + "]" + "退出房间" + '\n';
            scrollviewer.ScrollToBottom();
        }

        public void ShowTalk(string userName, string message)
        {
            this.ConversationBox.Text += "[" + userName + "]说：" + message + '\n';
            scrollviewer.ScrollToBottom();
        }


        //准备状态图片
        public void ShowIsReady(int readyid,bool func)
        {
            Console.WriteLine(readyid);
            Ready[readyid].Visibility = func ? Visibility.Visible : Visibility.Collapsed;
        }



        //信息
        public void ShowInfo(Userdata[] mydata)
        {
            userdatas = mydata;
            //this.U1.Text += " 昵称：" + us.Name + '\n' + " 等级：" + us.Grade + '\n' + '\n';
            Userdata[] usarr = new Userdata[CC.maxUserNum];
            int cnt = 0;
            foreach (var item in mydata)
            {
                usarr[cnt++] = item;
            }

            //初始化
            this.photo1.Source = null;
            this.photo2.Source = null;
            this.photo3.Source = null;
            this.photo4.Source = null;
            this.photo5.Source = null;
            this.photo6.Source = null;
            this.photo7.Source = null;
            this.photo8.Source = null;
            this.U1.Text = "";
            this.U2.Text = "";
            this.U3.Text = "";
            this.U4.Text = "";
            this.U5.Text = "";
            this.U6.Text = "";
            this.U7.Text = "";
            this.U8.Text = "";


            //得到了已登录的所有用户的信息
            if (usarr[0].Avart == null)
                usarr[0].Avart = "1.png";
            this.photo1.Source = new BitmapImage(new Uri("pack://application:,,,/image/" + usarr[0].Avart));
            this.U1.Text += usarr[0].Name + "\n 0";

            if (cnt == 1)
            {
                user1Btn1.Visibility = Visibility.Visible;
                user1Btn2.Visibility = Visibility.Collapsed;
                user1Btn3.Visibility = Visibility.Collapsed;
                user1Btn4.Visibility = Visibility.Collapsed;
                user1Btn5.Visibility = Visibility.Collapsed;
                user1Btn6.Visibility = Visibility.Collapsed;
                user1Btn7.Visibility = Visibility.Collapsed;
                user1Btn8.Visibility = Visibility.Collapsed;
                return;
            }

            if (usarr[1].Avart == null)
                usarr[1].Avart = "1.png";
            this.photo2.Source = new BitmapImage(new Uri("pack://application:,,,/image/" + usarr[1].Avart));
            this.U2.Text += usarr[1].Name + "\n 0";

            if (cnt == 2)
            {
                user1Btn1.Visibility = Visibility.Visible;
                user1Btn2.Visibility = Visibility.Visible;
                user1Btn3.Visibility = Visibility.Collapsed;
                user1Btn4.Visibility = Visibility.Collapsed;
                user1Btn5.Visibility = Visibility.Collapsed;
                user1Btn6.Visibility = Visibility.Collapsed;
                user1Btn7.Visibility = Visibility.Collapsed;
                user1Btn8.Visibility = Visibility.Collapsed;
                return;
            }

            if (usarr[2].Avart == null)
                usarr[2].Avart = "1.png";
            this.photo3.Source = new BitmapImage(new Uri("pack://application:,,,/image/" + usarr[2].Avart));
            this.U3.Text += usarr[2].Name + "\n 0";

            if (cnt == 3)
            {
                user1Btn1.Visibility = Visibility.Visible;
                user1Btn2.Visibility = Visibility.Visible;
                user1Btn3.Visibility = Visibility.Visible;
                user1Btn4.Visibility = Visibility.Collapsed;
                user1Btn5.Visibility = Visibility.Collapsed;
                user1Btn6.Visibility = Visibility.Collapsed;
                user1Btn7.Visibility = Visibility.Collapsed;
                user1Btn8.Visibility = Visibility.Collapsed;
                return;
            }

            if (usarr[3].Avart == null)
                usarr[3].Avart = "1.png";
            this.photo4.Source = new BitmapImage(new Uri("pack://application:,,,/image/" + usarr[3].Avart));
            this.U4.Text += usarr[3].Name + "\n 0";

            if (cnt == 4)
            {
                user1Btn1.Visibility = Visibility.Visible;
                user1Btn2.Visibility = Visibility.Visible;
                user1Btn3.Visibility = Visibility.Visible;
                user1Btn4.Visibility = Visibility.Visible;
                user1Btn5.Visibility = Visibility.Collapsed;
                user1Btn6.Visibility = Visibility.Collapsed;
                user1Btn7.Visibility = Visibility.Collapsed;
                user1Btn8.Visibility = Visibility.Collapsed;
                return;
            }

            if (usarr[4].Avart == null)
                usarr[4].Avart = "1.png";
            this.photo5.Source = new BitmapImage(new Uri("pack://application:,,,/image/" + usarr[4].Avart));
            this.U5.Text += usarr[4].Name + "\n 0";

            if (cnt == 5)
            {
                user1Btn1.Visibility = Visibility.Visible;
                user1Btn2.Visibility = Visibility.Visible;
                user1Btn3.Visibility = Visibility.Visible;
                user1Btn4.Visibility = Visibility.Visible;
                user1Btn5.Visibility = Visibility.Visible;
                user1Btn6.Visibility = Visibility.Collapsed;
                user1Btn7.Visibility = Visibility.Collapsed;
                user1Btn8.Visibility = Visibility.Collapsed;
                return;
            }

            if (usarr[5].Avart == null)
                usarr[5].Avart = "1.png";
            this.photo6.Source = new BitmapImage(new Uri("pack://application:,,,/image/" + usarr[5].Avart));
            this.U6.Text += usarr[5].Name + "\n 0";

            if (cnt == 6)
            {
                user1Btn1.Visibility = Visibility.Visible;
                user1Btn2.Visibility = Visibility.Visible;
                user1Btn3.Visibility = Visibility.Visible;
                user1Btn4.Visibility = Visibility.Visible;
                user1Btn5.Visibility = Visibility.Visible;
                user1Btn6.Visibility = Visibility.Visible;
                user1Btn7.Visibility = Visibility.Collapsed;
                user1Btn8.Visibility = Visibility.Collapsed;
                return;
            }

            if (usarr[6].Avart == null)
                usarr[6].Avart = "1.png";
            this.photo7.Source = new BitmapImage(new Uri("pack://application:,,,/image/" + usarr[6].Avart));
            this.U7.Text += usarr[6].Name + "\n 0";

            if (cnt == 7)
            {
                user1Btn1.Visibility = Visibility.Visible;
                user1Btn2.Visibility = Visibility.Visible;
                user1Btn3.Visibility = Visibility.Visible;
                user1Btn4.Visibility = Visibility.Visible;
                user1Btn5.Visibility = Visibility.Visible;
                user1Btn6.Visibility = Visibility.Visible;
                user1Btn7.Visibility = Visibility.Visible;
                user1Btn8.Visibility = Visibility.Collapsed;
                return;
            }

            if (usarr[7].Avart == null)
                usarr[7].Avart = "1.png";
            this.photo8.Source = new BitmapImage(new Uri("pack://application:,,,/image/" + usarr[7].Avart));
            this.U8.Text += usarr[7].Name + "\n 0";

            user1Btn1.Visibility = Visibility.Visible;
            user1Btn2.Visibility = Visibility.Visible;
            user1Btn3.Visibility = Visibility.Visible;
            user1Btn4.Visibility = Visibility.Visible;
            user1Btn5.Visibility = Visibility.Visible;
            user1Btn6.Visibility = Visibility.Visible;
            user1Btn7.Visibility = Visibility.Visible;
            user1Btn8.Visibility = Visibility.Visible;


        }



        #endregion

        #region 游戏的回调函数实现
        public void EnterRoom(string userName,int rooomId)
        {
            client.EnterRoom(userName, roomId);
        }
        public void ShowRoom()
        {
            //这个地方是为了显示用户列表的，不能清空
            //UserBox.Items.Clear();
            //显示各个选手得分
            //string[] s = roommeg.Split(',');
            //for (int i = 0; i+1 < s.Length; i += 2)
            //{
            //   // UserBox.Items.Add(s[i] + "---" + s[i + 1] + "分");
            //    if (us.Name == s[i]) ScoreLabel.Content = s[i + 1];
            //}


            //初始画板都不可使用
            //ScoreLabel.Content = 0;
            //inkcanvas.IsEnabled = false;
            //clear.IsEnabled = false;
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //readybtn.IsEnabled = false;
            if (readybtn.Content.ToString() == "取消准备")
            {
                client.CancelReadyGame(us.Name, roomId);
                readybtn.Content = "准备";
                return;
            }
            
            readybtn.Content = "取消准备";
            client.StartGame(us.Name, roomId);
        }
        public void ShowStart(string userName1, string answer, string tip)
        {
            inkcanvas.Strokes.Clear();
            //画图者
            if(us.Name==userName1)
            {
                clear.IsEnabled = true;
                undo.IsEnabled = true;
                random.IsEnabled = true;
                inkcanvas.IsEnabled = true;
                sendbtn.IsEnabled = false;
                SendBox.IsEnabled = false;
                sendbtn.Opacity = 0.5;
                SendBox.Opacity = 0.5;
                TipLabel.Content = "题目：" + answer;
                ConversationBox.Text += "系统提示：请开始绘画\n";
                TipCheck = answer;
            }
            //猜图者
            else
            {
                clear.IsEnabled = false;
                undo.IsEnabled = false;
                random.IsEnabled = false;
                inkcanvas.IsEnabled = false;
                sendbtn.IsEnabled = true;
                SendBox.IsEnabled = true;
                sendbtn.Opacity = 1;
                SendBox.Opacity = 1;
                TipLabel.Content = "提示：" + tip;
                ConversationBox.Text += "系统提示：请开始抢答\n";
            }
            scrollviewer.ScrollToBottom();
            restTimeTextBox.Text = "120";
        }

        public void ShowWin(string userName, string userName0)
        {

        }

        public void ShowGrade(int userId,int score,string name)
        {
            scores[userId-1].Text = name + "\n" + score.ToString();
        }
        public void ShowNewTurn(string roommeg, string userName1, string answer, string tip)
        {
            //更新用户列表和积分
            //UserBox.Items.Clear();
            //string[] s = roommeg.Split(',');
            //for (int i = 0; i + 1 < s.Length; i += 2)
            //{
            //    UserBox.Items.Add(s[i] + "---" + s[i + 1] + "分");
            //    if (us.Name == s[i]) ScoreLabel.Content = s[i + 1];
            //}
            //重新开始
            ShowStart(userName1, answer, tip);
        }

        public void ShowTime(int restTime)
        {
            restTimeTextBox.Text = restTime.ToString();
        }


        public void EndGame(string[] userNames, int[] scores)
        {
            ink_stack.Clear();
            readybtn.IsEnabled = true;
            readybtn.Content = "准备";
            restTimeTextBox.Text = "";
            clear.IsEnabled = false;
            undo.IsEnabled = false;
            random.IsEnabled = false;
            inkcanvas.Strokes.Clear();
            inkcanvas.IsEnabled = false;
            sendbtn.IsEnabled = true;
            sendbtn.Opacity = 1;
            SendBox.IsEnabled = true;
            SendBox.Opacity = 1;
            TipLabel.Content = "";
            TipCheck = "";
            //TODO
            string rank = "积分榜：\n";
            for(int i=0;i<userNames.Length;++i)
            {
                rank += userNames[i] + " " + scores[i] + "\n";
            }
            ConversationBox.Text += rank;
        }

        public void stopCancelReady()
        {
            readybtn.IsEnabled = false;
            readybtn.Content = "游戏中";
        }

        public void ShowNewQues(string roommeg, string userName1, string answer, string tip)
        {
            if (us.Name == userName1)
            {
                TipLabel.Content = "题目：" + answer;
                TipCheck = answer;
                ConversationBox.Text += "系统提示：你已更换题目\n";
            }
            //猜图者
            else
            {
                TipLabel.Content = "提示：" + tip;
                ConversationBox.Text += string.Format("系统提示：{0}更换题目\n",userName1);
            }
            scrollviewer.ScrollToBottom();
        }


        public void refreshRoomInfo(int[] roomPlayerNum, bool[] isStart)
        {

        }

        #endregion

        private void changeQues(object sender, RoutedEventArgs e)
        {
            client.changeQuestion(roomId, us.Acount);
        }

        private void question_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("不会玩吗？您只需要画出或者猜出相应词语就可以了。祝您玩的愉快！","彩蛋");
        }
    }
}
