using UnityEngine;

public class DayNightCycle : MonoBehaviour
{
    public static DayNightCycle Instance;

    public Light directionalLight;
    public float dayDurationInSeconds = 120f; // 2 dakikada 1 tam gün döner
    [Range(0f, 1f)] public float timeOfDay = 0.25f; // 0.25 = Sabah 06:00
    public int currentDay = 1;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        if (directionalLight == null)
            directionalLight = FindObjectOfType<Light>();
    }

    void Update()
    {
        if (NPCDialogue.isDialogueActive) return;

        // Zamanı ilerlet
        timeOfDay += Time.deltaTime / dayDurationInSeconds;

        // Gün bittiyse yeni güne geç
        if (timeOfDay >= 1f)
        {
            AdvanceToNextDay();
        }

        // PROTOTİP KISAYOLU: 'U' tuşuna basınca uyur ve hemen sabah olur (Sunumda çok işine yarar)
        if (Input.GetKeyDown(KeyCode.U))
        {
            AdvanceToNextDay();
        }

        UpdateLighting();
    }

    public void AdvanceToNextDay()
    {
        timeOfDay = 0.25f; // Sabah 06:00'da uyan
        currentDay++;

        // Sahnedeki tüm topraklara yeni günün geldiğini haber ver
        foreach (SoilTile tile in SoilTile.allTiles)
        {
            tile.OnNewDay();
        }

        Debug.Log($"Gün {currentDay} başladı! Sulanmış ekinler büyüdü.");
    }

    void UpdateLighting()
    {
        if (directionalLight != null)
        {
            float sunAngle = (timeOfDay * 360f) - 90f;
            directionalLight.transform.rotation = Quaternion.Euler(sunAngle, 170f, 0f);

            // Gece-gündüz ışık şiddeti
            float intensity = Mathf.Clamp01(Mathf.Sin(timeOfDay * Mathf.PI));
            directionalLight.intensity = Mathf.Lerp(0.15f, 1.2f, intensity);
        }
    }

    public string GetTimeString()
    {
        int totalHours = Mathf.FloorToInt(timeOfDay * 24f);
        int minutes = Mathf.FloorToInt(((timeOfDay * 24f) - totalHours) * 60f);
        return string.Format("{0:00}:{1:00}", totalHours, minutes);
    }
}