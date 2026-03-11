using ThreeStars;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class ScoreManager : MonoBehaviour
{
    public HandEvaluator handEvaluator;

    private void Awake()
    {
        handEvaluator = GetComponentInChildren<HandEvaluator>();

    }
    
    public int CalculateScore(List<CardInstance> hand, out HandEvaluator.HandRank rank)
    {
        var prog = GameManager.Instance.progressionManager;

        rank = handEvaluator.EvaluateHand(hand);
        if (rank == HandEvaluator.HandRank.None)
        {
            return 0;
        }
        float multiplier = GetTotalMult(hand, rank);
        float totalPips = 0;

        foreach (var card in hand)
        {
            totalPips += GetCardPips(card);
        }

        int finalScore = Mathf.RoundToInt(totalPips * multiplier);

        prog.RegisterPlay(hand);

        return finalScore;

    }

    public float GetCardPips(CardInstance card)
    {
        var prog = GameManager.Instance.progressionManager;
        float pips = GetBasePips(card.cardData.cardRank);

        foreach (var chef in prog.activeChefs)
        {
            if (chef.data.effectType == ChefEffectType.AdditivePips)
            {
                if (card.cardData.cardSuit == chef.data.targetSuit || 
                    (card.cardData.cardRank <= chef.data.targetRankHigh && card.cardData.cardRank >= chef.data.targetRankLow))
                {
                    pips += chef.data.effectMagnitude;
                }
            }
            else if (chef.data.effectType == ChefEffectType.MultiplicativePips)
            {
                if (card.cardData.cardSuit == chef.data.targetSuit || 
                    (card.cardData.cardRank <= chef.data.targetRankHigh && card.cardData.cardRank >= chef.data.targetRankLow))
                {
                    pips *= chef.data.effectMagnitude;
                }
            }
        }

        pips += prog.GetSuitBonusPips(card.cardData.cardSuit);
        pips += prog.GetRankBonusPips(card.cardData.cardRank);
        return pips;
    }

    public float GetTotalMult(List<CardInstance> hand, HandEvaluator.HandRank rank)
    {
        var prog = GameManager.Instance.progressionManager;
        float multiplier = GetHandMult(rank, prog);

        foreach (var chef in prog.activeChefs)
        {
            if (chef.data.effectType == ChefEffectType.Multiplier)
            {
                if (rank == chef.data.targetHand || chef.data.targetHand == HandEvaluator.HandRank.None)
                {
                    multiplier *= chef.data.effectMagnitude;
                }
            }
        }

        return multiplier;
    }
    public int GetBasePips(PlayingCard.CardRank rank)
    {
        return rank switch
        {
            PlayingCard.CardRank.Two => 2,
            PlayingCard.CardRank.Three => 3,
            PlayingCard.CardRank.Four => 4,
            PlayingCard.CardRank.Five => 5,
            PlayingCard.CardRank.Six => 6,
            PlayingCard.CardRank.Seven => 7,
            PlayingCard.CardRank.Eight => 8,
            PlayingCard.CardRank.Nine => 9,
            PlayingCard.CardRank.Ten => 10,
            PlayingCard.CardRank.Jack => 11,
            PlayingCard.CardRank.Queen => 12,
            PlayingCard.CardRank.King => 13,
            PlayingCard.CardRank.Ace => 14,
            _ => 0,
        };
    }

    public float GetHandMult(HandEvaluator.HandRank rank, ProgressionManager prog)
    {
        return rank switch
        {
            HandEvaluator.HandRank.OnePair => prog.pairMult,
            HandEvaluator.HandRank.TwoPair => prog.twoPairMult,
            HandEvaluator.HandRank.Rainbow => prog.rainbowMult,
            HandEvaluator.HandRank.ThreeOfAKind => prog.tripsMult,
            HandEvaluator.HandRank.HighCard => prog.highCardMult,
            HandEvaluator.HandRank.Straight => prog.straightMult,
            HandEvaluator.HandRank.Flush => prog.flushMult,
            HandEvaluator.HandRank.FourOfAKind => prog.quadsMult,
            HandEvaluator.HandRank.StraightFlush => prog.straightFlushMult,
            HandEvaluator.HandRank.RoyalFlush => prog.royalFlushMult,
            _ => 1.0f,
        };
    }
}
