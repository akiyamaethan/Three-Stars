using TMPro;
using UnityEngine;

public class CardRowUI : MonoBehaviour
{
    public TMP_Text typeText;
    public TMP_Text nameText;
    public TMP_Text levelText;
    public TMP_Text bonusText;

    public void SetRow(string type, string name, string level, string bonus)
    {
        if (typeText != null) typeText.text = type;
        if (nameText != null) nameText.text = name;
        if (levelText != null) levelText.text = level;
        if (bonusText != null) bonusText.text = bonus;
    }

    // New compatibility wrapper
    public void SetData(string type, string name, string level, string bonus)
    {
        SetRow(type, name, level, bonus);
    }

    // Old CSV loader compatibility wrapper
    public void SetData(string type, string name, int level, string bonus)
    {
        SetRow(type, name, level.ToString(), bonus);
    }
}