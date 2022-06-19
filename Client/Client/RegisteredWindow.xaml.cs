using System;
using System.Drawing;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using Client.LoginReference;

namespace Client
{
    //注册界面逻辑
    public partial class RegisteredWindow : Window
    {
        private string Verification;
        private LoginServiceClient client;

        public RegisteredWindow()
        {
            InitializeComponent();
            Verification = GetImage();
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            client = new LoginServiceClient();
        }

        //生成验证码图片
        public string GetImage()
        {
            string code = "";
            Bitmap bitmap = VerifyCodeHelper.CreateVerifyCode(out code);
            ImageSource imageSource = ImageFormatConvertHelper.ChangeBitmapToImageSource(bitmap);
            img.Source = imageSource;
            code = code.ToLower();
            return code;
        }

        //发送令牌
        private void Button_Click_SendCode(object sender, RoutedEventArgs e)
        {
            //判断验证码
            if (Code.Text.ToLower() != Verification)
            {
                MessageBox.Show("验证码输入错误！请重新输入", "提示", MessageBoxButton.OKCancel);
                Verification = GetImage();
                Code.Text = "";
                return;
            }
            //发送令牌邮件
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

        //关闭注册窗口
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            LoginWindow LW = new LoginWindow();
            LW.Show();
        }

        //点击注册按钮
        private void Button_Click_Register(object sender, RoutedEventArgs e)
        {
            if(AccountName.Text != null && code.Text != null && Account.Text != null  && PassWord.Password != null)
            {
                string flag = client.Registered(Account.Text, PassWord.Password, "Sign", AccountName.Text, code.Text);
                if (flag == "OK")
                {
                    if (MessageBox.Show("注册成功", "提示", MessageBoxButton.OK) == MessageBoxResult.OK)
                    {
                        Close();
                    }
                }
                else
                    MessageBox.Show(flag);
            }

        }

        //点击返回按钮
        private void Button_Click_Back(object sender, RoutedEventArgs e)
        {
            Close();
        }

        //回车注册
        private void Reg_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                Button_Click_Register(sender,e);
            }
        }

        //点击图片切换验证码
        private void img_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Verification = GetImage();
        }
    }
}
