using UnityEngine;

public class Lake : MonoBehaviour
{
    private bool isPlayerInRange = false;

    void OnTriggerEnter(Collider other)
    {
        // Death veya Player tag'ine sahip biri göle girdiðinde
        if (other.CompareTag("Player") || other.gameObject.name == "Death")
        {
            isPlayerInRange = true;
            Debug.Log("Death göle yaklaþtý. Su doldurmak için [E] tuþuna basabilirsin!");
        }
    }

    void OnTriggerExit(Collider other)
    {
        // Gölün alanýndan çýktýðýnda
        if (other.CompareTag("Player") || other.gameObject.name == "Death")
        {
            isPlayerInRange = false;
            Debug.Log("Death gölden uzaklaþtý.");
        }
    }

    void Update()
    {
        // Alan içindeyken E tuþuna basýlýrsa
        if (isPlayerInRange && Input.GetKeyDown(KeyCode.E))
        {
            FillBucket();
        }
    }

    public void FillBucket()
    {
        if (InventoryManager.Instance != null)
        {
            // Eðer envanterde Boþ Kova varsa VEYA zaten elinde kova varsa doldurabilsin
            if (InventoryManager.Instance.HasItem("EmptyBucket", 1))
            {
                InventoryManager.Instance.RemoveItem("EmptyBucket", 1);
                InventoryManager.Instance.AddItem("FullBucket", 1);
            }

            // Kovayý doldur ve suyunu max limitine eþitle
            if (InventoryManager.Instance.HasItem("FullBucket", 1))
            {
                InventoryManager.Instance.currentWater = InventoryManager.Instance.maxWaterUses;
                Debug.Log("Baþarýlý: Kova tamamen dolduruldu! Su hakký: " + InventoryManager.Instance.currentWater);
                // UI güncellensin diye tetikleyebiliriz
                InventoryManager.Instance.AddItem("FullBucket", 0);
            }
            else
            {
                Debug.Log("Death: Envanterinde hiç kova yok!");
            }
        }
    }
}