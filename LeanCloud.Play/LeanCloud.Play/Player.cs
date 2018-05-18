using LeanCloud.Storage.Internal;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace LeanCloud
{

	/// <summary>
	/// Player
	/// </summary>
	[PlaySynchronizeObjectName("Player")]
	public class Player : PlaySynchronousObject
	{

		/// <summary>
		/// new a player instance
		/// </summary>
		public Player() : base()
		{
			this.ObjectName = "Player";
			this.objectState = new AVObject(this.ObjectName);

			fixedKeys = new Dictionary<string, string>() { };
			fixedKeys.Add("customProperties", "props");
		}

		internal Player(PlayPeer peer, PlayRoom room) : this()
		{
			Peer = peer;
			SetRoom(room);
		}

		internal Player(PlayPeer peer) : this(peer, null)
		{

		}

		internal Player(string userId, int actorId) : this(new PlayPeer(userId))
		{
			this.ActorID = actorId;
		}

		internal PlayRoom Room { get; set; }
		internal PlayPeer Peer { get; set; }


		internal void SetRoom(PlayRoom room)
		{
			Room = room;
			if (room != null)
			{
				this.ActorID = room.GetActorIdByUserId(this.UserID);
			}
		}

		private int actorID = -1;
		/// <summary>
		/// 
		/// </summary>
		public int ActorID
		{
			get
			{
				return actorID;
			}
			internal set
			{
				actorID = value;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		public string UserID
		{
			get
			{
				return Peer.ID;
			}
		}

		/// <summary>
		/// name of the room that contains current player.
		/// </summary>
		public string RoomName
		{
			get
			{
				return Room.Name;
			}
		}


		/// <summary>
		/// 
		/// </summary>
		public bool IsMasterClient
		{
			get
			{
				return Room.MasterClientId == this.Peer.ID;
			}
		}

		/// <summary>
		/// judge the player is current player or not.
		/// </summary>
		public bool IsLocal
		{
			get
			{
				return UserID == Play.Player.UserID;
			}
		}


		internal override void Save(IDictionary<string, object> increment)
		{
			var updateCommand = new PlayCommand()
			{
				Body = new Dictionary<string, object>()
				{
					{ "cmd", "conv" },
					{ "op", "update-player-prop" },
					{ "cid", Play.Room.Name },
					{ "targetClientId",this.UserID },
					{ "playerProperty", increment }
				}
			};

			Play.RunSocketCommand(updateCommand);
		}

		private IEnumerable<IDictionary<string, object>> GrabPlayerProperties(PlayResponse response)
		{
			return response.Body as IList<IDictionary<string, object>>;
		}

		internal bool IsNew
		{
			get
			{
				return string.IsNullOrEmpty(this.PropertyObejctId);
			}
		}

		internal string PropertyObejctId
		{
			get
			{
				return this.objectState.ObjectId;
			}
		}
	}
}