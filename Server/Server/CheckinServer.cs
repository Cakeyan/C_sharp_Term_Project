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

        // 刷新房间信息
        private void refreshRoomInfo()
        {
            List<int> roomPlayerNum = new List<int>();
            List<bool> isStart = new List<bool>();
            for (int i = 1; i <= 4; ++i)
            {
                if (CC.Rooms.ContainsKey(i))
                {
                    roomPlayerNum.Add(CC.Rooms[i].users.Count());
                    isStart.Add(CC.Rooms[i].isGameStart);
                }
                else
                {
                    roomPlayerNum.Add(0);
                    isStart.Add(false);
                }
            }
            foreach (var item in CheckinCC.Users)
            {
                try
                {
                    item.Checkincallback.refreshRoomInfo(roomPlayerNum, isStart);
                }
                catch
                {

                }
            }
        }

        // 登录进大厅
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
            refreshRoomInfo();
        }
        
        //检查是否能进入房间
        public string Checkin(string userName, int roomnumber)
        {
            if (CC.Rooms.ContainsKey(roomnumber)&&(CC.Rooms[roomnumber].users.Count >= CC.Rooms[roomnumber].maxUserNum)) return "人数已满";
            if (CC.Rooms.ContainsKey(roomnumber)&&(CC.Rooms[roomnumber].isGameStart)) return "游戏已经开始";
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
            return "OK";
        }

        // 在大厅向所有用户发送消息
        public void Talk(string userName, string message)
        {
            foreach (var item in CheckinCC.Users)
            {
                try
                {
                    item.Checkincallback.ShowTalk(userName, message);
                }
                catch
                {

                }
               
            }
        }

        // 退出大厅
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
            logoutUser = null; 

        }
    }
}
