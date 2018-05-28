using LeanCloud.Realtime;
using System.Collections.Generic;

namespace LeanCloud
{
    internal class RoomMasterClientSwitchListener : RoomListenerBase
    {
        public RoomMasterClientSwitchListener()
        {
            MatchedOp = "master-client-changed";
        }

        public override void OnNoticeReceived(AVIMNotice notice)
        {
            var initBt = notice.RawData["initBy"] as string;
            var newMasterClientId = notice.RawData["masterClientId"] as string;
            Play.MasterClientSwitch(initBt, newMasterClientId);
            base.OnNoticeReceived(notice);
        }
    }
}
