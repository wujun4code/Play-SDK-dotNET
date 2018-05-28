using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeanCloud
{
    public class PlayFriend
    {
        public string UserId { get; internal set; }

        public bool IsOnline { get; internal set; }

        public string Room { get; internal set; }

        public bool IsInRoom { get { return IsOnline && !string.IsNullOrEmpty(this.Room); } }
    }
}
