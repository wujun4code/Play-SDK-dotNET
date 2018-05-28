using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeanCloud.Realtime;

namespace LeanCloud
{
    internal class RoomOnlineListener : RoomListenerBase
    {
        public RoomOnlineListener()
        {
            MatchedOp = "members-online";
        }

        public override void OnNoticeReceived(AVIMNotice notice)
        {
            lock (Play.roomLock)
            {
                var player = this.RestorePlayer(notice);
                if (player != null)
                {
                    Play.PlayerOnline(player);
                }
                base.OnNoticeReceived(notice);
            }
        }
    }
}
