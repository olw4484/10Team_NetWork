using UnityEngine;

public class InventoryHUDModel
{
    private GameObject owner;
    public SHI_Inventory Inventory { get; private set; }
    public SHI_ItemManager inventoryManager { get; private set; }


    public InventoryHUDModel(GameObject owner)
    {
        this.owner = owner;
        Inventory = owner.GetComponent<SHI_Inventory>();
        inventoryManager = owner.GetComponentInChildren<SHI_ItemManager>();
    }
}