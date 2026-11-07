using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance;

    private Dictionary<string, int> inventory = new Dictionary<string, int>();

    [Header("UI Referansý")]
    public TextMeshProUGUI inventoryText;

    [Header("Su ve Kova Mekaniði")]
    [Tooltip("Kovayla kaç kez sulama yapýlabileceði")]
    public int maxWaterUses = 2;
    public int currentWater = 0; // Kovadaki kalan su miktarý

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    void Start()
    {
        // Oyuna baþlarken test için 1 Boþ Kova verelim
        AddItem("EmptyBucket", 1);
        currentWater = 0;
        UpdateUI();
    }

    public void AddItem(string itemName, int amount)
    {
        if (inventory.ContainsKey(itemName))
        {
            inventory[itemName] += amount;
        }
        else
        {
            inventory[itemName] = amount;
        }
        UpdateUI();
    }

    public bool RemoveItem(string itemName, int amount)
    {
        if (inventory.ContainsKey(itemName) && inventory[itemName] >= amount)
        {
            inventory[itemName] -= amount;
            UpdateUI();
            return true;
        }
        return false;
    }

    public bool HasItem(string itemName, int amount = 1)
    {
        return inventory.ContainsKey(itemName) && inventory[itemName] >= amount;
    }

    void UpdateUI()
    {
        if (inventoryText != null)
        {
            string displayStr = "--- ENVANTER ---\n";
            foreach (KeyValuePair<string, int> item in inventory)
            {
                string displayName = item.Key;
                if (item.Key == "EmptyBucket") displayName = "Boþ Kova";
                if (item.Key == "FullBucket") displayName = $"Dolu Kova (Su: {currentWater}/{maxWaterUses})";
                if (item.Key == "Crop") displayName = "Ürün";

                displayStr += $"{displayName}: {item.Value}\n";
            }
            inventoryText.text = displayStr;
        }
    }
}