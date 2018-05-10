using LeanCloud.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeanCloud
{
    public class PlayCorePlugins
    {
        private static readonly PlayCorePlugins instance = new PlayCorePlugins();
        public static PlayCorePlugins Instance
        {
            get
            {
                return instance;
            }
        }
        private readonly object mutex = new object();

        private PlaySynchronousObjectSubclassController synchronousObjectSubclassController;

        /// <summary>
        /// 
        /// </summary>
        public PlaySynchronousObjectSubclassController SynchronousObjectSubclassController
        {
            get
            {
                lock (mutex)
                {
                    synchronousObjectSubclassController = synchronousObjectSubclassController ?? new PlaySynchronousObjectSubclassController();
                    return synchronousObjectSubclassController;
                }
            }
            internal set
            {
                lock (mutex)
                {
                    synchronousObjectSubclassController = value;
                }
            }
        }
    }
}
