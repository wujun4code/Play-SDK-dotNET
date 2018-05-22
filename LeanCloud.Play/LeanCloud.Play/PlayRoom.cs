using LeanCloud.Core.Internal;
using LeanCloud;
using LeanCloud.Realtime.Internal;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace LeanCloud
{
	/// <summary>
	/// 
	/// </summary>
	[PlaySynchronizeObjectName("_PlayRoom")]
	public class PlayRoom : PlaySynchronousObject
	{

		/// <summary>
		/// room config before create it.
		/// </summary>
		public struct RoomConfig
		{
			/// <summary>
			/// set a custom properties for room before create it.
			/// </summary>
			public Hashtable CustomRoomProperties { get; set; }

			/// <summary>
			/// the keys in CustomProperties to match a room in lobby.
			/// </summary>
			public IEnumerable<string> LobbyMatchKeys { get; set; }

			/// <summary>
			/// expected user userId(s)
			/// </summary>
			public IEnumerable<string> ExpectedUsers { get; set; }

			/// <summary>
			/// max player count in a Room
			/// </summary>
			public byte MaxPlayerCount { get; set; }

			/// <summary>
			/// visible in lobby
			/// </summary>
			public bool IsVisible { get; set; }
			/// <summary>
			/// open to join and find
			/// </summary>
			public bool IsOpen { get; set; }

			/// <summary>
			/// empty time to live for the Room in seconds. max value is 1800(seconds).
			/// </summary>
			public int EmptyTimeToLive { get; set; }

			/// <summary>
			/// time to keep a player when disconnect from room. max value is 60(seconds).
			/// </summary>
			public int PlayerTimeToKeep { get; set; }

			/// <summary>
			/// deafult config
			/// </summary>
			public static RoomConfig Default
			{
				get
				{
					return new RoomConfig()
					{
						IsOpen = true,
						IsVisible = true
					};
				}
			}
		}

		/// <summary>
		/// room config
		/// </summary>
		public RoomConfig Config { get; set; }


		/// <summary>
		/// room remote game server address with security.
		/// </summary>
		internal string RoomRemoteSecureAddress { get; set; }
		/// <summary>
		/// room remote game server.
		/// </summary>
		internal string RoomRemoteAddress { get; set; }

		/// <summary>
		/// expected user userId
		/// </summary>
		public IEnumerable<string> ExpectedUsers { get; set; }

		/// <summary>
		/// the keys in CustomProperties to match a room in lobby.
		/// </summary>
		public IEnumerable<string> LobbyMatchKeys { get; set; }

		/// <summary>
		/// current master user userId.
		/// </summary>
		public string MasterClientId
		{
			get
			{
				return this.objectState["masterClientId"] as string;
			}
		}


		internal IDictionary<string, Player> playerMap;

		/// <summary>
		/// players in Room.
		/// </summary>
		public IEnumerable<Player> Players
		{
			get
			{
				return ComputedPlayers;
			}
		}

		internal List<string> MemberUserIds
		{
			get
			{
				var objList = GetValue<List<object>>("m");
				if (objList != null)
					return objList.OfType<string>().ToList();
				return null;
			}
			set
			{
				this["m"] = value;
			}
		}

		internal List<int> MemberActorIds
		{
			get
			{
				var objList = GetValue<List<object>>("memberIds");
				if (objList != null)
					return objList.OfType<int>().ToList();
				return null;
			}
			set
			{
				this["memberIds"] = value;
			}
		}

		internal List<Player> ComputedPlayers
		{
			get
			{
				return this.playerMap.Select(kv => kv.Value).ToList();
			}
		}

		/// <summary>
		/// default constructor
		/// </summary>
		public PlayRoom() : base()
		{
			this.ObjectName = "_PlayRoom";
			this.objectState = new AVObject(this.ObjectName);

			fixedKeys = new Dictionary<string, string>() { };
			fixedKeys.Add("cid", "roomId");
			fixedKeys.Add("room_id", "roomId");
			fixedKeys.Add("master_client_id", "masterClientId");
			fixedKeys.Add("member_ids", "memberIds");
			fixedKeys.Add("attr", "props");

			playerMap = new Dictionary<string, Player>();
		}

		/// <summary>
		/// PlayRoom
		/// </summary>
		/// <param name="name">room name</param>
		public PlayRoom(string name)
			: this()
		{
			this.Name = name;
		}

		internal PlayRoom(RoomConfig config, string name) : this(name)
		{
			this.Config = config;

			this.LobbyMatchKeys = config.LobbyMatchKeys;
			this.MaxPlayerCount = config.MaxPlayerCount;
			this.SetProperty<bool>(config.IsOpen, "IsOpen");
			this.SetProperty<bool>(config.IsVisible, "IsVisible");
			this.ExpectedUsers = config.ExpectedUsers;
		}

		/// <summary>
		/// max player count in a Room
		/// </summary>
		public byte MaxPlayerCount { get; set; }

		/// <summary>
		/// 
		/// </summary>
		public static byte DefaultMaxPlayerCount = 10;

		/// <summary>
		/// 
		/// </summary>
		public static int DefaultMaxEmptyTimeToLive = 1800;


		/// <summary>
		/// 
		/// </summary>
		public static int DefaultMaxKeepPlayerTime = 60;

		/// <summary>
		/// global unqiue userId of Room.
		/// </summary>
		[PlaySynchronizeProperty("roomId")]
		public string Name
		{
			get
			{
				return this.GetProperty<string>(null, "Name");
			}
			set
			{
				this.SetProperty<string>(value, "Name");
			}
		}

		/// <summary>
		/// 
		/// </summary>
		[PlaySynchronizeProperty("visible")]
		public bool IsVisible
		{
			get
			{
				return this.GetProperty<bool>(true, "IsVisible");
			}
			set
			{
				this.SetProperty<bool>(value, "IsVisible");
				var cmd = new PlayCommand()
				{
					Body = new Dictionary<string, object>()
					{
						{ "cmd", "conv"},
						{ "op", "visible"},
						{ "toggle", value}
					}
				};
				Play.RunSocketCommand(cmd);
			}
		}

		public void SetVisible(bool visible)
		{
			this.SetProperty<bool>(visible, "IsVisible");
		}

		/// <summary>
		/// 
		/// </summary>
		[PlaySynchronizeProperty("open")]
		public bool IsOpen
		{
			get
			{
				return this.GetProperty<bool>(true, "IsOpen");
			}
			set
			{
				this.SetProperty<bool>(value, "IsOpen");
				var cmd = new PlayCommand()
				{
					Body = new Dictionary<string, object>()
					{
						{ "cmd", "conv"},
						{ "op", "open"},
						{ "toggle", value}
					}
				};
				Play.RunSocketCommand(cmd);
			}
		}
        
		public void SetOpened(bool opened)
		{
			this.SetProperty<bool>(opened, "IsOpen");
		}


		/// <summary>
		/// 
		/// </summary>
		/// <param name="propertiesToUpdateOrSet"></param>
		/// <param name="expectedValues"></param>
		public void SetCustomProperties(Hashtable propertiesToUpdateOrSet, Hashtable expectedValues = null)
		{
			this.Save(propertiesToUpdateOrSet, expectedValues);
		}

		/// <summary>
		/// empty time to live for the Room in seconds. max value is 1800(seconds).
		/// </summary>
		public int EmptyTimeToLive { get; set; }

		/// <summary>
		/// time to keep a player when disconnect from room. max value is 60(seconds).
		/// </summary>
		public int PlayerTimeToKeep { get; set; }

		internal override void Save(IDictionary<string, object> increment)
		{
			this.Save(increment.ToHashtable());
		}

		internal void Save(Hashtable propertiesToUpdateOrSet = null, Hashtable expectedValues = null)
		{
			if (this.Name == null) return;
			var updateCommand = new PlayCommand();
			updateCommand.Body = new Dictionary<string, object>();
			updateCommand.Body["cmd"] = "conv";
			updateCommand.Body["cid"] = this.Name;
			updateCommand.Body["op"] = "update";

			if (expectedValues != null)
			{
				if (propertiesToUpdateOrSet != null)
				{
					IDictionary<string, object> casAttr = new Dictionary<string, object>();
					expectedValues.Every(kv =>
					{
						var key = kv.Key;

						if (propertiesToUpdateOrSet.ContainsKey(key))
						{
							IDictionary<string, object> cas = new Dictionary<string, object>();
							var toUpdateValue = propertiesToUpdateOrSet[key];
							cas.Add("expect", kv.Value);
							cas.Add("value", toUpdateValue);
							casAttr.Add(key.ToString(), cas);
						}
					});

					updateCommand.Body["casAttr"] = casAttr;
				}

			}
			else
			{
				if (propertiesToUpdateOrSet != null)
				{
					updateCommand.Body["attr"] = propertiesToUpdateOrSet.ToDictionary();
				}
				else
				{
					updateCommand.Body["attr"] = this.EncodeAttributes();
				}

			}

			Play.RunSocketCommand(updateCommand, done: (request, response) =>
			{
				lock (this.metaDataMutex)
				{
					response.Body.GetValue<IDictionary<string, object>>("attr", null);
					IDictionary<string, object> attr = response.Body.GetValue<IDictionary<string, object>>("attr", null);
					this.CustomPropertiesMetaData.Merge(attr);
				}

			});
			base.Save();
		}

		internal static string RandomRoomName(int length = 32)
		{
			return Play.RandomHexString(length);
		}

		internal void Updated(IDictionary<string, object> transformation)
		{
			lock (this.metaDataMutex)
			{
				this.CustomPropertiesMetaData.Merge(transformation);
			}
		}

		internal void AddPlayer(Player player)
		{
			lock (metaDataMutex)
			{
				if (!playerMap.ContainsKey(player.UserID))
				{
					this.objectState.AddToList("m", player.UserID);
					this.objectState.AddToList("memberIds", player.ActorID);
					player.Room = this;
					playerMap.Add(new KeyValuePair<string, Player>(player.UserID, player));
				}
				else
				{
					// merge player properties.
				}
			}
		}

		internal void RemovePlayer(string playerUserId)
		{
			lock (metaDataMutex)
			{
				this.objectState.RemoveElement<object>("m", playerUserId);
				//this.objectState.RemoveAllFromList("m", new object[] { playerUserId });
				var actorId = this.UserIndexer[playerUserId].ActorID;
				this.objectState.RemoveElement<object>("memberIds", actorId);
				//his.objectState.RemoveAllFromList("memberIds", new object[] { actorId });
				this.playerMap.Remove(playerUserId);
			}
		}

		internal Player GetPlayer(string playerUserId)
		{
			lock (metaDataMutex)
			{
				if (this.playerMap.ContainsKey(playerUserId))
					return this.playerMap[playerUserId];
				return null;
			}
		}

		private PlayIndexer<Player, int> _actorIndexer;

		internal PlayIndexer<Player, int> ActorIndexer
		{
			get
			{
				if (_actorIndexer == null)
				{
					_actorIndexer = new PlayIndexer<Player, int>();
					_actorIndexer.Filter = (player, actorId) =>
					{
						return player.ActorID == actorId;
					};
				}
				_actorIndexer.metaList = this.ComputedPlayers;
				return _actorIndexer;
			}
		}

		private PlayIndexer<Player, string> _userIndexer;
		internal PlayIndexer<Player, string> UserIndexer
		{
			get
			{
				if (_userIndexer == null)
				{
					_userIndexer = new PlayIndexer<Player, string>();
					_userIndexer.Filter = (player, userId) =>
					{
						return player.UserID == userId;
					};
				}
				_userIndexer.metaList = this.ComputedPlayers;
				return _userIndexer;
			}
		}

		internal void SetProperties(IDictionary<string, object> metaData)
		{
			lock (metaDataMutex)
			{
				this.SetServerData(metaData);
				this.RebuildPlayerMap();
			}
		}

		internal int GetActorIdByUserId(string userId)
		{
			var index = MemberUserIds.IndexOf(userId);
			if (index > -1)
			{
				return MemberActorIds[index];
			}
			return -1;
		}

		internal void RebuildPlayerMap()
		{
			this.playerMap = new Dictionary<string, Player>();
			MemberUserIds.Every(userId =>
			{
				if (userId == Play.Player.UserID)
				{
					Play.Player.SetRoom(this);
					playerMap.Add(userId, Play.Player);
				}
				else
				{
					playerMap.Add(userId, new Player(new PlayPeer(userId), this));
				}
			});
		}
	}
}
