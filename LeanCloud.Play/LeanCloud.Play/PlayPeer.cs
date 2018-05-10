using LeanCloud.Storage.Internal;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LeanCloud
{
    public class PlayPeer
    {

        internal PlayPeer()
        {

        }

        internal PlayPeer(string userId)
        {
            this.ID = userId;
        }

        /// <summary>
        /// peer state
        /// </summary>
        public enum PlayPeerState
        {
            /// <summary>
            /// in lobby
            /// </summary>
            Aviable,
            /// <summary>
            /// searching Room
            /// </summary>
            SearchingRoom,
            /// <summary>
            /// in a Room
            /// </summary>
            InRoom,
        }

        /// <summary>
        /// peer state
        /// </summary>
        public PlayPeerState PeerState { get; internal set; }


        /// <summary>
        /// current peer authenticate
        /// </summary>
        /// <returns></returns>
        public bool Authenticate()
        {
            var authCommand = new PlayCommand()
            {
                Body = new Dictionary<string, object>()
                {
                    { "ua" , Play.PlayVersion + "_" + Play.GameVersion },
                    { "client_id", this.ID}
                },
                Method = "POST",
                RelativeUrl = "/authorization"
            };

            Play.RunHttpCommand(authCommand, PlayEventCode.OnAuthenticating, (req, resp) =>
            {
                this.SessionToken = resp.Body["token"] as string;
            });
            return true;
        }

        internal void SessionOpen(Action sessionOpened = null)
        {
            var sessionCommand = new PlayCommand()
            {
                Body = new Dictionary<string, object>()
                {
                    { "cmd", "session"},
                    { "op", "open"},
                    { "ua" ,Play.PlayVersion+"_" + Play.GameVersion},
                    { "st", SessionToken}
                }
            };
            Play.RunSocketCommand(sessionCommand, done: (req, response) =>
              {
                  if (sessionOpened != null)
                  {
                      sessionOpened();
                  }
              });
        }

        internal void SessionRoomJoin(PlayRoom room, Action<PlayRoom, PlayResponse> roomJoined = null)
        {
            var joinCommand = new PlayCommand()
            {
                Body = new Dictionary<string, object>()
                {
                    { "cmd", "conv" },
                    { "op", "add" },
                    { "cid", Play.Room.Name },
                }
            };
            Play.RunSocketCommand(joinCommand, done: (req, response) =>
            {
                if (roomJoined != null)
                {
                    roomJoined(room, response);
                }
            });
        }

        internal void SessionCreateRoom(PlayRoom room, Action<PlayRoom, PlayResponse> roomCreated = null)
        {
            IDictionary<string, object> body = new Dictionary<string, object>();
            if (room.Config.CustomRoomProperties != null)
            {
                body.Add("attr", room.Config.CustomRoomProperties.ToDictionary());
            }
            if (room.MaxPlayerCount > 0 && room.MaxPlayerCount != PlayRoom.DefaultMaxPlayerCount)
            {
                body.Add("maxMembers", room.MaxPlayerCount);
            }
            if (room.EmptyTimeToLive > 0 && room.EmptyTimeToLive != PlayRoom.DefaultMaxEmptyTimeToLive)
            {
                body.Add("emptyRoomTtl", room.EmptyTimeToLive);
            }
            if (room.PlayerTimeToKeep > 0 && room.PlayerTimeToKeep != PlayRoom.DefaultMaxKeepPlayerTime)
            {
                body.Add("playerTtl", room.PlayerTimeToKeep);
            }
            if (room.ExpectedUsers != null)
            {
                body.Add("expectMembers", room.ExpectedUsers);
            }
            if (!room.IsVisible)
            {
                body.Add("visible", room.IsVisible);
            }
            if (!room.IsOpen)
            {
                body.Add("open", room.IsVisible);
            }
            if (room.LobbyMatchKeys != null)
            {
                body.Add("lobbyAttrKeys", room.LobbyMatchKeys);
            }
            body.Add("cmd", "conv");
            body.Add("op", "start");
            body.Add("cid", room.Name);
            var createCommand = new PlayCommand()
            {
                Body = body
            };
            Play.RunSocketCommand(createCommand, done: (req, response) =>
            {
                if (response.IsSuccessful)
                {
                    room.MergeFromServer(response.Body);
                    if (roomCreated != null)
                    {
                        roomCreated(room, response);
                    }
                }
                else
                {
                    Play.InvokeEvent(PlayEventCode.OnCreateRoomFailed, response.ErrorCode, response.ErrorReason);
                }
            });
        }

        private string id;
        /// <summary>
        /// user id.
        /// </summary>
        public string ID
        {
            get
            {
                if (id == null)
                {
                    id = this.RandomClientID();
                }
                return id;
            }
            internal set
            {
                id = value;
            }
        }

        internal string RandomClientID(int length = 16)
        {
            return Play.RandomHexString(length);
        }

        private string sessionToken;
        public string SessionToken
        {
            get
            {
                return sessionToken;
            }
            internal set
            {
                sessionToken = value;
                PlayCommand.SetAuthentication(value);
            }
        }
    }
}
