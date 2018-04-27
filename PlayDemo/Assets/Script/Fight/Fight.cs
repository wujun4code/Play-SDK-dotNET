using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System;
using LeanCloud;
using Newtonsoft.Json;

public class Fight : PlayMonoBehaviour
{
    // 场景显示控制器
    public FightScene scene = null;
    // ui显示控制器
    public FightUI ui = null;
    // 牌生成器
    private PokerProvider pokerProvider = null;

    private LinkedList<int> playerIdList = null;

    void Start()
    {
        this.pokerProvider = new PokerProvider();

        // 绑定 Player 和 GameObject
        List<LeanCloud.Player> playerList = Play.Players.ToList();
        int myIndex = playerList.FindIndex(p => p.IsLocal);
        int offset = -myIndex;

        for (int i = 0; i < playerList.Count; i++) {
            LeanCloud.Player lcPlayer = playerList[i];
            int index = (i + offset + 4) % 4;
            scene.bind(lcPlayer, index);
            ui.bind(lcPlayer, index);
            ui.setPlayerName(lcPlayer, lcPlayer.UserID);
            if (lcPlayer.CustomProperties.ContainsKey(Constants.PROP_STATUS)) {
                int status = (int) lcPlayer.CustomProperties[Constants.PROP_STATUS];
                ui.setPlayerStatus(lcPlayer, status);
            }
        }
        ui.setUI(Play.Player);

        if (Play.Player.IsMasterClient)
        {
            var p = new Hashtable();
            p.Add(Constants.PROP_STATUS, Constants.PLAYER_STATUS_READY);
            Play.Player.CustomProperties = p;
        }
    }

    void newGame()
    {
        // 初始化玩家列表
        playerIdList = new LinkedList<int>();

        // 通知所有客户端开始
        Play.RPC("rpcStartGame", PlayRPCTargets.All);

        // 初始化牌数据
        this.pokerProvider.init();
        // 发牌
        foreach (LeanCloud.Player player in Play.Room.Players) {
            int status = (int)player.CustomProperties[Constants.PROP_STATUS];
            if (status == Constants.PLAYER_STATUS_READY)
            {
                Hashtable prop = new Hashtable();
                Poker[] pokers = this.pokerProvider.draw();
                prop.Add(Constants.PROP_STATUS, Constants.PLAYER_STATUS_PLAY);
                prop.Add(Constants.PROP_GOLD, 10000);
                string pokersJson = JsonConvert.SerializeObject(pokers);
                Debug.Log("pokers json: " + pokersJson);
                prop.Add(Constants.PROP_POKER, pokersJson);
                player.CustomProperties = prop;
                // 添加到玩家列表中
                playerIdList.AddLast(player.ActorID);
            }
        }

        // 初始化房间总金币
        Hashtable p = new Hashtable();
        p.Add(Constants.PROP_ROOM_GOLD, 0);
        Play.Room.CustomProperties = p;

        notifyPlayerChoose(Play.Player.ActorID);
    }

    void OnNewMsg()
    {
        newGame();
    }

    void notifyPlayerChoose(int playerId)
    {
        IEnumerable<LeanCloud.Player> players = Play.Room.Players;
        LeanCloud.Player player = players.FirstOrDefault<LeanCloud.Player>((p) => p.ActorID == playerId);
        Play.RPC("rpcChoose", PlayRPCTargets.All, playerId);
    }

#region LeanCloud message
    [PlayEvent]
    public override void OnNewPlayerJoinedRoom(LeanCloud.Player player)
    {
        Debug.Log("someone joined room");
        List<LeanCloud.Player> playerList = Play.Players.ToList();
        int myIndex = playerList.FindIndex(p => p.IsLocal);
        int index = playerList.FindIndex(p => p.UserID == player.UserID);
        int offset = index - myIndex;
        scene.bind(player, offset);
        ui.bind(player, offset);
        ui.setPlayerName(player, player.UserID);
    }

    [PlayEvent]
    public override void OnPlayerLeftRoom(LeanCloud.Player player) 
    {
        Debug.Log("someone left room");
        if (Play.IsMasterClient) 
        {
            int status = (int)player.CustomProperties[Constants.PROP_STATUS];
            if (status == Constants.PLAYER_STATUS_PLAY) 
            {
                // 玩家正在游戏中，重新生成玩家列表
                rpcDiscard(player.ActorID);
            }
            else 
            {
                // 移除处理

            }
        }
    }
#endregion

#region RoomProperty
    [PlayEvent]
    public override void OnRoomCustomPropertiesUpdated(Hashtable updatedProperties) {
        if (updatedProperties.ContainsKey(Constants.PROP_ROOM_GOLD)) {
            onRoomGoldUpdated(updatedProperties);
        }
    }

    void onRoomGoldUpdated(Hashtable updatedProperties) {
        int gold = (int)updatedProperties[Constants.PROP_ROOM_GOLD];
        this.scene.showTotalGold(gold);
    }
#endregion

#region PlayerProperty
    [PlayEvent]
    public override void OnPlayerCustomPropertiesChanged(LeanCloud.Player player, Hashtable updatedProperties) {
        foreach (var key in updatedProperties.Keys) {
            string keyStr = key as string;
            if (keyStr == Constants.PROP_STATUS)
            {
                onPlayerStatusPropertiesChanged(player, updatedProperties);
            }
            else if (keyStr == Constants.PROP_POKER)
            {
                onPlayerPokerPropertiesChanged(player, updatedProperties);
            }
            else if (keyStr == Constants.PROP_GOLD)
            {
                onPlayerGoldPropertiesChanged(player, updatedProperties);
            }
        }
    }

