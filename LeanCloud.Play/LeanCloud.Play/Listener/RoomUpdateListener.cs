using LeanCloud.Realtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeanCloud
{
    internal class RoomUpdateListener : RoomListenerBase
    {
        public RoomUpdateListener()
        {
            MatchedOp = "updated";
        }

        public override void OnNoticeReceived(AVIMNotice notice)
        {
            var transformation = notice.RawData["attr"] as IDictionary<string, object>;
            Play.RoomUpdated(transformation);
            base.OnNoticeReceived(notice);
        }
    }
}
