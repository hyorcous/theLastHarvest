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

    private GameObject currentPlant;
    private int growthStage = 0; // 0: Yeni ekildi, 2: Olgun

    private static SoilTile selectedTile;
    private static Animator playerAnimator;

    void Start()
    {
        tileRenderer = GetComponent<Renderer>();
        originalColor = tileRenderer.material.color;

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
            selectedTile.UpdateColor();
        }

        selectedTile = this;
        UpdateColor();
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
                        // UI yenilenmesi için tetik
                        InventoryManager.Instance.AddItem("FullBucket", 0);
                    }

                    isWatered = true;
                    growthStage = 2; // Anýnda olgunlaþtýr

                    if (currentPlant != null)
                    {
                        currentPlant.transform.localScale = new Vector3(0.3f, 1f, 0.3f);
                    }

                    UpdateColor();
                }
            }
            else
            {
                Debug.Log("Tarlayý sulamak için kovanda su olmalý! Önce göle gitmelisin.");
            }
        }
    }

    public void Harvest()
    {
        if (isPlanted && growthStage >= 2)
        {
            // Death'in toplama animasyonunu tetikler
            if (playerAnimator != null)
            {
                playerAnimator.SetTrigger("Pickup");
            }

            // Envantere 1 adet ürün ekle
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

            UpdateColor();

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
            if (Input.GetKeyDown(KeyCode.R)) WaterTile(); // <-- E yerine R yaptýk!
            if (Input.GetKeyDown(KeyCode.F)) PlantSeed();
            if (Input.GetKeyDown(KeyCode.H)) Harvest();
        }
    }

    void UpdateColor()
    {
        if (isWatered) tileRenderer.material.color = wetColor;
        else if (selectedTile == this) tileRenderer.material.color = selectedColor;
        else tileRenderer.material.color = originalColor;
    }
}