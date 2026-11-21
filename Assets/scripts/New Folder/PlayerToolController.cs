using UnityEngine;

public class PlayerToolController : MonoBehaviour
{
    public static PlayerToolController Instance;

    public enum Tool { Hoe = 1, Seeds = 2, WateringCan = 3, Scythe = 4 }
    public Tool currentTool = Tool.Hoe;

    public int harvestedCrops = 0;
    public int maxEnergy = 100;
    public int currentEnergy = 100;

    [Header("Mouse Orbit Kamera Ayarları")]
    public Transform playerTransform;
    public float cameraDistance = 8f;
    public float mouseSensitivity = 3.5f;
    public float cameraSmoothness = 10f;

    private float yaw = 0f;
    private float pitch = 35f; // Yukarıdan bakış açısı
    private Camera mainCam;

    private string notificationText = "";
    private float notificationTimer = 0f;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        mainCam = Camera.main;
        if (playerTransform == null)
        {
            GameObject player = GameObject.FindWithTag("Player");
            if (player == null) player = GameObject.Find("Mobile_FreeSample_male_1");
            if (player != null) playerTransform = player.transform;
        }

        // Kamerayı karakterin içinden otomatik çıkar (Garanti olsun)
        if (mainCam != null && mainCam.transform.parent != null)
        {
            mainCam.transform.SetParent(null);
        }
    }

    void Update()
    {
        if (notificationTimer > 0) notificationTimer -= Time.deltaTime;
        if (NPCDialogue.isDialogueActive) return;

        // Sağ Tık Basılı Tutulduğunda Kamerayı Mouse ile Döndür
        if (Input.GetMouseButton(1))
        {
            yaw += Input.GetAxis("Mouse X") * mouseSensitivity;
            pitch -= Input.GetAxis("Mouse Y") * mouseSensitivity;
            pitch = Mathf.Clamp(pitch, 15f, 65f); // Yerin altına girmesini engeller
        }

        // 1-2-3-4 Tuşları ile Alet Seçimi
        if (Input.GetKeyDown(KeyCode.Alpha1)) currentTool = Tool.Hoe;
        if (Input.GetKeyDown(KeyCode.Alpha2)) currentTool = Tool.Seeds;
        if (Input.GetKeyDown(KeyCode.Alpha3)) currentTool = Tool.WateringCan;
        if (Input.GetKeyDown(KeyCode.Alpha4)) currentTool = Tool.Scythe;

        // Mouse Tekerleği ile Alet Değiştirme
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll != 0f)
        {
            int toolIndex = (int)currentTool + (scroll > 0 ? -1 : 1);
            if (toolIndex < 1) toolIndex = 4;
            if (toolIndex > 4) toolIndex = 1;
            currentTool = (Tool)toolIndex;
        }
    }

    void LateUpdate()
    {
        if (playerTransform != null && mainCam != null)
        {
            Quaternion rotation = Quaternion.Euler(pitch, yaw, 0f);
            Vector3 targetPos = playerTransform.position + Vector3.up * 1.2f - (rotation * Vector3.forward * cameraDistance);

            mainCam.transform.position = Vector3.Lerp(mainCam.transform.position, targetPos, Time.deltaTime * cameraSmoothness);
            mainCam.transform.LookAt(playerTransform.position + Vector3.up * 1.2f);
        }
    }

    public bool UseEnergy(int amount)
    {
        if (currentEnergy < amount)
        {
            ShowNotification("Çok yoruldun! [U] tuşuna basıp uyu.");
            return false;
        }
        currentEnergy -= amount;
        return true;
    }

    public void RestoreEnergy()
    {
        currentEnergy = maxEnergy;
    }

    public void AddHarvestedCrop()
    {
        harvestedCrops++;
        ShowNotification("+1 Ekin Hasat Edildi!");
    }

    public void ShowNotification(string message)
    {
        notificationText = message;
        notificationTimer = 2.5f;
    }

    void OnGUI()
    {
        if (NPCDialogue.isDialogueActive) return;

        GUIStyle boxStyle = new GUIStyle(GUI.skin.box);
        boxStyle.fontSize = 15;
        boxStyle.richText = true;
        boxStyle.normal.textColor = Color.white;

        // Sol Üst Bilgi Paneli
        GUI.backgroundColor = new Color(0.1f, 0.1f, 0.15f, 0.9f);
        GUI.Box(new Rect(15, 15, 310, 140), "", boxStyle);
        GUI.Label(new Rect(25, 22, 290, 25), $"<b>Gün:</b> {(DayNightCycle.Instance != null ? DayNightCycle.Instance.currentDay : 1)}   |   <b>Saat:</b> {(DayNightCycle.Instance != null ? DayNightCycle.Instance.GetTimeString() : "06:00")}", boxStyle);
        GUI.Label(new Rect(25, 47, 290, 25), $"<b>Toplanan Ekin:</b> <color=#90EE90>{harvestedCrops}</color>", boxStyle);
        GUI.Label(new Rect(25, 72, 290, 25), $"<b>Enerji:</b> <color=#FFD700>{currentEnergy} / {maxEnergy}</color>", boxStyle);
        GUI.Label(new Rect(25, 97, 290, 25), "<size=13><color=#CCCCCC><i>[U]: Uyu (Yeni Gün) | [Sağ Tık]: Kamera</i></color></size>", boxStyle);
        GUI.Label(new Rect(25, 118, 290, 25), "<size=13><color=#CCCCCC><i>[Sol Tık]: Seçili Aleti Kullan</i></color></size>", boxStyle);

        // Ekran Ortasında Yüzen Bildirim
        if (notificationTimer > 0)
        {
            GUIStyle notifStyle = new GUIStyle(GUI.skin.box);
            notifStyle.fontSize = 17;
            notifStyle.fontStyle = FontStyle.Bold;
            notifStyle.alignment = TextAnchor.MiddleCenter;
            notifStyle.normal.textColor = new Color(1f, 0.9f, 0.4f);

            GUI.backgroundColor = new Color(0f, 0f, 0f, 0.85f);
            GUI.Box(new Rect((Screen.width - 360) / 2f, 80, 360, 40), notificationText, notifStyle);
        }

        // Alt Orta Stardew Valley Hotbar
        string[] toolNames = { "1: Çapa", "2: Tohum", "3: Sulama Kabı", "4: Orak (Hasat)" };
        float totalWidth = 520;
        float startX = (Screen.width - totalWidth) / 2f;
        float startY = Screen.height - 70;

        for (int i = 0; i < 4; i++)
        {
            bool isSelected = ((int)currentTool == i + 1);
            GUI.backgroundColor = isSelected ? new Color(1f, 0.85f, 0.2f, 1f) : new Color(0.2f, 0.2f, 0.2f, 0.85f);
            GUI.Box(new Rect(startX + (i * 130), startY, 120, 50), $"<b>{toolNames[i]}</b>", boxStyle);
        }
        GUI.backgroundColor = Color.white;
    }
}