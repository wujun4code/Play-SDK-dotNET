using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeanCloud
{
    public static class PlayStateMachine
    {
        public static PlayEventCode Next(PlayEventCode eventCode, PlayResponse response)
        {
            if (response.StatusCode > 201 || response.StatusCode < 200)
            {
                return Failed(eventCode);
            }
            else
            {
                return Successed(eventCode);
            }
        }

        public static PlayEventCode Successed(PlayEventCode eventCode)
        {
            return (PlayEventCode)(eventCode + 1);
        }

        public static PlayEventCode Failed(PlayEventCode eventCode)
        {
            return (PlayEventCode)((int)(eventCode + 1) * -1);
        }
    }
}
