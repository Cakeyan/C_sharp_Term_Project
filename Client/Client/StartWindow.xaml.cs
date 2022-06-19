using System.Windows;

namespace Client
{
    //启动界面逻辑
    public partial class StartWindow : Window
    {
        public StartWindow()
        {
            InitializeComponent();
        }

        //多开测试
        private void btn1_Click(object sender, RoutedEventArgs e)
        {
            StartNewWindow(0, 400);
            StartNewWindow(0, 0);
            StartNewWindow(800, 0);
            StartNewWindow(800, 400);
        }

        //正常启动
        private void btn2_Click(object sender, RoutedEventArgs e)
        {
            StartNewWindow(400, 400);
        }
        
        //启动客户端
        private void StartNewWindow(int left, int top)
        {
            LoginWindow w = new LoginWindow();
            w.Left = left;
            w.Top = top;
            //w.Owner = this;
            w.Closed += (sender, e) => this.Activate();
            w.Show();
        }

        //关闭程序
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            App.Current.Shutdown();
        }
    }
}
