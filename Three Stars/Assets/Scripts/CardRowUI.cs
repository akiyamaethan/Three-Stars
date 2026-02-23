using TMPro;
using UnityEngine;

public class CardRowUI : MonoBehaviour
{
    public TMP_Text typeText;
    public TMP_Text nameText;
    public TMP_Text levelText;
    public TMP_Text bonusText;

    // CHANGED: level is now a string so we can show "x1.25" for hands
    public void SetData(string type, string name, string level, string bonus)
    {
        if (typeText) typeText.text = type;
        if (nameText) nameText.text = name;
        if (levelText) levelText.text = level;
        if (bonusText) bonusText.text = bonus;
        Debug.Log($"SetData: {type} | {name} | level={level} | bonus={bonus}");
    }
}