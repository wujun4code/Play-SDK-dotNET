using NUnit.Framework;
using System;

using LeanCloud;

namespace UnitTest.Mono
{
	[TestFixture()]
	public class JoinTest : TestBase
	{
		public JoinTest() : base()
		{

		}

		[Test()]
		[Timeout(300000)]
		public void Join()
		{
			Play.UserID = "SiYuan";
			Play.Connect("0.0.1");

			Assert.That(Done, Is.True.After(2000000));
		}

		[PlayEvent]
		public override void OnAuthenticated()
		{
			Play.Log("OnAuthenticated");
			Play.JoinRoom("xman");
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
