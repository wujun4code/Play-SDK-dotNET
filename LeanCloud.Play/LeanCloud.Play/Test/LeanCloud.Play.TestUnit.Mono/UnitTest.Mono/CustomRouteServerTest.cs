using NUnit.Framework;
using System;
using LeanCloud;
using System.Collections;
using System.Linq;

namespace UnitTest.Mono
{
    [TestFixture()]
    public class CustomRouteServerTest : TestBase
    {
        [Test]
        [Timeout(300000)]
        public void CustomRouteServer()
        {
            Play.UserID = RandomClientId;
            Play.SetRouteServer("http://localhost:5000/play/");
            Play.Connect("0.0.1");

            Assert.That(true, Is.True.After(2000000));
        }

        [PlayEvent]
        public override void OnAuthenticated()
        {

        }

    }
}
