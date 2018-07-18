using System;
using System.Collections.Generic;
using System.Linq;
using LeanCloud.Realtime;

namespace LeanCloud
{
    internal class LobbyRoomListener : IAVIMListener
    {
        public LobbyRoomListener()
        {
        }

        public void OnNoticeReceived(AVIMNotice notice)
        {
            var rooms = PlayRoom.FromServerNotice(notice.RawData["list"]);
            Play.lobby.AddRooms(rooms);
        }

        public bool ProtocolHook(AVIMNotice notice)
        {
            if (notice.CommandName != "lobby") return false;
            if (!notice.RawData.ContainsKey("op")) return false;
            if (notice.RawData.ContainsKey("i")) return false;
            if (notice.RawData["op"].ToString() != "room-list") return false;
            return true;
        }
    }
}
