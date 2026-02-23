using TMPro;
using UnityEngine;

public class CardRowUI : MonoBehaviour
{
    public TMP_Text typeText;
    public TMP_Text nameText;
    public TMP_Text levelText;
    public TMP_Text bonusText;

    public void SetData(string type, string name, int level, string bonus)
    {
        typeText.text = type;
        nameText.text = name;
        levelText.text = level.ToString();
        bonusText.text = bonus;
    }
}
