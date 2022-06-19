using System;
using System.Drawing;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using Client.LoginReference;

namespace Client
{
    //找回密码逻辑
    public partial class ForgetPwWindow : Window
    {
        private string Verification;
        private LoginServiceClient client;

        public ForgetPwWindow()
        {
            client = new LoginServiceClient();
            InitializeComponent();
            Verification = GetImage();
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

        //关闭找回窗口
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            LoginWindow LW = new LoginWindow();
            LW.Show();
        }

        //回车提交
        private void Forget_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                Button_Click_ResetPassword(sender, e);
            }
        }

        //发送令牌
        private void Button_Click_SendCode(object sender, RoutedEventArgs e)
        {

            //判断验证码
            if (Code.Text.ToLower() != Verification)
            {
                MessageBox.Show("验证码输入错误！请重新输入", "提示", MessageBoxButton.OKCancel, MessageBoxImage.Asterisk);
                Verification = GetImage();
                Code.Text = "";
                return;
            }
            //发送令牌邮件
            try
            {
                bool flag = client.sendEmail(Account.Text);
                if (flag)
                    MessageBox.Show("令牌发送成功！");
                else
                    MessageBox.Show("令牌发送失败，请检查账号是否输入有误！");
            }
            catch (Exception)
            {
                MessageBox.Show("与远程服务器连接失败！", "提示", MessageBoxButton.OKCancel, MessageBoxImage.Asterisk);
            }
        }

        //修改密码
        private void Button_Click_ResetPassword(object sender, RoutedEventArgs e)
        {
            try
            {
                string flag = client.ForgetPassword(Account.Text, PassWord.Password,mailCode.Text);
                if (flag == "OK")
                {
                    if (MessageBox.Show("重置密码成功", "提示", MessageBoxButton.OK) == MessageBoxResult.OK)
                    {
                        Close();
                    }
                }
                else
                    MessageBox.Show(flag);
            }
            catch (Exception)
            {
                MessageBox.Show("与远程服务器连接失败！", "提示", MessageBoxButton.OKCancel, MessageBoxImage.Asterisk);
            }
        }

        //点击返回按钮
        private void Button_Click_Back(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        //点击图片切换验证码
        private void img_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Verification = GetImage();
        }

    }
}
