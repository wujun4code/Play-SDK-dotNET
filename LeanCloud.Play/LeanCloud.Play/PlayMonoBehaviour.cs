using System.Collections;
using System.Collections.Generic;

#if UNITY
using UnityEngine;
#endif

namespace LeanCloud
{
	/// <summary>
	/// PlayMonoBehaviour
	/// </summary>
	public class PlayMonoBehaviour : MonoBehaviour
	{
#if !UNITY
		/// <summary>
		/// only for unit test in windows or mono.
		/// </summary>
		public PlayMonoBehaviour()
		{
			Play.RegisterBehaviour(this);
		}
#endif
        /// <summary>
        /// Awake this instance.
        /// </summary>
		protected virtual void Awake()
		{
			Play.RegisterBehaviour(this);
		}

        /// <summary>
        /// On the destroy.
        /// </summary>
		protected virtual void OnDestroy()
		{
			Play.UnregisterBehaviour(this);
		}

		#region authentication
        /// <summary>
        /// On the authenticat failed.
        /// </summary>
		public virtual void OnAuthenticatFailed()
		{

		}
        /// <summary>
        /// On the authenticating.
        /// </summary>
		public virtual void OnAuthenticating()
		{

		}
        /// <summary>
        /// On the authenticated.
        /// </summary>
		public virtual void OnAuthenticated() { }
		#endregion

		#region create room
        /// <summary>
        /// On the create room failed.
        /// </summary>
        /// <param name="errorCode">Error code.</param>
        /// <param name="reason">Reason.</param>
		public virtual void OnCreateRoomFailed(int errorCode, string reason)
		{

		}
        /// <summary>
        /// On the creating room.
        /// </summary>
		public virtual void OnCreatingRoom() { }

        /// <summary>
        /// On the created room.
        /// </summary>
		public virtual void OnCreatedRoom() { }
		#endregion

		#region join room
        /// <summary>
        /// On the join room failed.
        /// </summary>
        /// <param name="errorCode">Error code.</param>
        /// <param name="reason">Reason.</param>
		public virtual void OnJoinRoomFailed(int errorCode, string reason) { }
        /// <summary>
        /// On the joining room.
        /// </summary>
		public virtual void OnJoiningRoom() { }
        /// <summary>
        /// On the joined room.
        /// </summary>
		public virtual void OnJoinedRoom() { }
		#endregion

		#region random join
        /// <summary>
        /// on the random join room failed.
        /// </summary>
        /// <param name="errorCode">Error code.</param>
        /// <param name="reason">Reason.</param>
		public virtual void OnRandomJoinRoomFailed(int errorCode, string reason) { }

        /// <summary>
        /// on the random joining room.
        /// </summary>
		public virtual void OnRandomJoiningRoom() { }
		#endregion

		#region join or create
        /// <summary>
        /// on the join or create room failed.
        /// </summary>
		public virtual void OnJoinOrCreateRoomFailed() { }

        /// <summary>
        /// on the join or creating room.
        /// </summary>
		public virtual void OnJoinOrCreatingRoom() { }
        /// <summary>
        /// on the join or created room.
        /// </summary>
		public virtual void OnJoinOrCreatedRoom() { }
		#endregion

		#region rejoin room
        /// <summary>
        /// on the rejoin room failed.
        /// </summary>
        /// <param name="errorCode">Error code.</param>
        /// <param name="reason">Reason.</param>
		//public virtual void OnRejoinRoomFailed(int errorCode, string reason) { }

        /// <summary>
        /// on the rejoining room.
        /// </summary>
		//public virtual void OnRejoiningRoom() { }

        /// <summary>
        /// on the rejoined room.
        /// </summary>
		//public virtual void OnRejoinedRoom() { }
		#endregion

		#region
        /// <summary>
        /// on the leave room failed.
        /// </summary>
		public virtual void OnLeaveRoomFailed() { }
        /// <summary>
        /// on the leaving room.
        /// </summary>
		public virtual void OnLeavingRoom() { }

        /// <summary>
        /// on the left room.
        /// </summary>
		public virtual void OnLeftRoom() { }
		#endregion

