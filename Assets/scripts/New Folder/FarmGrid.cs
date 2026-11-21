using UnityEngine;

public class FarmGrid : MonoBehaviour
{
    public GameObject soilPrefab;

    public int width = 5;
    public int height = 5;

    public float spacing = 1.2f; // Veya senin kullandýðýn spacing deðeri

    void Start()
    {
        CreateGrid();
    }

    void CreateGrid()
    {
        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < height; z++)
            {
                Vector3 position = new Vector3(
                    x * spacing,
                    0.1f,
                    z * spacing
                );

                // --- BURAYI DEÐÝÞTÝRÝYORUZ ---
                // Tarlayý oluþtururken X ekseninde 90 derece döndürerek yatay hale getiriyoruz
                Quaternion rotation = Quaternion.Euler(90f, 0f, 0f);
                Instantiate(soilPrefab, position, rotation);
            }
        }
    }
}