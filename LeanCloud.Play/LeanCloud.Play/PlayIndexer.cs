using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeanCloud
{
    public class PlayIndexer<T, TK> : IEnumerable<T>
        where TK : IEquatable<TK>
    {
        private readonly Object mutex = new Object();
        internal IList<T> metaList;
        internal Func<T, TK, bool> Filter { get; set; }

        public IEnumerator<T> GetEnumerator()
        {
            lock (mutex)
            {
                return metaList.GetEnumerator();
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            lock (mutex)
            {
                return metaList.GetEnumerator();
            }
        }

        public virtual T this[TK key]
        {
            get
            {
                return Get(key);
            }
            set
            {
                Set(key, value);
            }
        }

        public virtual T Get(TK key)
        {
            lock (mutex)
            {
                return metaList.Where(t => Filter(t, key)).FirstOrDefault();
            }
        }

        public virtual void Set(TK key, T value)
        {
            var older = Get(key);
            older = value;
        }
    }
}
