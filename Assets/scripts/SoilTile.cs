using System.Collections;
using UnityEngine;

public class SoilTile : MonoBehaviour
{
    public GameObject plantPrefab;

    public Color dryColor = new Color(0.55f, 0.32f, 0.16f);
    public Color wetColor = new Color(0.30f, 0.16f, 0.08f);
    public Color selectedColor = new Color(0.70f, 0.45f, 0.25f);

    private Renderer tileRenderer;
    private Color originalColor;

    private bool isPlanted = false;
    private bool isWatered = false;
    private bool isGrowing = false;

    private GameObject currentPlant;
    private int growthStage = 0; // 0: Yeni ekildi, 1: Orta, 2: Olgun

    public float growthDelay = 2f;

    private static SoilTile selectedTile;

    // Karakterin animasyonunu tetiklemek için referans
    private static Animator playerAnimator;

    void Start()
    {
        tileRenderer = GetComponent<Renderer>();
        originalColor = tileRenderer.material.color;

        // Sahnedeki karakterin Animator bileþenini otomatik bulur
        if (playerAnimator == null)
        {
            GameObject player = GameObject.FindWithTag("Player");
            if (player != null)
            {
                playerAnimator = player.GetComponent<Animator>();
            }
            else
            {
                // Eðer tag eklemediysen isme göre de arayabilir:
                player = GameObject.Find("Mobile_FreeSample_male_1"); // Sahnedeki karakterinin ismi
                if (player != null) playerAnimator = player.GetComponent<Animator>();
            }
        }
    }

    void OnMouseDown()
    {
        SelectTile();
    }

    public void SelectTile()
    {
        if (selectedTile != null && selectedTile != this)
        {
            selectedTile.UpdateColor();
        }

        selectedTile = this;
        UpdateColor();
    }

    // ==========================================
    // ORTAK ÝÞLEM FONKSÝYONLARI (Hem Klavye hem UI Butonlarý Ýçin)
    // ==========================================

    public void PlantSeed()
    {
        if (!isPlanted && plantPrefab != null)
        {
            isPlanted = true;
            growthStage = 0;
            isGrowing = false;

            Vector3 spawnPosition = transform.position + new Vector3(0, 0.2f, 0);
            currentPlant = Instantiate(plantPrefab, spawnPosition, Quaternion.identity);
            currentPlant.transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);

            Debug.Log("Bitki ekildi!");
        }
    }

    public void WaterTile()
    {
        if (isPlanted && !isWatered && !isGrowing && growthStage < 2)
        {
            isWatered = true;
            UpdateColor();
            Debug.Log("Toprak sulandý, büyüme baþladý...");

            StartCoroutine(GrowRoutine());
        }
    }

    IEnumerator GrowRoutine()
    {
        isGrowing = true;

        yield return new WaitForSeconds(growthDelay);

        growthStage++;

        if (growthStage == 1)
        {
            if (currentPlant != null)
                currentPlant.transform.localScale = new Vector3(0.2f, 0.6f, 0.2f);
            Debug.Log("Bitki büyüdü (Orta boy)!");
        }
        else if (growthStage >= 2)
        {
            if (currentPlant != null)
                currentPlant.transform.localScale = new Vector3(0.3f, 1f, 0.3f);
            Debug.Log("Bitki tamamen olgunlaþtý, hasat edilebilir!");
        }

        isGrowing = false;

        if (growthStage < 2)
        {
            isWatered = false;
            UpdateColor();
        }
    }


    public void Harvest()
    {
        if (isPlanted && growthStage >= 2)
        {
            // --- BURASI: Hasat yapýldýðýnda Pickup animasyonunu tetikler ---
            if (playerAnimator != null)
            {
                playerAnimator.SetTrigger("Pickup");
            }
            // -------------------------------------------------------------

            if (currentPlant != null)
            {
                Destroy(currentPlant);
            }

            isPlanted = false;
            isWatered = false;
            isGrowing = false;
            growthStage = 0;
            currentPlant = null;

            UpdateColor();

            Debug.Log("Bitki hasat edildi! Tarla yeniden ekime hazýr.");
        }
        else
        {
            Debug.Log("Bu bitki henüz büyümedi, hasat edemezsin!");
        }

    }

    // ==========================================
    // STATÝK BUTON TETÝKLEYÝCÝLERÝ (Mobil UI Ýçin)
    // ==========================================
    public static void UI_Plant()
    {
        if (selectedTile != null) selectedTile.PlantSeed();
    }

    public static void UI_Water()
    {
        if (selectedTile != null) selectedTile.WaterTile();
    }

    public static void UI_Harvest()
    {
        if (selectedTile != null) selectedTile.Harvest();
    }


    void Update()
    {
        if (selectedTile == this)
        {
            if (Input.GetKeyDown(KeyCode.E)) WaterTile();
            if (Input.GetKeyDown(KeyCode.F)) PlantSeed();
            if (Input.GetKeyDown(KeyCode.H)) Harvest();
        }
    }

    void UpdateColor()
    {
        if (isWatered)
        {
            tileRenderer.material.color = wetColor;
        }
        else if (selectedTile == this)
        {
            tileRenderer.material.color = selectedColor;
        }
        else
        {
            tileRenderer.material.color = originalColor;
        }
    }
}