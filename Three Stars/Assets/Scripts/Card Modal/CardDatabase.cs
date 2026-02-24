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
            string line = lines[i].Trim();
            if (string.IsNullOrWhiteSpace(line)) continue;

            // Simple CSV parser for quoted strings
            List<string> values = new List<string>();
            bool inQuotes = false;
            string currentField = "";
            
            for (int j = 0; j < line.Length; j++)
            {
                char c = line[j];
                if (c == '\"')
                {
                    inQuotes = !inQuotes;
                }
                else if (c == ',' && !inQuotes)
                {
                    values.Add(currentField.Trim());
                    currentField = "";
                }
                else
                {
                    currentField += c;
                }
            }
            values.Add(currentField.Trim());

            if (values.Count >= 4)
            {
                CardData card = new CardData
                {
                    Type = values[0],
                    Name = values[1],
                    Level = int.Parse(values[2]),
                    Bonus = float.Parse(values[3])
                };
                AllCards.Add(card);
            }
        }
    }
}
