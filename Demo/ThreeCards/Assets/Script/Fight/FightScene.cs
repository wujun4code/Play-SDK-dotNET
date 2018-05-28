using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using LeanCloud;

public class FightScene : MonoBehaviour
{
    public Player[] players = null;
    public Text text = null;

    private Dictionary<string, Player> playerDict = null;

    void Awake()
    {
        playerDict = new Dictionary<string, Player>();
    }

    public void bind(LeanCloud.Player lclayer, int index) {
        Player player = players[index];
        playerDict.Add(lclayer.UserID, player);
    }

    public void draw(LeanCloud.Player lcPlayer, Poker[] pokers)
    {
        Player player = null;
        if (playerDict.TryGetValue(lcPlayer.UserID, out player))
        {
            player.draw(lcPlayer, pokers);
        }
        else
        {
            // exception
            Debug.LogError("no player: " + lcPlayer.UserID);
        }
    }

    public Player GetPlayer(LeanCloud.Player lcPlaer) {
        Player player = null;
        if (playerDict.TryGetValue(lcPlaer.UserID, out player)) {
            return player;
        }
        // TODO Exception

        return null;
    }

	public void draw(int playerIndex, LeanCloud.Player lcPlayer, Poker[] pokers) {
        Player player = this.players[playerIndex];
        player.draw(lcPlayer, pokers);
    }

    public void showTotalGold(int gold) {
        this.text.text = string.Format("{0}", gold);
    }
}
