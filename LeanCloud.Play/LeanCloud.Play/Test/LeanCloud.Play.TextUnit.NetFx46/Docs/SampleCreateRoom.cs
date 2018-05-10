using LeanCloud;
using System.Collections;

namespace TestUnit.NetFx46.Docs
{

    public class SampleCreateRoom : PlayMonoBehaviour
    {
        /// <summary>
        /// 并且一定要定无参数的构造函数，而且一定要调用父类的构造函数
        /// </summary>
        public SampleCreateRoom() : base()
        {

        }

        void start()
        {
            // 打开调试日志
            Play.ToggleLog(true);
            // 设置 UserID
            Play.UserID = "hjiang";
            // 声明游戏版本，不同游戏版本的玩家会被分配在不同的隔离区域中
            Play.Connect("0.0.1");
        }


        [PlayEvent]
        public override void OnAuthenticated()
        {
            Play.Log("OnAuthenticated");
            // 设置房间全局唯一的名称为 「test-game-001」 
            Play.CreateRoom("test-game-001");

            Play.CreateRoom();

            Play.CreateRoom(new string[] { "bill", "steve" });

            var roomConfig = new PlayRoom.PlayRoomConfig()
            {
                IsVisible = false,
                IsOpen = false
            };

            Play.CreateRoom(roomConfig);

        }

        /// <summary>
        /// 如果成功创建，则会回调到 OnCreatedRoom
        /// </summary>
        [PlayEvent]
        public override void OnCreatedRoom()
        {
            // 打印房间名称
            Play.Log("OnCreatedRoom", Play.Room.Name);
        }


        /// <summary>
        /// 如果创建失败，则会回调 OnCreateRoomFailed
        /// </summary>
        /// <param name="errorCode"></param>
        /// <param name="reason"></param>
        [PlayEvent]
        public override void OnCreateRoomFailed(int errorCode, string reason)
        {
            Play.LogError(errorCode, reason);
        }
    }
}
