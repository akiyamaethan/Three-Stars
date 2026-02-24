using ThreeStars;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance { get; private set; }
    private HandEvaluator handEvaluator;

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
            handEvaluator = GetComponent<HandEvaluator>();
        }
        handEvaluator = GetComponentInChildren<HandEvaluator>();

    }
    
    public int CalculateScore(List<CardInstance> hand, out HandEvaluator.HandRank rank)
    {
        rank = handEvaluator.EvaluateHand(hand);
        if (rank == HandEvaluator.HandRank.None)
        {
            return 0;
        }

        float totalPips = 0;
        var prog = ProgressionManager.Instance;
        foreach (var card in hand)
        {
            float basePips = GetBasePips(card.cardData.cardRank);
            basePips += prog.GetSuitBonusPips(card.cardData.cardSuit);
            basePips += prog.GetRankBonusPips(card.cardData.cardRank);
            totalPips += basePips;
        }

        float multiplier = GetHandMult(rank, prog);
        int finalScore = Mathf.RoundToInt(totalPips * multiplier);

        prog.RegisterPlay(hand);

        return finalScore;

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
