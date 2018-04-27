using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Constants {
    // Room properties
    // 房间总筹码
    public const string PROP_ROOM_GOLD = "PROP_ROOM_GOLD";

    // Player properties
    // 玩家状态key
    public const string PROP_STATUS = "PROP_STATUS";
    // 牌属性key
    public const string PROP_POKER = "PROP_POKER";
    // 金币属性key
    public const string PROP_GOLD = "PROP_GOLD";

    // 自定义事件码
    // 开始游戏
    public const byte CODE_START_GAME = 1;
    // 比赛结果事件
    public const byte CODE_RESULT = 3;
    // 选择事件
    // 参数：需要选择的Player ID
    public const byte CODE_CHOOSE = 4;
    // 跟牌
    public const byte CODE_FOLLOW = 5;
    // 弃牌
    public const byte CODE_DISCARD = 6;
    // 比牌
    public const byte CODE_COMPARE = 7;

    // 玩家空闲状态
    public const int PLAYER_STATUS_IDLE = 0;
    // 玩家准备状态
    public const int PLAYER_STATUS_READY = 1;
    // 玩家游戏状态
    public const int PLAYER_STATUS_PLAY = 2;
    // 玩家棋牌状态
    public const int PLAYER_STATUS_DISCARD = 3;

    // 自定义RPC
}
