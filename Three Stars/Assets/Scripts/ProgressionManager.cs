using System.Collections.Generic;
using UnityEngine;

namespace ThreeStars
{
    public class ProgressionManager : MonoBehaviour
    {
        public static ProgressionManager Instance { get; private set; }

        // Player variables
        public int handSize = 7;

        // Card progression variables
        private Dictionary<string, int> playCounts = new Dictionary<string, int>();
        public int entreeSuitLevel = 1;
        public int sideSuitLevel = 1;
        public int vegSuitLevel = 1;
        public int sauceSuitLevel = 1;
        public int twoRankLevel = 1;
        public int threeRankLevel = 1;
        public int fourRankLevel = 1;
        public int fiveRankLevel = 1;
        public int sixRankLevel = 1;
        public int sevenRankLevel = 1;
        public int eightRankLevel = 1;
        public int nineRankLevel = 1;
        public int tenRankLevel = 1;
        public int jackRankLevel = 1;
        public int queenRankLevel = 1;
        public int kingRankLevel = 1;
        public int aceRankLevel = 1;

        // Hand progression variables
        public float pairMult = 1.0f;
        public float twoPairMult = 1.0f;
        public float rainbowMult = 1.0f;
        public float tripsMult = 1.0f;
        public float highCardMult = 1.0f;
        public float straightMult = 1.0f;
        public float flushMult = 1.0f;
        public float quadsMult = 1.0f;
        public float straightFlushMult = 1.0f;
        public float royalFlushMult = 1.0f;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
            }
            else
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
        }

        public void RegisterPlay(PlayingCard.CardSuit suit, PlayingCard.CardRank rank)
        {
            string suitKey = "Suit_" + suit.ToString();
            IncrementPlay(suitKey, GetSuitLevel(suit), (newLevel) => SetSuitLevel(suit, newLevel));

            string rankKey = "Rank_" + rank.ToString();
            IncrementPlay(rankKey, GetRankLevel(rank), (newLevel) => SetRankLevel(rank, newLevel));
        }

        private void IncrementPlay(string key, int currentLevel, System.Action<int> setLevelCallback)
        {
            if (!playCounts.ContainsKey(key))
            {
                playCounts[key] = 0;
            }
            playCounts[key]++;

            int requiredPlays = 1;
            if (currentLevel <= 5) requiredPlays = 1;
            else if (currentLevel <= 15) requiredPlays = 3;
            else requiredPlays = 10;

            if (playCounts[key] >= requiredPlays) 
            {
                playCounts[key] = 0;
                setLevelCallback(currentLevel + 1);
                Debug.Log($"Leveled up {key} to level {currentLevel + 1}");
            }
        }

        // Setters and getters for suit
        private void SetSuitLevel(PlayingCard.CardSuit suit, int newLevel)
        {
            switch (suit)
            {
                case PlayingCard.CardSuit.Entree: entreeSuitLevel = newLevel; break;
                case PlayingCard.CardSuit.Side: sideSuitLevel = newLevel; break;
                case PlayingCard.CardSuit.Vegetable: vegSuitLevel = newLevel; break;
                case PlayingCard.CardSuit.Sauce: sauceSuitLevel = newLevel; break;
            }
        }
        public int GetSuitLevelBonusPips(PlayingCard.CardSuit suit)
        {
            return GetSuitLevel(suit) - 1;
        }

        public int GetSuitLevel(PlayingCard.CardSuit suit)
        {
            return suit switch
            {
                PlayingCard.CardSuit.Entree => entreeSuitLevel,
                PlayingCard.CardSuit.Side => sideSuitLevel,
                PlayingCard.CardSuit.Vegetable => vegSuitLevel,
                PlayingCard.CardSuit.Sauce => sauceSuitLevel,
                _ => 1
            };
        }

        // Setters and getters for rank
        private void SetRankLevel(PlayingCard.CardRank rank, int newLevel)
        {
            switch (rank)
            {
                case PlayingCard.CardRank.Two: twoRankLevel = newLevel; break;
                case PlayingCard.CardRank.Three: threeRankLevel = newLevel; break;
                case PlayingCard.CardRank.Four: fourRankLevel = newLevel; break;
                case PlayingCard.CardRank.Five: fiveRankLevel = newLevel; break;
                case PlayingCard.CardRank.Six: sixRankLevel = newLevel; break;
                case PlayingCard.CardRank.Seven: sevenRankLevel = newLevel; break;
                case PlayingCard.CardRank.Eight: eightRankLevel = newLevel; break;
                case PlayingCard.CardRank.Nine: nineRankLevel = newLevel; break;
                case PlayingCard.CardRank.Ten: tenRankLevel = newLevel; break;
                case PlayingCard.CardRank.Jack: jackRankLevel = newLevel; break;
                case PlayingCard.CardRank.Queen: queenRankLevel = newLevel; break;
                case PlayingCard.CardRank.King: kingRankLevel = newLevel; break;
                case PlayingCard.CardRank.Ace: aceRankLevel = newLevel; break;
            }
        }

        public int GetRankLevelBonusPips(PlayingCard.CardRank rank)
        {
            return GetRankLevel(rank) - 1;
        }
        public int GetRankLevel(PlayingCard.CardRank rank)
        {
            return rank switch
            {
                PlayingCard.CardRank.Two => twoRankLevel,
                PlayingCard.CardRank.Three => threeRankLevel,
                PlayingCard.CardRank.Four => fourRankLevel,
                PlayingCard.CardRank.Five => fiveRankLevel,
                PlayingCard.CardRank.Six => sixRankLevel,
                PlayingCard.CardRank.Seven => sevenRankLevel,
                PlayingCard.CardRank.Eight => eightRankLevel,
                PlayingCard.CardRank.Nine => nineRankLevel,
                PlayingCard.CardRank.Ten => tenRankLevel,
                PlayingCard.CardRank.Jack => jackRankLevel,
                PlayingCard.CardRank.Queen => queenRankLevel,
                PlayingCard.CardRank.King => kingRankLevel,
                PlayingCard.CardRank.Ace => aceRankLevel,
                _ => 1
            };
        }
    }
}

