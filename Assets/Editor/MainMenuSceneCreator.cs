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
        var bgSprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/sprites/menu_background.jpg");
        var bgGO = MakePanel(canvasGO.transform, "Background",
            Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero,
            Color.white);
        if (bgSprite != null)
        {
            var bgImg = bgGO.GetComponent<Image>();
            bgImg.sprite = bgSprite;
            bgImg.preserveAspect = false;
        }

        // ── Başlık / Logo ─────────────────────────────────────────────
        var logoSprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/sprites/game_logo.png");
        if (logoSprite != null)
        {
            var logoGO = new GameObject("Title");
            logoGO.transform.SetParent(canvasGO.transform, false);
            var logoRect = logoGO.AddComponent<RectTransform>();
            SetAnchored(logoRect, new Vector2(0.5f, 0.77f), new Vector2(750, 281));
            var logoImg = logoGO.AddComponent<Image>();
            logoImg.sprite = logoSprite;
            logoImg.preserveAspect = true;
        }

        // ── Tabela Direği (SignPole) ──────────────────────────────────
        // Butonların arkasında dikey ahşap direk
        var poleSprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/sprites/plank_medium_h2.png");
        var poleGO = new GameObject("SignPole");
        poleGO.transform.SetParent(canvasGO.transform, false);
        var poleRect = poleGO.AddComponent<RectTransform>();
        poleRect.anchorMin = new Vector2(0.5f, 0.5f);
        poleRect.anchorMax = new Vector2(0.5f, 0.5f);
        poleRect.pivot = new Vector2(0.5f, 0.5f);
        poleRect.anchoredPosition = new Vector2(-580f, -170f);
        poleRect.sizeDelta = new Vector2(60f, 740f);
        var poleImg = poleGO.AddComponent<Image>();
        poleImg.sprite = poleSprite;
        poleImg.type = Image.Type.Simple;
        poleImg.preserveAspect = false;
        poleImg.color = Color.white;

        // ── Buton Konteyneri ─────────────────────────────────────────
        var containerGO   = new GameObject("ButtonContainer");
        containerGO.transform.SetParent(canvasGO.transform, false);
        var containerRect = containerGO.AddComponent<RectTransform>();
        containerRect.anchorMin = new Vector2(0.5f, 0.5f);
        containerRect.anchorMax = new Vector2(0.5f, 0.5f);
        containerRect.pivot = new Vector2(0.5f, 0.5f);
        containerRect.anchoredPosition = new Vector2(-580f, -127.5f);
        containerRect.sizeDelta = new Vector2(370f, 535f);

        var vlg = containerGO.AddComponent<VerticalLayoutGroup>();
        vlg.spacing                = 15f;
        vlg.childForceExpandWidth  = false;
        vlg.childForceExpandHeight = false;
        vlg.childControlWidth      = true;
        vlg.childControlHeight     = true;
        vlg.childAlignment         = TextAnchor.MiddleCenter;
        vlg.padding                = new RectOffset(0, 0, 0, 0);

        // ── Butonlar ─────────────────────────────────────────────────
        var newGameBtn  = MakeButton(containerGO.transform, "NewGameButton",  "Yeni Oyun");
        var loadBtn     = MakeButton(containerGO.transform, "LoadButton",     "Y\u00fckle");
        var settingsBtn = MakeButton(containerGO.transform, "SettingsButton", "Ayarlar");
        var quitBtn     = MakeButton(containerGO.transform, "QuitButton",     "\u00c7\u0131k\u0131\u015f");

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
        verTMP.color     = new Color(1f, 1f, 1f, 0.65f);

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
    static GameObject MakePanel(Transform parent, string name,
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
        return go;
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
        rect.sizeDelta = new Vector2(370, 122);

        var img = btnGO.AddComponent<Image>();
        var woodSprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/sprites/button_wood.png");
        if (woodSprite != null)
        {
            img.sprite = woodSprite;
            img.type   = Image.Type.Simple;
            img.preserveAspect = false;
            img.color  = Color.white;
        }
        else
        {
            img.color = new Color(0.35f, 0.20f, 0.08f, 1f);
        }

        var btn    = btnGO.AddComponent<Button>();
        var colors = btn.colors;
        colors.normalColor      = Color.white;
        colors.highlightedColor = new Color(1.00f, 0.92f, 0.80f, 1f);
        colors.pressedColor     = new Color(0.70f, 0.50f, 0.35f, 1f);
        colors.selectedColor    = Color.white;
        colors.fadeDuration     = 0.10f;
        btn.colors       = colors;
        btn.targetGraphic = img;

        var le = btnGO.AddComponent<LayoutElement>();
        le.preferredWidth  = 370;
        le.preferredHeight = 122;

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
        tmp.fontSize        = 32;
        tmp.fontStyle       = FontStyles.Bold;
        tmp.alignment       = TextAlignmentOptions.Center;
        tmp.color           = Color.white;
        tmp.characterSpacing = 1f;

        return btn;
    }
}
