using GuessServer.GuessDao;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace SecretGardenServer
{
    public class SecretGardenService : ISecretGardenService
    {
        private static int roomNumber = 1000;
        private static Dictionary<int, Room> rooms = new Dictionary<int, Room>();
        private static Dictionary<string, User> users = new Dictionary<string, User>();

        /// <summary>
        /// 注册
        /// </summary>
        public void Regist(string user, string password)
        {
            OperationContext context = OperationContext.Current;
            ISecretGardenCallback callback = context.GetCallbackChannel<ISecretGardenCallback>();
            User u = new User(user, callback);

            // todo 添加数据库逻辑
            u.callback.CallbackShowRegistStatus(0, u);
        }

        /// <summary>
        ///  登录
        ///  状态值含义如下：
        ///         0: normal
        ///     other: error
        /// </summary>
        public void Login(string name, string password)
        {
            OperationContext context = OperationContext.Current;
            ISecretGardenCallback callback = context.GetCallbackChannel<ISecretGardenCallback>();
            User user = new User(name, callback);

            // 验证用户名密码
            DBPlayer p = (new UserDao()).getPlayer(name);
            if (p.playerPwd != password)
            {
                user.callback.CallbackShowLoginStatus(-1, null);
                return;
            }

            // 不允许重复登录
            if (users.ContainsKey(name))
            {
                user.callback.CallbackShowLoginStatus(-1, null);
                return;
            }

            // 添加进用户列表
            users.Add(name, user);
            user.icon = p.playerAva;
            // Callback显示信息
            user.callback.CallbackShowLoginStatus(0, user);
            // 登陆成功，自动进大厅
            EnterHall(name);
        }

        /// <summary>
        /// 登出
        /// </summary>
        public void Logout(string user)
        {
            Console.WriteLine(string.Format("[{0}]Logout:{1}", DateTime.Now, user));
            try
            {
                // 获得用户
                User u = users[user];
                // 删除用户
                if (u.inRoom != 0)
                {
                    EnterHall(u.name);
                }
                users.Remove(user);
            }
            catch { }
            DeliverHallToAll();
        }

        /// <summary>
        /// 进入大厅
        /// </summary>
        public void EnterHall(string user)
        {
            try
            {
                // Get the user
                User u = users[user];
                // 如果现在在房间里面，那么退出房间。
                // 可以在这里加入“中途逃跑”的判断代码
                if (u.inRoom != 0)
                {
                    int temp = -1;
                    for (int i = 0; i < rooms[u.inRoom].users.Count(); i++)
                    {
                        if (rooms[u.inRoom].users[i].name == u.name)
                        {
                            temp = i;
                            break;
                        }
                    }
                    if (temp != (-1))
                    {
                        rooms[u.inRoom].users.RemoveAt(temp);
                    }
                    // 如果房间空了，那么清除这个房间
                    if (rooms[u.inRoom].users.Count() == 0)
                    {
                        rooms.Remove(u.inRoom);
                    }
                }
                u.inRoom = 0;
                DeliverHallToAll();
            }
            catch
            {
                Logout(user);
            }
        }

        /// <summary>
        /// 新开房间并进入
        /// </summary>
        public void RegistRoom(string user, string name)
        {           
            Room room = new Room();
            roomNumber++;
            room.id = roomNumber;
            room.name = name;
            room.users = new List<User>();
            rooms.Add(room.id, room);
            EnterRoom(user, room.id);
        }

        /// <summary>
        /// 进入房间并直接开始游戏
        /// </summary>
        public void EnterRoom(string user, int roomNumber)
        {
            try
            {
                // Get the user
                User u = users[user];
                // Enter the room
                u.inRoom = roomNumber;
                rooms[roomNumber].users.Add(u);
                //Callback and show it
                u.callback.CallbackEnterRoom(rooms[roomNumber]);
                u.callback.CallbackShowRoom(rooms[roomNumber]);
                DeliverHallToAll();
            }
            catch
            {
                Logout(user);
            }
        }


        /// <summary>
        /// 发消息，并判断答案
        /// </summary>
        public void SendMessage(int room, string user, string message)
        {
            try
            {
                DeliverMessageToRoom(room, user, message);
            }
            catch
            {
                Logout(user);
            }
        }


        /// <summary>
        /// 发送墨迹
        /// </summary>
        public void SendInk(int room, string ink)
        {
            foreach (var u in rooms[room].users)
            {
                try
                {
                    u.callback.CallbackShowInk(ink);
                }
                catch
                {
                    Logout(u.name);
                }
            }
        }

        /// <summary>
        /// 分发：给所有人分发所有房间信息
        /// </summary>
        private void DeliverHallToAll()
        {
            foreach (var u in users.Values)
            {
                try
                {
                    u.callback.CallbackShowHall(rooms);
                }
                catch
                {
                    Logout(u.name);
                }
            }
        }

        /// <summary>
        /// 分发：给房间的人分发消息
        /// </summary>
        private void DeliverMessageToRoom(int room, string user, string message)
        {
            foreach (var u in rooms[room].users)
            {
                try
                {
                    u.callback.CallbackShowMessage(user, message);
                }
                catch
                {
                    Logout(u.name);
                }
            }
        }

    }
}