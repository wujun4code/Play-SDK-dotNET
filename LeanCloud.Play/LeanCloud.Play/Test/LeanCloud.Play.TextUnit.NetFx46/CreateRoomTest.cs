using System;
using System.Collections;
using System.Diagnostics;
using System.Linq;
using LeanCloud;
using NUnit.Framework;

namespace TestUnit.NetFx46
{
    [TestFixture]
    public class CreateRoomTest : TestBase
    {
        public CreateRoomTest() : base()
        {
        }

        [Test]
        [Timeout(300000)]
        public void CreateRandomRoom()
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
                MaxPlayerCount = 4
            };

            Play.CreateRoom(roomConfig, RandomRoomName);
        }

        [PlayEvent]
        public override void OnCreatingRoom()
        {
            Trace.WriteLine("OnCreatingRoom");
        }

        [PlayEvent]
        public override void OnCreatedRoom()
        {
            var roomName = Play.Room.Name;

            var initData = new Hashtable();
            initData.Add("ready", false);
            initData.Add("gold", 200);
            Play.Player.CustomProperties = initData;
        }

        [PlayEvent]
        public override void OnJoinedRoom()
        {
            //Play.RPC("OnNewPlayerJoined", PlayRPCTargets.All, Play.UserID);
            //Play.RPC("SayHello", PlayRPCTargets.All, Play.UserID, "wakakaka");
            Play.Log("OnJoinedRoom");

            Play.Log(Play.Room.Players.Count());
        }
    }
}
