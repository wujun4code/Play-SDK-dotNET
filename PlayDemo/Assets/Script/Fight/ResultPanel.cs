using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResultPanel : MonoBehaviour {
    public Text resultText = null;

	public void showWin() {
        this.gameObject.SetActive(true);
        this.resultText.text = "Win";
	}

    public void showLose() {
        this.gameObject.SetActive(true);
        this.resultText.text = "Lose";
    }
}
