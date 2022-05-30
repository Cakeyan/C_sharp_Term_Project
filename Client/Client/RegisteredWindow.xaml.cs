using Client.ServiceReference;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Client.LoginReference;

namespace Client
{
    /// <summary>
    /// RegisteredWindow.xaml 的交互逻辑
    /// </summary>
    public partial class RegisteredWindow : Window
    {
        //验证码
        private string Verification;
        private LoginServiceClient client;
        public RegisteredWindow()
        {
            InitializeComponent();
            Verification = GetImage();
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            client = new LoginServiceClient();
        }

        public string GetImage()
        {
            string code = "";
            Bitmap bitmap = VerifyCodeHelper.CreateVerifyCode(out code);
            ImageSource imageSource = ImageFormatConvertHelper.ChangeBitmapToImageSource(bitmap);
            img.Source = imageSource;
            //为了实现不区分大小写
            code = code.ToLower();
            return code;
        }

        private void Button_Click_SendCode(object sender, RoutedEventArgs e)
        {
            if (Code.Text.ToLower() != Verification)
            {
                MessageBox.Show("验证码输入错误！请重新输入", "提示", MessageBoxButton.OKCancel);
                Verification = GetImage();
                Code.Text = "";
                return;
            }
            try
            {
                sendCode.IsEnabled = false;
                bool flag = client.sendEmail(Account.Text);
                if (flag)
                {
                    MessageBox.Show("令牌发送成功！");
                }
                else
                {
                    MessageBox.Show("令牌发送失败，该账号可能已存在！");
                    sendCode.IsEnabled = true;
                }
                    
            }
            catch (Exception)
            {
                MessageBox.Show("与远程服务器连接失败！", "提示", MessageBoxButton.OKCancel, MessageBoxImage.Asterisk);
            }
            
        }

        //关闭窗口事件
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            LoginWindow LW = new LoginWindow();
            LW.Show();
        }

        private void Button_Click_Register(object sender, RoutedEventArgs e)
        {
            bool flag = client.Registered(Account.Text, PassWord.Password, "签名文本删掉", AccountName.Text, code.Text);
            if (flag)
                MessageBox.Show("注册成功！");
            else
                MessageBox.Show("注册失败，该账号可能已存在！");
        }

        private void Reg_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                Button_Click_Register(sender,e);
            }
        }

        private void img_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Verification = GetImage();
        }
    }
}
