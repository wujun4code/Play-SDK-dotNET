using System;
using System.Collections;
using System.Diagnostics;
using LeanCloud;
using NUnit.Framework;

namespace TestUnit.NetFx46
{
    [TestFixture]
    public class RoomListFetchTest : TestBase
    {
        public RoomListFetchTest() : base()
        {

        }

        /// <summary>
        /// 测试当前用户随机加入房间之后，其他客户端的房主修改了当前用户的自定义属性，然后当前用户是否触发 OnCustomPropertiesUpdated
        /// </summary>
        [Test]
        [Timeout(300000)]
        public void RoomListFetch()
        {
            Play.UserID = RandomClientId;
            Play.Connect("0.0.1");

            Assert.That(true, Is.True.After(2000000));
        }

        [PlayEvent]
        public override void OnAuthenticated()
        {
            Play.FetchRoomList();
        }

        [PlayEvent]
        public override void OnNewPlayerJoinedRoom(Player player)
        {
            var c = player.CustomProperties;
            var initData = new Hashtable();
            initData.Add("ready", false);
            initData.Add("gold", 200);

            player.CustomProperties = initData;
        }
    }
}
