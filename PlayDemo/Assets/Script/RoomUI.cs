using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using LeanCloud;

public class RoomUI : PlayMonoBehaviour {
	public InputField roomIdInputField = null;

	// 点击事件
	public void onCreateRoomBtnClicked() {
		string roomId = roomIdInputField.text;
		if (string.IsNullOrEmpty(roomId)) {
			Debug.Log("room id is null");
			return;
		}

        Debug.Log("creating room...");
		GlobalUI.Instantce.ShowLoading();
        PlayRoom room = new PlayRoom(roomId);
        room.MaxPlayerCount = 4;
        Play.CreateRoom(room);
	}

	public void onJoinRoomBtnClicked() {
		string roomId = roomIdInputField.text;
		if (string.IsNullOrEmpty(roomId)) {
			Debug.Log("room id is null");
			return;
		}

		GlobalUI.Instantce.ShowLoading();
        Play.JoinRoom(roomId);
	}

	// LeanCloud
    [PlayEvent]
    public override void OnCreatingRoom() {
        Debug.Log("creating room...");
    }

    [PlayEvent]
    public override void OnCreatedRoom() {
        Debug.Log("created room");
    }

    [PlayEvent]
    public override void OnCreateRoomFailed(int errorCode, string reason) {
        Debug.Log("create room failed");
        GlobalUI.Instantce.HideLoading();
    }

    [PlayEvent]
    public override void OnJoiningRoom() {
        Debug.Log("joining room...");
        GlobalUI.Instantce.HideLoading();
    }

    [PlayEvent]
    public override void OnJoinRoomFailed(int errorCode, string reason) {
        Debug.Log("join room failed: " + reason);
        GlobalUI.Instantce.HideLoading();
    }

    [PlayEvent]
    public override void OnJoinedRoom() 
    {
        Debug.Log("joined room");
        GlobalUI.Instantce.HideLoading();

        Hashtable prop = new Hashtable();
        prop.Add(Constants.PROP_STATUS, Constants.PLAYER_STATUS_IDLE);
        Play.Player.CustomProperties = prop;
        if (Play.IsMasterClient) 
        {
            Debug.Log("i am master");
        }
        SceneManager.LoadScene("Fight");
	}
}
