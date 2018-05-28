using System;
using LeanCloud.Realtime;

namespace LeanCloud {
	internal class RoomVisibleListener: RoomListenerBase {
		public RoomVisibleListener ()
		{
			MatchedOp = "visible";
		}

		public override void OnNoticeReceived (AVIMNotice notice)
		{
			bool visible = notice.RawData.GetValue<bool>("toggle", true);
			Play.Room.SetVisible(visible);
			base.OnNoticeReceived(notice);
		}
	}
}
