using System.Linq;
using System.Linq.Expressions;

#if UNITY
using UnityEngine;
#endif

namespace LeanCloud
{
    /// <summary>
    /// Play event message.
    /// </summary>
	public class PlayEventMessage
	{
        /// <summary>
        /// Gets or sets the name of the method.
        /// </summary>
        /// <value>The name of the method.</value>
		public string MethodName { get; set; }

        /// <summary>
        /// Gets or sets the method parameters.
        /// </summary>
        /// <value>The method parameters.</value>
		public object[] MethodParameters { get; set; }

        /// <summary>
        /// Invoke this instance.
        /// </summary>
		public virtual void Invoke()
		{
			// get all the behaviours that subscibed the callback method with the same name.
			if (Play.EventBehaviours.ContainsKey(MethodName))
			{
				// all behaviours subscibed the callback method.
				var behaviours = Play.EventBehaviours[MethodName];
				behaviours.Every(behaviour =>
				{
					// find the method object in a behaviour.
					var method = Play.Find<PlayEventAttribute>(behaviour, MethodName);
					// invoke it.
					method.Invoke(behaviour, MethodParameters.Length > 0 ? MethodParameters : null);
				});
			}
		}
	}

    /// <summary>
    /// Play RPCE vent message.
    /// </summary>
	public class PlayRPCEventMessage : PlayEventMessage
	{
        /// <summary>
        /// The rpc message.
        /// </summary>
		public PlayRpcMessage rpcMessage;

        /// <summary>
        /// Invoke this instance.
        /// </summary>
		public override void Invoke()
		{
			var pt = rpcMessage.Paramters.Select(p => p.GetType()).ToArray();

			var rpcMethods = from behaviour in Play.Behaviours
							 let method = Play.Find(behaviour, rpcMessage.MethodName, pt)
							 where method != null
							 select new
							 {
								 host = behaviour,
								 method = method
							 };
			rpcMethods.Every(hm => hm.method.Invoke(hm.host, parameters: rpcMessage.Paramters.ToArray()));
		}
	}

	/// <summary>
	/// PlayInitializeBehaviour
	/// </summary>
#if UNITY
    public class PlayInitializeBehaviour : AVInitializeBehaviour
#else
	public class PlayInitializeBehaviour
#endif
	{
        /// <summary>
        /// 
        /// </summary>
        public virtual void Update()
		{
			if (Play.EevntMessageQueue.Count == 0)
			{

			}
			else
			{
				lock (Play.mutexEventMessageLock)
				{
					while (Play.EevntMessageQueue.Count > 0)
					{
						var em = Play.EevntMessageQueue.Dequeue();
						em.Invoke();
					}
				}

			}
		}

#if UNITY
        [SerializeField]
        public string playRouter;
        /// <summary>
        /// 
        /// </summary>
        public override void Awake()
        {
            //if (Play.playMQB == null)
            //{
            //    Play.playMQB = this;
            //}
            if (!string.IsNullOrEmpty(playRouter))
            {
                PlayCommand.CustomGameRouter = playRouter;
            }
            base.Awake();
        }

		private void OnApplicationQuit ()
		{
			Play.CloseConnect();
		}
#endif
    }
}
