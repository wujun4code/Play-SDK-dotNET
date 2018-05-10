using UnityEngine;

namespace LeanCloud
{
    internal static class PlayLogger
    {
        static PlayLogger()
        {

        }

        internal static void Open()
        {
            Play.Logger = Debug.Log;
            Play.ErrorLogger = Debug.LogError;
        }

        internal static void Close()
        {
            Play.Logger = null;
            Play.ErrorLogger = null;
        }
    }
}
