﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Windows.Threading;

namespace Server
{
    // 服务端等整体框架已建好，服务接口和回调接口框架已建好，数据协定等用到时自建
    // 操作协定
    [ServiceContract(Namespace = "MyService", CallbackContract = typeof(IServiceCallback))]

    //服务接口
    public interface IService
    {

        [OperationContract]
        bool test();


        #region 画板的服务接口
        //发送数字墨迹
        [OperationContract(IsOneWay = true)]
        void SendInk(int room, string ink);

        //[OperationContract(IsOneWay = true)]
        //void SendMem(int room, MemoryStream memoryStream);
        #endregion

        #region 聊天室的服务接口
        [OperationContract(IsOneWay = true)]
        void Login(int id,string userName);

        [OperationContract(IsOneWay = true)]
        void Logout(int id,string userName);

        [OperationContract(IsOneWay = true)]
        void Talk(string userName, string message);

        //[OperationContract(IsOneWay = true)]
        //void Info(string account);

        #endregion

        #region 游戏的服务接口
        //进入房间
        [OperationContract(IsOneWay = true)]
        void EnterRoom(string userName, int roomId);

        //开始游戏
        [OperationContract(IsOneWay = true)]
        void StartGame(string userName, int roomId);

        [OperationContract(IsOneWay = true)]
        void CancelReadyGame(string userName, int roomId);

        [OperationContract(IsOneWay = true)]
        void changeQuestion(int roomid, string Account);
        #endregion

    }

    //回调接口
    public interface IServiceCallback
    {
        #region 画板的回调接口
        //回调显示墨迹 
        [OperationContract(IsOneWay = true)]
        void ShowInk(string ink);

        //[OperationContract(IsOneWay = true)]
        //void ShowMem(byte[] bytesStroke);
        #endregion

        #region 聊天室的回调接口
        [OperationContract(IsOneWay = true)]
        void ShowLogin(string loginUserName);

        [OperationContract(IsOneWay = true)]
        void ShowLogout(string userName);

        [OperationContract(IsOneWay = true)]
        void ShowTalk(string userName, string message);


        //为了刷新用户列表
        [OperationContract(IsOneWay = true)]
        void ShowInfo(List<Userdata> userdatas);

        [OperationContract(IsOneWay = true)]
        void ShowTime(int restTime);
        #endregion

        #region 游戏的回调接口
        //回调进入房间
        [OperationContract(IsOneWay = true)]
        void ShowRoom();

        //回调开始游戏
        [OperationContract(IsOneWay = true)]
        void ShowStart(string userName1,string answer,string tip);
        //回调胜利
        [OperationContract(IsOneWay = true)]
        void ShowWin(string userName,string userName0);
        //回调开始新游戏
        [OperationContract(IsOneWay = true)]
        void ShowNewTurn(string roommeg, string userName1, string answer, string tip);

        [OperationContract(IsOneWay = true)]
        void EndGame(List<string> userNames, List<int> scores);

        [OperationContract(IsOneWay = true)]
        void stopCancelReady();

        [OperationContract(IsOneWay = true)]
        void ShowNewQues(string roommeg, string userName1, string answer, string tip);
        #endregion
    }
    [DataContract]
    public class questions
    {
        [DataMember]
        public string answer { get; set; }
        [DataMember]
        public string tip { get; set; }
        private Random r=new Random();
        private int randNum { get; set; }
        public questions()
        {
            MyDbEntities myDbEntities = new MyDbEntities();
            var ques = from t in myDbEntities.Questions select t;
            randNum = r.Next(1, ques.Count() - 8);
            var q = from t in myDbEntities.Questions
                    where t.Id == randNum
                    select t;
            if (q.Count() > 0)
            {
                var Q = q.First();
                answer = Q.Question;
                tip = Q.Tip;
            }
        }
        public void update()
        {
            randNum += 1;
            MyDbEntities myDbEntities = new MyDbEntities();
            var q = from t in myDbEntities.Questions
                    where t.Id == randNum
                    select t;
            if (q.Count() > 0)
            {
                var Q = q.First();
                answer = Q.Question;
                tip = Q.Tip;
            }
        }
    }

    [DataContract]
    public class Room
    {
        [DataMember]
        public int id { get; set; }
        [DataMember]
        public List<MyUser> users { get; set; }

        [DataMember]
        public int maxUserNum = 8;

        [DataMember]
        public questions question { get; set; }
        
        [DataMember]
        public int correctNum { get; set; }

        [DataMember]
        public MyTimer timer { get; set; }
        [DataMember]
        public int currentTurn { get; set; }
        [DataMember]
        public bool isGameStart { get; set; }
        [DataMember]
        public int leastBeginNum = 2;
    }

    [DataContract]
    public class Userdata
    {
        [DataMember]
        public string Acount { get; set; }
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public string Avart { get; set; }
        [DataMember]
        public int Grade { get; set; }
        [DataMember]
        public string Sign { get; set; }
        [DataMember]
        public Nullable<int> Score { get; set; }
        [DataMember]
        public Nullable<int> Room { get; set; }
    }
}
