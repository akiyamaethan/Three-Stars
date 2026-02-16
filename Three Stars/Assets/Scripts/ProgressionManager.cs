using System.Collections.Generic;
using UnityEngine;

namespace ThreeStars
{
    public class ProgressionManager : MonoBehaviour
    {
        public int currentXP { get; private set; }
        public int currentLevel { get; private set; } = 1;
        public int xpToNextLevel { get; private set; } = 1000;
        public delegate void onLevelUp(int newLevel);
        public event onLevelUp LevelUpEvent;

        public void AddXP(int amount)
        {
            currentXP += amount;
            while (currentXP >= xpToNextLevel)
            {
                LevelUp();
            }
        }

        public void LevelUp()
        {
            currentXP -= xpToNextLevel;
            currentLevel++;
            xpToNextLevel = Mathf.RoundToInt(xpToNextLevel * 1.5f); // Increase XP requirement by 50% each level
            LevelUpEvent?.Invoke(currentLevel);
        }

    }
}

