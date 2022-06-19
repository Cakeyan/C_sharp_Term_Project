using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;
using System.Timers;

namespace Server
{
    // 用来存储当前在线的用户类
    public class CC
    {
        public static List<User> LoginUsers { get; set; }
        public static List<MyUser> Users { get; set; }

        public static Dictionary<int, Room> Rooms = new Dictionary<int, Room>();
        public static MyUser GetUser(string username)
        {
            MyUser user = null;
            foreach (var item in Users)
            {
                if (item.Name == username)
                {
                    user = item;
                    break;
                }
            }
            return user;
        }
    }

    // 游戏计时的时钟类
    public class MyTimer: Timer
    {
        public int restTime { get; set; }
        public int roomId { get; set; }

        public int gameTime = 120;
    }
}
