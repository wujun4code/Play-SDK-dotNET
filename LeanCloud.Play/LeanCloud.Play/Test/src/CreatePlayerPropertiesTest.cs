using System;
using System.Collections;
using System.Diagnostics;
using LeanCloud;
using NUnit.Framework;

namespace TestUnit.NetFx46
{
    /// <summary>
    /// Summary description for CreatePlayerPropertiesTest
    /// </summary>
    [TestFixture]
    public class CreatePlayerPropertiesTest : TestBase
    {
        public CreatePlayerPropertiesTest() : base()
        {

        }

        [Test, Timeout(30000)]
        public void CreatePlayerProperties()
        {
            Play.UserID = RandomClientId;
            Play.Connect("0.0.1");


            Assert.That(true, Is.True.After(2000000));
        }

        [PlayEvent]
        public override void OnAuthenticated()
        {
            Play.CreateRoom(RandomRoomName);
        }

        [PlayEvent]
        public override void OnCreatedRoom()
        {
            var initData = new Hashtable
            {
                { "ready", false },
                { "gold", 200 }
            };
            Play.Player.CustomProperties = initData;
        }

        [PlayEvent]
        public override void OnPlayerCustomPropertiesChanged(Player player, Hashtable updatedProperties)
        {
            var gold = player.CustomProperties["gold"];
            var actorId = Play.Player.ActorID;

            Assert.NotNull(gold);
        }

    }
}
