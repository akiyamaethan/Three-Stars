using TMPro;
using UnityEngine;

public class CardView : MonoBehaviour
{
    public CardData Data { get; private set; }

    [SerializeField] private TMP_Text label;

    public void Init(CardData data, string shortName)
    {
        Data = data;

        if (label == null)
        {
            label = GetComponentInChildren<TMP_Text>();
        }

        if (label != null)
        {
            label.text = shortName;
        }
    }
}
