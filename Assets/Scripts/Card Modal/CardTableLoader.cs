using UnityEngine;

public class CardTableLoader : MonoBehaviour
{
    public TextAsset cardsCSV;
    public Transform contentParent;
    public CardRowUI cardRowPrefab;

    void Start()
    {
        LoadCards();
    }

    void LoadCards()
    {
        if (cardsCSV == null || cardRowPrefab == null || contentParent == null)
        {
            Debug.LogError("CardTableLoader not wired correctly!");
            return;
        }

        string[] lines = cardsCSV.text.Split('\n');

        for (int i = 1; i < lines.Length; i++) // skip header
        {
            string line = lines[i].Trim();
            if (string.IsNullOrEmpty(line)) continue;

            string[] values = line.Split(',');

            string type = values[0];
            string name = values[1];
            int level = int.Parse(values[2]);
            string bonus = $"x{values[3]}";

            CardRowUI row = Instantiate(cardRowPrefab, contentParent);
            row.SetData(type, name, level.ToString(), bonus);
        }
    }
}
