using UnityEngine;
using TMPro;

public class DayNightCycle : MonoBehaviour
{
    [Header("Zaman Ayarlarý")]
    [Tooltip("Bir günün gerçek hayatta kaç saniye süreceði (Örn: 120 saniye = 2 dakika)")]
    public float fullDayLength = 120f;

    [Header("Iþýk Ayarlarý")]
    public Light directionalLight; // Sahnedeki Güneþ (Directional Light)

    [Header("UI Ayarlarý")]
    public TextMeshProUGUI timeText; // Ekranda saati gösterecek yazý

    [Range(0f, 1f)]
    public float currentTimeOfDay = 0.25f; // 0.25 = Sabah 06:00 (Baþlangýç saati)

    void Update()
    {
        // Zamanýn ilerlemesi
        currentTimeOfDay += Time.deltaTime / fullDayLength;

        if (currentTimeOfDay >= 1f)
        {
            currentTimeOfDay = 0f; // Yeni bir gün baþlýyor!
        }

        // Güneþin (Directional Light) dönmesi
        float sunAngle = currentTimeOfDay * 360f - 90f;
        if (directionalLight != null)
        {
            directionalLight.transform.rotation = Quaternion.Euler(sunAngle, 170f, 0f);

            // Gece ve gündüze göre ýþýk þiddetini (intensity) ayarlayalým
            if (currentTimeOfDay <= 0.23f || currentTimeOfDay >= 0.77f)
            {
                // Gece vakti: Iþýðý kýs (Ay ýþýðý efekti)
                directionalLight.intensity = Mathf.Lerp(directionalLight.intensity, 0.1f, Time.deltaTime * 2f);
            }
            else
            {
                // Gündüz vakti: Iþýðý aç
                directionalLight.intensity = Mathf.Lerp(directionalLight.intensity, 1f, Time.deltaTime * 2f);
            }
        }

        // UI Saat Yazýsýný Güncelle
        UpdateClockUI();
    }

    void UpdateClockUI()
    {
        if (timeText != null)
        {
            // 0 ile 1 arasýndaki deðeri 24 saatlik formata çeviriyoruz
            float totalHours = currentTimeOfDay * 24f;
            int hours = Mathf.FloorToInt(totalHours);
            int minutes = Mathf.FloorToInt((totalHours - hours) * 60f);

            // Saat ve dakikayý ekrana yazdýr (Örn: 08:30)
            timeText.text = string.Format("{0:00}:{1:00}", hours, minutes);
        }
    }
}