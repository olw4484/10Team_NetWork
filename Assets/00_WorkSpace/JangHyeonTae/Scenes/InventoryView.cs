using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class InventoryView : JHT_BaseUI
{
    [Header("슬롯 프리팹 및 슬롯 부모")]
    [SerializeField] private SHI_InventorySlot slotPrefab;
    [SerializeField] private Transform slotParent;

    private List<SHI_InventorySlot> slotList = new();

    private void Start()
    {
        // 슬롯 UI 초기화
        InitSlots();

        // 이벤트 연결
        SHI_Inventory.instance.itemadd.AddListener(RefreshUI);
        SHI_Inventory.instance.itemremove.AddListener(RefreshUI);

        // 초기 상태 반영
        RefreshUI();
    }

    private void InitSlots()
    {
        int slotCount = SHI_Inventory.instance.maxSlots;

        for (int i = 0; i < slotCount; i++)
        {
            SHI_InventorySlot slot = Instantiate(slotPrefab, slotParent);
            slot.ClearSlot(); // 초기에는 비워둔다
            slotList.Add(slot);
        }
    }

    private void RefreshUI()
    {
        List<SHI_ItemBase> items = SHI_Inventory.instance.items;

        for (int i = 0; i < slotList.Count; i++)
        {
            if (i < items.Count)
            {
                slotList[i].SetItem(items[i]);
            }
            else
            {
                slotList[i].ClearSlot();
            }
        }
    }
}
