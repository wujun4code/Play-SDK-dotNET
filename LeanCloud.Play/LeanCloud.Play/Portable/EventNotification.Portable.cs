using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeanCloud
{
    internal static class EventNotification
    {
        public static void NotifyUnityGameObjects(string methodString, params object[] parameters)
        {
            Play.Log("NotifyUnityGameObjects: " + methodString + parameters);
        }
    }
}
