using LeanCloud.Realtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeanCloud
{
    internal class PlayRPCListener : IAVIMListener
    {
        public void OnNoticeReceived(AVIMNotice notice)
        {
            var rpcMessage = new PlayRpcMessage(notice);
            Play.ReceivedRPC(rpcMessage);
        }

        public bool ProtocolHook(AVIMNotice notice)
        {
            if (notice.CommandName != "direct") return false;
            return true;
        }
    }
}
