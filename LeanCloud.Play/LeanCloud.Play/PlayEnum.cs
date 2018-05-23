using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeanCloud
{
	/// <summary>
	/// Play RPCT argets.
	/// </summary>
	public enum PlayRPCTargets
	{
		/// <summary>
		/// all via server
		/// </summary>
		All,
		/// <summary>
		/// others via server
		/// </summary>
		Others,
		/// <summary>
		/// if current player is master client, will receive from server
		/// </summary>
		MasterClient,
		/// <summary>
		/// all via server and will cached it.
		/// </summary>
		AllBuffered,
		/// <summary>
		/// others via server and will cached it.
		/// </summary>
		OthersBuffered,
	}

	/// <summary>
	/// Play event code.
	/// </summary>
	public enum PlayEventCode
	{
		/// <summary>
		/// none.
		/// </summary>
		None = 0,
		#region authentication
		/// <summary>
		/// on authenticat failed.
		/// </summary>
		OnAuthenticatFailed = -1002,
		/// <summary>
		/// on authenticating.
		/// </summary>
		OnAuthenticating = 1001,
		/// <summary>
		/// on authenticated.
		/// </summary>
		OnAuthenticated = 1002,
		#endregion

		#region create room
		/// <summary>
		/// on create room failed.
		/// </summary>
		OnCreateRoomFailed = -2002,
		/// <summary>
		/// on creating room.
		/// </summary>
		OnCreatingRoom = 2001,
		/// <summary>
		/// on created room.
		/// </summary>
		OnCreatedRoom = 2002,
		#endregion

		#region join room
		/// <summary>
		/// on join room failed.
		/// </summary>
		OnJoinRoomFailed = -2003,
		/// <summary>
		/// on joining room.
		/// </summary>
		OnJoiningRoom = 2003,
		/// <summary>
		/// on joined room.
		/// </summary>
		OnJoinedRoom = 2004,
		#endregion

		#region rejoin room
		//OnRejoinRoomFailed = -2006,
		//OnRejoiningRoom = 2005,
		//OnRejoinedRoom = 2006,
		#endregion

		#region create or join
		/// <summary>
		/// on join or create room failed.
		/// </summary>
		OnJoinOrCreateRoomFailed = -2008,

		/// <summary>
		/// on join or creating room.
		/// </summary>
		OnJoinOrCreatingRoom = 2007,

		/// <summary>
		/// join or created room.
		/// </summary>
		OnJoinOrCreatedRoom = 2008,
		#endregion

		#region random join room
		/// <summary>
		/// random join room failed.
		/// </summary>
		OnRandomJoinRoomFailed = -2010,
		/// <summary>
		/// on random joining room.
		/// </summary>
		OnRandomJoiningRoom = 2009,
		/// <summary>
		/// on random joined room.
		/// </summary>
		OnRandomJoinedRoom = 2010,
		#endregion

		#region
		/// <summary>
		/// on leave room failed.
		/// </summary>
		OnLeaveRoomFailed = -2012,
		/// <summary>
		/// on leaving room.
		/// </summary>
		OnLeavingRoom = 2011,
		/// <summary>
		/// on left room.
		/// </summary>
		OnLeftRoom = 2012,
		#endregion

		#region room list
		/// <summary>
		/// on fetch room list failed.
		/// </summary>
		OnFetchRoomListFailed = -2014,

		/// <summary>
		/// on fetching room list.
		/// </summary>
		OnFetchingRoomList = 2013,
		/// <summary>
		/// on fetched room list.
		/// </summary>
		OnFetchedRoomList = 2014,
		#endregion

		#region
		/// <summary>
		/// on connect failed.
		/// </summary>
		OnConnectFailed = -8002,
		/// <summary>
		/// on connecting.
		/// </summary>
		OnConnecting = 8001,
		/// <summary>
		/// on connected.
		/// </summary>
		OnConnected = 8002,
		#endregion

		#region 
		/// <summary>
		/// on disconnect failed.
		/// </summary>
		OnDisconnectFailed = -8004,
		/// <summary>
		/// on disconneting.
		/// </summary>
		OnDisconneting = 8003,
		/// <summary>
		/// on disconnected.
		/// </summary>
		OnDisconnected = 8004,
		#endregion

		#region
		/// <summary>
		/// on RPC send failed.
		/// </summary>
		OnRPCSendFailed = -9002,
		/// <summary>
		/// on RPC sending.
		/// </summary>
		OnRPCSending = 9001,
		/// <summary>
		/// on RPC sent.
		/// </summary>
		OnRPCSent = 9002,
		#endregion

		/// <summary>
		/// The on RPC received.
		/// </summary>
		OnRPCReceived = 9003,


		#region room property begin with a prefix 21
		/// <summary>
		/// on room custom properties update failed.
		/// </summary>
		OnRoomCustomPropertiesUpdateFailed = -2102,
		/// <summary>
		/// on room custom properties updating.
		/// </summary>
		OnRoomCustomPropertiesUpdating = 2101,
		/// <summary>
		/// on room custom properties updated.
		/// </summary>
		OnRoomCustomPropertiesUpdated = 2102,
		#endregion

		#region players in room
		/// <summary>
		/// on new player joined room.
		/// </summary>
		OnNewPlayerJoinedRoom = 2201,
		/// <summary>
		/// on player connected room.
		/// </summary>
		OnPlayerConnectedRoom = 2202,
		/// <summary>
		/// on player disconnected room.
		/// </summary>
		OnPlayerDisconnectedRoom = 2203,
		/// <summary>
		/// on player reconnected room.
		/// </summary>
		OnPlayerReconnectedRoom = 2204,

		/// <summary>
        /// on player IsInactive changed.
        /// </summary>
		OnPlayerActivityChanged = 2206,

		/// <summary>
		/// on player left room.
		/// </summary>
		OnPlayerLeftRoom = 2207,
		#endregion

		#region on master client switched
		/// <summary>
		/// on master client switched.
		/// </summary>
		OnMasterClientSwitched = 2301,
		#endregion

		#region find friends
		/// <summary>
		/// on find friends failed.
		/// </summary>
		OnFindFriendsFailed = -3002,
		/// <summary>
		/// on finding friends.
		/// </summary>
		OnFindingFriends = 3001,
		/// <summary>
		/// on friends found.
		/// </summary>
		OnFriendsFound = 3002,
		#endregion

		#region properties changed
		/// <summary>
		/// on player custom properties changed.
		/// </summary>
		OnPlayerCustomPropertiesChanged = 3101,
		//OnCustomPropertiesUpdateFailed = -3004,
		//OnCustomPropertiesUpdating = 3003,
		//OnCustomPropertiesUpdated = 3004,
		#endregion

		#region properties fetch
		/// <summary>
		/// on properties fetch failed.
		/// </summary>
		OnPropertiesFetchFailed = -3006,
		/// <summary>
		/// on properties fetching.
		/// </summary>
		OnPropertiesFetching = 3005,
		/// <summary>
		/// on properties fetched.
		/// </summary>
		OnPropertiesFetched = 3006,
		#endregion

		#region 
		#endregion
	}
}
