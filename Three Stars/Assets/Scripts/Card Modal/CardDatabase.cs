using System.Collections.Generic;
using UnityEngine;

public class CardDatabase : MonoBehaviour
{
    public List<CardData> AllCards = new List<CardData>();

    void Awake()
    {
        LoadCardsFromCSV();
    }

    void LoadCardsFromCSV()
    {
        TextAsset csvFile = Resources.Load<TextAsset>("cards");
        if (csvFile == null)
        {
            Debug.LogError("cards.csv not found in Resources!");
            return;
        }

        string[] lines = csvFile.text.Split('\n');

        // Skip header row (index 0)
        for (int i = 1; i < lines.Length; i++)
        {
            if (string.IsNullOrWhiteSpace(lines[i])) continue;

            string[] values = lines[i].Split(',');

            CardData card = new CardData
            {
                Type = values[0].Trim(' ', '\"'),
                Name = values[1].Trim(' ', '\"'),
                Level = int.Parse(values[2].Trim(' ', '\"')),
                Bonus = float.Parse(values[3].Trim(' ', '\"'))
            };

            AllCards.Add(card);
        }
    }
}
