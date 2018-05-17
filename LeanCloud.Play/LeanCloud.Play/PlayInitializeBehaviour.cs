using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#if UNITY
using UnityEngine;
#endif

namespace LeanCloud
{
    /// <summary>
    /// 
    /// </summary>
    public class PlayEventMessage
    {
        public string MethodName { get; set; }

        public object[] MethodParameters { get; set; }

        public virtual void Invoke()
        {
            Play.EventBehaviours.When(kv => kv.Key == MethodName).Every(kv =>
            {
                kv.Value.Every(b => Play.Find<PlayEventAttribute>(b, kv.Key).Invoke(b, MethodParameters.Length > 0 ? MethodParameters : null));
            });
        }
    }

    public class PlayRPCEventMessage : PlayEventMessage
    {
        public PlayRpcMessage rpcMessage;

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
        /// <summary>
        /// 
        /// </summary>
        public override void Awake()
        {
            //if (Play.playMQB == null)
            //{
            //    Play.playMQB = this;
            //}
            base.Awake();
        }

		private void OnApplicationQuit ()
		{
			Play.CloseConnect();
		}
#endif
	}
}
