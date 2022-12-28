using UnityEngine;
using UnityEngine.Events;

namespace FateGames
{
    public static class PlayerProgression
    {
        public static UnityEvent<int> OnMoneyChanged { get; private set; } = new UnityEvent<int>();
        private static PlayerData playerData;
        public static PlayerData PlayerData { get => playerData; }

        public static int CurrentLevel
        {
            get => playerData.CurrentLevel;
            set
            {
                playerData.CurrentLevel = value;
                SaveManager.Save(playerData);
            }
        }
        public static int MONEY { get => playerData.Money; set { playerData.Money = value; OnMoneyChanged.Invoke(value); } }

        public static void InitializePlayerData()
        {
            playerData = SaveManager.Load<PlayerData>();
            if (playerData == null)
            {
                playerData = new PlayerData();
                SaveManager.Save(playerData);
            }
        }

    }

}