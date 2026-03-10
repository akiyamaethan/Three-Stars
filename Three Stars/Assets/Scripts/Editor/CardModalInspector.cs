using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public static class CardModalInspector
{
    private const string ExactSpritePath = "Assets/Scripts/Card Modal/NewCardModal.png";
    private const string SpriteName = "NewCardModal";
    private const string CardRowPrefabPath = "Assets/Prefabs/CardRow.prefab";

    private struct RunSummary
    {
        public bool sceneReady;
        public bool foundCanvasModal;
        public bool foundCardModalRoot;
        public bool foundModalPanel;
        public bool foundContent;
        public bool foundLeftArea;
        public bool foundRightArea;
        public bool foundGameManager;
        public bool foundSprite;
        public bool assignedSprite;
        public bool modalImageExists;
        public bool leftScrollOk;
        public bool rightScrollOk;
        public bool cardRowPrefabOk;
        public bool cardRowRefsAssigned;
        public bool sceneModified;
    }

    [MenuItem("Tools/Card Modal/Inspect And Apply Initial Background")]
    public static void PerformInspectionAndSetup()
    {
        RunSummary summary = new RunSummary();

        Debug.Log("<color=cyan><b>========== CARD MODAL INSPECTION START ==========\n</b></color>");

        Scene activeScene = SceneManager.GetActiveScene();
        Debug.Log($"<b>Active Scene:</b> {activeScene.name}");

        if (!activeScene.IsValid() || !activeScene.isLoaded)
        {
            Debug.LogError("No valid loaded scene is open. Open Test.unity before running this tool.");
            return;
        }

        if (activeScene.name != "Test")
        {
            Debug.LogWarning($"Expected scene name 'Test', but current scene is '{activeScene.name}'. Continuing anyway.");
        }

        // Find key scene objects
        GameObject canvasModal = GameObject.Find("CanvasModal");
        GameObject cardModalRoot = GameObject.Find("CardModalRoot");
        GameObject gameManager = GameObject.Find("Game Manager");

        summary.foundCanvasModal = canvasModal != null;
        summary.foundCardModalRoot = cardModalRoot != null;
        summary.foundGameManager = gameManager != null;

        LogFound("CanvasModal", canvasModal);
        LogFound("CardModalRoot", cardModalRoot);
        LogFound("Game Manager", gameManager);

        Transform modalPanel = null;
        Transform content = null;
        Transform leftTableArea = null;
        Transform rightLevelsArea = null;

        if (cardModalRoot != null)
        {
            modalPanel = cardModalRoot.transform.Find("ModalPanel");
            content = cardModalRoot.transform.Find("ModalPanel/Content");
            leftTableArea = cardModalRoot.transform.Find("ModalPanel/Content/LeftTableArea");
            rightLevelsArea = cardModalRoot.transform.Find("ModalPanel/Content/RightLevelsArea");
        }

        summary.foundModalPanel = modalPanel != null;
        summary.foundContent = content != null;
        summary.foundLeftArea = leftTableArea != null;
        summary.foundRightArea = rightLevelsArea != null;

        LogFound("ModalPanel", modalPanel);
        LogFound("ModalPanel/Content", content);
        LogFound("LeftTableArea", leftTableArea);
        LogFound("RightLevelsArea", rightLevelsArea);

        InspectAndSetupModalPanel(modalPanel, ref summary);
        InspectContentArea(content, ref summary);
        InspectGameManager(gameManager);
        InspectCardRowPrefab(ref summary);

        summary.sceneReady =
            summary.foundCanvasModal &&
            summary.foundCardModalRoot &&
            summary.foundModalPanel &&
            summary.foundContent &&
            summary.foundLeftArea &&
            summary.foundRightArea &&
            summary.foundGameManager;

        PrintFinalSummary(summary);

        if (summary.sceneModified)
        {
            Debug.Log("<color=yellow><b>Scene was modified.</b></color> Save the scene if the background assignment looks correct.");
        }

        Debug.Log("<color=cyan><b>=========== CARD MODAL INSPECTION END ===========</b></color>");
    }

    private static void InspectAndSetupModalPanel(Transform panelTransform, ref RunSummary summary)
    {
        Debug.Log("<b>--- ModalPanel Status ---</b>");

        if (panelTransform == null)
        {
            Debug.LogError("ModalPanel not found. Cannot inspect or assign background.");
            return;
        }

        GameObject panel = panelTransform.gameObject;
        RectTransform rect = panel.GetComponent<RectTransform>();
        Image img = panel.GetComponent<Image>();

        summary.modalImageExists = img != null;

        Debug.Log($"Panel Name: {panel.name}");
        Debug.Log($"Has Image Component: {summary.modalImageExists}");

        if (rect != null)
        {
            Debug.Log($"Rect Size: {rect.rect.width} x {rect.rect.height}");
            Debug.Log($"Anchor Min: {rect.anchorMin}");
            Debug.Log($"Anchor Max: {rect.anchorMax}");
            Debug.Log($"Pivot: {rect.pivot}");
            Debug.Log($"Local Scale: {rect.localScale}");
        }
        else
        {
            Debug.LogWarning("ModalPanel is missing RectTransform.");
        }

        Debug.Log($"Direct Children: {GetChildrenNames(panelTransform)}");

        if (img == null)
        {
            Debug.LogError("ModalPanel has no Image component, so background cannot be assigned.");
            return;
        }

        Sprite sprite = FindSpriteByNameOrPath(SpriteName, ExactSpritePath);
        summary.foundSprite = sprite != null;

        if (sprite == null)
        {
            Debug.LogError($"Could not find sprite '{SpriteName}'. Checked exact path: {ExactSpritePath}");
            return;
        }

        Debug.Log($"Found Sprite: {sprite.name}");
        Debug.Log($"Sprite Border: {sprite.border}");

        Undo.RecordObject(img, "Assign Card Modal Background");

        img.sprite = sprite;
        img.color = Color.white;
        img.raycastTarget = true;

        if (sprite.border.sqrMagnitude > 0.0001f)
        {
            img.type = Image.Type.Sliced;
            Debug.Log("<color=green>Sprite has borders. Image Type set to Sliced.</color>");
        }
        else
        {
            img.type = Image.Type.Simple;
            Debug.LogWarning("Sprite has no borders defined. Image Type set to Simple.");
        }

        EditorUtility.SetDirty(img);
        EditorSceneManager.MarkSceneDirty(panel.scene);

        summary.assignedSprite = img.sprite == sprite;
        summary.sceneModified = summary.assignedSprite;

        Debug.Log($"Background Assigned Successfully: {summary.assignedSprite}");
        Debug.Log($"Image Type After Assignment: {img.type}");
    }

    private static void InspectContentArea(Transform content, ref RunSummary summary)
    {
        Debug.Log("<b>--- Content Area Status ---</b>");

        if (content == null)
        {
            Debug.LogError("Content object not found.");
            return;
        }

        Debug.Log($"Content Direct Children: {GetChildrenNames(content)}");

        Transform leftArea = content.Find("LeftTableArea");
        Transform rightArea = content.Find("RightLevelsArea");

        InspectTableArea(leftArea, "LeftTableArea", ref summary.leftScrollOk);
        InspectTableArea(rightArea, "RightLevelsArea", ref summary.rightScrollOk);
    }

    private static void InspectTableArea(Transform area, string label, ref bool areaOk)
    {
        Debug.Log($"<b>--- {label} ---</b>");

        if (area == null)
        {
            Debug.LogError($"{label} is missing.");
            areaOk = false;
            return;
        }

        Debug.Log($"{label} Children: {GetChildrenNames(area)}");

        ScrollRect scrollRect = area.GetComponentInChildren<ScrollRect>(true);
        if (scrollRect == null)
        {
            Debug.LogWarning($"{label}: No ScrollRect found under this area.");
            areaOk = false;
            return;
        }

        Debug.Log($"{label}: ScrollRect found on '{scrollRect.gameObject.name}'");

        Transform viewport = scrollRect.viewport != null ? scrollRect.viewport : scrollRect.transform.Find("Viewport");
        RectTransform contentRef = scrollRect.content;

        Debug.Log($"{label}: Viewport Reference = {(viewport != null ? viewport.name : "NULL")}");
        Debug.Log($"{label}: Content Reference = {(contentRef != null ? contentRef.name : "NULL")}");

        bool hasMask = false;
        if (viewport != null)
        {
            hasMask = viewport.GetComponent<Mask>() != null || viewport.GetComponent<RectMask2D>() != null;
            Debug.Log($"{label}: Viewport Has Mask/RectMask2D = {hasMask}");
            Debug.Log($"{label}: Viewport Children = {GetChildrenNames(viewport)}");
        }

        bool hasLayoutGroup = false;
        bool hasContentSizeFitter = false;
        if (contentRef != null)
        {
            hasLayoutGroup =
                contentRef.GetComponent<VerticalLayoutGroup>() != null ||
                contentRef.GetComponent<HorizontalLayoutGroup>() != null ||
                contentRef.GetComponent<GridLayoutGroup>() != null;

            hasContentSizeFitter = contentRef.GetComponent<ContentSizeFitter>() != null;

            Debug.Log($"{label}: Content Has Layout Group = {hasLayoutGroup}");
            Debug.Log($"{label}: Content Has ContentSizeFitter = {hasContentSizeFitter}");
            Debug.Log($"{label}: Content Children = {GetChildrenNames(contentRef)}");
        }

        areaOk = viewport != null && contentRef != null;
    }

    private static void InspectGameManager(GameObject manager)
    {
        Debug.Log("<b>--- Game Manager Script Status ---</b>");

        if (manager == null)
        {
            Debug.LogError("Game Manager not found.");
            return;
        }

        CheckComponent(manager, "GameManager");
        CheckComponent(manager, "CardModalController");
        CheckComponent(manager, "CardDatabase");
        CheckComponent(manager, "CardTableLoader");
    }

    private static void InspectCardRowPrefab(ref RunSummary summary)
    {
        Debug.Log("<b>--- CardRow Prefab Status ---</b>");

        GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(CardRowPrefabPath);
        if (prefab == null)
        {
            Debug.LogError($"CardRow prefab not found at path: {CardRowPrefabPath}");
            summary.cardRowPrefabOk = false;
            return;
        }

        summary.cardRowPrefabOk = true;

        RectTransform rect = prefab.GetComponent<RectTransform>();
        Image image = prefab.GetComponent<Image>();
        LayoutElement layoutElement = prefab.GetComponent<LayoutElement>();
        HorizontalLayoutGroup hlg = prefab.GetComponent<HorizontalLayoutGroup>();
        CardRowUI rowUI = prefab.GetComponent<CardRowUI>();

        Debug.Log($"Prefab Loaded: {prefab.name}");
        Debug.Log($"Has RectTransform: {rect != null}");
        if (rect != null)
        {
            Debug.Log($"Prefab Rect Size: {rect.rect.width} x {rect.rect.height}");
            Debug.Log($"Prefab Anchor Min: {rect.anchorMin}");
            Debug.Log($"Prefab Anchor Max: {rect.anchorMax}");
            Debug.Log($"Prefab Pivot: {rect.pivot}");
        }

        Debug.Log($"Has Image: {image != null}");
        Debug.Log($"Has LayoutElement: {layoutElement != null}");
        if (layoutElement != null)
        {
            Debug.Log($"Preferred Height: {layoutElement.preferredHeight}");
            Debug.Log($"Preferred Width: {layoutElement.preferredWidth}");
        }

        Debug.Log($"Has HorizontalLayoutGroup: {hlg != null}");

        if (rowUI == null)
        {
            Debug.LogError("CardRowUI component is missing from CardRow prefab.");
            summary.cardRowRefsAssigned = false;
            return;
        }

        Debug.Log("CardRowUI: Found");

        bool typeAssigned = rowUI.typeText != null;
        bool nameAssigned = rowUI.nameText != null;
        bool levelAssigned = rowUI.levelText != null;
        bool bonusAssigned = rowUI.bonusText != null;

        summary.cardRowRefsAssigned = typeAssigned && nameAssigned && levelAssigned && bonusAssigned;

        LogTMPField("TypeText", rowUI.typeText);
        LogTMPField("NameText", rowUI.nameText);
        LogTMPField("LevelText", rowUI.levelText);
        LogTMPField("BonusText", rowUI.bonusText);

        Debug.Log($"All 4 TMP References Assigned: {summary.cardRowRefsAssigned}");
    }

    private static void LogTMPField(string label, TMP_Text textField)
    {
        if (textField == null)
        {
            Debug.LogWarning($"{label}: NULL");
            return;
        }

        string componentType = textField.GetType().Name;
        Debug.Log($"{label}: Assigned to '{textField.gameObject.name}' ({componentType})");
    }

    private static void CheckComponent(GameObject go, string typeName)
    {
        Component comp = go.GetComponent(typeName);
        if (comp == null)
        {
            Debug.LogWarning($"{typeName}: NOT FOUND");
            return;
        }

        bool enabled = true;
        if (comp is Behaviour behaviour)
        {
            enabled = behaviour.enabled;
        }

        Debug.Log($"{typeName}: Found (Enabled = {enabled})");
    }

    private static void LogFound(string label, Object obj)
    {
        Debug.Log($"{label}: {(obj != null ? "FOUND" : "MISSING")}");
    }

    private static string GetChildrenNames(Transform parent)
    {
        if (parent == null) return "None";

        List<string> names = new List<string>();
        for (int i = 0; i < parent.childCount; i++)
        {
            names.Add(parent.GetChild(i).name);
        }

        return names.Count > 0 ? string.Join(", ", names) : "None";
    }

    private static Sprite FindSpriteByNameOrPath(string spriteName, string exactPath)
    {
        Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>(exactPath);
        if (sprite != null) return sprite;

        string[] guids = AssetDatabase.FindAssets($"{spriteName} t:Sprite");
        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            Sprite candidate = AssetDatabase.LoadAssetAtPath<Sprite>(path);
            if (candidate != null && candidate.name == spriteName)
            {
                return candidate;
            }
        }

        return null;
    }

    private static void PrintFinalSummary(RunSummary summary)
    {
        string modalStructure = (
            summary.foundCanvasModal &&
            summary.foundCardModalRoot &&
            summary.foundModalPanel &&
            summary.foundContent &&
            summary.foundLeftArea &&
            summary.foundRightArea
        ) ? "PASS" : "FAIL";

        string backgroundStatus = (summary.foundSprite && summary.modalImageExists && summary.assignedSprite) ? "PASS" : "FAIL";

        string gmStatus = summary.foundGameManager ? "PASS/WARN" : "FAIL";
        string rowStatus = (summary.cardRowPrefabOk && summary.cardRowRefsAssigned) ? "PASS" : "WARN";
        string ready = (summary.sceneReady && summary.foundSprite && summary.modalImageExists) ? "YES" : "NO";

        Debug.Log(
            "<color=yellow><b>========== FINAL SUMMARY ==========\n" +
            $"Modal Structure Status: {modalStructure}\n" +
            $"Background Assignment Status: {backgroundStatus}\n" +
            $"Left Scroll Status: {(summary.leftScrollOk ? "PASS" : "WARN")}\n" +
            $"Right Scroll Status: {(summary.rightScrollOk ? "PASS" : "WARN")}\n" +
            $"Game Manager Script Status: {gmStatus}\n" +
            $"CardRow Prefab Status: {rowStatus}\n" +
            $"Ready For Next Phase: {ready}\n" +
            "===================================</b></color>"
        );
    }
}