using System;
using System.Collections;
using System.Diagnostics;
using System.Linq;
using LeanCloud;
using NUnit.Framework;

namespace TestUnit.NetFx46
{
    [TestFixture]
    public class SetPlayerPropertiesTest : TestBase
    {
        public SetPlayerPropertiesTest() : base()
        {
        }

        [Test]
        [Timeout(300000)]
        public void SetPlayerProperties()
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
        public override void OnCreatedRoom()
        {

        }

        [PlayEvent]
        public override void OnJoinedRoom()
        {
            var players = Play.Room.Players;
            var initData = new Hashtable();
            initData.Add("ready", false);
            initData.Add("gold", 200);
            Play.Player.CustomProperties = initData;
        }

        [PlayEvent]
        public override void OnPlayerCustomPropertiesChanged(Player player, Hashtable updatedProperties)
        {
            Play.Log("OnPlayerCustomPropertiesChanged");
            Play.Log(player.UserID, updatedProperties.ToLog());

            var cards = new Hashtable();
            cards.Add("cards", new string[] { "1", "2", "3" });
            Play.Player.CustomProperties = cards;
        }
    }
}
