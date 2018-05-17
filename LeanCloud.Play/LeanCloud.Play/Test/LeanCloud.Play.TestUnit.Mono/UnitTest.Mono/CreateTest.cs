using NUnit.Framework;
using System;

using LeanCloud;
using System.Collections;

namespace UnitTest.Mono
{
	[TestFixture()]
	public class CreateTest : TestBase
	{
        [Test()]
        [Timeout(300000)]
        public void Create()
        {
            Play.UserID = "xxx";
            Play.Connect("0.0.1");

            Assert.That(Done, Is.True.After(2000000));
        }

        [PlayEvent]
        public override void OnAuthenticated()
        {
            Play.Log("OnAuthenticated");
			Play.CreateRoom();
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
