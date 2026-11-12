using UnityEngine;

public class SoilTile : MonoBehaviour
{
    [Header("Tarla Prefab'leri")]
    public GameObject drySoilPrefab; // Kuru tarla prefab'i
    public GameObject wetSoilPrefab; // Islak tarla prefab'i

    [Header("Bitki Prefab'i")]
    public GameObject plantPrefab; // Buðday (Wheat) Prefab'i

    private bool isPlanted = false;
    private bool isWatered = false;

    private GameObject currentPlant;
    private int growthStage = 0; // 0: Yeni ekildi, 2: Olgun

    private static SoilTile selectedTile;
    private static Animator playerAnimator;

    void Start()
    {
        // Death'in Animator bileþenini otomatik bulur
        if (playerAnimator == null)
        {
            GameObject player = GameObject.FindWithTag("Player");
            if (player == null) player = GameObject.Find("Death");

            if (player != null)
            {
                playerAnimator = player.GetComponent<Animator>();
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
            selectedTile.DeselectVisual();
        }

        selectedTile = this;
        // Seçildiðinde görsel bir efekt verebilirsin (isteðe baðlý)
    }

    public void DeselectVisual()
    {
        // Baþka tarla seçildiðinde yapýlacak görsel sýfýrlama (gerekirse)
    }

    public void PlantSeed()
    {
        if (!isPlanted && plantPrefab != null)
        {
            isPlanted = true;
            growthStage = 0;

            Vector3 spawnPosition = transform.position + new Vector3(0, 0.2f, 0);
            currentPlant = Instantiate(plantPrefab, spawnPosition, Quaternion.identity);
            currentPlant.transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);

            Debug.Log("Death tohum ekti!");
        }
    }

    public void WaterTile()
    {
        if (InventoryManager.Instance != null)
        {
            // Envanterde Dolu Kova var mý VE kovada en az 1 su var mý?
            if (InventoryManager.Instance.HasItem("FullBucket", 1) && InventoryManager.Instance.currentWater > 0)
            {
                if (isPlanted && !isWatered && growthStage < 2)
                {
                    // Kovadaki suyu 1 azalt
                    InventoryManager.Instance.currentWater--;

                    // Eðer su tamamen bittiyse, Dolu Kovayý alýp Boþ Kova ver
                    if (InventoryManager.Instance.currentWater <= 0)
                    {
                        InventoryManager.Instance.RemoveItem("FullBucket", 1);
                        InventoryManager.Instance.AddItem("EmptyBucket", 1);
                        Debug.Log("Kovadaki su bitti, kova boþaldý!");
                    }
                    else
                    {
                        Debug.Log("Tarla sulandý! Kalan su hakký: " + InventoryManager.Instance.currentWater);
                        InventoryManager.Instance.AddItem("FullBucket", 0); // UI yenilenmesi için
                    }

                    isWatered = true;
                    growthStage = 2; // Anýnda olgunlaþtýr

                    if (currentPlant != null)
                    {
                        currentPlant.transform.localScale = new Vector3(0.3f, 1f, 0.3f);
                    }

                    // --- ÝÞTE BURADA KURU Tarlayý ISLAK Tarla ile Deðiþtiriyoruz ---
                    SwitchToWetSoil();
                }
            }
            else
            {
                Debug.Log("Tarlayý sulamak için kovanda su olmalý! Önce göle gitmelisin.");
            }
        }
    }

    void SwitchToWetSoil()
    {
        if (wetSoilPrefab != null)
        {
            // Islak tarla prefab'ini bu tarlanýn olduðu pozisyonda oluþtur
            Vector3 pos = transform.position;
            Quaternion rot = transform.rotation;

            // Eðer tarlanýn üzerinde bitki varsa, yeni oluþturduðumuz ýslak tarlanýn çocuðu yapalým ki kaybolmasýn
            GameObject newWetTile = Instantiate(wetSoilPrefab, pos, rot);

            SoilTile newSoilScript = newWetTile.GetComponent<SoilTile>();
            if (newSoilScript != null)
            {
                newSoilScript.isPlanted = this.isPlanted;
                newSoilScript.isWatered = true;
                newSoilScript.growthStage = this.growthStage;
                newSoilScript.currentPlant = this.currentPlant;

                // Bitki varsa pozisyonunu koru
                if (newSoilScript.currentPlant != null)
                {
                    newSoilScript.currentPlant.transform.SetParent(newWetTile.transform);
                }

                // Eðer bu tarla þu an seçiliyse, yeni oluþan ýslak tarlayý seçili yap
                if (selectedTile == this)
                {
                    selectedTile = newSoilScript;
                }
            }

            // Eski kuru tarlayý sahneden sil
            Destroy(gameObject);
        }
    }

    public void Harvest()
    {
        if (isPlanted && growthStage >= 2)
        {
            if (playerAnimator != null)
            {
                playerAnimator.SetTrigger("Pickup");
            }

            if (InventoryManager.Instance != null)
            {
                InventoryManager.Instance.AddItem("Crop", 1);
            }

            if (currentPlant != null)
            {
                Destroy(currentPlant);
            }

            isPlanted = false;
            isWatered = false;
            growthStage = 0;
            currentPlant = null;

            // Ýsteðe baðlý: Hasat edilince tekrar kuru tarla modeline dönebilir veya ýslak kalabilir.
            Debug.Log("Death bitkiyi hasat etti!");
        }
        else
        {
            Debug.Log("Bu tarla boþ veya bitki henüz olgunlaþmadý!");
        }
    }

    public static void UI_Plant() { if (selectedTile != null) selectedTile.PlantSeed(); }
    public static void UI_Water() { if (selectedTile != null) selectedTile.WaterTile(); }
    public static void UI_Harvest() { if (selectedTile != null) selectedTile.Harvest(); }

    void Update()
    {
        if (selectedTile == this)
        {
            if (Input.GetKeyDown(KeyCode.R)) WaterTile();
            if (Input.GetKeyDown(KeyCode.F)) PlantSeed();
            if (Input.GetKeyDown(KeyCode.H)) Harvest();
        }
    }
}