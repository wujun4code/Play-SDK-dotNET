using LeanCloud.Realtime.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace LeanCloud
{
	/// <summary>
	/// websocket client decorator
	/// </summary>
	public class PlayConnection : IWebSocketClient
	{
		public string Url { get; set; }
		public string Protocol { get; set; }
		public bool KeepAlive { get; set; }
		public Action<Action> Reopen { get; set; }
		public IWebSocketClient WebSocketClient { get; set; }

		public Queue<string> received;

		// 心跳
		private const double KEEP_ALIVE_RATE = 1000;
		private const double KEEP_ALIVE_DURATION = 10000;
		private System.Timers.Timer keepAliveTimer = null;
		private double keepAliveDuration = 0;

		public bool IsOpen
		{
			get
			{
				return WebSocketClient.IsOpen;
			}
		}

		public void Ping()
		{
			Play.Log("ping");
			this.WebSocketClient.Send("{}");
		}

		public PlayConnection(IWebSocketClient webSocketClient)
		{
			this.WebSocketClient = webSocketClient;
			this.KeepAlive = true;
		}

		public event Action<int, string, string> OnClosed
		{
			add
			{
				WebSocketClient.OnClosed += value;
			}
			remove
			{
				WebSocketClient.OnClosed -= value;
			}
		}

		public event Action<string> OnError
		{
			add
			{
				WebSocketClient.OnError += value;
			}
			remove
			{
				WebSocketClient.OnError -= value;
			}
		}

		public event Action<string> OnLog
		{
			add
			{
				WebSocketClient.OnLog += value;
			}
			remove
			{
				WebSocketClient.OnLog -= value;
			}
		}

		public event Action<string> OnMessage
		{
			add
			{
				WebSocketClient.OnMessage += value;
			}
			remove
			{
				WebSocketClient.OnMessage -= value;
			}
		}

		public event Action OnOpened
		{
			add
			{
				WebSocketClient.OnOpened += value;
			}
			remove
			{
				WebSocketClient.OnOpened -= value;
			}
		}

		public void Close()
		{
			if (WebSocketClient != null && WebSocketClient.IsOpen)
				WebSocketClient.Close();
			if (keepAliveTimer != null) {
				keepAliveTimer.Stop();
				keepAliveTimer = null;
			}
		}

		public void Open(string url, string protocol = null)
		{
			WebSocketClient.Open(url, protocol);
			this.Url = url;
			this.Protocol = protocol;
			this.WebSocketClient.OnClosed += WebSocketClient_OnClosed;
			// 开启心跳
			if (keepAliveTimer != null) {
				keepAliveTimer.Stop();
				keepAliveTimer = null;
			}
			keepAliveTimer = new System.Timers.Timer(KEEP_ALIVE_RATE);
			keepAliveTimer.Elapsed += KeepAlive_Elapsed;
			keepAliveTimer.Start();
		}

		private void WebSocketClient_OnClosed(int arg1, string arg2, string arg3)
		{
			if (KeepAlive)
			{
				if (Reopen != null)
				{
					Reopen(() => { });
				}
			}
			keepAliveTimer.Stop();
			keepAliveTimer = null;
		}

		public void Send(string message)
		{
			try
			{
				WebSocketClient.Send(message);
				// 重置心跳时间
				keepAliveDuration = KEEP_ALIVE_DURATION;
			}
			catch
			{
				Connect(() =>
				{
					this.Send(message);
				});
			}
		}

		public void Connect(Action opened)
		{
			this.Open(this.Url, this.Protocol);
			Action onOpened = null;
			onOpened = () =>
			{
				WebSocketClient.OnOpened -= onOpened;
				this.Reopen(() =>
				{
					opened();
				});
			};
			WebSocketClient.OnOpened += onOpened;
		}

		private void KeepAlive_Elapsed (System.Object sender, ElapsedEventArgs args)
		{
			keepAliveDuration -= 1000;
			if (keepAliveDuration < 0) {
				Ping();
				keepAliveDuration = KEEP_ALIVE_DURATION;
			}
		}
	}
}
