using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using LeanCloud;

public class Player : UnityEngine.MonoBehaviour
{
    // 表现控制器
    public PokerVC[] pokerVCs = null;
    // LeanCloud玩家对象
    private LeanCloud.Player lcPlayer = null;
    // 数据
    private Poker[] pokers = null;
    // 牌型
    private PokerType pokerType = null;

    // UI面板
    public PlayerUI ui = null;

    void Awake() {
        this.pokerVCs = GetComponentsInChildren<PokerVC>();
        this.gameObject.SetActive(false);
    }

    public void draw(LeanCloud.Player player, Poker[] pokers) {
        this.gameObject.SetActive(true);
        this.lcPlayer = player;
        this.pokers = pokers;
        this.pokerType = PokerType.CalcPokerType(this.pokers);
        for (int i = 0; i < pokers.Length; i++) {
            Poker poker = this.pokers[i];
            PokerVC pokerVC = this.pokerVCs[i];
            if (this.lcPlayer == Play.Player) {
                pokerVC.show(poker);
            } else {
                pokerVC.showBack();
            }
        }
    }

    public PokerType getPokerType() {
        return this.pokerType;
    }

    public int getScore() {
        return this.pokerType.Score;
    }
}
