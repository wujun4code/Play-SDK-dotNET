using LeanCloud.Realtime;
using System.Collections.Generic;

namespace LeanCloud
{
    internal class RoomOfflineListener : RoomListenerBase
    {
        public RoomOfflineListener()
        {
            MatchedOp = "members-offline";
        }

        public override void OnNoticeReceived(AVIMNotice notice)
        {
            var player = this.RestorePlayer(notice);
            Play.PlayerOffline(player);
            base.OnNoticeReceived(notice);
        }
    }
}
