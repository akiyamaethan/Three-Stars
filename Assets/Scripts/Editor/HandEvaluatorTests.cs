using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using ThreeStars;

public class HandEvaluatorTests : MonoBehaviour
{
    private HandEvaluator handEvaluator;
    [SetUp]
    public void Setup()
    {
        GameObject go = new GameObject();
        handEvaluator = go.AddComponent<HandEvaluator>();
    }

    private List<CardInstance> CreateHand(params (PlayingCard.CardSuit suit, PlayingCard.CardRank rank)[] cards)
    {
        var hand = new List<CardInstance>();
        foreach (var card in cards)
        {
            var cardData = ScriptableObject.CreateInstance<PlayingCard>();
            cardData.cardSuit = card.suit;
            cardData.cardRank = card.rank;
            hand.Add(new CardInstance(cardData));
        }
        return hand;
    }

    [Test]
    public void HandEvaluatorCorrectlyReturnsRoyalFlush()
    {
        var hand = CreateHand(
            (PlayingCard.CardSuit.Entree, PlayingCard.CardRank.Ace),
            (PlayingCard.CardSuit.Entree, PlayingCard.CardRank.King),
            (PlayingCard.CardSuit.Entree, PlayingCard.CardRank.Queen),
            (PlayingCard.CardSuit.Entree, PlayingCard.CardRank.Jack)
        );

        var result = handEvaluator.EvaluateHand(hand);
        Assert.AreEqual(HandEvaluator.HandRank.RoyalFlush, result);
    }

    [Test]
    public void HandEvaluatorCorrectlyReturnsStraightFlush()
    {
        var hand = CreateHand(
            (PlayingCard.CardSuit.Entree, PlayingCard.CardRank.Ten),
            (PlayingCard.CardSuit.Entree, PlayingCard.CardRank.King),
            (PlayingCard.CardSuit.Entree, PlayingCard.CardRank.Queen),
            (PlayingCard.CardSuit.Entree, PlayingCard.CardRank.Jack)
        );

        var result = handEvaluator.EvaluateHand(hand);
        Assert.AreEqual(HandEvaluator.HandRank.StraightFlush, result);
    }

    [Test]
    public void HandEvaluatorCorrectlyReturnsQuads()
    {
        var hand = CreateHand(
            (PlayingCard.CardSuit.Entree, PlayingCard.CardRank.Ace),
            (PlayingCard.CardSuit.Side, PlayingCard.CardRank.Ace),
            (PlayingCard.CardSuit.Vegetable, PlayingCard.CardRank.Ace),
            (PlayingCard.CardSuit.Sauce, PlayingCard.CardRank.Ace)
        );

        var result = handEvaluator.EvaluateHand(hand);
        Assert.AreEqual(HandEvaluator.HandRank.FourOfAKind, result);
    }

    [Test]
    public void HandEvaluatorCorrectlyReturnsFlush()
    {
        var hand = CreateHand(
            (PlayingCard.CardSuit.Entree, PlayingCard.CardRank.Ten),
            (PlayingCard.CardSuit.Entree, PlayingCard.CardRank.Ace),
            (PlayingCard.CardSuit.Entree, PlayingCard.CardRank.Queen),
            (PlayingCard.CardSuit.Entree, PlayingCard.CardRank.Jack)
        );

        var result = handEvaluator.EvaluateHand(hand);
        Assert.AreEqual(HandEvaluator.HandRank.Flush, result);
    }

    [Test]
    public void HandEvaluatorCorrectlyReturnsStraight()
    {
        var hand = CreateHand(
            (PlayingCard.CardSuit.Entree, PlayingCard.CardRank.Ten),
            (PlayingCard.CardSuit.Sauce, PlayingCard.CardRank.King),
            (PlayingCard.CardSuit.Entree, PlayingCard.CardRank.Queen),
            (PlayingCard.CardSuit.Side, PlayingCard.CardRank.Jack)
        );

        var result = handEvaluator.EvaluateHand(hand);
        Assert.AreEqual(HandEvaluator.HandRank.Straight, result);
    }

    [Test]
    public void HandEvaluatorCorrectlyReturnsTrips()
    {
        var hand = CreateHand(
            (PlayingCard.CardSuit.Entree, PlayingCard.CardRank.Ten),
            (PlayingCard.CardSuit.Sauce, PlayingCard.CardRank.Ten),
            (PlayingCard.CardSuit.Entree, PlayingCard.CardRank.Queen),
            (PlayingCard.CardSuit.Side, PlayingCard.CardRank.Ten)
        );

        var result = handEvaluator.EvaluateHand(hand);
        Assert.AreEqual(HandEvaluator.HandRank.ThreeOfAKind, result);
    }

    [Test]
    public void HandEvaluatorCorrectlyReturns2Pair()
    {
        var hand = CreateHand(
            (PlayingCard.CardSuit.Entree, PlayingCard.CardRank.Ten),
            (PlayingCard.CardSuit.Sauce, PlayingCard.CardRank.Ten),
            (PlayingCard.CardSuit.Entree, PlayingCard.CardRank.Queen),
            (PlayingCard.CardSuit.Side, PlayingCard.CardRank.Queen)
        );

        var result = handEvaluator.EvaluateHand(hand);
        Assert.AreEqual(HandEvaluator.HandRank.TwoPair, result);
    }

    [Test]
    public void HandEvaluatorCorrectlyReturnsPair()
    {
        var hand = CreateHand(
            (PlayingCard.CardSuit.Entree, PlayingCard.CardRank.Ten),
            (PlayingCard.CardSuit.Sauce, PlayingCard.CardRank.Ten),
            (PlayingCard.CardSuit.Entree, PlayingCard.CardRank.Eight),
            (PlayingCard.CardSuit.Side, PlayingCard.CardRank.Queen)
        );

        var result = handEvaluator.EvaluateHand(hand);
        Assert.AreEqual(HandEvaluator.HandRank.OnePair, result);
    }

    [Test]
    public void HandEvaluatorCorrectlyReturnsRainbow()
    {
        var hand = CreateHand(
            (PlayingCard.CardSuit.Entree, PlayingCard.CardRank.Ten),
            (PlayingCard.CardSuit.Sauce, PlayingCard.CardRank.Four),
            (PlayingCard.CardSuit.Vegetable, PlayingCard.CardRank.Eight),
            (PlayingCard.CardSuit.Side, PlayingCard.CardRank.Five)
        );

        var result = handEvaluator.EvaluateHand(hand);
        Assert.AreEqual(HandEvaluator.HandRank.Rainbow, result);
    }

    [Test]
    public void HandEvaluatorCorrectlyReturnsHighcard()
    {
        var hand = CreateHand(
            (PlayingCard.CardSuit.Entree, PlayingCard.CardRank.Ten),
            (PlayingCard.CardSuit.Sauce, PlayingCard.CardRank.Four),
            (PlayingCard.CardSuit.Vegetable, PlayingCard.CardRank.Eight),
            (PlayingCard.CardSuit.Vegetable, PlayingCard.CardRank.Five)
        );

        var result = handEvaluator.EvaluateHand(hand);
        Assert.AreEqual(HandEvaluator.HandRank.HighCard, result);
    }
}
