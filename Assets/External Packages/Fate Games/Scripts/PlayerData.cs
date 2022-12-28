using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FateGames
{
    [System.Serializable]
    public class PlayerData : Data
    {
        public int CurrentLevel = 1;
        public int BestAchivedCarLevel = 0;
        public int Money = 1200000;
        public List<SlotData> SlotsData = new();


        [System.Serializable]
        public class SlotData
        {
            public int SlotId;
            public bool Opened;
            public int CarId;
            public int UpgradeLevel;
            public float TotalMile;

            public SlotData(int slotId)
            {
                SlotId = slotId;
                Opened = false;
                CarId = -1;
                UpgradeLevel = 0;
                TotalMile = 0;
            }
        }
    }
}
