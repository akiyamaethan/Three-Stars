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

        string[] lines = csvFile.text.Split(new[] { "\r\n", "\r", "\n" }, System.StringSplitOptions.None);

        // Skip header row (index 0)
        for (int i = 1; i < lines.Length; i++)
        {
            if (string.IsNullOrWhiteSpace(lines[i])) continue;

            string[] values = lines[i].Split(',');

            CardData card = new CardData
            {
                Type = values[0].Trim(' ', '\"', '\r', '\n'),
                Name = values[1].Trim(' ', '\"', '\r', '\n'),
                Level = int.Parse(values[2].Trim(' ', '\"', '\r', '\n')),
                Bonus = float.Parse(values[3].Trim(' ', '\"', '\r', '\n'))
            };

            AllCards.Add(card);
        }
    }
}
