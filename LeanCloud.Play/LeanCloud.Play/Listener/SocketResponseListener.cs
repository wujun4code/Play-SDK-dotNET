using System;
using LeanCloud.Realtime;

namespace LeanCloud
{
	internal class SocketResponseListener : IAVIMListener
	{
		public SocketResponseListener()
		{

		}

		public PlayCommand Command { get; set; }
		public PlayEventCode EventCode { get; set; }
		public Action<PlayCommand, PlayResponse> Done { get; set; }

		public virtual void OnNoticeReceived(AVIMNotice notice)
		{
			var response = new PlayResponse(notice.RawData);
			if (response.IsSuccessful)
			{
				if (Done != null)
				{
					Done(Command, response);
				}
			}
			//Play.LogCommand(Command, null, Play.CommandType.WebSocket);
			//Play.LogCommand(null, response, Play.CommandType.WebSocket);
			if (EventCode != PlayEventCode.None)
			{
				var next = PlayStateMachine.Next(EventCode, response);
				if (response.IsSuccessful)
				{
					Play.InvokeEvent(next);
				}
				else
				{
					Play.InvokeEvent(next, response.ErrorCode, response.ErrorReason);
				}
			}
			Play.UnsubscribeNoticeReceived(this);
		}

		public virtual bool ProtocolHook(AVIMNotice notice)
		{
			if (!notice.RawData.ContainsKey("i")) return false;
			if (int.Parse(notice.RawData["i"].ToString()) != Command.SocketCommandId) return false;
			return true;
		}

	}
}
