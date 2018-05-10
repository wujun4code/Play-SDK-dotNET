using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PokerVC : MonoBehaviour {
    public Image image = null;
	public Sprite[] pokerSprites = null;

	public void show(Poker poker) {
		int index = poker.Flower * 13 + poker.Num - 2;
        image.sprite = this.pokerSprites[index];
	}

    public void showBack() {
        image.sprite = this.pokerSprites[52];
    }
}
