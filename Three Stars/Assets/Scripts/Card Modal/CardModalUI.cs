using System.Collections.Generic;
using UnityEngine;
using ThreeStars;

public class CardModalUI : MonoBehaviour
{
    [Header("Left Side: Cards")]
    [SerializeField] private RectTransform leftContent;
    [SerializeField] private GameObject cardRowPrefab;

    [Header("Right Side: Hands")]
    [SerializeField] private RectTransform rightContent;
    [SerializeField] private GameObject handRowPrefab;

    [Header("Hover Preview")]
    [SerializeField] public HandHoverPreviewController hoverController;
    [SerializeField] private List<HandPreviewMapping> handPreviews = new();

    [Header("Optional")]
    [SerializeField] private bool refreshOnEnable = false;

    [System.Serializable]
    public struct HandPreviewMapping
    {
        public HandEvaluator.HandRank rank;
        public Sprite previewSprite;
    }

    private readonly List<GameObject> spawnedLeftRows = new();
    private readonly List<GameObject> spawnedRightRows = new();

    private void OnEnable()
    {
        if (refreshOnEnable)
        {
            Refresh();
        }
    }

    public void Refresh()
    {
        if (!ValidateReferences()) return;

        if (GameManager.Instance == null || GameManager.Instance.progressionManager == null)
        {
            Debug.LogWarning("CardModalUI: ProgressionManager is not available.");
            return;
        }

        ClearSpawnedRows();
        PopulateLeftCards();
        PopulateRightHands();
    }

    private bool ValidateReferences()
    {
        bool ok = true;

        if (leftContent == null)
        {
            Debug.LogError("CardModalUI: leftContent is not assigned.");
            ok = false;
        }

        if (cardRowPrefab == null)
        {
            Debug.LogError("CardModalUI: cardRowPrefab is not assigned.");
            ok = false;
        }

        if (rightContent == null)
        {
            Debug.LogError("CardModalUI: rightContent is not assigned.");
            ok = false;
        }

        if (handRowPrefab == null)
        {
            Debug.LogError("CardModalUI: handRowPrefab is not assigned.");
            ok = false;
        }

        return ok;
    }

    private void ClearSpawnedRows()
    {
        if (hoverController != null)
        {
            hoverController.HidePreview();
        }
        foreach (var row in spawnedLeftRows)
        {
            if (row != null) Destroy(row);
        }
        spawnedLeftRows.Clear();

        foreach (var row in spawnedRightRows)
        {
            if (row != null) Destroy(row);
        }
        spawnedRightRows.Clear();
    }

    private void PopulateLeftCards()
    {
        var prog = GameManager.Instance.progressionManager;

        foreach (PlayingCard.CardSuit suit in System.Enum.GetValues(typeof(PlayingCard.CardSuit)))
        {
            int level = prog.GetSuitLevel(suit);
            int bonus = prog.GetSuitBonusPips(suit);
            string suitName = FormatSuitName(suit);

            CardRowUI row = Instantiate(cardRowPrefab, leftContent).GetComponent<CardRowUI>();
            if (row == null)
            {
                Debug.LogError("CardModalUI: cardRowPrefab is missing CardRowUI.");
                continue;
            }

            row.SetRow("Suit", suitName, level.ToString(), bonus.ToString());
            spawnedLeftRows.Add(row.gameObject);
        }

        foreach (PlayingCard.CardRank rank in System.Enum.GetValues(typeof(PlayingCard.CardRank)))
        {
            int level = prog.GetRankLevel(rank);
            int bonus = prog.GetRankBonusPips(rank);
            string rankName = FormatRankName(rank);

            CardRowUI row = Instantiate(cardRowPrefab, leftContent).GetComponent<CardRowUI>();
            if (row == null)
            {
                Debug.LogError("CardModalUI: cardRowPrefab is missing CardRowUI.");
                continue;
            }

            row.SetRow("Rank", rankName, level.ToString(), bonus.ToString());
            spawnedLeftRows.Add(row.gameObject);
        }
    }

