using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InventoryManager : MonoBehaviour
{
    public int maxStackedItems = 999;
    public InventorySlot[] inventorySlots;
    public GameObject inventoryItemPrefab;

    int selectedSlot = -1;

    private void Start()
    {
        ChangeSelectedSlot(0);
    }

    private void Update()
    {
            if (Keyboard.current == null) return;

        // O New Input System tem uma propriedade chamada 'anyKey' 
        for (int i = 1; i <= 9; i++)
        {
            Key key = (Key)System.Enum.Parse(typeof(Key), "Digit" + i);
            
            if (Keyboard.current[key].wasPressedThisFrame)
            {
                if (i <= inventorySlots.Length)
                {
                    ChangeSelectedSlot(i - 1);
                }
                break; 
            }
        }
    }

    void ChangeSelectedSlot(int newValue)
    {
        if (selectedSlot != -1) inventorySlots[selectedSlot].Deselect();

        inventorySlots[newValue].Select();
        selectedSlot = newValue;
    }

    public bool AddItem(Item item)
    {
        for (int i = 0; i < inventorySlots.Length; i++)
        {
            InventorySlot slot = inventorySlots[i];
            InventoryItem ItemInSlot = slot.GetComponentInChildren<InventoryItem>();
            if (ItemInSlot != null &&
                ItemInSlot.item == item &&
                ItemInSlot.count < maxStackedItems &&
                ItemInSlot.item.stackable == true)
            {
                ItemInSlot.count++;
                ItemInSlot.RefreshCount();
                return true;
            }
        }

        for (int i = 0; i < inventorySlots.Length; i++)
        {
            InventorySlot slot = inventorySlots[i];
            InventoryItem ItemInSlot = slot.GetComponentInChildren<InventoryItem>();
            if (ItemInSlot == null)
            {
                SpawnNewItem(item, slot);
                return true;
            }
        }

        return false;
    }
    public void SpawnNewItem(Item item, InventorySlot slot)
    {
        GameObject newItemGo = Instantiate(inventoryItemPrefab, slot.transform);
        InventoryItem inventoryItem = newItemGo.GetComponent<InventoryItem>();
        inventoryItem.InitialiseItem(item);
    }

    public Item GetSelectedItem(bool use)
    {
        InventorySlot slot = inventorySlots[selectedSlot];
        InventoryItem ItemInSlot = slot.GetComponentInChildren<InventoryItem>();
        if (ItemInSlot != null)
        {
            Item item = ItemInSlot.item;
            if (use == true)
            {
                ItemInSlot.count--;
                if (ItemInSlot.count <= 0)
                {
                    Destroy(ItemInSlot.gameObject);
                } else
                {
                    ItemInSlot.RefreshCount();
                }
            }
            return ItemInSlot.item;
        }
        return null;
    }
}
