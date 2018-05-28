using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeanCloud
{
    /// <summary>
    /// Lobby in Play.
    /// </summary>
    [PlaySynchronizeObjectName("_PlayLobby")]
    public class PlayLobby : PlaySynchronousObject
    {
        /// <summary>
        /// 
        /// </summary>
        public List<PlayRoom> RoomsInLobby = new List<PlayRoom>();
        private PlayQueryEnumerator<List<PlayRoom>> listEnumerator;

        /// <summary>
        /// 
        /// </summary>
        public PlayLobby()
        {
            this.ObjectName = "PlayLobby";
            this.objectState = new AVObject(this.ObjectName);

            ResetEnumerators();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="limit"></param>
        public void FetchRoom(int limit = 20)
        {
            if (limit != 20 && limit > 0)
            {
                this.listEnumerator.Limit = limit;
            }
            this.listEnumerator.Next();
        }

        /// <summary>
        /// 
        /// </summary>
        public void ResetEnumerators()
        {
            listEnumerator = new PlayQueryEnumerator<List<PlayRoom>>();
            listEnumerator.Command = new PlayCommand()
            {
                RelativeUrl = "/rooms",
                Method = "GET",
                UrlParameters = new Dictionary<string, object>()
            };

            listEnumerator.Limit = 20;

            listEnumerator.Decoder = (resp) =>
            {
                if (resp.IsSuccessful)
                {
                    var roomMetaDataObjs = resp.Body["rooms"] as List<object>;
                    if (roomMetaDataObjs != null)
                    {
                        var result = roomMetaDataObjs.Select(roomMetaDataObj =>
                          {
                              var room = new PlayRoom();
                              IDictionary<string, object> roomMetaData = roomMetaDataObj as IDictionary<string, object>;
                              room.MergeFromServer(roomMetaData);
                              return room;
                          });
                        return result.ToList();
                    }
                }
                else
                {
                    Play.InvokeEvent(PlayEventCode.OnFetchRoomListFailed, resp.ErrorCode, resp.ErrorReason);
                }
                return new List<PlayRoom>();
            };

            listEnumerator.Before = () =>
            {
                Play.InvokeEvent(PlayEventCode.OnFetchingRoomList);
            };

            listEnumerator.Callback = (rooms) =>
            {
                AddRooms(rooms);
                Play.InvokeEvent(PlayEventCode.OnFetchedRoomList, rooms);
            };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="rooms"></param>
        public void AddRooms(IEnumerable<PlayRoom> rooms)
        {
            lock (metaDataMutex)
            {
                RoomsInLobby.Concat(rooms);
            }
        }
    }

    internal class PlayQueryEnumerator<T>
    {
        public string Cursor { get; set; }
        public int Limit { get; set; }
        public PlayCommand Command { get; set; }
        public Func<PlayResponse, T> Decoder { get; set; }
        public Action<T> Callback { get; set; }
        public Action Before { get; set; }

        public void Next()
        {
            if (Cursor != null)
            {
                Command.UrlParameters["cursor"] = Cursor;
            }
            if (Limit > 0)
            {
                Command.UrlParameters["limit"] = Limit;
            }
            Command.UrlParameters["client_id"] = Play.peer.ID;
            Play.RunHttpCommand(Command, PlayEventCode.None, (req, resp) =>
            {
                var t = Decoder(resp);
                Callback(t);
                if (resp.Body.ContainsKey("cursor"))
                {
                    Cursor = resp.Body["cursor"] as string;
                }
            });
        }

        public void Previous()
        {

        }
    }
}
