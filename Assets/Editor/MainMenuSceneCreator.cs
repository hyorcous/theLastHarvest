using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditor.Events;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class MainMenuSceneCreator
{
    // ─────────────────────────────────────────────────────────────────
    //  Tools > 🌾 Main Menu Sahnesi Oluştur   menüsünden çalıştırın
    // ─────────────────────────────────────────────────────────────────
    [MenuItem("Tools/\U0001f33e Main Menu Sahnesi Oluştur")]
    public static void CreateMainMenuScene()
    {
        // Yeni boş sahne
        var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

        // ── Kamera ──────────────────────────────────────────────────
        var camGO = new GameObject("Main Camera");
        camGO.tag = "MainCamera";
        var cam = camGO.AddComponent<Camera>();
        cam.clearFlags   = CameraClearFlags.SolidColor;
        cam.backgroundColor = new Color(0.051f, 0.039f, 0.020f);
        cam.orthographic = true;

        // ── Event System ─────────────────────────────────────────────
        var esGO = new GameObject("EventSystem");
        esGO.AddComponent<EventSystem>();
        esGO.AddComponent<StandaloneInputModule>();

        // ── Canvas ───────────────────────────────────────────────────
        var canvasGO  = new GameObject("Canvas");
        var canvas    = canvasGO.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;

        var scaler = canvasGO.AddComponent<CanvasScaler>();
        scaler.uiScaleMode        = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        scaler.screenMatchMode    = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
        scaler.matchWidthOrHeight  = 0.5f;

        canvasGO.AddComponent<GraphicRaycaster>();

        // MainMenuUI bileşeni
        var menuUI = canvasGO.AddComponent<MainMenuUI>();

        // ── Arkaplan ─────────────────────────────────────────────────
        MakePanel(canvasGO.transform, "Background",
            Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero,
            new Color(0.051f, 0.039f, 0.020f, 1f));

        // Hafif karanlık üst katman
        MakePanel(canvasGO.transform, "DarkOverlay",
            Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero,
            new Color(0f, 0f, 0f, 0.25f));

        // Sol altın şerit
        MakePanel(canvasGO.transform, "LeftAccent",
            new Vector2(0f, 0f), new Vector2(0.005f, 1f), Vector2.zero, Vector2.zero,
            new Color(0.784f, 0.588f, 0.165f, 0.7f));

        // Sağ altın şerit
        MakePanel(canvasGO.transform, "RightAccent",
            new Vector2(0.995f, 0f), new Vector2(1f, 1f), Vector2.zero, Vector2.zero,
            new Color(0.784f, 0.588f, 0.165f, 0.7f));

        // Orta yarı-saydam panel (içerik alanı)
        MakePanel(canvasGO.transform, "ContentPanel",
            new Vector2(0.28f, 0.05f), new Vector2(0.72f, 0.95f), Vector2.zero, Vector2.zero,
            new Color(0.09f, 0.06f, 0.03f, 0.65f));

        // ── Başlık ───────────────────────────────────────────────────
        MakeTMP(canvasGO.transform, "Title",
            new Vector2(0.5f, 0.76f), new Vector2(860, 130),
            "THE LAST HARVEST",
            80, FontStyles.Bold, TextAlignmentOptions.Center,
            new Color(0.784f, 0.588f, 0.165f), letterSpacing: 10f);

        // Alt başlık
        MakeTMP(canvasGO.transform, "Subtitle",
            new Vector2(0.5f, 0.67f), new Vector2(680, 45),
            "\u2014  \u00c7iftlik Maceras\u0131  \u2014",
            24, FontStyles.Italic, TextAlignmentOptions.Center,
            new Color(0.62f, 0.46f, 0.20f), letterSpacing: 4f);

        // Ayırıcı çizgi
        MakePanel(canvasGO.transform, "Divider",
            new Vector2(0.5f, 0.615f), new Vector2(0.5f, 0.615f),
            new Vector2(-240f, -1f), new Vector2(240f, 1f),
            new Color(0.784f, 0.588f, 0.165f, 0.35f));

        // ── Buton Konteyneri ─────────────────────────────────────────
        var containerGO   = new GameObject("ButtonContainer");
        containerGO.transform.SetParent(canvasGO.transform, false);
        var containerRect = containerGO.AddComponent<RectTransform>();
        SetAnchored(containerRect, new Vector2(0.5f, 0.40f), new Vector2(400, 340));

        var vlg = containerGO.AddComponent<VerticalLayoutGroup>();
        vlg.spacing             = 20;
        vlg.childForceExpandWidth  = true;
        vlg.childForceExpandHeight = false;
        vlg.childControlHeight     = true;
        vlg.childAlignment         = TextAnchor.MiddleCenter;
        vlg.padding = new RectOffset(0, 0, 0, 0);

        containerGO.AddComponent<ContentSizeFitter>().verticalFit =
            ContentSizeFitter.FitMode.PreferredSize;

        // ── Butonlar ─────────────────────────────────────────────────
        var newGameBtn  = MakeButton(containerGO.transform, "NewGameButton",  ">  Yeni Oyun");
        var loadBtn     = MakeButton(containerGO.transform, "LoadButton",     ">  Y\u00fckle");
        var settingsBtn = MakeButton(containerGO.transform, "SettingsButton", ">  Ayarlar");
        var quitBtn     = MakeButton(containerGO.transform, "QuitButton",     ">  \u00c7\u0131k\u0131\u015f");

        // onClick olaylarını MainMenuUI metodlarına bağla
        UnityEventTools.AddPersistentListener(newGameBtn.onClick,  menuUI.OnNewGameClicked);
        UnityEventTools.AddPersistentListener(loadBtn.onClick,     menuUI.OnLoadClicked);
        UnityEventTools.AddPersistentListener(settingsBtn.onClick, menuUI.OnSettingsClicked);
        UnityEventTools.AddPersistentListener(quitBtn.onClick,     menuUI.OnQuitClicked);

        // ── Versiyon metni ───────────────────────────────────────────
        var verGO   = new GameObject("VersionText");
        verGO.transform.SetParent(canvasGO.transform, false);
        var verRect = verGO.AddComponent<RectTransform>();
        verRect.anchorMin        = new Vector2(1f, 0f);
        verRect.anchorMax        = new Vector2(1f, 0f);
        verRect.pivot            = new Vector2(1f, 0f);
        verRect.anchoredPosition = new Vector2(-20f, 20f);
        verRect.sizeDelta        = new Vector2(180f, 30f);
        var verTMP = verGO.AddComponent<TextMeshProUGUI>();
        verTMP.text      = "v0.1.0";
        verTMP.fontSize  = 18;
        verTMP.alignment = TextAlignmentOptions.Right;
        verTMP.color     = new Color(0.5f, 0.4f, 0.2f, 0.55f);

        // ── Sahneyi kaydet ────────────────────────────────────────────
        const string scenePath = "Assets/Scenes/MainMenu.unity";
        EditorSceneManager.SaveScene(scene, scenePath);

        // ── Build Settings ────────────────────────────────────────────
        EditorBuildSettings.scenes = new[]
        {
            new EditorBuildSettingsScene(scenePath, true),
            new EditorBuildSettingsScene("Assets/Scenes/SampleScene.unity", true),
        };

        AssetDatabase.Refresh();
        Debug.Log("✅ MainMenu sahnesi oluşturuldu: " + scenePath);
    }

    // ─── Yardımcı: Tam ekran / konumlu Image paneli ──────────────────
    static void MakePanel(Transform parent, string name,
        Vector2 ancMin, Vector2 ancMax,
        Vector2 offMin, Vector2 offMax,
        Color color)
    {
        var go   = new GameObject(name);
        go.transform.SetParent(parent, false);
        var rect = go.AddComponent<RectTransform>();
        rect.anchorMin  = ancMin;
        rect.anchorMax  = ancMax;
        rect.offsetMin  = offMin;
        rect.offsetMax  = offMax;
        go.AddComponent<Image>().color = color;
    }

    // ─── Yardımcı: Ortalanmış TextMeshProUGUI ────────────────────────
    static void MakeTMP(Transform parent, string name,
        Vector2 anchor, Vector2 size,
        string text, float fontSize,
        FontStyles style, TextAlignmentOptions align,
        Color color, float letterSpacing = 0f)
    {
        var go   = new GameObject(name);
        go.transform.SetParent(parent, false);
        var rect = go.AddComponent<RectTransform>();
        rect.anchorMin        = anchor;
        rect.anchorMax        = anchor;
        rect.pivot            = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = Vector2.zero;
        rect.sizeDelta        = size;
        var tmp = go.AddComponent<TextMeshProUGUI>();
        tmp.text            = text;
        tmp.fontSize        = fontSize;
        tmp.fontStyle       = style;
        tmp.alignment       = align;
        tmp.color           = color;
        tmp.characterSpacing = letterSpacing;
    }

    // ─── Yardımcı: RectTransform ortalayıcı ─────────────────────────
    static void SetAnchored(RectTransform rect, Vector2 anchor, Vector2 size)
    {
        rect.anchorMin        = anchor;
        rect.anchorMax        = anchor;
        rect.pivot            = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = Vector2.zero;
        rect.sizeDelta        = size;
    }

    // ─── Yardımcı: Menü butonu ───────────────────────────────────────
    static Button MakeButton(Transform parent, string name, string label)
    {
        var btnGO  = new GameObject(name);
        btnGO.transform.SetParent(parent, false);

        var rect = btnGO.AddComponent<RectTransform>();
        rect.sizeDelta = new Vector2(400, 65);

        var img = btnGO.AddComponent<Image>();
        img.color = new Color(0.13f, 0.09f, 0.04f, 1f);

        var btn    = btnGO.AddComponent<Button>();
        var colors = btn.colors;
        colors.normalColor      = new Color(0.13f, 0.09f, 0.04f, 1f);
        colors.highlightedColor = new Color(0.35f, 0.22f, 0.07f, 1f);
        colors.pressedColor     = new Color(0.07f, 0.05f, 0.02f, 1f);
        colors.selectedColor    = new Color(0.35f, 0.22f, 0.07f, 1f);
        colors.fadeDuration     = 0.12f;
        btn.colors       = colors;
        btn.targetGraphic = img;

        var le = btnGO.AddComponent<LayoutElement>();
        le.preferredHeight = 65;
        le.preferredWidth  = 400;

        // Buton etiketi
        var lblGO   = new GameObject("Label");
        lblGO.transform.SetParent(btnGO.transform, false);
        var lblRect = lblGO.AddComponent<RectTransform>();
        lblRect.anchorMin = Vector2.zero;
        lblRect.anchorMax = Vector2.one;
        lblRect.offsetMin = Vector2.zero;
        lblRect.offsetMax = Vector2.zero;

        var tmp = lblGO.AddComponent<TextMeshProUGUI>();
        tmp.text            = label;
        tmp.fontSize        = 28;
        tmp.fontStyle       = FontStyles.Bold;
        tmp.alignment       = TextAlignmentOptions.Center;
        tmp.color           = new Color(0.96f, 0.90f, 0.78f, 1f);
        tmp.characterSpacing = 2f;

        return btn;
    }
}
