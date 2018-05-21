using NUnit.Framework;
using System;
using LeanCloud;
using System.Linq;
using System.Collections;

namespace UnitTest.Mono
{
	[TestFixture()]
	public class JoinOrCreateTest : TestBase
	{
		public JoinOrCreateTest() : base()
		{

		}

		[Test()]
		[Timeout(300000)]
		public void JoinOrCreate()
		{
			Play.UserID = "WuJun";
			Play.Connect("0.0.1");

			Assert.That(Done, Is.True.After(2000000));
		}

		[PlayEvent]
		public override void OnAuthenticated()
		{
			Play.JoinOrCreateRoom(RandomRoomName);
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
