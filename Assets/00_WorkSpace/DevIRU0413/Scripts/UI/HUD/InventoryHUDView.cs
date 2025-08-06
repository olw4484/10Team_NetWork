using System.Collections.Generic;
using UnityEngine;

public class InventoryHUDView : YSJ_HUDBaseUI
{
    [SerializeField] private GameObject slotPrefab;
    [SerializeField] private Transform slotParent;

    private List<SHI_InventorySlot> slots = new List<SHI_InventorySlot>();

    public void InitSlots(InventoryHUDModel model)
    {
        ClearSlots();

        for (int i = 0; i < model.Inventory.maxSlots; i++)
        {
            GameObject go = Instantiate(slotPrefab, slotParent);
            SHI_InventorySlot slot = go.GetComponent<SHI_InventorySlot>();
            slot.InitSlot(model.inventoryManager, model.Inventory);
            slots.Add(slot);
        }
    }

    public void UpdateSlots(List<SHI_ItemBase> items)
    {
        for (int i = 0; i < slots.Count; i++)
        {
            if (i < items.Count)
                slots[i].SetItem(items[i]);
            else
                slots[i].ClearSlot();
        }
    }

    private void ClearSlots()
    {
        foreach (var slot in slots)
        {
            Destroy(slot.gameObject);
        }
        slots.Clear();
    }

    public override void Open()
    {
        base.Open();
        gameObject.SetActive(true);
    }

    public override void Close()
    {
        base.Close();
        gameObject.SetActive(false);
    }
}
