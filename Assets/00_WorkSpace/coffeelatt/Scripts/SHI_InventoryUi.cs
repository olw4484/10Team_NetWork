using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SHI_InventoryUI : MonoBehaviour
{
    public GameObject slotPrefab; // 인벤토리 슬롯 프리팹
    public Transform slotParent;  // 슬롯을 붙일 부모 (ex. GridPanel)
    [SerializeField]
    private List<SHI_InventorySlot> slots = new List<SHI_InventorySlot>();
    public SHI_Inventory inventory;



    private void Start()
    {
        //inventory = GameObject.Find("Hero1").GetComponent<SHI_Inventory>();
        //for (int i = 0; i < SHI_Inventory.instance.maxSlots; i++)
        //{
        //    slots.Add(Instantiate(slotPrefab, slotParent));

        //}

        inventory.itemadd.AddListener(UpdateUI);
        inventory.itemremove.RemoveListener(UpdateUI);
       InitSlots(inventory.maxSlots);
    }
    private void InitSlots(int count)
    {
        for (int i = 0; i < count; i++)
        {
            GameObject go = Instantiate(slotPrefab, slotParent);
            SHI_InventorySlot slot = go.GetComponent<SHI_InventorySlot>();
            slots.Add(slot);
            //slot.ClearSlot(); // 초기화
        }
    }
    
    // UI 갱신
    private void UpdateUI()
    {
        List<SHI_ItemBase> items = inventory.items;

        for (int i = 0; i < slots.Count; i++)
        {
            if (i < items.Count)
            {
                slots[i].SetItem(items[i]);
            }
            else
            {
                slots[i].ClearSlot();
            }
        }
    }
}