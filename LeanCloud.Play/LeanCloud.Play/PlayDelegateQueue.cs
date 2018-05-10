using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LeanCloud
{
    /// <summary>
    /// a thread safe delegate queue.
    /// </summary>
    public class PlayMainThreadQueue
    {
        private readonly Queue<PlayWebsocketMessage> _queue;

        public Guid Id { get; }
        private SynchronizationContext syncContext;
        private Timer timer;

        public PlayMainThreadQueue()
        {
            _queue = new Queue<PlayWebsocketMessage>();
            syncContext = SynchronizationContext.Current;
            Id = Guid.NewGuid();
        }
        public int Count
        {
            get
            {
                return _queue.Count;
            }
        }
        public object SyncRoot
        {
            get { return ((ICollection)_queue).SyncRoot; }
        }

        public static PlayMainThreadQueue CreateQueue()
        {
            return new PlayMainThreadQueue();
        }

        public void Enqueue(PlayWebsocketMessage item)
        {
            lock (SyncRoot)
            {
                _queue.Enqueue(item);
            }
        }

        public void Dequeue()
        {
            var pwm = _queue.Dequeue();
            pwm.Execute();
        }

    }

    public class PlayWebsocketMessage
    {
        public PlayWebsocketMessageDeligate Method { get; set; }

        public object[] MethodParameters { get; set; }

        public void Execute()
        {
            Method.Invoke(MethodParameters);
        }
    }

    public delegate void PlayWebsocketMessageDeligate(params object[] objects);

}
