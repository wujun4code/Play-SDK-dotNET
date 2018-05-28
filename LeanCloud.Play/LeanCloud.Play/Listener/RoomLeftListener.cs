using LeanCloud.Realtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeanCloud
{
    internal class RoomLeftListener : RoomListenerBase
    {
        public RoomLeftListener()
        {
            MatchedOp = "members-left";
        }

        public override void OnNoticeReceived(AVIMNotice notice)
        {
            var player = this.RestorePlayer(notice);
            Play.Room.RemovePlayer(player.UserID);
            Play.PlayerLeft(player);
            base.OnNoticeReceived(notice);
        }
    }
}
