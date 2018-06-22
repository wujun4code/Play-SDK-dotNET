using LeanCloud.Realtime;
using LeanCloud.Realtime.Internal;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LeanCloud
{
    internal class PlayerPropertyListener : RoomListenerBase
    {
        public PlayerPropertyListener()
        {
            MatchedOp = "player-props";
        }

        public override void OnNoticeReceived(AVIMNotice notice)
        {
            var propObjs = notice.RawData["props"] as List<object>;

            if (propObjs != null)
            {

                propObjs.Every(metaData =>
                {
                    var prop = metaData as IDictionary<string, object>;
                    if (prop != null)
                    {
                        var actorId = int.Parse(prop["actorId"].ToString());

                        if (actorId > 0)
                        {
                            var player = Play.Room.GetPlayerByActorId(actorId);

                            var customProperties = prop["customProperties"] as IDictionary<string, object>;
                            if (player != null)
                            {
                                player.CustomPropertiesMetaData.Merge(customProperties);
                            }
                            Play.InvokeEvent(PlayEventCode.OnPlayerCustomPropertiesChanged, player, customProperties.ToHashtable());
                        }
                    }
                });
            }
        }
    }
}
