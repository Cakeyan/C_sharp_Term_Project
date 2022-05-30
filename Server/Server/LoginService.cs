using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace Server
{
    // 注意: 使用“重构”菜单上的“重命名”命令，可以同时更改代码和配置文件中的类名“LoginService”。
    public class LoginService : ILoginService
    {
        #region 远程登录服务函数实现
        //远程登录
        public bool Login(string id, string pw)
        {
            //用户信息
            User us;
            //数据库实例
            MyDbEntities myDbEntities = new MyDbEntities();
            //选中这一条数据
            if (CC.LoginUsers == null)
            {
                CC.LoginUsers = new List<User>();
            }

            foreach (var user in CC.LoginUsers)
            {
                try
                {
                    if (user.Acount == id) return false;
                }
                catch
                {
                    return false;
                }

            }

            var q = from t in myDbEntities.User
                    where t.Acount == id
                    select t;
            
            if (q != null)
            {
                us = q.FirstOrDefault();
                if (us == null)
                    return false;
                if (us.Password == pw)
                {
                    CC.LoginUsers.Add(us);
                    return true;
                }
            }
            return false;
        }
        //远程注册
        public bool Registered(string id, string pw, string sn, string name,string code)
        {
            User us = new User();
            MyDbEntities myDbEntities = new MyDbEntities();
            us.Acount = id;
            us.Password = pw;
            us.Sign = sn;
            us.Name = name;

            var q = from t in myDbEntities.Table
                    orderby t.ableTime descending
                    where t.email == id
                    select t;
            if (q == null) return false;
            Table table = q.FirstOrDefault();
            if (table.ableTime < DateTime.Now) return false;
            if (table.code != code) return false;

            try
            {
                myDbEntities.User.Add(us);
                myDbEntities.Table.Remove(table);
                myDbEntities.SaveChanges();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
            return false;
        }

        private string getCode()
        {
            string letters = "ABCDEFGHIJKLMNPQRSTUVWXYZ0123456789";
            Random r = new Random();
            StringBuilder sb = new StringBuilder();
            for (int x = 0; x < 6; x++)
            {
                string letter = letters.Substring(r.Next(0, letters.Length - 1), 1);
                sb.Append(letter);
            }
            string code = sb.ToString();
            return code;
        }

        public bool sendEmail(string email)
        {
            string smtpService = "smtp.qq.com";
            string sendEmail = "1930675022@qq.com";
            string sendpwd = "vnyncxvfxfmpbjca";


            SmtpClient smtpclient = new SmtpClient();
            smtpclient.Host = smtpService;


            MailAddress sendAddress = new MailAddress(sendEmail);
            MailAddress receiveAddress = new MailAddress(email);

            MailMessage mailMessage = new MailMessage(sendAddress, receiveAddress);
            mailMessage.Subject = "Draw&Guess";
            mailMessage.SubjectEncoding = System.Text.Encoding.UTF8;

            string code = getCode();

            mailMessage.Body = string.Format("尊敬的用户：\n  您好！您的验证码为{0}。感谢您对我们游戏的大力支持", code);
            mailMessage.BodyEncoding = System.Text.Encoding.UTF8;

            smtpclient.DeliveryMethod = SmtpDeliveryMethod.Network;

            smtpclient.EnableSsl = true;
            try
            {

                smtpclient.UseDefaultCredentials = false;

                NetworkCredential networkCredential = new NetworkCredential(sendEmail, sendpwd);
                smtpclient.Credentials = networkCredential;

                smtpclient.Send(mailMessage);
                MyDbEntities myDbEntities = new MyDbEntities();
                Table table = new Table();
                table.email = email;
                table.code = code;
                table.ableTime = DateTime.Now.AddMinutes(2);
                myDbEntities.Table.Add(table);
                myDbEntities.SaveChanges();
            }
            catch (System.Net.Mail.SmtpException ex) 
            { 
                Console.WriteLine(ex.Message, "发送邮件出错");
                return false;
            }

            return true;
        }
    

        //修改密码
        public bool ForgetPassword(string id, string pw, string code)
        {
            //用户信息
            User us;
            //数据库实例
            MyDbEntities myDbEntities = new MyDbEntities();

            var tmp = from t in myDbEntities.Table
                    orderby t.ableTime descending
                    where t.email == id
                    select t;
            if (tmp == null) return false;
            Table table = tmp.FirstOrDefault();
            if (table.ableTime < DateTime.Now) return false;
            if (table.code != code) return false;

            
            //选中这一条数据
            var q = from t in myDbEntities.User
                    where t.Acount == id
                    select t;
            if (q != null)
            {
                us = q.FirstOrDefault();
                if (us == null)
                    return false;
                us.Password = pw;
                try
                {
                    myDbEntities.SaveChanges();
                }
                catch (Exception)
                {
                    return false;
                }
                return true;
            }
            return false;
        }

        public User Userinfo(string id)
        {
            User us=null;
            //数据库实例
            MyDbEntities myDbEntities = new MyDbEntities();
            //选中这一条数据
            var q = from t in myDbEntities.User
                    where t.Acount == id
                    select t;
            us = q.FirstOrDefault();
            return us;
        }

        public int GetUserNum()
        {
            if (CC.Users != null) return CC.Users.Count;
            else return 0;
        }
        #endregion
    }
}
