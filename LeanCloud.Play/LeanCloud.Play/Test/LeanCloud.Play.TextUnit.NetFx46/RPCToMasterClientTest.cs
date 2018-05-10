using System;
using System.Collections;
using System.Diagnostics;
using System.Linq;
using LeanCloud;
using NUnit.Framework;

namespace TestUnit.NetFx46
{
    [TestFixture]
    public class RPCToMasterClientTest : TestBase
    {
        public RPCToMasterClientTest() : base()
        {
        }

        [Test]
        [Timeout(300000)]
        public void RPCToMasterClient()
        {
            Play.UserID = "WuJun";
            Play.Connect("0.0.1");

            Assert.That(Done, Is.True.After(2000000));
        }

        [PlayEvent]
        public override void OnAuthenticated()
        {
            var roomConfig = new PlayRoom.PlayRoomConfig()
            {
                MaxPlayerCount = 4
            };

            Play.CreateRoom(roomConfig, RandomRoomName);
        }

        [PlayEvent]
        public override void OnCreatedRoom()
        {
            Play.RPC("OnSomebodySayHello", PlayRPCTargets.MasterClient, "hello");

            Play.RPC("OnSomebodySayHello", PlayRPCTargets.MasterClient, "hello");
            Play.RPC("OnSomebodySayHello", PlayRPCTargets.Others, "hello");
            Play.RPC("OnSomebodySayHello", PlayRPCTargets.OthersBuffered, "hello");
        }

        [PlayRPC]
        public void OnSomebodySayHello(string words)
        {
            var length = words.Length;
        }
    }
}
