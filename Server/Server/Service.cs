using System;
using System.Collections.Generic;
using System.IO.Packaging;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Windows.Automation.Peers;
using System.Windows;
using System.IO;
using System.Windows.Threading;

namespace Server
{
    // 实现函数的时候，用三个/ 来定义一下函数说明，说明函数的大概功能、（参数，返回值，可选可不选）
    // 整体框架已建好，注意分割线，画板函数实现在#region 画板... 这里面，聊天室在# region 聊天室这里

    public class Service : IService
    {
        /// <summary>
        /// 测试用例
        /// </summary>
        public bool test()
        {
            return true;
        }


        /*-----------------------------------------------------  分割线   ---------------------------------------------------------------*/

        #region 画板的服务端函数实现
        /// <summary>
        /// 发送数字墨迹
        /// </summary>
        public void SendInk(int room, string ink)
        {
            foreach (var v in CC.Rooms[room].users)
            {
                try
                {
                    v.callback.ShowInk(ink);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }

            //foreach (var v in CC.Users)
            //{
            //    v.callback.ShowInk(ink);
            //}
        }

        //public void SendMem(int room, MemoryStream memoryStream)
        //{
        //    foreach (var v in CC.Rooms[room].users)
        //    {
        //        try
        //        {
        //            v.callback.ShowMem(memoryStream.GetBuffer());
        //        }
        //        catch (Exception ex)
        //        {
        //            Console.WriteLine(ex.Message);
        //        }
        //    }
        //}

        #endregion



        /*-----------------------------------------------------  分割线   ---------------------------------------------------------------*/



        #region 聊天室的服务端函数实现

        public Service()
        {
            if (CC.Users == null)
            {
                CC.Users = new List<MyUser>();
            }
        }

        public void Login(int id,string userName)
        {
            // throw new NotImplementedException();
            OperationContext context = OperationContext.Current;
            IServiceCallback callback = context.GetCallbackChannel<IServiceCallback>();
            MyUser newUser = new MyUser(userName, callback);

            User tmp = null;
            //数据库实例
            MyDbEntities myDbEntities = new MyDbEntities();
            //选中这一条数据
            var q = from p in myDbEntities.User
                    where p.Name == userName
                    select p;

            tmp = q.FirstOrDefault();
            newUser.Acount = tmp.Acount;
            newUser.Avart = tmp.Avart;
            newUser.Grade = tmp.Grade;
            newUser.Name = tmp.Name;
            newUser.Room = tmp.Room;
            newUser.Score = tmp.Score;
            newUser.Sign = tmp.Sign;
            newUser.id = id;
            newUser.isCorrect = false;

            CC.Users.Add(newUser);
            List<Userdata> userdatas = new List<Userdata>();
            foreach (var item in CC.Users)
            {
                if (item.id == id)
                {
                    Userdata t = new Userdata();
                    t.Acount = item.Acount;
                    t.Avart = item.Avart;
                    t.Grade = item.Grade;
                    t.Name = item.Name;
                    t.Room = item.Room;
                    t.Score = item.Score;
                    t.Sign = item.Sign;
                    userdatas.Add(t);
                }
            }
            foreach (var item in CC.Users)
            {
                if(item.id==id)
                {
                    try
                    {
                        item.callback.ShowLogin(userName);
                        item.callback.ShowInfo(userdatas);
                    }
                    catch
                    {

                    }
                }
            }
        }

        public void Talk(string userName, string message)
        {
            MyUser user = CC.GetUser(userName);
            string ans = CC.Rooms[user.inRoom].question.answer;

            if (user.isCorrect && message.Contains(ans))
            {
                user.callback.ShowTalk("系统消息", "警告不得在猜中后再发送与答案相关内容");
                return;
            }

            if (message == ans)
            {
                foreach (var item in CC.Rooms[user.inRoom].users)
                {
                    if (item.Name == userName)
                    {  
                        try
                        {
                            item.callback.ShowTalk("系统消息", "你猜中了"); 
                        }
                        catch
                        {

                        }
                        item.Score += CC.Rooms[user.inRoom].users.Count - CC.Rooms[user.inRoom].correctNum - 1;
                        //item.Score += 1;
                        CC.Rooms[user.inRoom].users[0].Score += 1;
                        foreach(var user1 in CC.Rooms[user.inRoom].users)
                        {
                            user1.callback.ShowGrade(item.locid, (int)item.Score, item.Name);
                            user1.callback.ShowGrade(CC.Rooms[user.inRoom].users[0].locid, (int)CC.Rooms[user.inRoom].users[0].Score, CC.Rooms[user.inRoom].users[0].Name);
                        }
                        item.isCorrect = true;
                        CC.Rooms[user.inRoom].correctNum += 1;
                    }
                    else
                    {
                        try
                        {
                            item.callback.ShowTalk("系统消息", string.Format("{0}猜中了", userName));
                        }
                        catch
                        {

                        }
                    }
                    //item.callback.ShowWin(userName, CC.Rooms[user.inRoom].users.First().Name);
                }
                if (CC.Rooms[user.inRoom].correctNum == CC.Rooms[user.inRoom].users.Count-1)
                {
                    RollUserAndRestart(user.inRoom);
                }
            }
            else
            {
                foreach (var item in CC.Rooms[user.inRoom].users)
                {
                    try
                    {
                        item.callback.ShowTalk(userName, message);
                    }
                    catch(Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                }
            }
            
        }

        /// <summary>用户退出</summary>
        public void Logout(int id,string userName)
        {
            MyUser logoutUser = CC.GetUser(userName);
            CC.Users.Remove(logoutUser);
            foreach(var user in CC.Rooms[id].users)
            {
                if (userName == user.Name)
                {
                    CC.Rooms[id].users.Remove(user);
                    break;
                }
            }
            List<Userdata> userdatas = new List<Userdata>();
            foreach (var item in CC.Users)
            {
                if (item.id == id)
                {
                    Userdata t = new Userdata();
                    t.Acount = item.Acount;
                    t.Avart = item.Avart;
                    t.Grade = item.Grade;
                    t.Name = item.Name;
                    t.Room = item.Room;
                    t.Score = item.Score;
                    t.Sign = item.Sign;
                    userdatas.Add(t);
                }
            }
            foreach (var item in CC.Users)
            {
                if (item.id == id)
                {
                    try
                    {
                        item.callback.ShowLogout(userName);
                        item.callback.ShowInfo(userdatas);
                    }
                    catch(Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                }
            }
            logoutUser = null; //将其设置为null后，WCF会自动关闭该客户端
            refreshRoomInfo();

            for(int i=0;i<CC.Rooms[id].users.Count;++i)
            {
                for(int j=0;j<CC.Rooms[id].users.Count;++j)
                {
                    CC.Rooms[id].users[j].callback.ShowIsReady(i, CC.Rooms[id].users[i].ready);
                }
            }
        }




        #endregion

        #region 游戏的服务端接口实现
        private void refreshRoomInfo()
        {
            List<int> roomPlayerNum = new List<int>();
            List<bool> isStart = new List<bool>();
            for(int i=1;i<=4;++i)
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
        private void timer_Tick(object sender, EventArgs e)
        {
            MyTimer timer = sender as MyTimer;

            //CC.Rooms[timer.roomId].timer.Stop();
            //RollUserAndRestart(timer.roomId);
            //return;
            timer.restTime -= 1;

            foreach (MyUser user in CC.Rooms[timer.roomId].users)
            {
                try
                {
                    user.callback.ShowTime(timer.restTime);
                }
                catch
                {

                }
            }

            if (timer.restTime == 0)
            {
                //TODO
                CC.Rooms[timer.roomId].timer.Enabled = false;
                RollUserAndRestart(timer.roomId);
                return;
            }
        }

        //进入房间
        public void EnterRoom(string userName, int roomId)
        {
            MyUser user = CC.GetUser(userName);
            user.inRoom = roomId;
            user.Score = 0;
            //初始化新房间
            if (CC.Rooms.ContainsKey(roomId) == false)
            {
                CC.Rooms.Add(roomId, new Room());
                CC.Rooms[roomId].isGameStart = false;
                CC.Rooms[roomId].users = new List<MyUser>();
                CC.Rooms[roomId].question = new questions();
                CC.Rooms[roomId].timer = new MyTimer();
                CC.Rooms[roomId].timer.Interval = 1000;
                CC.Rooms[roomId].timer.restTime = 60;
                //CC.Rooms[roomId].timer.Enabled = true;
                
                CC.Rooms[roomId].timer.Elapsed += timer_Tick;
                CC.Rooms[roomId].timer.roomId = roomId;
            }
            user.locid = CC.Rooms[roomId].users.Count+1;
            //该用户添加到房间
            CC.Rooms[roomId].users.Add(user);

            //给该房间的所有用户发送最新用户消息
            foreach (var item in CC.Rooms[roomId].users)
            {
                //string s = "";
                //foreach (var v in CC.Rooms[roomId].users)
                //{
                //    s += v.Name + "," + v.Score.ToString() + ",";
                //}
                try
                {
                    item.callback.ShowRoom();
                }
                catch
                {

                }
                
            }
            for(int i=0;i< CC.Rooms[roomId].users.Count; ++i)
            {
                user.callback.ShowIsReady(i, CC.Rooms[roomId].users[i].ready);
            }
            refreshRoomInfo();
        }
        public void StartGame(string userName, int roomId)
        {
            //当前用户已准备
            MyUser user = CC.GetUser(userName);
            user.ready = true;
            int pos = -1;
            for (int i = 0; i < CC.Rooms[roomId].users.Count; ++i)
            {
                if (CC.Rooms[roomId].users[i].Name == userName)
                {
                    pos = i;
                    break;
                }
            }
            foreach (var user1 in CC.Rooms[roomId].users)
            {
                try
                {
                    user1.callback.ShowIsReady(pos, true);
                }
                catch
                {

                }
                
            }
            CC.Rooms[roomId].correctNum = 0;

            if (CC.Rooms[roomId].users.Count < CC.Rooms[roomId].leastBeginNum) return;
            //判断当前房间内所有用户是否准备好
            foreach (var item in CC.Rooms[roomId].users)
            {
                if (!item.ready) return;
            }
            CC.Rooms[roomId].isGameStart = true;
            foreach (var item in CC.Rooms[roomId].users)
            {
                try
                {
                    item.callback.ShowStart(CC.Rooms[roomId].users.First().Name, CC.Rooms[roomId].question.answer, CC.Rooms[roomId].question.tip);
                    item.callback.stopCancelReady();
                }
                catch
                {

                }
                item.Score = 0;
            }
            
            CC.Rooms[roomId].timer.restTime = CC.Rooms[roomId].timer.gameTime;
            CC.Rooms[roomId].timer.Enabled = true;
            CC.Rooms[roomId].currentTurn = 1;
            refreshRoomInfo();


            

            for (int i = 0; i < CC.Rooms[roomId].users.Count; ++i)
            {
                for (int j = 0; j < CC.Rooms[roomId].users.Count; ++j)
                {
                    CC.Rooms[roomId].users[j].callback.ShowGrade(CC.Rooms[roomId].users[i].locid, 0, CC.Rooms[roomId].users[i].Name);
                }
            }
        }


        private void EndGame(int roomId)
        {
            CC.Rooms[roomId].isGameStart = false;
            CC.Rooms[roomId].timer.Enabled = false;
            refreshRoomInfo();
            List<int> scores = new List<int>();
            List<string> userNames = new List<string>();
            foreach(var user in CC.Rooms[roomId].users)
            {
                user.ready = false;
                user.isCorrect = false;
                scores.Add((int)user.Score);
                user.Score = 0;
                userNames.Add(user.Name);
            }
            
            foreach(var user in CC.Rooms[roomId].users)
            {
                try
                {
                    user.callback.EndGame(userNames, scores);
                }
                catch
                {

                }
            }

            for (int i = 0; i < CC.Rooms[roomId].users.Count; ++i)
            {
                for (int j = 0; j < CC.Rooms[roomId].users.Count; ++j)
                {
                    try 
                    {
                        CC.Rooms[roomId].users[j].callback.ShowIsReady(i, CC.Rooms[roomId].users[i].ready);
                    }
                    catch
                    {

                    }
                    
                    
                }
            }
        }

        private void RollUserAndRestart(int roomId)
        {
            if (CC.Rooms[roomId].currentTurn >= CC.Rooms[roomId].users.Count)
            {
                EndGame(roomId);
                return;
            }
            CC.Rooms[roomId].currentTurn += 1;
            MyUser newuser = CC.Rooms[roomId].users.First();
            CC.Rooms[roomId].users.RemoveAt(0);
            CC.Rooms[roomId].users.Add(newuser);
            CC.Rooms[roomId].question.update();
            CC.Rooms[roomId].correctNum = 0;
            string s = "";
            foreach (var v in CC.Rooms[roomId].users)
            {
                v.isCorrect = false;
                s += v.Name + "," + v.Score.ToString() + ",";
            }
            
            foreach (var item in CC.Rooms[roomId].users)
            {
                try
                {
                    item.callback.ShowNewTurn(s, CC.Rooms[roomId].users.First().Name, CC.Rooms[roomId].question.answer, CC.Rooms[roomId].question.tip);
                }
                catch
                {

                }
            }
            CC.Rooms[roomId].timer.restTime = CC.Rooms[roomId].timer.gameTime;
            CC.Rooms[roomId].timer.Enabled = true;
        }

        public void CancelReadyGame(string userName, int roomId)
        {
            MyUser user = CC.GetUser(userName);
            user.ready = false;
            int pos = -1;
            for(int i = 0; i < CC.Rooms[roomId].users.Count; ++i)
            {
                if (CC.Rooms[roomId].users[i].Name == userName)
                {
                    pos = i;
                    break;
                }
            }
            foreach(var user1 in CC.Rooms[roomId].users)
            {
                try
                {
                    user1.callback.ShowIsReady(pos, false);

                }
                catch { Console.WriteLine("有玩家退出无法返回信息！"); }
            }
        }

        public void changeQuestion(int roomid,string Account)
        {
            CC.Rooms[roomid].question.update();
            foreach (var item in CC.Rooms[roomid].users)
            {
                try
                {
                    item.callback.ShowNewQues("", CC.Rooms[roomid].users.First().Name, CC.Rooms[roomid].question.answer, CC.Rooms[roomid].question.tip);
                }
                catch
                {

                }
            }
        }
        #endregion

    }
}
