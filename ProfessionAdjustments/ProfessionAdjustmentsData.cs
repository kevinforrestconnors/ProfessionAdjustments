using System;
using System.Collections.Generic;
using StardewValley;

namespace ProfessionAdjustments
{
    public class ProfessionAdjustmentsData
    {

        public Dictionary<string, int> itemsWithChangedValues = new Dictionary<string, int>();

        public ProfessionAdjustmentsData()
        {
            this.itemsWithChangedValues = new Dictionary<string, int>();
        }
    }
}
