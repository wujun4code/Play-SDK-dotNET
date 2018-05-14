using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        /// <summary>
        /// create a instance of PlayMonoBehaviour
        /// </summary>
        public PlayMonoBehaviour()
        {
            Play.RegisterBehaviour(this);
        }

        #region authentication
        public virtual void OnAuthenticatFailed()
        {

        }
        public virtual void OnAuthenticating()
        {

        }
        public virtual void OnAuthenticated() { }
        #endregion

        #region create room
        public virtual void OnCreateRoomFailed(int errorCode, string reason)
        {

        }
        public virtual void OnCreatingRoom() { }
        public virtual void OnCreatedRoom() { }
        #endregion

        #region join room
        public virtual void OnJoinRoomFailed(int errorCode, string reason) { }
        public virtual void OnJoiningRoom() { }
        public virtual void OnJoinedRoom() { }
        #endregion

        #region join or create
        public virtual void OnJoinOrCreateRoomFailed() { }
        public virtual void OnJoinOrCreatingRoom() { }
        public virtual void OnJoinOrCreatedRoom() { }
        #endregion

        #region rejoin room
		public virtual void OnRejoinRoomFailed(int errorCode, string reason ) { }
        public virtual void OnRejoiningRoom() { }
        public virtual void OnRejoinedRoom() { }
        #endregion

        #region
        public virtual void OnLeaveRoomFailed() { }
        public virtual void OnLeavingRoom() { }
        public virtual void OnLeftRoom() { }
        #endregion

        #region
        public virtual void OnConnectFailed() { }
        public virtual void OnConnecting() { }
        public virtual void OnConnected() { }
        #endregion

        #region 
        public virtual void OnDisconnectFailed() { }
        public virtual void OnDisconneting() { }
        public virtual void OnDisconnected() { }
        #endregion

        #region
        //public virtual void OnRPCSendFailed() { }
        //public virtual void OnRPCSending() { }
        //public virtual void OnRPCSent() { }
        #endregion

        #region
        public virtual void OnRoomCustomPropertiesUpdateFailed(int errorCode, string reason) { }
        public virtual void OnRoomCustomPropertiesUpdating() { }
        public virtual void OnRoomCustomPropertiesUpdated(Hashtable updatedProperties) { }
        #endregion

        #region
        public virtual void OnNewPlayerJoinedRoom(Player player) { }
        public virtual void OnPlayerConnectedRoom(Player player) { }
        public virtual void OnPlayerDisconnectedRoom(Player player) { }
        public virtual void OnPlayerReconnectedRoom(Player player) { }
        public virtual void OnPlayerLeftRoom(Player player) { }
        #endregion

        public virtual void OnMasterClientSwitched(Player masterPlayer) { }

        public virtual void OnFriendsFound() { }

        public virtual void OnPlayerCustomPropertiesChanged(Player player, Hashtable updatedProperties)
        {

        }

        //public virtual void OnCustomPropertiesUpdateFailed(int errorCode, string reason) { }

        //public virtual void OnCustomPropertiesUpdated() { }

        //public virtual void OnCustomPropertiesUpdating() { }

        public virtual void OnPropertiesFetchFailed(int errorCode, string reason) { }
        public virtual void OnPropertiesFetching() { }
        public virtual void OnPropertiesFetched() { }

        public virtual void OnFetchRoomListFailed(int errorCode, string reason) { }
        public virtual void OnFetchingRoomList() { }
        public virtual void OnFetchedRoomList(IEnumerable<PlayRoom> rooms) { }
    }
}
