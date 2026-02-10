// Assets/Scripts/ScoreRules.cs
using System.Collections.Generic;

public static class ScoreRules
{
    // Simple scoring:
    // Rank value (2..14) * category weight based on suit.
    // Spades(Entree)=4, Hearts(Side)=3, Diamonds(Veg)=2, Clubs(Sauce)=1
    public static int ScorePlayedCards(List<CardData> cards)
    {
        int score = 0;

        foreach (var c in cards)
        {
            if (c == null) continue;

            int r = (int)c.Rank;
            int weight = c.Suit switch
            {
                Suit.Spades => 4,
                Suit.Hearts => 3,
                Suit.Diamonds => 2,
                Suit.Clubs => 1,
                _ => 1
            };

            score += r * weight;
        }

        return score;
    }
}
