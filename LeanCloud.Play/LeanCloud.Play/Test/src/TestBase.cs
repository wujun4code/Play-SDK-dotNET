using System;
using System.Collections;
using System.Diagnostics;
using System.Linq;
using LeanCloud;
using NUnit.Framework;

namespace TestUnit.NetFx46
{
    /// <summary>
    /// Summary description for UpdatePlayerPropertiesTest
    /// </summary>
    [TestFixture]
    public class TestBase : PlayMonoBehaviour
    {
        public TestBase() : base()
        {

        }
        public bool Done = false;
        [SetUp]
        public virtual void SetUp()
        {
            AVClient.Initialize("315XFAYyIGPbd98vHPCBnLre-9Nh9j0Va", "Y04sM6TzhMSBmCMkwfI3FpHc");
            Websockets.Net.WebsocketConnection.Link();
            Play.Logger = (message) =>
            {
                System.Diagnostics.Debug.WriteLine(message);
                Trace.WriteLine(message);
            };
        }
        private static Random random = new Random();
        public static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789abcdefghijklmnopqrstuvwxyz";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }
        public virtual string RandomRoomName
        {
            get
            {
                return "room_" + RandomString(6);
            }
        }

        public virtual string RandomClientId
        {
            get
            {
                return "client_" + RandomString(6);
            }
        }
    }
}
