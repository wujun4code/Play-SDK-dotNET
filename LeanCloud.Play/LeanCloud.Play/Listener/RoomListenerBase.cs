using LeanCloud.Realtime;

namespace LeanCloud
{
    internal abstract class RoomListenerBase : IAVIMListener
    {
        public string MatchedOp { get; set; }
        public virtual void OnNoticeReceived(AVIMNotice notice)
        {

        }

        public virtual bool ProtocolHook(AVIMNotice notice)
        {
            if (notice.CommandName != "conv") return false;
            if (!notice.RawData.ContainsKey("op")) return false;
            if (notice.RawData.ContainsKey("i")) return false;
            if (notice.RawData["op"].ToString() != this.MatchedOp) return false;
            return true;
        }

        public virtual Player RestorePlayer(AVIMNotice notice)
        {
            var clientId = notice.RawData["initBy"] as string;
            var player = Play.Room.GetPlayer(clientId);
            return player;
        }

        public virtual Player NewPlayer(AVIMNotice notice)
        {
            var clientId = notice.RawData["initBy"] as string;
            var actorId = int.Parse(notice.RawData["memberId"].ToString());
            var player = new Player(clientId, actorId);
            return player;
        }
    }
}
