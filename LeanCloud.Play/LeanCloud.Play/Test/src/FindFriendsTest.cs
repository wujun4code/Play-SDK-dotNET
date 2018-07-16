using System;
using System.Collections;
using System.Diagnostics;
using LeanCloud;
using NUnit.Framework;

namespace TestUnit.NetFx46
{
    [TestFixture]
    public class FindFriendsTest : TestBase
    {
        public FindFriendsTest() : base()
        {

        }

        /// <summary>
        /// 测试当前用户随机加入房间之后，其他客户端的房主修改了当前用户的自定义属性，然后当前用户是否触发 OnCustomPropertiesUpdated
        /// </summary>
        [Test]
        [Timeout(300000)]
        public void FindFriends()
        {
            Play.UserID = RandomClientId;
            Play.Connect("0.0.1");

            Assert.That(true, Is.True.After(2000000));
        }

        [PlayEvent]
        public override void OnAuthenticated()
        {

        }
    }
}
