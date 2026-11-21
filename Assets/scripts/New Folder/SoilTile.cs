using System.Collections.Generic;
using UnityEngine;

public class SoilTile : MonoBehaviour
{
    public static List<SoilTile> allTiles = new List<SoilTile>();

    public GameObject plantPrefab;

    public Color unplowedColor = new Color(0.45f, 0.38f, 0.28f); // Çapalanmamýþ ham toprak
    public Color dryColor = new Color(0.55f, 0.32f, 0.16f);      // Çapalanmýþ kuru toprak
    public Color wetColor = new Color(0.30f, 0.16f, 0.08f);      // Sulanmýþ toprak
    public Color highlightColor = new Color(0.75f, 0.55f, 0.35f);// Mouse üzerindeyken

    private Renderer tileRenderer;

    private bool isPlowed = false;
    private bool isPlanted = false;
    private bool isWatered = false;
    private bool isHovered = false;

    private GameObject currentPlant;
    private int growthStage = 0; // 0: Tohum, 1: Olgun (1 günde büyür)

    private static Transform playerTransform;
    private static Animator playerAnimator;
    public float interactDistance = 3.5f; // Stardew tarzý yakýnlýk sýnýrý

    void OnEnable() { allTiles.Add(this); }
    void OnDisable() { allTiles.Remove(this); }

    void Start()
    {
        tileRenderer = GetComponent<Renderer>();

        if (playerTransform == null)
        {
            GameObject player = GameObject.FindWithTag("Player");
            if (player == null) player = GameObject.Find("Mobile_FreeSample_male_1");

            if (player != null)
            {
                playerTransform = player.transform;
                playerAnimator = player.GetComponent<Animator>();
            }
        }
        UpdateColor();
    }

    void OnMouseEnter()
    {
        isHovered = true;
        UpdateColor();
    }

    void OnMouseExit()
    {
        isHovered = false;
        UpdateColor();
    }

    void OnMouseDown()
    {
        // Diyalog açýksa iþlem yapma
        if (NPCDialogue.isDialogueActive) return;

        // Karakter topraða yeterince yakýn mý?
        if (playerTransform != null && Vector3.Distance(playerTransform.position, transform.position) > interactDistance)
        {
            Debug.Log("Çok uzaktasýn! Topraða yaklaþ.");
            return;
        }

        // Stardew Valley tarzý seçili alete göre aksiyon
        if (PlayerToolController.Instance != null)
        {
            PlayerToolController.Tool currentTool = PlayerToolController.Instance.currentTool;

            // Karakteri týklanan topraða doðru döndür
            if (playerTransform != null)
            {
                Vector3 lookDir = transform.position - playerTransform.position;
                lookDir.y = 0;
                if (lookDir != Vector3.zero) playerTransform.rotation = Quaternion.LookRotation(lookDir);
            }

            switch (currentTool)
            {
                case PlayerToolController.Tool.Hoe:
                    PlowTile();
                    break;
                case PlayerToolController.Tool.Seeds:
                    PlantSeed();
                    break;
                case PlayerToolController.Tool.WateringCan:
                    WaterTile();
                    break;
                case PlayerToolController.Tool.Scythe:
                    Harvest();
                    break;
            }
        }
    }

    public void PlowTile()
    {
        if (!isPlowed)
        {
            isPlowed = true;
            UpdateColor();
            Debug.Log("Toprak çapalandý, ekime hazýr!");
        }
    }

    public void PlantSeed()
    {
        if (!isPlowed)
        {
            Debug.Log("Önce topraðý çapalamalýsýn (Tuþ 1)!");
            return;
        }

        if (!isPlanted && plantPrefab != null)
        {
            isPlanted = true;
            growthStage = 0;

            Vector3 spawnPosition = transform.position + new Vector3(0, 0.2f, 0);
            currentPlant = Instantiate(plantPrefab, spawnPosition, Quaternion.identity);
            currentPlant.transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);

            Debug.Log("Tohum ekildi! Büyümesi için sulamalýsýn (Tuþ 3).");
        }
    }

    public void WaterTile()
    {
        if (isPlowed && !isWatered)
        {
            isWatered = true;
            UpdateColor();
            Debug.Log("Toprak sulandý! Ertesi sabah büyüyecek.");
        }
    }

    // Gece-Gündüz döngüsünde yeni gün baþladýðýnda çaðrýlýr
    public void OnNewDay()
    {
        if (isPlanted && isWatered && growthStage < 1)
        {
            growthStage = 1; // 1 Günde tam olgunlaþýr
            if (currentPlant != null)
            {
                currentPlant.transform.localScale = new Vector3(0.4f, 1.1f, 0.4f);
            }
            Debug.Log("Yeni gün: Ekinler olgunlaþtý, hasat edebilirsin!");
        }

        // Yeni günde toprak kurur
        isWatered = false;
        UpdateColor();
    }

    public void Harvest()
    {
        if (isPlanted && growthStage >= 1)
        {
            if (playerAnimator != null)
            {
                playerAnimator.SetTrigger("Pickup");
            }

            if (currentPlant != null)
            {
                Destroy(currentPlant);
            }

            isPlanted = false;
            isWatered = false;
            growthStage = 0;
            currentPlant = null;

            if (PlayerToolController.Instance != null)
            {
                PlayerToolController.Instance.AddHarvestedCrop();
            }

            UpdateColor();
            Debug.Log("Bitki hasat edildi!");
        }
        else if (isPlanted)
        {
            Debug.Log("Bu ekin henüz olgunlaþmadý! Sulayýp ertesi günü bekle.");
        }
    }

    void UpdateColor()
    {
        if (tileRenderer == null) return;

        if (isHovered)
        {
            tileRenderer.material.color = highlightColor;
        }
        else if (!isPlowed)
        {
            tileRenderer.material.color = unplowedColor;
        }
        else if (isWatered)
        {
            tileRenderer.material.color = wetColor;
        }
        else
        {
            tileRenderer.material.color = dryColor;
        }
    }
}