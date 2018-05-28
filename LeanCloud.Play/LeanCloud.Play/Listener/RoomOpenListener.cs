using System;
using LeanCloud.Realtime;

namespace LeanCloud {
	internal class RoomOpenListener: RoomListenerBase {
		public RoomOpenListener ()
		{
			MatchedOp = "opened";
		}

		public override void OnNoticeReceived (AVIMNotice notice)
		{
			bool opened = notice.RawData.GetValue<bool>("toggle", true);
			Play.Room.SetOpened(opened);
			base.OnNoticeReceived(notice);
		}
	}
}
