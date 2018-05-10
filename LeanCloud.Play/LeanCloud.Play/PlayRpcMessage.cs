using LeanCloud.Realtime;
using LeanCloud.Storage.Internal;
using System;
using System.Collections.Generic;

namespace LeanCloud
{
    public class PlayRpcMessage
    {
        public PlayRpcMessage()
        {

        }

        internal PlayRpcMessage(AVIMNotice notice)
        {
            this.Deserialize(notice.RawData["msg"] as string);
        }

        public string MethodName { get; set; }

        public IEnumerable<object> Paramters { get; set; }

        internal bool Echo { get; set; }

        internal bool Cached { get; set; }

        internal List<string> ToPeers { get; set; }

        public string Serialize()
        {
            var msgBody = new Dictionary<string, object>();
            msgBody["m_n"] = this.MethodName;
            msgBody["m_p"] = this.Paramters;
            return Json.Encode(msgBody);
        }

        public void Deserialize(string msg)
        {
            var data = Json.Parse(msg) as IDictionary<string, object>;
            this.MethodName = data["m_n"] as string;
            this.Paramters = data["m_p"] as IList<object>;
        }

    }
}
