using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayPanel : MonoBehaviour {
	private void Start() {
        this.gameObject.SetActive(false);
        this.disablePlay();
	}

	public void enablePlay() {
        Button[] buttons = GetComponentsInChildren<Button>();
        for (int i = 0; i < buttons.Length; i++) {
            Button btn = buttons[i];
            btn.interactable = true;
        }
    }

    public void disablePlay() {
        Button[] buttons = GetComponentsInChildren<Button>();
        for (int i = 0; i < buttons.Length; i++) {
            Button btn = buttons[i];
            btn.interactable = false;
        }
    }
}