		#region
        /// <summary>
        /// on the connect failed.
        /// </summary>
		public virtual void OnConnectFailed() { }
        /// <summary>
        /// on the connecting.
        /// </summary>
		public virtual void OnConnecting() { }
        /// <summary>
        /// on the connected.
        /// </summary>
		public virtual void OnConnected() { }
		#endregion

		#region 
        /// <summary>
        /// on the disconnect failed.
        /// </summary>
		public virtual void OnDisconnectFailed() { }
        /// <summary>
        /// on the disconneting.
        /// </summary>
		public virtual void OnDisconneting() { }
        /// <summary>
        /// on the disconnected.
        /// </summary>
		public virtual void OnDisconnected() { }
		#endregion

		#region
		//public virtual void OnRPCSendFailed() { }
		//public virtual void OnRPCSending() { }
		//public virtual void OnRPCSent() { }
		#endregion

		#region
        /// <summary>
        /// on the room custom properties update failed.
        /// </summary>
        /// <param name="errorCode">Error code.</param>
        /// <param name="reason">Reason.</param>
		public virtual void OnRoomCustomPropertiesUpdateFailed(int errorCode, string reason) { }
        
        /// <summary>
        /// on the room custom properties updating.
        /// </summary>
		public virtual void OnRoomCustomPropertiesUpdating() { }

        /// <summary>
        /// on the room custom properties updated.
        /// </summary>
        /// <param name="updatedProperties">Updated properties.</param>
		public virtual void OnRoomCustomPropertiesUpdated(Hashtable updatedProperties) { }
		#endregion

		#region
        /// <summary>
        /// On the new player joined room.
        /// </summary>
        /// <param name="player">Player.</param>
		public virtual void OnNewPlayerJoinedRoom(Player player) { }


		//public virtual void OnPlayerConnectedRoom(Player player) { }
		//public virtual void OnPlayerDisconnectedRoom(Player player) { }
		//public virtual void OnPlayerReconnectedRoom(Player player) { }

        /// <summary>
        /// On the player left room.
        /// </summary>
        /// <param name="player">Player.</param>
		public virtual void OnPlayerLeftRoom(Player player) { }

        /// <summary>
        /// On the player activity changed.
        /// </summary>
        /// <param name="player">Player.</param>
		public virtual void OnPlayerActivityChanged(Player player) { }
		#endregion

        /// <summary>
        /// On the master client switched.
        /// </summary>
        /// <param name="masterPlayer">Master player.</param>
		public virtual void OnMasterClientSwitched(Player masterPlayer) { }

        /// <summary>
        /// On the friends found.
        /// </summary>
		public virtual void OnFriendsFound() { }

        /// <summary>
        /// On the player custom properties changed.
        /// </summary>
        /// <param name="player">Player.</param>
        /// <param name="updatedProperties">Updated properties.</param>
		public virtual void OnPlayerCustomPropertiesChanged(Player player, Hashtable updatedProperties)
		{

		}

		//public virtual void OnCustomPropertiesUpdateFailed(int errorCode, string reason) { }

		//public virtual void OnCustomPropertiesUpdated() { }

		//public virtual void OnCustomPropertiesUpdating() { }

        /// <summary>
        /// On the properties fetch failed.
        /// </summary>
        /// <param name="errorCode">Error code.</param>
        /// <param name="reason">Reason.</param>
		public virtual void OnPropertiesFetchFailed(int errorCode, string reason) { }

        /// <summary>
        /// On the properties fetching.
        /// </summary>
		public virtual void OnPropertiesFetching() { }

        /// <summary>
        /// On the properties fetched.
        /// </summary>
		public virtual void OnPropertiesFetched() { }

        /// <summary>
        /// On the fetch room list failed.
        /// </summary>
        /// <param name="errorCode">Error code.</param>
        /// <param name="reason">Reason.</param>
		public virtual void OnFetchRoomListFailed(int errorCode, string reason) { }

        /// <summary>
        /// On the fetching room list.
        /// </summary>
		public virtual void OnFetchingRoomList() { }

        /// <summary>
        /// On the fetched room list.
        /// </summary>
        /// <param name="rooms">Rooms.</param>
		public virtual void OnFetchedRoomList(IEnumerable<PlayRoom> rooms) { }
	}
}
