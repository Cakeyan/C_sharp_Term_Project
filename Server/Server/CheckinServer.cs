using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace Server
{
    // 注意: 使用“重构”菜单上的“重命名”命令，可以同时更改代码和配置文件中的类名“CheckinServer”。
    public class CheckinServer : ICheckinServer
    {    
        public CheckinServer()
        {
            if (CheckinCC.Users == null)
            {
                CheckinCC.Users = new List<CheckinUser>();
            }
        }

        public void Login(string userName)
        {
            OperationContext context = OperationContext.Current;
            ICheckinServerCallback callback = context.GetCallbackChannel<ICheckinServerCallback>();
            CheckinUser newUser = new CheckinUser(userName, callback);
            CheckinCC.Users.Add(newUser);
            foreach (var user in CheckinCC.Users)
            {
                try
                {
                    user.Checkincallback.ShowLogin(userName);
                }
                catch(Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }

        public bool Checkin(string userName, int roomnumber)
        {
            if (CC.Rooms.ContainsKey(roomnumber)&&(CC.Rooms[roomnumber].users.Count >= CC.Rooms[roomnumber].maxUserNum || CC.Rooms[roomnumber].isGameStart)) return false;
            foreach (var item in CheckinCC.Users)
            {
                try
                {
                    if (userName != item.Name) item.Checkincallback.ShowCheckin(userName, roomnumber);
                }
                catch
                {

                }
            }
            return true;
        }

        public void Talk(string userName, string message)
        {
            foreach (var item in CheckinCC.Users)
            {
                item.Checkincallback.ShowTalk(userName, message);
            }
        }

        public void Logout(string userName)
        {
            CheckinUser logoutUser = CheckinCC.GetUser(userName);
            
            
            foreach(var user in CC.LoginUsers)
            {
                if (user.Name == userName)
                {
                    CC.LoginUsers.Remove(user);
                    break;
                }
            }
            CheckinCC.Users.Remove(logoutUser);
            foreach (var user in CheckinCC.Users)
            {
                try
                {
                    user.Checkincallback.ShowLogout(userName);
                }
                catch
                {

                }
            }
            logoutUser = null; //将其设置为null后，WCF会自动关闭该客户端

        }
    }
}
