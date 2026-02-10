using System;

public enum Suit { Clubs, Diamonds, Hearts, Spades }

public enum Rank
{
    Two = 2, Three, Four, Five, Six, Seven, Eight, Nine, Ten,
    Jack, Queen, King, Ace
}

[Serializable]
public class CardData
{
    public Suit Suit;
    public Rank Rank;

    public CardData(Suit suit, Rank rank)
    {
        Suit = suit;
        Rank = rank;
    }

    public override string ToString()
    {
        return $"{Rank} of {Suit}";
    }
}
