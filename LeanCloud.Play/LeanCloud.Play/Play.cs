using LeanCloud.Realtime;
using LeanCloud.Realtime.Internal;
using LeanCloud.Storage.Internal;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading;

#if UNITY
using UnityEngine;
#endif

namespace LeanCloud
{
    /// <summary>
    /// LeanCloud Play core class.
    /// </summary>
    public static class Play
    {
        #region constructors
        static Play()
        {
            peer = new PlayPeer();
            Player = new Player(peer);
            lobby = new PlayLobby();
            EevntMessageQueue = new Queue<PlayEventMessage>();
            Play.OnPlayEvent += InvokeEvent;

            Play.SubscribeNoticeReceived(new PlayRPCListener());
            Play.SubscribeNoticeReceived(new RoomJoinListener());
            Play.SubscribeNoticeReceived(new RoomOnlineListener());
            Play.SubscribeNoticeReceived(new RoomOfflineListener());
            Play.SubscribeNoticeReceived(new RoomUpdateListener());
            Play.SubscribeNoticeReceived(new RoomLeftListener());
            Play.SubscribeNoticeReceived(new RoomMasterClientSwitchListener());
            Play.SubscribeNoticeReceived(new PlayerPropertyListener());
            Play.SubscribeNoticeReceived(new RoomOpenListener());
            Play.SubscribeNoticeReceived(new RoomVisibleListener());
            Play.StartListen();

            Play.RegisterSynchronousObjectType<PlayRoom>();
            Play.RegisterSynchronousObjectType<Player>();
            Play.RegisterSynchronousObjectType<PlayLobby>();


        }
        internal static PlayMainThreadQueue playMainThreadQueue = new PlayMainThreadQueue();
        private static EventHandler<AVIMNotice> m_NoticeReceived;
        internal static event EventHandler<AVIMNotice> NoticeReceived
        {
            add
            {
                m_NoticeReceived += value;
            }
            remove
            {
                m_NoticeReceived -= value;
            }
        }
        internal static object mutexEventMessageLock = new object();
        /// <summary>
        /// Gets or sets the eevnt message queue.
        /// </summary>
        /// <value>The eevnt message queue.</value>
        public static Queue<PlayEventMessage> EevntMessageQueue { get; set; }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="eventCode"></param>
        /// <param name="parameters"></param>
        public static void InvokeEvent(PlayEventCode eventCode, params object[] parameters)
        {
#if UNITY
			EnqueueEventMessage(eventCode, parameters);
#else
            Play.EventBehaviours.When(kv => kv.Key == eventCode.ToString()).Every(kv =>
            {
                kv.Value.Every(b =>
                {
                    var callbackMethod = Play.Find<PlayEventAttribute>(b, kv.Key);
                    if (callbackMethod != null)
                    {
                        callbackMethod.Invoke(b, parameters.Length > 0 ? parameters : null);
                    }
                });
            });
#endif
        }

        /// <summary>
        /// Enqueues the event message.
        /// </summary>
        /// <param name="eventCode">Event code.</param>
        /// <param name="parameters">Parameters.</param>
        public static void EnqueueEventMessage(PlayEventCode eventCode, params object[] parameters)
        {
            EnqueueEventMessage(new PlayEventMessage()
            {
                MethodName = eventCode.ToString(),
                MethodParameters = parameters
            });
        }

        /// <summary>
        /// Enqueues the event message.
        /// </summary>
        /// <param name="eventMessage">Event message.</param>
        public static void EnqueueEventMessage(PlayEventMessage eventMessage)
        {
            lock (mutexEventMessageLock)
            {
                EevntMessageQueue.Enqueue(eventMessage);
            }
        }
        /// <summary>
        /// Dequeues the event message.
        /// </summary>
        /// <param name="eventCode">Event code.</param>
        public static void DequeueEventMessage(PlayEventCode eventCode)
        {

        }
        #endregion

        #region internal variales & properties
        internal static PlayInitializeBehaviour playMQB;
        private static PlayConnection roomConnection;
        internal static PlayConnection RoomConnection
        {
            get
            {
                roomConnection = roomConnection ?? ResetConnection();

                return roomConnection;
            }
            set
            {
                roomConnection = value;
            }
        }

        internal static PlayConnection ResetConnection()
        {
            return new PlayConnection(new DefaultWebSocketClient());
        }

        internal static PlayGameServer GameServer;
        internal static PlayPeer peer;
        internal static PlayLobby lobby;
        internal static List<PlayFriend> friends;

        internal static object roomLock = new object();
        /// <summary>
        /// current Room.
        /// </summary>
        public static PlayRoom Room { get; set; }


        /// <summary>
        /// players in current Room.
        /// </summary>
        public static IEnumerable<Player> Players
        {
            get
            {
                return Room.Players;
            }
        }