    void onPlayerStatusPropertiesChanged(LeanCloud.Player player, Hashtable updatedProperties) {
        // 刷新玩家 UI
        int status = (int) updatedProperties[Constants.PROP_STATUS];
        ui.setPlayerStatus(player, status);
        if (Play.Player.IsMasterClient) {
            // 计算得到「准备」玩家的数量
            int readyPlayersCount = Play.Players.Where(p =>
            {
                int s = (int)p.CustomProperties[Constants.PROP_STATUS];
                return s == Constants.PLAYER_STATUS_READY;
            }).Count();
            if (readyPlayersCount > 1 && readyPlayersCount == Play.Players.Count()) {
                ui.enableStartButton();
            }
        }
    }

    void onPlayerPokerPropertiesChanged(LeanCloud.Player player, Hashtable updatedProperties) {
        string pokersJson = player.CustomProperties[Constants.PROP_POKER] as string;
        Poker[] pokers = JsonConvert.DeserializeObject<Poker[]>(pokersJson);
        if (player.IsLocal) {
            scene.draw(player, pokers);
        } else {
            scene.draw(player, pokers);
        }
    }

    void onPlayerGoldPropertiesChanged(LeanCloud.Player player, Hashtable updatedProperties) {
        int gold = (int) updatedProperties[Constants.PROP_GOLD];
        ui.updateGoldUI(player, gold);
    }
#endregion

#region RPC
    // RPC处理
    // 开始游戏
    [PlayRPC]
    public void rpcStartGame() {
        this.ui.showPlayPanel();
    }

    // 玩家选择
    [PlayRPC]
    public void rpcChoose(int playerID) {
        if (Play.Player.ActorID == playerID) {
            this.ui.enablePlayPanel();
        } else {
            this.ui.disablePlayPanel();
        }
    }

    // 跟牌
    [PlayRPC]
    public void rpcFollow(int playerId) 
    {
        // 扣除玩家金币
        IEnumerable<LeanCloud.Player> players = Play.Room.Players;
        LeanCloud.Player player = players.FirstOrDefault(p => p.ActorID == playerId);
        int gold = (int)player.CustomProperties[Constants.PROP_GOLD];
        gold -= 100;
        Hashtable goldProp = new Hashtable();
        goldProp.Add(Constants.PROP_GOLD, gold);
        player.CustomProperties = goldProp;

        // 增加房间金币
        int roomGold = (int)Play.Room.CustomProperties[Constants.PROP_ROOM_GOLD];
        roomGold += 100;
        Hashtable prop = new Hashtable();
        prop.Add(Constants.PROP_ROOM_GOLD, roomGold);
        Play.Room.CustomProperties = prop;

        int nextPlayerId = getNextPlayerId(playerId);
        notifyPlayerChoose(nextPlayerId);
    }

    // 弃牌
    [PlayRPC]
    public void rpcDiscard(int playerId) 
    {
        IEnumerable<LeanCloud.Player> players = Play.Room.Players;
        // 设置棋牌玩家状态
        LeanCloud.Player player = players.FirstOrDefault(p => p.ActorID == playerId);
        if (player != null) {
            Hashtable prop = new Hashtable();
            prop.Add(Constants.PROP_STATUS, Constants.PLAYER_STATUS_DISCARD);
            player.CustomProperties = prop;
        }
        // 从当前玩家列表中移除
        playerIdList.Remove(playerId);

        if (playerIdList.Count > 1) 
        {
            // 请下一个玩家做出选择
            int nextPlayerId = getNextPlayerId(playerId);
            notifyPlayerChoose(nextPlayerId);
        } 
        else 
        {
            // 剩者为王
            int winnerId = playerIdList.First.Value;
            Play.RPC("rpcResult", PlayRPCTargets.All, winnerId);
        }
    }

    // 比牌
    [PlayRPC]
    public void rpcCompare() 
    {
        var playersByScoreOrder = Play.Players.Where(p =>
        {
            int status = (int)p.CustomProperties[Constants.PROP_STATUS];
            return status == Constants.PLAYER_STATUS_PLAY;
        }).OrderByDescending(p =>
        {
            // 查找
            Player player = scene.GetPlayer(p);
            return player.getScore();
        });
        LeanCloud.Player winPlayer = playersByScoreOrder.FirstOrDefault();
        Play.RPC("rpcResult", PlayRPCTargets.All, winPlayer.ActorID);
    }

    // 结果通知
    [PlayRPC]
    public void rpcResult(int winnerId) 
    {
        Debug.Log("winnerId: " + winnerId);
        if (winnerId == Play.Player.ActorID) 
        {
            ui.showWin();
        } 
        else 
        {
            ui.showLose();
        }
    }
#endregion

    private int getNextPlayerId(int playerId) {
        LinkedListNode<int> nextPlayer = playerIdList.Find(playerId).Next;
        if (nextPlayer == null)
        {
            nextPlayer = playerIdList.First;
        }
        return nextPlayer.Value;
    }
}
