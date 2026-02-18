using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ThreeStars;

public class HandEvaluator : MonoBehaviour
{
    public enum HandRank
    {
        None,
        HighCard,
        Rainbow,
        OnePair,
        TwoPair,
        ThreeOfAKind,
        Straight,
        Flush,
        FourOfAKind,
        StraightFlush,
        RoyalFlush
    }

    // Main hand evaluator:
    public HandRank EvaluateHand(List<CardInstance> hand)
    {
        if (hand.Count != 4)
            return HandRank.None;
        bool isRoyalFlush = IsRoyalFlush(hand);
        bool isFlush = IsFlush(hand);
        bool isStraight = IsStraight(hand);
        bool isRainbow = IsRainbow(hand);
        var rankCounts = GetRankCounts(hand);
        if (isRoyalFlush)
            return HandRank.RoyalFlush;
        if (isFlush && isStraight)
            return HandRank.StraightFlush;
        if (rankCounts.Contains(4))
            return HandRank.FourOfAKind;
        if (isFlush)
            return HandRank.Flush;
        if (isStraight)
            return HandRank.Straight;
        if (rankCounts.Contains(3))
            return HandRank.ThreeOfAKind;
        if (rankCounts.FindAll(x => x == 2).Count == 2)
            return HandRank.TwoPair;
        if (rankCounts.Contains(2))
            return HandRank.OnePair;
        if (isRainbow)
            return HandRank.Rainbow;
        return HandRank.HighCard;
    }

    // Helper Functions:

    private bool IsRoyalFlush(List<CardInstance> hand)
    {
        var ranks = new HashSet<PlayingCard.CardRank>();
        foreach (var card in hand)
        {
            ranks.Add(card.cardData.cardRank);
        }
        var isBroadway = ranks.Contains(PlayingCard.CardRank.Jack) &&
               ranks.Contains(PlayingCard.CardRank.Queen) &&
               ranks.Contains(PlayingCard.CardRank.King) &&
               ranks.Contains(PlayingCard.CardRank.Ace);

        return isBroadway && IsFlush(hand);
    }
    private bool IsFlush(List<CardInstance> hand)
    {
        var firstSuit = hand[0].cardData.cardSuit;
        foreach (var card in hand)
        {
            if (card.cardData.cardSuit != firstSuit)
                return false;
        }
        return true;
    }

    private bool IsRainbow(List<CardInstance> hand)
    {
        var suits = new HashSet<PlayingCard.CardSuit>();
        foreach (var card in hand)
        {
            suits.Add(card.cardData.cardSuit);
        }
        return suits.Count == hand.Count;
    }

    private bool IsStraight(List<CardInstance> hand)
    {
        var uniqueRanks = new HashSet<int>();
        foreach (var card in hand)
        {
            uniqueRanks.Add((int)card.cardData.cardRank);
        }

        if (uniqueRanks.Count != 4) return false;

        var sortedRanks = new List<int>(uniqueRanks);
        sortedRanks.Sort();

        bool isAceLow = sortedRanks[0] == (int)PlayingCard.CardRank.Two &&
                        sortedRanks[1] == (int)PlayingCard.CardRank.Three &&
                        sortedRanks[2] == (int)PlayingCard.CardRank.Four &&
                        sortedRanks[3] == (int)PlayingCard.CardRank.Ace;
        if (isAceLow)
        {
            return true;
        }

        for (int i = 0; i < sortedRanks.Count - 1; i++)
        {
            if (sortedRanks[i + 1] != sortedRanks[i] + 1)
                return false;
        }
        return true;
    }

    private List<int> GetRankCounts(List<CardInstance> hand)
    {
        var ranks = new List<int>();
        var counts = new Dictionary<int, int>();
        var result = new List<int>();
        foreach (var card in hand)
        {
            ranks.Add((int)card.cardData.cardRank);
        }
        foreach (var rank in ranks)
        {
            if (!counts.ContainsKey(rank))
                counts[rank] = 0;
            counts[rank]++;
        }
        foreach (var key in counts.Keys)
        {
            result.Add(counts[key]);
        }
        return result;
    }

}
