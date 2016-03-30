using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Timers;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;

namespace SecretGardenServer
{
    // 服务器可以做的事情
    [ServiceContract(
        Namespace = "SecretGardenServer",
        SessionMode = SessionMode.Required,
        CallbackContract = typeof(ISecretGardenCallback))]
    public interface ISecretGardenService
    {
        //注册
        [OperationContract(IsOneWay = true)]
        void Regist(string user, string password);

        //登录
        [OperationContract(IsOneWay = true)]
        void Login(string user, string password);

        //登出
        [OperationContract(IsOneWay = true)]
        void Logout(string user);

        // 进入大厅
        [OperationContract(IsOneWay = true)]
        void EnterHall(string user);

        // 申请房间
        [OperationContract(IsOneWay = true)]
        void RegistRoom(string user, string name);

        // 进入房间
        [OperationContract(IsOneWay = true)]
        void EnterRoom(string user, int roomNumber);

        //发送消息
        [OperationContract(IsOneWay = true)]
        void SendMessage(int room, string user, string message);

        //发送墨迹
        [OperationContract(IsOneWay = true)]
        void SendInk(int room, string ink);
    }

    // 客户端必须完成的事情
    [ServiceContract]
    public interface ISecretGardenCallback
    {
        // 回调注册状态
        [OperationContract(IsOneWay = true)]
        void CallbackShowRegistStatus(int loginCode, User user);

        // 回调登录状态
        [OperationContract(IsOneWay = true)]
        void CallbackShowLoginStatus(int loginCode, User user);


        // 回调进入大厅
        [OperationContract(IsOneWay = true)]
        void CallbackShowHall(Dictionary<int, Room> rooms);

        // 回调进入房间
        [OperationContract(IsOneWay = true)]
        void CallbackEnterRoom(Room room);

        // 回调显示房间信息
        [OperationContract(IsOneWay = true)]
        void CallbackShowRoom(Room room);

        //回调显示信息
        [OperationContract(IsOneWay = true)]
        void CallbackShowMessage(string who, string message);

        //回调显示墨迹 
        [OperationContract(IsOneWay = true)]
        void CallbackShowInk(string ink);

        //回调开始新游戏
        [OperationContract(IsOneWay = true)]
        void CallbackStartNewTurn(Room room);
    }

    [DataContract]
    public class User
    {
        [DataMember]
        public string name { get; set; }
        [DataMember]
        public int inRoom { get; set; }
        [DataMember]
        public string icon;
        [DataMember]
        public readonly ISecretGardenCallback callback;

        public User(string name, ISecretGardenCallback callback)
        {
            this.name = name;
            this.callback = callback;
        }
    }

    [DataContract]
    public class Room
    {
        [DataMember]
        public int id;
        [DataMember]
        public string name;
        [DataMember]
        public List<User> users;
        public Room()
        {
            users = new List<User>();
        }

    }
}