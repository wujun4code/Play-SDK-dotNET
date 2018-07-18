using System;
using LeanCloud;
using Xunit;

namespace UnitTest.NetCore
{
    public class ConnectTest : PlayMonoBehaviour
    {
        [Fact]
        public void ConnnectNow()
        {
            AVClient.Initialize("315XFAYyIGPbd98vHPCBnLre-9Nh9j0Va", "Y04sM6TzhMSBmCMkwfI3FpHc");
            Play.ToggleLog(true);
            Play.UserID = "wujun";
            Play.SetRouteServer("http://localhost:3000/play");
            Play.Connect("0.0.1");
        }

        [PlayEvent]
        public override void OnAuthenticated()
        {
            Play.Log("OnAuthenticated");
        }
    }
}
