using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using LeanCloud;

public class FightUI : PlayMonoBehaviour {
    public PlayerUI[] playerUIs = null;

    public Button startOrReadyButton = null;
    public PlayPanel playPanel = null;
    public ResultPanel resultPanel = null;

    private Dictionary<string, PlayerUI> playerUIDict = null;

	protected override void Awake ()
	{
		base.Awake();
		playerUIDict = new Dictionary<string, PlayerUI>();
	}

    public void bind(LeanCloud.Player lcPlayer, int index) {
        PlayerUI playerUI = playerUIs[index];
        playerUIDict.Add(lcPlayer.UserID, playerUI);
    }

	// 接口
    public void setUI(LeanCloud.Player lcPlayer) {
        if (lcPlayer.IsMasterClient) {
            Text text = this.startOrReadyButton.GetComponentInChildren<Text>();
            text.text = "Start";
            this.startOrReadyButton.onClick.AddListener(onStartButtonClicked);
            this.startOrReadyButton.interactable = false;
        } else {
            Text text = this.startOrReadyButton.GetComponentInChildren<Text>();
            text.text = "Ready";
            this.startOrReadyButton.onClick.AddListener(onReadyButtonClicked);
        }
    }

    public void enableStartButton() {
        this.startOrReadyButton.interactable = true;
    }

    public void showPlayPanel() {
        this.playPanel.gameObject.SetActive(true);
    }

    public void enablePlayPanel() {
        this.playPanel.enablePlay();
    }

    public void disablePlayPanel() {
        this.playPanel.disablePlay();
    }

    public void setPlayerName(LeanCloud.Player player, string name)
    {
        PlayerUI playerUI = null;
        if (playerUIDict.TryGetValue(player.UserID, out playerUI))
        {
            playerUI.setName(name);
        }
        else {
            // Exception
            Debug.LogError("not found player ui: " + player.UserID);
        }
    }

    public void setPlayerStatus(LeanCloud.Player player, int status) {
        PlayerUI playerUI = null;
        if (playerUIDict.TryGetValue(player.UserID, out playerUI)) {
            playerUI.setStatus(status);
        } else {
            // Exception
            Debug.LogError("not found player ui: " + player.UserID);
        }
    }

    public void updateGoldUI(LeanCloud.Player player, int gold) {
        PlayerUI playerUI = null;
        if (playerUIDict.TryGetValue(player.UserID, out playerUI))
        {
            playerUI.setGold(gold);
        }
        else
        {
            // Exception
            Debug.LogError("not found player ui: " + player.UserID);
        }
    }

    public void showWin() {
        this.resultPanel.showWin();
    }

    public void showLose() {
        this.resultPanel.showLose();
    }

    PlayerUI getPlayerUI(int index) {
        return this.playerUIs[index];
    }

	// 操作事件
    public void onStartButtonClicked() {
        this.gameObject.SendMessageUpwards("OnNewMsg");
        this.startOrReadyButton.gameObject.SetActive(false);
    }

    public void onReadyButtonClicked() {
        Hashtable prop = new Hashtable();
        prop.Add(Constants.PROP_STATUS, Constants.PLAYER_STATUS_READY);
        Play.Player.CustomProperties = prop;
        this.startOrReadyButton.gameObject.SetActive(false);
    }

	public void onBackBtnClicked() {
        Play.LeaveRoom();
	}

	public void onDiscardBtnClicked() {
        this.disablePlayPanel();
        Play.RPC("rpcDiscard", PlayRPCTargets.MasterClient, Play.Player.ActorID);
	}

	public void onCompareBtnClicked() {
        this.disablePlayPanel();
        Play.RPC("rpcCompare", PlayRPCTargets.MasterClient);
	}

	public void onFollowBtnClicked() {
        this.disablePlayPanel();
        Play.RPC("rpcFollow", PlayRPCTargets.MasterClient, Play.Player.ActorID);
	}

	// LeanCloud
    [PlayEvent]
    public override void OnLeavingRoom() {
        Debug.Log("leaving room");
    }

    [PlayEvent]
    public override void OnLeftRoom() {
        Debug.Log("left room");
		GlobalUI.Instantce.HideLoading();
		SceneManager.LoadScene("Room");
	}

    [PlayEvent]
    public override void OnLeaveRoomFailed() {
        Debug.Log("left room failed");
    }
}
