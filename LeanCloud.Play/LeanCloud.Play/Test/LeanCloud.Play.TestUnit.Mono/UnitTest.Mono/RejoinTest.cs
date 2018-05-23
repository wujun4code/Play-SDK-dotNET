using NUnit.Framework;
using LeanCloud;
using System.Collections;

namespace UnitTest.Mono
{
	[TestFixture()]
	public class RejoinTest : TestBase
	{
		[Test()]
		public void Rejoin()
		{
			Play.UserID = "WuJun";
			Play.Connect("0.0.1");

			Assert.That(Done, Is.True.After(2000000));
		}

		[PlayEvent]
		public override void OnAuthenticated()
		{
			Play.Log("OnAuthenticated");

			var config = PlayRoom.RoomConfig.Default;

			config.CustomRoomProperties = new Hashtable()
			{
				{ "rankedPoints", 104 }
			};

			config.LobbyMatchKeys = new string[] { "rankedPoints" };
            
			config.PlayerTimeToKeep = 30;

			Play.CreateRoom(config);
		}

		[PlayEvent]
		public override void OnCreatedRoom()
		{
			var roomName = Play.Room.Name;
		}

		[PlayEvent]
		public override void OnNewPlayerJoinedRoom(Player player)
		{
			var actorId = player.ActorID;
			var userId = player.UserID;
		}

		[PlayEvent]
		public override void OnPlayerActivityChanged(Player player)
		{
			Play.Log(player.UserID, player.ActorID);
		}
	}
}
