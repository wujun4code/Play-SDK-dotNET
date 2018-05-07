# Play Demo 介绍

## 目录结构

```
├── Assets                                      // 资源主目录
    ├── LeanCloud                               // LeanCloud 插件目录
	├── Poker                                       // 扑克牌素材资源
	├── Resources                                   // 预制体资源
	├── Scene                                       // 场景资源
	├── Script                                      // 代码
		├── GlobalUI.cs                                 // 全局 UI 控制脚本，处于最上层
		├── ConnectUI.cs                                // 连接场景控制脚本
		├── RoomUI.cs                                   // 房间场景控制脚本
		├── Fight                                       // 战斗代码
			├── Fight.cs                                    // 战斗核心控制脚本
			├── FightUI.cs                                  // 战斗 UI 控制脚本，用于控制玩家 UI 逻辑
			├── FightScene.cs                               // 战斗对象控制脚本，用于控制玩家扑克牌对象逻辑
			├── Player.cs                                   // 玩家控制脚本
			├── PlayerUI.cs                                 // 玩家 UI 控制脚本
			├── PlayPanel.cs                                // 玩家操作 UI 控制脚本
			├── Poker.cs                                    // 扑克牌数据类脚本
			├── PokerProvider.cs                            // 扑克牌数据提供者，用于初始化，洗牌，抓牌
			├── PokerVC.cs                                  // 扑克牌控制脚本
			├── ResultPanel.cs                              // 结果 UI 控制脚本
			├── PokerType                                   // 算分逻辑，不关心玩法的同学可以忽略
...
```