using LeanCloud.Realtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LeanCloud
{
    internal class RoomJoinListener : RoomListenerBase
    {
        public RoomJoinListener()
        {
            MatchedOp = "members-joined";
        }

        public override void OnNoticeReceived(AVIMNotice notice)
        {
            var newPlayer = this.NewPlayer(notice);
            Play.Room.AddPlayer(newPlayer);
			if(newPlayer.UserID != Play.Player.UserID)
            	Play.NewPlayerJoined(newPlayer);
            base.OnNoticeReceived(notice);
        }
    }
}
