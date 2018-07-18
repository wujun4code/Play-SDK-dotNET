using System;
using LeanCloud;

namespace ConsoleApp.Minimum
{
    public class ConnectManager : PlayMonoBehaviour
    {
        public ConnectManager()
            : base()
        {

        }

        public Action<ConnectManager> ConnectedLobby { get; set; }

        public string UserId { get; set; }
        public ConnectManager(string uerId)
            : this()
        {
            AVClient.Initialize("315XFAYyIGPbd98vHPCBnLre-9Nh9j0Va", "Y04sM6TzhMSBmCMkwfI3FpHc");
            Play.ToggleLog(true);
            UserId = uerId;
            Play.UserID = uerId;
        }

        public ConnectManager Connect(string uaVersion)
        {
            Play.Connect(uaVersion);
            return this;
        }

        public ConnectManager UseLobby(string lobbyRouterUrl, string uaVersion)
        {
            Play.SetRouteServer(lobbyRouterUrl);
            Play.Connect(uaVersion);
            return this;
        }

        public void CreateRoom()
        {
            Play.CreateRoom();
        }

        [PlayEvent]
        public override void OnAuthenticated()
        {
            Play.Log("OnAuthenticated");
            if (ConnectedLobby != null)
            {
                ConnectedLobby(this);
            }
        }
    }
}
