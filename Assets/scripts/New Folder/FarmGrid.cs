
using UnityEngine;

public class FarmGrid : MonoBehaviour
{
    public GameObject soilPrefab;

    public int width = 5;
    public int height = 5;

    public float spacing = 1f;

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

                Instantiate(soilPrefab, position, Quaternion.identity);
            }
        }
    }
}
