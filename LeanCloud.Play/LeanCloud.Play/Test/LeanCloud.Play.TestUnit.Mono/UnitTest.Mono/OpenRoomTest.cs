using NUnit.Framework;
using System;

using LeanCloud;

namespace UnitTest.Mono
{

	[TestFixture()]
	public class OpenRoomTest : TestBase
	{
		public OpenRoomTest() : base()
		{

		}

		[Test()]
		[Timeout(300000)]
		public void OpenTest()
		{
			Play.UserID = "xxx";
			Play.Connect("0.0.1");

			Assert.That(Done, Is.True.After(2000000));
		}

		[PlayEvent]
		public override void OnAuthenticated()
		{
			Play.Log("OnAuthenticated");
			var conf = new PlayRoom.RoomConfig()
			{
				IsOpen = false,
				IsVisible = false,
			};
			Play.CreateRoom(conf, "123");
		}

		[PlayEvent]
		public override void OnCreatedRoom()
		{
			Play.Log("OnCreatedRoom");
			var roomName = Play.Room.Name;

		}

		[PlayEvent]
		public override void OnCreateRoomFailed(int errorCode, string reason)
		{
			Play.Log("OnCreateRoomFailed: " + reason);
		}

		[PlayEvent]
		public override void OnJoinedRoom()
		{
			Play.Log("OnJoinedRoom");
			Play.Log("IsOpen: " + Play.Room.IsOpen + ", " + Play.Room.IsVisible);
			Play.Room.IsOpen = false;
			Play.Room.IsVisible = true;
		}

		[PlayEvent]
		public override void OnJoinRoomFailed(int errorCode, string reason)
		{
			Play.Log("OnJoinRoomFailed: " + reason);
		}
	}
}
