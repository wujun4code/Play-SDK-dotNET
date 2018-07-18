using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeanCloud
{
    public class PlayServer : PlaySynchronousObject
    {
        /// <summary>
        /// Gets or sets the URL.
        /// </summary>
        /// <value>The URL.</value>
        public string Url { get; set; }

        /// <summary>
        /// Gets or sets the secondary URL.
        /// </summary>
        /// <value>The secondary URL.</value>
        public string SecondaryUrl { get; set; }

        /// <summary>
        /// Gets or sets the fetched at.
        /// </summary>
        /// <value>The fetched at.</value>
        public DateTime FetchedAt { get; set; }

        /// <summary>
        /// Gets or sets the ttl.
        /// </summary>
        /// <value>The ttl.</value>
        public int TTL { get; set; }

        /// <summary>
        /// Gets or sets the service mode.
        /// </summary>
        /// <value>The service mode.</value>
        public Mode ServiceMode { get; set; }

        /// <summary>
        /// Gets or sets the comunication protocol.
        /// </summary>
        /// <value>The comunication protocol.</value>
        public Protocol ComunicationProtocol { get; set; }

        /// <summary>
        /// Mode.
        /// </summary>
        public enum Mode
        {
            /// <summary>
            /// The public.
            /// </summary>
            Public = 0,
            /// <summary>
            /// The private.
            /// </summary>
            Private = 1,
            /// <summary>
            /// The self host.
            /// </summary>
            SelfHost = 2
        }

        /// <summary>
        /// Protocol.
        /// </summary>
        public enum Protocol
        {
            /// <summary>
            /// The web sokcet.
            /// </summary>
            WebSokcet = 0,
            /// <summary>
            /// The tcp.
            /// </summary>
            TCP = 1,
            /// <summary>
            /// The UDP.
            /// </summary>
            UDP = 2
        }
    }

    /// <summary>
    /// Lobby in Play.
    /// </summary>
    [PlaySynchronizeObjectName("_PlayLobby")]
    public class PlayLobby : PlayServer
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
        internal void AddRooms(IEnumerable<PlayRoom> rooms)
        {
            lock (metaDataMutex)
            {
                RoomsInLobby.Concat(rooms);
            }
        }

        public void Join()
        {
            var joinLobbyCommand = new PlayCommand()
            {
                Body = new Dictionary<string, object>()
                {
                    { "cmd" , "lobby" },
                    { "op" , "add" },
                },
            };

            Play.RunSocketCommand(joinLobbyCommand);
        }

        public void Leave()
        {
            var leaveLobbyCommand = new PlayCommand()
            {
                Body = new Dictionary<string, object>()
                {
                    { "cmd" , "lobby" },
                    { "op" , "remove" },
                },
            };

            Play.RunSocketCommand(leaveLobbyCommand); 
        }

        /// <summary>
        /// Fetchs from public cloud.
        /// </summary>
        /// <returns>The from public cloud.</returns>
        /// <param name="response">Response.</param>
        internal static PlayLobby FetchFromPublicCloud(PlayResponse response)
        {
            if (response.IsSuccessful)
            {
                return new PlayLobby()
                {
                    FetchedAt = DateTime.Now,
                    Url = response.Body["server"] as string,
                    SecondaryUrl = response.Body["secondary"] as string,
                    TTL = int.Parse(response.Body["ttl"].ToString()),
                    ServiceMode = Mode.Public,
                    ComunicationProtocol = Protocol.WebSokcet
                };
            }
            return null;
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
