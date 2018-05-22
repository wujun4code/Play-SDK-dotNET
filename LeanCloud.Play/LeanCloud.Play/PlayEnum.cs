using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeanCloud
{
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

	public enum PlayEventCode
	{
		None = 0,
		#region authentication
		OnAuthenticatFailed = -1002,
		OnAuthenticating = 1001,
		OnAuthenticated = 1002,
		#endregion

		#region create room
		OnCreateRoomFailed = -2002,
		OnCreatingRoom = 2001,
		OnCreatedRoom = 2002,
		#endregion

		#region join room
		OnJoinRoomFailed = -2003,
		OnJoiningRoom = 2003,
		OnJoinedRoom = 2004,
		#endregion

		#region rejoin room
		//OnRejoinRoomFailed = -2006,
		//OnRejoiningRoom = 2005,
		//OnRejoinedRoom = 2006,
		#endregion

		#region create or join
		OnJoinOrCreateRoomFailed = -2008,
		OnJoinOrCreatingRoom = 2007,
		OnJoinOrCreatedRoom = 2008,
		#endregion

		#region random join room
		OnRandomJoinRoomFailed = -2010,
		OnRandomJoiningRoom = 2009,
		OnRandomJoinedRoom = 2010,
		#endregion

		#region
		OnLeaveRoomFailed = -2012,
		OnLeavingRoom = 2011,
		OnLeftRoom = 2012,
		#endregion

		#region room list
		OnFetchRoomListFailed = -2014,
		OnFetchingRoomList = 2013,
		OnFetchedRoomList = 2014,
		#endregion

		#region
		OnConnectFailed = -8002,
		OnConnecting = 8001,
		OnConnected = 8002,
		#endregion

		#region 
		OnDisconnectFailed = -8004,
		OnDisconneting = 8003,
		OnDisconnected = 8004,
		#endregion

		#region
		OnRPCSendFailed = -9002,
		OnRPCSending = 9001,
		OnRPCSent = 9002,
		#endregion

		OnRPCReceived = 9003,


		#region room property begin with a prefix 21
		OnRoomCustomPropertiesUpdateFailed = -2102,
		OnRoomCustomPropertiesUpdating = 2101,
		OnRoomCustomPropertiesUpdated = 2102,
		#endregion

		#region players in room
		OnNewPlayerJoinedRoom = 2201,
		OnPlayerConnectedRoom = 2202,
		OnPlayerDisconnectedRoom = 2203,
		OnPlayerReconnectedRoom = 2204,
		OnPlayerLeftRoom = 2205,
		#endregion

		#region on master client switched
		OnMasterClientSwitched = 2301,
		#endregion

		#region find friends
		OnFindFriendsFailed = -3002,
		OnFindingFriends = 3001,
		OnFriendsFound = 3002,
		#endregion

		#region properties changed
		OnPlayerCustomPropertiesChanged = 3101,
		//OnCustomPropertiesUpdateFailed = -3004,
		//OnCustomPropertiesUpdating = 3003,
		//OnCustomPropertiesUpdated = 3004,
		OnPlayerActivityChanged = 3105,
		#endregion

		#region properties fetch
		OnPropertiesFetchFailed = -3006,
		OnPropertiesFetching = 3005,
		OnPropertiesFetched = 3006,
		#endregion
        
		#region 

		#endregion
	}
}
