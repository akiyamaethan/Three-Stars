using TMPro;
using UnityEngine;

public class CardRowUI : MonoBehaviour
{
    public TMP_Text typeText;
    public TMP_Text nameText;
    public TMP_Text levelText;
    public TMP_Text bonusText;

    // Existing (cards)
    public void SetData(string type, string name, int level, string bonus)
    {
        SetData(type, name, level.ToString(), bonus);
    }

    // NEW (hands or anything)
    public void SetData(string type, string name, string level, string bonus)
    {
        if (typeText) typeText.text = type;
        if (nameText) nameText.text = name;
        if (levelText) levelText.text = level;
        if (bonusText) bonusText.text = bonus;
    }
}