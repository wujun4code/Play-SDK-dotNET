# Play SDK for Unity (C#)

Play 是 LeanCloud 针对实时对战游戏推出的后端云服务，目前以 Unity(C#) SDK 的方式提供服务，详情请查看官网文档。


## 产品简介

请阅读我们的博客:[LeanCloud Play 内测邀请—不搭建后端，快速上线多人对战游戏](https://blog.leancloud.cn/6177/)

## 相关文档

- [Play 服务总览](https://leancloud.cn/docs/play.html)
- [快速入门 Unity（C#）](https://leancloud.cn/docs/play-quick-start.html)
- [开发指南 Unity（C#）](https://leancloud.cn/docs/play-unity.html)
- [实现小游戏「炸金花」](https://leancloud.cn/docs/play-unity-demo.html)

## 代码简介

整个代码的架构是基于 .NET 3.5，并没有利用到 4.6 的  Task 来实现异步操作请求，是充分考虑到目前 Unity 主流的 Mono 编译环境并没有升级，因此异步的回调方式可能不太优雅，但是足够可用。

代码还在不断调整中，争取做到优雅而不失准确性，科学而不失可读性。

## 需求和 bug 反馈

目前 Play 处于高速迭代的周期内，很多需求是亟待使用者来讨论和实现的，我们热烈欢迎各方玩家或者开发者来出谋划策，不胜感激。

Bug 反馈直接提 issue 即可，我们会实时跟进修复和发布版本的。