using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PokerProvider {
	// 当前套牌
	private Poker[] pokers = null;
	// 当前抓牌索引
	private int drawIndex = 0;

	public void init() {
		pokers = initNewPokers();
		drawIndex = 0;
	}

	public Poker[] draw() {
		Poker[] drawPokers = new Poker[3];
		for (int i = 0; i < 3; i++) {
			int index = drawIndex + i;
			Poker poker = pokers[index];
			drawPokers[i] = poker;
		}
		drawIndex += 3;
		return drawPokers;
	}

	// 每局开始生成一套新牌
    private Poker[] initNewPokers() {
		Poker[] pokers = new Poker[52];
		// 生成初始套牌
		for (int i = 0; i < 52; i++) {
			int flower = i / 13;
			int num = i % 13 + 2;
			pokers[i] = new Poker(flower, num);
		}
		// 洗牌
		for (int i = 0; i < pokers.Length; i++) {
			int random = Random.Range(0, pokers.Length);
			Poker poker = pokers[i];
			pokers[i] = pokers[random];
			pokers[random] = poker;
		}
		return pokers;
	}
}
