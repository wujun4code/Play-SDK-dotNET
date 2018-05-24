using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using LeanCloud;

public class Game : PlayMonoBehaviour {
	public string roomName = null;

	void Start ()
	{
		Play.ToggleLog();
		// 随机生成一个用户 ID
		string userId = Random.Range(0, int.MaxValue).ToString();
		Debug.Log("userId: " + userId);
		// 设置用户 ID，请保证游戏内唯一
		Play.UserID = userId;
		Play.Connect("1.0.0");
	}

	#region PlayMonoBehaviour
	// 连接成功回调接口
	[PlayEvent]
	public override void OnAuthenticated ()
	{
		// 根据房间名称「加入或创建房间」
		Play.JoinOrCreateRoom(roomName);
	}

	// 加入或创建房间成功回调接口
	[PlayEvent]
	public override void OnJoinOrCreatedRoom ()
	{
		Debug.Log("OnJoinOrCreatedRoom");
	}

	// 加入或创建房间失败回调接口
	[PlayEvent]
	public override void OnJoinOrCreateRoomFailed ()
	{
		Debug.Log("OnJoinOrCreateRoomFailed");
	}

	// 加入房间后成功回调接口
	[PlayEvent]
	public override void OnJoinedRoom ()
	{
		Debug.Log("OnJoinedRoom: " + Play.Room.Name);
	}

	// 其他玩家加入回调接口
	[PlayEvent]
	public override void OnNewPlayerJoinedRoom (Player player)
	{
		Debug.Log("OnNewPlayerJoinedRoom");
		if (Play.Player.IsMasterClient) {
			// 如果当前玩家是房主，则由当前玩家执行分配点数，判断胜负逻辑
			int count = Play.Room.Players.Count();
			if (count == 2) {
				// 两个人即开始游戏
				foreach (Player p in Play.Players) {
					Hashtable prop = new Hashtable();
					if (p.IsMasterClient) {
						// 如果是房主，则设置 10 分
						prop.Add("POINT", 10);
					} else {
						// 否则设置 5 分
						prop.Add("POINT", 5);
					}
					// 通过设置玩家的 Properties，可以触发所有玩家的 OnPlayerCustomPropertiesChanged(Player player, Hashtable updatedProperties) 回调
					p.CustomProperties = prop;
				}
				// 使用房主作为胜利者，将其 UserId 作为 RPC 参数，通知所有玩家
				Play.RPC("RPCResult", PlayRPCTargets.All, Play.Room.MasterClientId);
			}
		}
	}

	// 玩家属性变化回调接口
	[PlayEvent]
	public override void OnPlayerCustomPropertiesChanged (Player player, Hashtable updatedProperties)
	{
		if (updatedProperties.ContainsKey("POINT")) {
			Debug.LogFormat("{0}: {1}", player.UserID, updatedProperties["POINT"]);
		}
	}
	#endregion

	[PlayRPC]
	public void RPCResult(string winnerId) {
		// 如果胜利者是自己，则输出胜利日志，否则输出失败日志
		if (winnerId == Play.Player.UserID) {
			Debug.Log("win");
		} else {
			Debug.Log("lose");
		}
	}
}
