﻿using LeanCloud.Realtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeanCloud
{
    interface IPlayNoticeLisenter : IAVIMListener
    {
        string Key { get; }
    }
}
