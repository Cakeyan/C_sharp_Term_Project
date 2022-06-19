using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using Client.LoginReference;

namespace Client
{
    //登录界面逻辑
    public partial class LoginWindow : Window
    {
        private User item;
        private LoginServiceClient client;
        private LoginReference.User us;

        public LoginWindow()
        {
            InitializeComponent();
            client = new LoginServiceClient();
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
        }
        
        //回车登录
        private void Login_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                Button_Click(sender, e);
            }
        }

        //点击按钮事件
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //找回
            if (e.Source == forgetPw)
            {
                this.Close();
                ForgetPwWindow FP = new ForgetPwWindow();
                FP.Show();
            }
            //注册
            else if (e.Source == sign_for)
            {
                this.Close();
                RegisteredWindow RW = new RegisteredWindow();
                RW.Show();
            }
            //登录
            else
            {
                try
                {
                    //登录判断
                    bool flag = client.Login(account.Text, passward.Password);
                    if (flag)
                    {
                        //获取登录用户信息
                        us = client.Userinfo(account.Text);
                        if (CC.Users == null)
                        {
                            CC.Users = new List<User>();
                        }
                        User newuser = new User(us.Acount);
                        CC.Users.Add(newuser);
                        item = CC.GetUser(us.Acount);
                        item.LoginWindow = this;
                        item.LoginWindow.Close();

                        //显示大厅界面
                        RoomWindow RW = new RoomWindow(us);
                        item.RoomWindow = RW;
                        item.RoomWindow.Show();
                    }
                    else
                    {
                        MessageBox.Show("登录失败！");
                    }
                }
                catch (Exception)
                {
                    MessageBox.Show("未连接到服务器！");
                }
            }
        }
    }
}
