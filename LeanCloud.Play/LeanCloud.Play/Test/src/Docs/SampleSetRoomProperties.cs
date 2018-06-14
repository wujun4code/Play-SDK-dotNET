using LeanCloud;
using System.Collections;

namespace TestUnit.NetFx46.Docs
{

    public class SampleSetRoomProperties : PlayMonoBehaviour
    {
        /// <summary>
        /// 并且一定要定无参数的构造函数，而且一定要调用父类的构造函数
        /// </summary>
        public SampleSetRoomProperties() : base()
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

            Play.JoinRoom("RichMen");

            Play.JoinRandomRoom();

            var randomLobbyMatchKeys = new Hashtable
            {
                { "rankPoints", 3001 }
            };

            Play.JoinRandomRoom(randomLobbyMatchKeys);

        }

        [PlayEvent]
        public override void OnJoinedRoom()
        {
            Play.Log("OnJoinedRoom");

            var name = Play.Room.Name;

            
        }

        [PlayEvent]
        public override void OnRoomCustomPropertiesUpdated(Hashtable updatedProperties)
        {
            Play.Log("OnRoomCustomPropertiesUpdated");
            Play.Log(updatedProperties.ToLog());
        }
    }
}