    private void PopulateRightHands()
    {
        var displayOrder = new List<HandEvaluator.HandRank>
        {
            HandEvaluator.HandRank.RoyalFlush,
            HandEvaluator.HandRank.StraightFlush,
            HandEvaluator.HandRank.FourOfAKind,
            HandEvaluator.HandRank.Flush,
            HandEvaluator.HandRank.Straight,
            HandEvaluator.HandRank.ThreeOfAKind,
            HandEvaluator.HandRank.TwoPair,
            HandEvaluator.HandRank.OnePair,
            HandEvaluator.HandRank.Rainbow,
            HandEvaluator.HandRank.HighCard,

        };
        foreach (HandEvaluator.HandRank rank in displayOrder)
        { 
            float mult = GetHandMultiplier(rank);
            string handName = HandEvaluator.GetThemedHandName(rank);

            CardRowUI row = Instantiate(handRowPrefab, rightContent).GetComponent<CardRowUI>();
            if (row == null)
            {
                Debug.LogError("CardModalUI: handRowPrefab is missing CardRowUI.");
                continue;
            }

            HandHoverTarget hoverTarget = row.GetComponent<HandHoverTarget>();
            if (hoverTarget != null && hoverController != null)
            {
                Sprite previewSprite = GetPreviewSpriteForRank(rank);
                string description = GetHandDescription(rank);
                hoverTarget.Setup(hoverController, previewSprite, description);
            }
            else if (hoverTarget == null)
            {
                Debug.LogWarning("Missing hover target");
            }

            row.SetRow("", handName, "", FormatMultiplier(mult));
            spawnedRightRows.Add(row.gameObject);
        }
    }

    private Sprite GetPreviewSpriteForRank(HandEvaluator.HandRank rank)
    {
        if (rank == HandEvaluator.HandRank.None) return null;

        foreach (HandPreviewMapping mapping in handPreviews)
        {
            if (mapping.rank == rank)
            {
                return mapping.previewSprite;
            }
        }

        Debug.LogWarning($"CardModalUI: No preview sprite assigned for hand rank {rank}.");
        return null;
    }

    private float GetHandMultiplier(HandEvaluator.HandRank rank)
    {
        var prog = GameManager.Instance.progressionManager;

        return rank switch
        {
            HandEvaluator.HandRank.HighCard => prog.highCardMult,
            HandEvaluator.HandRank.OnePair => prog.pairMult,
            HandEvaluator.HandRank.TwoPair => prog.twoPairMult,
            HandEvaluator.HandRank.ThreeOfAKind => prog.tripsMult,
            HandEvaluator.HandRank.Straight => prog.straightMult,
            HandEvaluator.HandRank.Flush => prog.flushMult,
            HandEvaluator.HandRank.FourOfAKind => prog.quadsMult,
            HandEvaluator.HandRank.StraightFlush => prog.straightFlushMult,
            HandEvaluator.HandRank.RoyalFlush => prog.royalFlushMult,
            HandEvaluator.HandRank.Rainbow => prog.rainbowMult,
            _ => 1.0f
        };
    }

    private string FormatMultiplier(float mult)
    {
        return $"x{mult:0.##}";
    }

    private string FormatSuitName(PlayingCard.CardSuit suit)
    {
        return suit switch
        {
            PlayingCard.CardSuit.Entree => "Entree",
            PlayingCard.CardSuit.Side => "Side",
            PlayingCard.CardSuit.Sauce => "Sauce",
            PlayingCard.CardSuit.Vegetable => "Vegetable",
            _ => suit.ToString()
        };
    }

    private string FormatRankName(PlayingCard.CardRank rank)
    {
        return rank switch
        {
            PlayingCard.CardRank.Ace => "Ace",
            PlayingCard.CardRank.Two => "Two",
            PlayingCard.CardRank.Three => "Three",
            PlayingCard.CardRank.Four => "Four",
            PlayingCard.CardRank.Five => "Five",
            PlayingCard.CardRank.Six => "Six",
            PlayingCard.CardRank.Seven => "Seven",
            PlayingCard.CardRank.Eight => "Eight",
            PlayingCard.CardRank.Nine => "Nine",
            PlayingCard.CardRank.Ten => "Ten",
            PlayingCard.CardRank.Jack => "Jack",
            PlayingCard.CardRank.Queen => "Queen",
            PlayingCard.CardRank.King => "King",
            _ => rank.ToString()
        };
    }

    private string GetHandDescription(HandEvaluator.HandRank rank)
    {
        return rank switch
        {
            HandEvaluator.HandRank.HighCard => "Dish that satisfies no other dish requirements",
            HandEvaluator.HandRank.OnePair => "Two ingredients of the same rank",
            HandEvaluator.HandRank.TwoPair => "Two sets of two ingredients with the same rank",
            HandEvaluator.HandRank.ThreeOfAKind => "Three ingredients of the same rank",
            HandEvaluator.HandRank.Straight => "Four ingredients whose ranks form a sequence",
            HandEvaluator.HandRank.Flush => "Four ingredients of the same suit",
            HandEvaluator.HandRank.FourOfAKind => "Four ingredients of the same rank",
            HandEvaluator.HandRank.StraightFlush => "Dish that satisfies both Buffet and Flight requirements",
            HandEvaluator.HandRank.RoyalFlush => "J, Q, K, A of the same suit",
            HandEvaluator.HandRank.Rainbow => "One ingredient of each suit",
            _=> ""
        };
    }
}