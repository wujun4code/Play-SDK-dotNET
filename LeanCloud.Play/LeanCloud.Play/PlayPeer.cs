using LeanCloud.Storage.Internal;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LeanCloud
{
    internal class PlayPeer
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
        /// Get the lobby server.
        /// </summary>
        /// <param name="secure">If set to <c>true</c> secure.</param>
        /// <param name="lobbyLoaded">after lobby loaded.</param>
		public void GetLobbyServer(bool secure = true, Action<PlayGameServer> lobbyLoaded = null)
        {
            var lobbyGetCmd = new PlayCommand()
            {
                RelativeUrl = "/router",
                UrlParameters = new Dictionary<string, object>()
                {
                    { "appId", AVClient.CurrentConfiguration.ApplicationId },
                    { "secure" , secure }
                },
            };

            Play.RunHttpCommand(lobbyGetCmd, done: (request, response) =>
            {
                var gameServer = Play.GameServer = PlayGameServer.FetchFromPublicCloud(response);
                if (lobbyLoaded != null)
                {
                    lobbyLoaded(gameServer);
                }
            });
        }

        public void ConnectGameServer()
        {
            Play.DoConnectToGameSever(Play.GameServer, () =>
            {
                var sessionOpenCmd = new PlayCommand()
                {
                    Body = new Dictionary<string, object>()
                    {
                        { "cmd", "session"},
                        { "op", "open"},
                        { "ua" ,Play.PlayVersion+"_" + Play.GameVersion},
                        { "peerId", this.ID }
                    }
                };

                Play.RunSocketCommand(sessionOpenCmd, PlayEventCode.OnAuthenticating, done: (req, res) =>
                 {
                     this.SessionToken = res.Body["st"] as string;
                 });
            });
        }

        /// <summary>
        /// Authenticates from game server.
        /// </summary>
        public void AuthenticateFromLobbyServer()
        {
            GetLobbyServer(lobbyLoaded: (gameServer) =>
            {
                ConnectGameServer();
            });
        }

        /// <summary>
        /// current peer authenticate
        /// </summary>
        /// <returns></returns>
        [Obsolete("please use AuthenticateFromLobbyServer instaed.")]
        public void AuthenticateFromRouteServer()
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

        internal void SessionRoomJoin(PlayRoom room, bool isRejoin = false, Action<PlayRoom, PlayResponse> roomJoined = null)
        {
            var joinCommand = new PlayCommand()
            {
                Body = new Dictionary<string, object>()
                {
                    { "cmd", "conv" },
                    { "op", "add" },
                    { "cid", room.Name },
                }
            };
            if (isRejoin)
            {
                joinCommand.Body.Add("rejoin", isRejoin);
            }
            Play.RunSocketCommand(joinCommand, done: (req, response) =>
            {
                if (roomJoined != null)
                {
                    roomJoined(room, response);
                }
            });
        }

        internal void SessionCreateRoom(string roomName, PlayRoom.RoomConfig roomConfig, Action<PlayRoom> roomCreated = null)
        {
            IDictionary<string, object> body = new Dictionary<string, object>();
            if (roomConfig.CustomRoomProperties != null)
            {
                body.Add("attr", roomConfig.CustomRoomProperties.ToDictionary());
            }
            if (roomConfig.MaxPlayerCount > 0 && roomConfig.MaxPlayerCount != PlayRoom.DefaultMaxPlayerCount)
            {
                body.Add("maxMembers", roomConfig.MaxPlayerCount);
            }
            if (roomConfig.EmptyTimeToLive > 0 && roomConfig.EmptyTimeToLive != PlayRoom.DefaultMaxEmptyTimeToLive)
            {
                body.Add("emptyRoomTtl", roomConfig.EmptyTimeToLive);
            }
            if (roomConfig.PlayerTimeToKeep > 0 && roomConfig.PlayerTimeToKeep != PlayRoom.DefaultMaxKeepPlayerTime)
            {
                body.Add("playerTtl", roomConfig.PlayerTimeToKeep);
            }
            if (roomConfig.ExpectedUsers != null)
            {
                body.Add("expectMembers", roomConfig.ExpectedUsers);
            }
            if (!roomConfig.IsVisible)
            {
                body.Add("visible", roomConfig.IsVisible);
            }
            if (!roomConfig.IsOpen)
            {
                body.Add("open", roomConfig.IsOpen);
            }
            if (roomConfig.LobbyMatchKeys != null)
            {
                body.Add("lobbyAttrKeys", roomConfig.LobbyMatchKeys);
            }
            body.Add("cmd", "conv");
            body.Add("op", "start");
            body.Add("cid", roomName);
            var createCommand = new PlayCommand()
            {
                Body = body
            };

            Play.RunSocketCommand(createCommand, done: (req, response) =>
            {
                if (response.IsSuccessful)
                {
                    var room = new PlayRoom(roomConfig, roomName);
                    Play.DoSetRoomProperties(room, response);
                    if (roomCreated != null)
                    {
                        roomCreated(room);
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