        /// <summary>
        /// current Player
        /// </summary>
        public static Player Player { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public static IEnumerable<PlayFriend> Friends
        {
            get
            {
                return friends;
            }
            internal set
            {
                friends = value.ToList();
            }
        }

        public static PlayLobby Lobby
        {
            get
            {
                return lobby;
            }
            internal set
            {
                lobby = value;
            }
        }

        //public static IEnumerable<PlayRoom> Rooms
        //{
        //    get
        //    {
        //        return Lobby.RoomsInLobby;
        //    }
        //}


        internal static string AppId
        {
            get
            {
                return AVClient.CurrentConfiguration.ApplicationId;
            }
        }

        internal static string SessionToken
        {
            get
            {
                return peer.SessionToken;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this <see cref="T:LeanCloud.Play"/> is master client.
        /// </summary>
        /// <value><c>true</c> if is master client; otherwise, <c>false</c>.</value>
		public static bool IsMasterClient
        {
            get
            {
                return Room.MasterClientId == peer.ID;
            }
        }

        /// <summary>
        /// game version.
        /// </summary>
        public static readonly string PlayVersion = "0.0.1";
        #endregion

        #region public variables & properties
        /// <summary>
        /// clients with the same game version can play toghter.
        /// </summary>
        public static string GameVersion { get; set; }

        #endregion

        #region public methods
        /// <summary>
        /// Server type.
        /// </summary>
        public enum ServerType
        {
            /// <summary>
            /// route for lobby
            /// </summary>
            Route = 0,
            /// <summary>
            /// lobby server
            /// </summary>
            Lobby = 1,
            /// <summary>
            /// game server
            /// </summary>
            Game = 2,
        }

        /// <summary>
        /// Connect the specified gameVersion.
        /// </summary>
        /// <param name="gameVersion">Game version.</param>
        public static void Connect(string gameVersion)
        {
            Connect(gameVersion, ServerType.Lobby);
        }

        /// <summary>
        /// Connect the specified gameVersion and serverType.
        /// </summary>
        /// <param name="gameVersion">Game version.</param>
        /// <param name="serverType">Server type.</param>
        internal static void Connect(string gameVersion, ServerType serverType)
        {
            GameVersion = gameVersion;

            RoomConnection.OnError += RoomConnection_OnError;
            RoomConnection.OnClosed += RoomConnection_OnClosed;
            RoomConnection.OnMessage += RoomConnection_OnMessage;

            if (serverType == ServerType.Lobby)
            {
                peer.AuthenticateFromLobbyServer();
            }
            else if (serverType == ServerType.Route)
            {
                peer.AuthenticateFromRouteServer();
            }
        }

        /// <summary>
        /// user id.
        /// </summary>
        public static string UserID
        {
            get
            {
                return peer.ID;
            }
            set
            {
                peer.ID = value;
            }
        }

        /// <summary>
        /// state of current peer.
        /// </summary>
        internal static PlayPeer.PlayPeerState PeerState
        {
            get { return peer.PeerState; }
        }

        /// <summary>
        /// Creates the room.
        /// </summary>
        /// <param name="roomName">Room name.</param>
        /// <param name="expectedUsers">Expected users.</param>
        public static void CreateRoom(string roomName = null, IEnumerable<string> expectedUsers = null)
        {
            var config = PlayRoom.RoomConfig.Default;
            config.ExpectedUsers = expectedUsers;

            CreateRoom(config, roomName);
        }

        /// <summary>
        /// create a room with config and name.
        /// </summary>
        /// <param name="roomConfig">config of Room</param>
        /// <param name="roomName">name of Room</param>
        public static void CreateRoom(PlayRoom.RoomConfig roomConfig, string roomName = null)
        {
            var createRoomCommand = new PlayCommand()
            {
                Body = new Dictionary<string, object>()
                {
                    { "cmd", "conv"},
                    { "op", "start"},
                },
            };

            if (string.IsNullOrEmpty(roomName))
            {
                roomName = PlayRoom.RandomRoomName(24);
            }

            RunSocketCommand(createRoomCommand, done: (request, response) =>
            {
                // get game server address from game router
                var roomRemoteSecureAddress = response.Body["secureAddr"] as string;
                var roomRemoteAddress = response.Body["addr"] as string;

                // open websocket connection with game server
                DoConnectToGameSever(roomRemoteSecureAddress, () =>
                {
                    Play.InvokeEvent(PlayEventCode.OnCreatingRoom);
                    // create room at game server
                    peer.SessionCreateRoom(roomName, roomConfig, (room) =>
                     {
                         Play.Room = room;
                         Play.InvokeEvent(PlayEventCode.OnCreatedRoom);
                         Play.InvokeEvent(PlayEventCode.OnJoinedRoom);
                     });
                });
            });
        }

        /// <summary>
        /// join a random Room.
        /// </summary>
        /// <returns></returns>
        public static void JoinRandomRoom(Hashtable matchProperties = null)
        {
            var joinRandomRoomCommand = new PlayCommand()
            {
                Body = new Dictionary<string, object>()
                {
                    { "cmd", "conv"},
                    { "op", "start"},
                },
            };
            if (matchProperties != null)
            {
                joinRandomRoomCommand.Body.Add("expectAttr", matchProperties.ToDictionary<string, object>());
            }
            RunSocketCommand(joinRandomRoomCommand, PlayEventCode.OnRandomJoiningRoom, DoJoinRoom);
        }

        /// <summary>
        /// join a Room by name.
        /// </summary>
        /// <param name="roomName">unique id for the Room to join in.</param>
        /// <returns></returns>
        public static void JoinRoom(string roomName)
        {
            if (string.IsNullOrEmpty(roomName))
            {
                Play.InvokeEvent(PlayEventCode.OnJoinRoomFailed, "-100", "can NOT join a room with the name is NULL.");
                return;
            }
            var joinRoomCommand = new PlayCommand()
            {
                Body = new Dictionary<string, object>()
                {
                    { "cmd", "conv"},
                    { "op", "add"},
                    { "cid", roomName },
                },
            };
            RunSocketCommand(joinRoomCommand, done: DoJoinRoom);
        }

        /// <summary>
        /// Join or create room.
        /// </summary>
        /// <param name="roomName">Room name.</param>
        public static void JoinOrCreateRoom(string roomName)
        {
            JoinOrCreateRoom(roomName, PlayRoom.RoomConfig.Default);
        }
        /// <summary>
        /// Join or create room.
        /// </summary>
        /// <param name="roomName">Room name.</param>
        /// <param name="roomConfig">Room config.</param>
        public static void JoinOrCreateRoom(string roomName, PlayRoom.RoomConfig roomConfig)
        {
            if (string.IsNullOrEmpty(roomName))
            {
                LogError("roomName can NOT be null or empty.");
                return;
            }
            var joinOrCreateRoomCommand = new PlayCommand()
            {
                Body = new Dictionary<string, object>()
                {
                    { "cmd", "conv"},
                    { "op", "add"},
                    { "cid", roomName},
                    { "createOnNotFound", true },
                },
            };

            if (roomConfig.ExpectedUsers != null)
            {
                joinOrCreateRoomCommand.Body.Add("expectMembers", roomConfig.ExpectedUsers.ToArray());
            }

            RunSocketCommand(joinOrCreateRoomCommand, PlayEventCode.OnJoinOrCreatingRoom, (req, res) =>
            {
                if (res.Body.ContainsKey("cid"))
                {
                    DoJoinRoom(req, res);
                }
                else
                {
                    CreateRoom(roomConfig, roomName);
                }
            });
        }

        /// <summary>
        /// Rejoin the room.
        /// </summary>
        /// <param name="roomName">Room name.</param>
        public static void RejoinRoom(string roomName = null)
        {
            if (string.IsNullOrEmpty(roomName))
            {
                if (Play.Room == null)
                {
                    Play.InvokeEvent(PlayEventCode.OnJoinRoomFailed, "-100", "can NOT join a room with the name is NULL.");
                    return;
                }
                roomName = Play.Room.Name;
            }

            var joinRoomCommand = new PlayCommand()
            {
                Body = new Dictionary<string, object>()
                {
                    { "cmd", "conv"},
                    { "op", "add"},
                    { "cid", roomName },
                    { "rejoin", true},
                },
            };
            RunSocketCommand(joinRoomCommand, done: DoRejoinRoom);
        }

        /// <summary>
        /// fetch rooms from server.
        /// </summary>
        /// <param name="limit">count for fetch</param>
        /// <param name="reset">reset room list indexer</param>
        public static void FetchRoomList(int limit = 20, bool reset = false)
        {
            if (reset)
            {
                lobby.ResetEnumerators();
            }
            lobby.FetchRoom(limit);
        }

        /// <summary>
        /// player
        /// </summary>
        /// <param name="player"></param>
        public static void SetMasterClient(Player player)
        {
            var setMasterClientCommand = new PlayCommand()
            {
                Body = new Dictionary<string, object>()
                {
                    { "cmd" , "conv" },
                    { "op" , "update-master-client" },
                    { "masterClientId" , player.UserID },
                    { "cid" , Play.Room.Name },
                }
            };

            RunSocketCommand(setMasterClientCommand, PlayEventCode.None);
        }

        /// <summary>
        /// find friends.
        /// </summary>
        public static void FindFriends(IEnumerable<string> friendsToFind)
        {
            var friendIdInStr = string.Join(",", friendsToFind.ToArray());
            var findFriendsCommand = new PlayCommand()
            {
                Body = new Dictionary<string, object>()
                {
                    { "friend_ids" , friendIdInStr },
                    { "client_id" , Play.peer.ID },
                }
            };

            RunHttpCommand(findFriendsCommand, PlayEventCode.OnFindingFriends, (req, res) =>
            {
                lock (friendListMutex)
                {
                    var onlineListStr = res.Body["online_list"] as string;
                    var onlineList = onlineListStr.Split(',');
                    var inRoomListAStr = res.Body["roomid_list"] as string;
                    var inRoomList = inRoomListAStr.Split(',');
                    var friendsToFindArray = friendsToFind.ToArray();
                    for (var i = 0; i < friendsToFindArray.Length; i++)
                    {
                        var userId = friendsToFindArray[i];
                        var inCacheRoom = Play.Friends.FirstOrDefault(f => f.UserId == userId);
                        if (inCacheRoom != null)
                        {
                            inCacheRoom.IsOnline = bool.Parse(onlineList[i]);
                            inCacheRoom.Room = inRoomList[i] == "null" ? "" : inRoomList[i];
                        }
                        else
                        {
                            Play.friends.Add(new PlayFriend()
                            {
                                UserId = userId,
                                IsOnline = bool.Parse(onlineList[i]),
                                Room = inRoomList[i] == "null" ? "" : inRoomList[i]
                            });
                        }
                    }
                }
            });
        }


        /// <summary>
        /// leave current game Room.
        /// </summary>
        /// <returns></returns>
        public static void LeaveRoom()
        {
            var leaveRoomCommand = new PlayCommand()
            {
                Body = new Dictionary<string, object>()
                {
                    { "cmd" , "conv" },
                    { "op" , "remove" },
                    { "cid", Room.Name }
                },
            };
            RunSocketCommand(leaveRoomCommand, PlayEventCode.OnLeavingRoom, DoLeaveRoom);
        }

        private static object friendListMutex = new object();
        internal static void DoUpdateFriends(PlayCommand request, PlayResponse response)
        {
            lock (friendListMutex)
            {
                var onlineListStr = response.Body["online_list"] as string;
                var onlineList = onlineListStr.Split(',');
            }
        }

        internal static void DoJoinRoom(PlayCommand request, PlayResponse response)
        {
            if (!response.IsSuccessful)
            {
                Play.InvokeEvent(PlayEventCode.OnJoinRoomFailed, response.ErrorCode, response.ErrorReason);
                return;
            }
            var room = GetRoomWhenGet(request, response);
            Play.InvokeEvent(PlayEventCode.OnJoiningRoom);
            ConnectRoom(room);
        }

        internal static void DoRejoinRoom(PlayCommand request, PlayResponse response)
        {
            if (!response.IsSuccessful)
            {
                Play.InvokeEvent(PlayEventCode.OnJoinRoomFailed, response.ErrorCode, response.ErrorReason);
                return;
            }
            var room = GetRoomWhenGet(request, response);
            Play.InvokeEvent(PlayEventCode.OnJoiningRoom);
            ConnectRoom(room, true);
        }

        internal static void DoLeaveRoom(PlayCommand request, PlayResponse response)
        {
            DisconnectRoom(Play.Room);
        }

        internal static PlayRoom GetRoomWhenGet(PlayCommand request, PlayResponse response)
        {
            var room = new PlayRoom();
            room.RoomRemoteSecureAddress = response.Body["secureAddr"] as string;
            room.RoomRemoteAddress = response.Body["addr"] as string;

            var roomProperties = response.Body;
            room.SetProperties(roomProperties);

            return room;
        }

        internal static Player GetPlayer(string clientID)
        {
            return Play.Room.GetPlayer(clientID);
        }

        internal static void ConnectRoom(PlayRoom room, bool isRejoin = false)
        {
            lock (roomLock)
            {
                DoConnectToGameSever(room, () =>
                {
                    peer.SessionRoomJoin(room, isRejoin: isRejoin, roomJoined: (roomm, response) =>
                       {
                           DoSetRoomProperties(roomm, response);
                           Play.Room = roomm;
                           Play.InvokeEvent(PlayEventCode.OnJoinedRoom);
                       });
                });
            }
        }

        internal static void DisconnectRoom(PlayRoom room)
        {
            room = null;
            Player = new Player(peer);
            Play.RoomConnection = ResetConnection();
        }

        internal static void DoSetRoomProperties(PlayRoom room, PlayResponse response)
        {
            var invalidKeys = new string[] { "cmd", "peerId", "op", "appId", "i" };

            room.SetProperties(response.Body.Filter(invalidKeys));
        }

        internal static void DoConnectToGameSever(PlayGameServer server, Action connected)
        {
            if (string.IsNullOrEmpty(server.Url))
            {
                LogError("can NOT connect Room withouth remote addresss");
                return;
            }
            RoomConnection.Open(server.Url);
            Log(string.Format("try to connect with server address:{0}", server.Url));
            Action onOpened = null;
            onOpened = () =>
            {
                Log(string.Format("connected with server address:{0}", server.Url));
                RoomConnection.OnOpened -= onOpened;
                if (connected != null)
                {
                    connected();
                }
            };
            RoomConnection.OnOpened += onOpened;
        }

        internal static void DoConnectToGameSever(string serverAddress, Action connected)
        {
            if (string.IsNullOrEmpty(serverAddress))
            {
                LogError("can NOT connect Room withouth remote addresss");
                return;
            }
            RoomConnection.Open(serverAddress);
            Log(string.Format("try to connect with server address:{0}", serverAddress));
            Action onOpened = null;
            onOpened = () =>
            {
                Log(string.Format("connected with server address:{0}", serverAddress));
                RoomConnection.OnOpened -= onOpened;
                Play.peer.SessionOpen(connected);
            };
            RoomConnection.OnOpened += onOpened;
        }

        internal static void DoConnectToGameSever(PlayRoom room, Action connected = null)
        {
            if (room.RoomRemoteSecureAddress == null)
            {
                LogError("can NOT connect Room withouth remote addresss");
                return;
            }
            RoomConnection.Open(room.RoomRemoteSecureAddress);
            Action onOpened = null;
            onOpened = () =>
            {
                Play.Player.Room = room;
                Log(string.Format("connected Room(unique id:{0}) at server address:{1}", room.Name, room.RoomRemoteSecureAddress));
                RoomConnection.OnOpened -= onOpened;
                Play.peer.SessionOpen(connected);
            };
            RoomConnection.OnOpened += onOpened;
        }

        private static void RoomConnection_OnClosed(int arg1, string arg2, string arg3)
        {
            Play.InvokeEvent(PlayEventCode.OnDisconnected);

            Log("sokcet closed: " + arg1 + arg2 + arg3);
        }

        private static void RoomConnection_OnError(string obj)
        {
            Log("sokcet error: " + obj);
        }

        private static void RoomConnection_OnMessage(string obj)
        {
            try
            {
                lock (noticeMutext)
                {
                    Log("sokcet<=" + obj);

                    var metaNoticeFromServer = Json.Parse(obj) as IDictionary<string, object>;

                    var validator = AVIMNotice.IsValidLeanCloudProtocol(metaNoticeFromServer);

                    if (validator)
                    {
                        var notice = new AVIMNotice(metaNoticeFromServer);
                        m_NoticeReceived?.Invoke(peer, notice);
                    }
                }
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null)
                {
                    Log(ex.InnerException.Source);
                }
                if (ex.Source != null)
                {
                    Log(ex.Source);
                }
                if (ex.StackTrace != null)
                {
                    Log(ex.StackTrace);
                }

                Log(ex.Message);
            }
        }
        #endregion

        #region Log
        public static Action<string> Logger { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="toggle"></param>
        public static void ToggleLog(bool toggle = true)
        {
            if (toggle)
                PlayLogger.Open();
            else PlayLogger.Close();
        }


        internal static object logMutext = new object();
        /// <summary>
        /// print logs to console.
        /// </summary>
        /// <param name="message"></param>
        public static void Log(params object[] message)
        {
            Print(Logger, message);
        }

        internal static Action<string> ErrorLogger { get; set; }


        /// <summary>
        /// log error to console.
        /// </summary>
        /// <param name="message"></param>
        public static void LogError(params object[] message)
        {
            Print(ErrorLogger, message);
        }

        internal static void Print(Action<string> printer, params object[] message)
        {
            if (printer == null) return;
            var converted = message.Select(m => m != null ? m.ToString() : "null").ToArray();
            var merged = string.Join(",", converted);
            printer(merged);
        }

        internal enum CommandType
        {
            Http = 0,
            WebSocket = 1,
        }

        internal static void LogCommand(PlayCommand command, PlayResponse response = null, CommandType commandType = CommandType.Http, Action<string> printer = null)
        {
            if (printer == null) printer = Logger;
            lock (logMutext)
            {
                if (command != null)
                {
                    IDictionary<string, object> requestDictionary = new Dictionary<string, object>();
                    if (commandType == CommandType.Http)
                    {
                        requestDictionary.Add("url", command.RelativeUrl);
                        requestDictionary.Add("method", command.Method);
                        requestDictionary.Add("headers", PlayCommand.Headers);
                        requestDictionary.Add("body", command.Body);
                    }
                    else
                    {
                        requestDictionary = command.Body;
                    }
                    var requestId = requestDictionary.ContainsKey("i") ? requestDictionary["i"].ToString() : "";
                    Print(printer, commandType + " request:" + requestId + "=>" + requestDictionary.ToJsonLog());
                }

                if (response != null)
                {
                    IDictionary<string, object> responseDictionary = new Dictionary<string, object>();
                    if (commandType == CommandType.Http)
                    {
                        responseDictionary.Add("statusCode", response.StatusCode);
                        responseDictionary.Add("body", response.Body);
                    }
                    else
                    {
                        responseDictionary = response.Body;
                    }

                    var responseId = responseDictionary.ContainsKey("i") ? responseDictionary["i"].ToString() : "";
                    Print(printer, commandType + " response:" + responseId + "<=" + responseDictionary.ToJsonLog());
                }
            }


        }
        #endregion

        #region events
        internal delegate void PlayEvent(PlayEventCode eventCode, params object[] parameters);
        private static PlayEvent m_OnEvent;
        internal static event PlayEvent OnPlayEvent
        {
            add
            {
                m_OnEvent += value;
            }
            remove
            {
                m_OnEvent -= value;
            }
        }
        internal static List<PlayMonoBehaviour> Behaviours = new List<PlayMonoBehaviour>();
        internal static IDictionary<string, IList<PlayMonoBehaviour>> EventBehaviours = new Dictionary<string, IList<PlayMonoBehaviour>>();
        internal static IDictionary<string, IList<PlayMonoBehaviour>> RPCBehaviours = new Dictionary<string, IList<PlayMonoBehaviour>>();

        /// <summary>
        /// register message noticitication.
        /// </summary>
        /// <param name="behaviour">PlayMonoBehaviour instance</param>
        public static void RegisterBehaviour(PlayMonoBehaviour behaviour)
        {
            if (Behaviours.Contains(behaviour)) return;
            Behaviours.Add(behaviour);
            RegisterBehaviour<PlayEventAttribute>(behaviour, EventBehaviours);
            RegisterBehaviour<PlayRPCAttribute>(behaviour, RPCBehaviours);
        }

        public static void UnregisterBehaviour(PlayMonoBehaviour behaviour)
        {
            if (!Behaviours.Contains(behaviour)) return;
            Behaviours.Remove(behaviour);
            UnregisterBehaviour(behaviour, EventBehaviours);
            UnregisterBehaviour(behaviour, RPCBehaviours);
        }

        internal static void RegisterBehaviour<T>(PlayMonoBehaviour behaviour, IDictionary<string, IList<PlayMonoBehaviour>> methodBehaviours)
                       where T : Attribute
        {
            var methods = Play.Find<T>(behaviour);
            methods.Every(m =>
            {
                if (methodBehaviours.ContainsKey(m.Name))
                {
                    var behaviours = methodBehaviours[m.Name];
                    if (behaviours == null)
                    {
                        behaviours = new List<PlayMonoBehaviour>();
                    }
                    behaviours.Add(behaviour);
                }
                else
                {
                    methodBehaviours.Add(m.Name, new List<PlayMonoBehaviour>() { behaviour });
                }
            });
        }

        internal static void UnregisterBehaviour(PlayMonoBehaviour behaviour, IDictionary<string, IList<PlayMonoBehaviour>> methodBehaviours)
        {
            var methodBehaviourListCollection = methodBehaviours.Values;
            methodBehaviourListCollection.Every(behaviours =>
            {
                if (behaviours.Contains(behaviour))
                {
                    behaviours.Remove(behaviour);
                }
            });
        }


        internal static object lisentersMutex = new object();
        internal static IList<IAVIMListener> lisenters = new List<IAVIMListener>();
        internal static void SubscribeNoticeReceived(IAVIMListener listener, Func<AVIMNotice, bool> runtimeHook = null)
        {
            //NoticeReceived += new EventHandler<AVIMNotice>((sender, notice) =>
            //{
            //    var approved = runtimeHook == null ? listener.ProtocolHook(notice) : runtimeHook(notice) && listener.ProtocolHook(notice);
            //    if (approved)
            //    {
            //        listener.OnNoticeReceived(notice);
            //    }
            //});
            lock (lisentersMutex)
            {
                lisenters.Add(listener);
            }

        }

        internal static void UnsubscribeNoticeReceived(IAVIMListener listener)
        {
            lock (lisentersMutex)
            {
                lisenters.Remove(listener);
            }
        }


        internal static void StartListen()
        {
            NoticeReceived += Play_NoticeReceived;
        }

        internal static void StopListen()
        {
            NoticeReceived -= Play_NoticeReceived;
        }

        private static object noticeMutext = new object();

        private static void Play_NoticeReceived(object sender, AVIMNotice e)
        {
            foreach (var listener in lisenters)
            {
                var approved = listener.ProtocolHook(e);
                if (approved)
                {
                    listener.OnNoticeReceived(e);
                    break;
                }
            }
        }
        #endregion

        #region command & http executor
        internal static void ExecuteHttp(HttpRequest httpRequest, Action<Tuple<HttpStatusCode, string>> done)
        {
            HttpExecutor.Execute(httpRequest, done);
        }

        internal static void RunHttpCommand(PlayCommand command, PlayEventCode eventCode = PlayEventCode.None, Action<PlayCommand, PlayResponse> done = null)
        {
            if (eventCode != PlayEventCode.None)
            {
                Play.InvokeEvent(eventCode);
            }
            var httpRequest = command.HttpEncode;
            Play.ExecuteHttp(httpRequest, (tuple =>
            {
                int statusCode = (int)HttpStatusCode.BadRequest;
                IDictionary<string, object> body = new Dictionary<string, object>();
                try
                {
                    body = Json.Parse(tuple.Item2) as IDictionary<string, object>;
                    statusCode = (int)tuple.Item1;
                }
                catch
                {

                }
                var response = new PlayResponse()
                {
                    Body = body as IDictionary<string, object>,
                    StatusCode = (int)tuple.Item1
                };

                if (response.IsSuccessful)
                {
                    LogCommand(command, response);
                    if (done != null)
                    {
                        done(command, response);
                    }
                }
                else
                {
                    LogCommand(command, response, printer: ErrorLogger);
                }

                if (eventCode != PlayEventCode.None)
                {
                    var next = PlayStateMachine.Next(eventCode, response);
                    if (response.IsSuccessful)
                    {
                        Play.InvokeEvent(next);
                    }
                    else
                    {
                        Play.InvokeEvent(next, response.ErrorCode, response.ErrorReason);
                    }
                }

            }));
        }

        internal static void RunSocketCommand(PlayCommand command, PlayEventCode eventCode = PlayEventCode.None, Action<PlayCommand, PlayResponse> done = null)
        {
            var encoded = command.SokcetEncode;
            Play.Log("socket=>" + encoded);
            Play.RoomConnection.Send(encoded);

            var repsonseListener = new SocketResponseListener()
            {
                Command = command,
                Done = done,
                EventCode = eventCode
            };

            Play.SubscribeNoticeReceived(repsonseListener);

            //Action<string> onMessage = null;

            //LogCommand(command, null, CommandType.WebSocket);
            //onMessage = (messgae) =>
            //{
            //	var messageJson = Json.Parse(messgae) as IDictionary<string, object>;
            //	if (messageJson.Keys.Contains("i"))
            //	{
            //		if (command.Body["i"].ToString() == messageJson["i"].ToString())
            //		{
            //			RoomConnection.OnMessage -= onMessage;
            //			var response = new PlayResponse(messageJson);
            //			LogCommand(null, response, CommandType.WebSocket);
            //			if (response.IsSuccessful)
            //			{
            //				if (done != null)
            //				{
            //					done(command, response);
            //				}
            //			}

            //			if (eventCode != PlayEventCode.None)
            //			{
            //				var next = PlayStateMachine.Next(eventCode, response);
            //				if (response.IsSuccessful)
            //				{
            //					Play.InvokeEvent(next);
            //				}
            //				else
            //				{
            //					Play.InvokeEvent(next, response.ErrorCode, response.ErrorReason);
            //				}
            //			}
            //		}
            //	}
            //};

            //RoomConnection.OnMessage += onMessage;
        }

        #endregion

        #region rpc

        /// <summary>
        /// invoke a remote precedure call.
        /// </summary>
        /// <param name="rpcMethodName">method name</param>
        /// <param name="targets"> <see cref="PlayRPCTargets"/>.</param>
        /// <param name="rpcParameters"></param>
        public static void RPC(string rpcMethodName, PlayRPCTargets targets, params object[] rpcParameters)
        {
            var rpcMessage = new PlayRpcMessage();
            rpcMessage.MethodName = rpcMethodName;
            rpcMessage.Paramters = rpcParameters;

            if (targets.IsIn(PlayRPCTargets.All, PlayRPCTargets.AllBuffered))
            {
                rpcMessage.Echo = true;
            }

            if (targets.IsIn(PlayRPCTargets.OthersBuffered, PlayRPCTargets.AllBuffered))
            {
                rpcMessage.Cached = true;
            }

            if (targets.IsIn(PlayRPCTargets.MasterClient))
            {
                rpcMessage.ToPeers = new List<string>()
                {
                    Room.MasterClientId,
                };
                if (Play.Player.UserID == Play.Room.MasterClientId)
                {
                    rpcMessage.Echo = true;
                }
            }

            Play.ExecuteRPC(rpcMessage);
        }

        /// <summary>
        /// call
        /// </summary>
        /// <param name="rpcMethodName"></param>
        /// <param name="userIds"></param>
        /// <param name="rpcParameters"></param>
        public static void RPC(string rpcMethodName, IEnumerable<string> userIds, params object[] rpcParameters)
        {
            var rpcMessage = new PlayRpcMessage();
            rpcMessage.MethodName = rpcMethodName;
            rpcMessage.Paramters = rpcParameters;
            rpcMessage.ToPeers = new List<string>(userIds);
            Play.ExecuteRPC(rpcMessage);
        }

        internal static void ExecuteRPC(PlayRpcMessage rpcMessage)
        {
            var rpcCommand = new PlayCommand();
            rpcCommand.Body = new Dictionary<string, object>();
            rpcCommand.Body["cmd"] = "direct";
            rpcCommand.Body["cid"] = Room.Name;
            rpcCommand.Body["msg"] = rpcMessage.Serialize();
            rpcCommand.Body["echo"] = rpcMessage.Echo;
            rpcCommand.Body["cached"] = rpcMessage.Cached;
            rpcCommand.Body["toPeerIds"] = rpcMessage.ToPeers;
            Play.RunSocketCommand(rpcCommand);
        }

        internal static void ReceivedRPC(PlayRpcMessage rpcMessage)
        {
            //EventNotification.NotifyUnityGameObjects(rpcMessage.MethodName, rpcMessage.Paramters.ToArray());
            var playRpcEventMessage = new PlayRPCEventMessage()
            {
                rpcMessage = rpcMessage
            };
#if UNITY
			EnqueueEventMessage(playRpcEventMessage);
#else
            var pt = rpcMessage.Paramters.Select(p => p.GetType()).ToArray();

            var rpcMethods = from behaviour in Play.Behaviours
                             let method = Play.Find(behaviour, rpcMessage.MethodName, pt)
                             where method != null
                             select new
                             {
                                 host = behaviour,
                                 method = method
                             };
            rpcMethods.Every(hm => hm.method.Invoke(hm.host, parameters: rpcMessage.Paramters.ToArray()));
#endif


        }
        #endregion

        #region operations notification

        internal static void RoomUpdated(IDictionary<string, object> transformation)
        {
            Play.Room.Updated(transformation);
            Play.InvokeEvent(PlayEventCode.OnRoomCustomPropertiesUpdated, transformation.ToHashtable());
        }

        internal static void MasterClientSwitch(string handler, string master)
        {
            //var handledPlayer = GetPlayer(handler);
            var newMasterPlayer = GetPlayer(master);
            Play.InvokeEvent(PlayEventCode.OnMasterClientSwitched, newMasterPlayer);
        }

        #region player join a room
        internal static void NewPlayerJoined(Player newPlayer)
        {
            Play.InvokeEvent(PlayEventCode.OnNewPlayerJoinedRoom, newPlayer);
        }

        internal static void PlayerOnline(Player player)
        {
            player.IsInactive = false;
            Play.InvokeEvent(PlayEventCode.OnPlayerActivityChanged, player);
        }

        internal static void PlayerOffline(Player player)
        {
            player.IsInactive = true;
            Play.InvokeEvent(PlayEventCode.OnPlayerActivityChanged, player);
        }

        internal static void PlayerRejoined(Player player)
        {
            Play.InvokeEvent(PlayEventCode.OnPlayerReconnectedRoom, player);
        }

        internal static void PlayerLeft(Player player)
        {
            Play.InvokeEvent(PlayEventCode.OnPlayerLeftRoom, player);
        }
        #endregion

        #endregion

        #region method register
        internal static IEnumerable<MethodInfo> Find<T>() where T : Attribute
        {
            Assembly currentAssem = Assembly.GetExecutingAssembly();
            return currentAssem.GetTypes()
                     .SelectMany(t => t.GetMethods())
                     .Where(m => m.GetCustomAttributes(typeof(T), false).Length > 0)
                     .ToArray();
        }
        internal static IEnumerable<MethodInfo> Find<T>(PlayMonoBehaviour host)
            where T : Attribute
        {
            return host.GetType().GetMethods()
                .Where(m => m.GetCustomAttributes(typeof(T), false).Length > 0)
                .ToArray();
        }

        internal static MethodInfo Find(PlayMonoBehaviour host, string methodName)
        {
            return host.GetType().GetMethod(methodName);
        }

        internal static MethodInfo Find<T>(PlayMonoBehaviour host, string methodName) where T : Attribute
        {
            return host.GetType().GetMethods()
               .Where(m => m.GetCustomAttributes(typeof(T), false).Length > 0 && m.Name == methodName).FirstOrDefault();
        }

        internal static MethodInfo Find(PlayMonoBehaviour host, string methodName, Type[] parameterTypes)
        {
            return host.GetType().GetMethod(methodName, parameterTypes);
        }

        #endregion

        #region random
        internal static string RandomHexString(int length = 32)
        {
            StringBuilder builder = new StringBuilder();
            Enumerable
               .Range(65, 26)
                .Select(e => ((char)e).ToString())
                .Concat(Enumerable.Range(97, 26).Select(e => ((char)e).ToString()))
                .Concat(Enumerable.Range(0, 10).Select(e => e.ToString()))
                .OrderBy(e => Guid.NewGuid())
                .Take(length)
                .ToList().ForEach(e => builder.Append(e));
            string id = builder.ToString();
            return id;
        }
        #endregion


        /// <summary>
        /// register subclass of PlaySynchronousObject
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public static void RegisterSynchronousObjectType<T>() where T : PlaySynchronousObject
        {
            PlayCorePlugins.Instance.SynchronousObjectSubclassController.RegisterSubclass(typeof(T));
        }

        // 关闭 WebSocket 连接
        public static void CloseConnect()
        {
            if (RoomConnection != null)
                RoomConnection.Close();
        }
    }
}
