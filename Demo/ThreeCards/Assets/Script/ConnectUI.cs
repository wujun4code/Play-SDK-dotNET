using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using LeanCloud;

public class ConnectUI : PlayMonoBehaviour {
	public InputField userIdInputField = null;

	public void onConnectBtnClicked() {
		string userId = userIdInputField.text;
        if (string.IsNullOrEmpty(userId)) {
			Debug.Log("user id is null");
			return;
		}

		GlobalUI.Instantce.ShowLoading();

        Play.UserID = userId;
        Play.Connect("0.0.1");
    }

	// 连接完成回调
    [PlayEvent]
    public override void OnAuthenticated() {
        Debug.Log("OnAuthenticated");
        SceneManager.LoadScene("room");
        GlobalUI.Instantce.HideLoading();
    }
}
