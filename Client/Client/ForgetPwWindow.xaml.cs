using Client.ServiceReference;
using System;
using System.Collections.Generic;
using System.Drawing;
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
using System.Windows.Shapes;
using Client.LoginReference;

namespace Client
{
    /// <summary>
    /// ForgetPasswordWindow.xaml 的交互逻辑
    /// </summary>
    public partial class ForgetPwWindow : Window
    {
        
        //验证码
        private string Verification;
        private LoginServiceClient client;
        public ForgetPwWindow()
        {
            client = new LoginServiceClient();
            InitializeComponent();
            Verification = GetImage();
        }

        //以字符串返回验证码
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

        //Closing事件
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            LoginWindow LW = new LoginWindow();
            LW.Show();
        }

        private void Forget_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                Button_Click_ResetPassword(sender, e);
            }
        }


        //点击按钮事件
        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }
        private void Button_Click_SendCode(object sender, RoutedEventArgs e)
        {

            //判断是否为机器，验证码的真伪
            if (Code.Text.ToLower() != Verification)
            {
                MessageBox.Show("验证码输入错误！请重新输入", "提示", MessageBoxButton.OKCancel, MessageBoxImage.Asterisk);
                Verification = GetImage();
                Code.Text = "";
                return;
            }

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

        private void Button_Click_ResetPassword(object sender, RoutedEventArgs e)
        {
            //开始向服务端申请，尝试修改密码
            try
            {
                bool flag = client.ForgetPassword(Account.Text, PassWord.Password,mailCode.Text);
                if (flag)
                    MessageBox.Show("修改成功！");
                else
                    MessageBox.Show("修改失败，请检查账号是否输入有误！");
            }
            catch (Exception)
            {
                MessageBox.Show("与远程服务器连接失败！", "提示", MessageBoxButton.OKCancel, MessageBoxImage.Asterisk);
            }
        }

        private void img_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Verification = GetImage();
        }
    }
}
