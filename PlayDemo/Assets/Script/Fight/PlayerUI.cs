using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour {
	public Text nameText = null;
	public Text statusText = null;

	public void setName(string name) {
		this.nameText.text = name;
	}

    public void setStatus(int status) {
        if (status == Constants.PLAYER_STATUS_IDLE) {
            this.statusText.color = Color.red;
            this.statusText.text = "idle";
        } else if (status == Constants.PLAYER_STATUS_READY) {
            this.statusText.color = Color.green;
            this.statusText.text = "ready";
        } else if (status == Constants.PLAYER_STATUS_DISCARD) {
            this.statusText.color = Color.gray;
            this.statusText.text = "discard";
        }
    }

    public void setGold(int gold) {
        this.statusText.color = Color.yellow;
        this.statusText.text = string.Format("{0}", gold);
    }
}
