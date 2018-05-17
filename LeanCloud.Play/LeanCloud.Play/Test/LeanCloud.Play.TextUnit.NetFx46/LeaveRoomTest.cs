using System;
using System.Collections;
using System.Diagnostics;
using System.Linq;
using LeanCloud;
using NUnit.Framework;

namespace TestUnit.NetFx46
{
    [TestFixture]
    public class LeaveRoomTest : TestBase
    {
        public LeaveRoomTest() : base()
        {
        }

        [Test]
        [Timeout(300000)]
        public void LeaveRoom()
        {
            Play.UserID = "WuJun";
            Play.Connect("0.0.1");

            Assert.That(Done, Is.True.After(2000000));
        }


        [PlayEvent]
        public override void OnAuthenticated()
        {
            var roomConfig = new PlayRoom.RoomConfig()
            {
                CustomRoomProperties = new Hashtable
                {
                    { "level", 1000 },
                    { "rankPoints", 3011 }
                },
                LobbyMatchKeys = new string[] { "rankPoints" }
            };

            Play.CreateRoom(roomConfig);
        }

        [PlayEvent]
        public override void OnCreatedRoom()
        {
            Play.Log("OnCreatedRoom");
            Play.LeaveRoom();
        }

        [PlayEvent]
        public override void OnPlayerLeftRoom(Player player)
        {
            Play.Log(player.ActorID, player.UserID);
        }
    }
}
