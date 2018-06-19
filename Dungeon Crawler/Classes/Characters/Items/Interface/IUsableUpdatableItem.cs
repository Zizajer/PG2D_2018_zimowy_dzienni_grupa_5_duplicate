﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dungeon_Crawler
{
    public interface IUsableUpdatableItem : IUsableItem, IUpdatableItem
    {
        bool IsCurrentlyInUse { get; }
        bool HasRecentUsageJustFinished { get; set; }
    }
}
