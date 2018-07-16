using System;
using System.Collections;
using System.Diagnostics;
using LeanCloud;
using NUnit.Framework;

namespace TestUnit.NetFx46
{
    [TestFixture]
    public class CreateRoomWithCustomPropertiesTest : TestBase
    {
        /// <summary>
        /// 测试当前用户随机加入房间之后，其他客户端的房主修改了当前用户的自定义属性，然后当前用户是否触发 OnCustomPropertiesUpdated
        /// </summary>
        [Test]
        [Timeout(300000)]
        public void CreateRoomWithCustomProperties()
        {
            Play.UserID = RandomClientId;
            Play.SetRouteServer("http://localhost:5000/play/");
            Play.Connect("0.0.1");
            Assert.That(true, Is.True.After(2000000));
        }

        [PlayEvent]
        public override void OnAuthenticated()
        {
            var roomConfig = PlayRoom.RoomConfig.Default;

            roomConfig.CustomRoomProperties = new Hashtable()
            {
                    { "level", 1000 },
                    { "rankPoints", 3011 }
            };
            roomConfig.LobbyMatchKeys = new string[] { "rankPoints" };

            Play.CreateRoom(roomConfig);
        }

        [PlayEvent]
        public override void OnJoinedRoom()
        {
            var toUpdate = new Hashtable()
            {
                { "level", 1200 }
            };

            var when = new Hashtable()
            {
                { "level", 1000 }
            };
            Play.Room.SetCustomProperties(toUpdate, when);
        }

        [PlayEvent]
        public override void OnNewPlayerJoinedRoom(Player player)
        {
            var initData = new Hashtable
            {
                { "ready", false },
                { "gold", 200 }
            };
            player.CustomProperties = initData;
        }

        [PlayEvent]
        public override void OnRoomCustomPropertiesUpdated(Hashtable updatedProperties)
        {
            var level = updatedProperties["level"];
            Console.WriteLine("level", level.ToString());
        }
    }
}
