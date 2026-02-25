using UnityEngine;
using ThreeStars;

public class CardModalDisplay : MonoBehaviour
{
    [Header("Prefabs (drag your prefabs here)")]
    [Tooltip("LEFT table row prefab (has Type/Name/Level/Bonus).")]
    [SerializeField] private CardRowUI cardRowPrefab;

    [Tooltip("RIGHT table row prefab (hands). This can be your HandRow prefab (Name/Level only).")]
    [SerializeField] private CardRowUI handRowPrefab;

    [Header("Content Parents (drag the ScrollView Content objects here)")]
    [Tooltip("Left side Content: 4 suits + 13 ranks (+ optional header row).")]
    [SerializeField] private Transform cardsContent;

    [Tooltip("Right side Content: hand entries (+ optional header row).")]
    [SerializeField] private Transform handsContent;

    [Header("Options")]
    [SerializeField] private bool addHeaderRows = true;

    [Header("Formatting")]
    [SerializeField] private int multiplierDecimals = 2;

    private void OnEnable()
    {
        BuildAndRefresh();
    }

    public void BuildAndRefresh()
    {
        var pm = GameManager.Instance.progressionManager;
        if (pm == null)
        {
            Debug.LogError("CardModalDisplay: ProgressionManager.Instance is NULL. Ensure ProgressionManager exists before opening the modal.");
            return;
        }

        if (cardRowPrefab == null || handRowPrefab == null || cardsContent == null || handsContent == null)
        {
            Debug.LogError("CardModalDisplay: Missing references. Assign BOTH prefabs + Cards Content + Hands Content in the Inspector.");
            return;
        }

        ClearChildren(cardsContent);
        ClearChildren(handsContent);

        // ---------- LEFT TABLE ----------
        if (addHeaderRows)
        {
            // Type | Name | Level | Bonus
            AddCardRow(cardsContent, "Type", "Name", 0, "Bonus", isHeader: true);
        }

        // Suits (4)
        AddCardRow(cardsContent, "Suit", "Entree", pm.entreeSuitLevel);
        AddCardRow(cardsContent, "Suit", "Side", pm.sideSuitLevel);
        AddCardRow(cardsContent, "Suit", "Vegetable", pm.vegSuitLevel);
        AddCardRow(cardsContent, "Suit", "Sauce", pm.sauceSuitLevel);

        // Ranks (13)
        AddCardRow(cardsContent, "Rank", "Two", pm.twoRankLevel);
        AddCardRow(cardsContent, "Rank", "Three", pm.threeRankLevel);
        AddCardRow(cardsContent, "Rank", "Four", pm.fourRankLevel);
        AddCardRow(cardsContent, "Rank", "Five", pm.fiveRankLevel);
        AddCardRow(cardsContent, "Rank", "Six", pm.sixRankLevel);
        AddCardRow(cardsContent, "Rank", "Seven", pm.sevenRankLevel);
        AddCardRow(cardsContent, "Rank", "Eight", pm.eightRankLevel);
        AddCardRow(cardsContent, "Rank", "Nine", pm.nineRankLevel);
        AddCardRow(cardsContent, "Rank", "Ten", pm.tenRankLevel);
        AddCardRow(cardsContent, "Rank", "Jack", pm.jackRankLevel);
        AddCardRow(cardsContent, "Rank", "Queen", pm.queenRankLevel);
        AddCardRow(cardsContent, "Rank", "King", pm.kingRankLevel);
        AddCardRow(cardsContent, "Rank", "Ace", pm.aceRankLevel);

        // ---------- RIGHT TABLE ----------
        if (addHeaderRows)
        {
            // Name | Mult
            AddHandRow(handsContent, "Name", 0f, isHeader: true);
        }

        AddHandRow(handsContent, "Pair", pm.pairMult);
        AddHandRow(handsContent, "Two Pair", pm.twoPairMult);
        AddHandRow(handsContent, "Three-of-a-Kind", pm.tripsMult);
        AddHandRow(handsContent, "Straight", pm.straightMult);
        AddHandRow(handsContent, "Flush", pm.flushMult);
        AddHandRow(handsContent, "Rainbow", pm.rainbowMult);
        AddHandRow(handsContent, "Quads", pm.quadsMult);
        AddHandRow(handsContent, "Straight Flush", pm.straightFlushMult);
        AddHandRow(handsContent, "Royal Flush", pm.royalFlushMult);
        AddHandRow(handsContent, "High Card", pm.highCardMult);
    }

    private static void ClearChildren(Transform parent)
    {
        for (int i = parent.childCount - 1; i >= 0; i--)
            Object.Destroy(parent.GetChild(i).gameObject);
    }

    // LEFT: Type/Name/Level/Bonus
    private void AddCardRow(Transform parent, string type, string name, int level, string bonusOverride = null, bool isHeader = false)
    {
        var row = Instantiate(cardRowPrefab, parent);

        if (isHeader)
        {
            row.SetData(type, name, "Level", bonusOverride ?? "Bonus");
            return;
        }

        string bonus = bonusOverride ?? $"+{Mathf.Max(0, level - 1)}";
        row.SetData(type, name, level.ToString(), bonus);
    }

    // RIGHT: Name/Mult (uses HandRow prefab so it won’t have Type/Bonus wired)
    private void AddHandRow(Transform parent, string name, float multiplier, bool isHeader = false)
    {
        var row = Instantiate(handRowPrefab, parent);

        if (isHeader)
        {
            // Put "Name" in name column, "Mult" in level column.
            row.SetData("", name, "Mult", "");
            return;
        }

        string multText = $"x{multiplier.ToString($"F{multiplierDecimals}")}";
        row.SetData("", name, multText, "");
    }
}