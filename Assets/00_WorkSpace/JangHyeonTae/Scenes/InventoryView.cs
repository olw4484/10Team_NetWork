using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class InventoryView : JHT_BaseUI
{
    [Header("���� ������ �� ���� �θ�")]
    [SerializeField] private SHI_InventorySlot slotPrefab;
    [SerializeField] private Transform slotParent;

    private List<SHI_InventorySlot> slotList = new();

    private void Start()
    {
        // ���� UI �ʱ�ȭ
        InitSlots();

        // �̺�Ʈ ����
        SHI_Inventory.instance.itemadd.AddListener(RefreshUI);
        SHI_Inventory.instance.itemremove.AddListener(RefreshUI);

        // �ʱ� ���� �ݿ�
        RefreshUI();
    }

    private void InitSlots()
    {
        int slotCount = SHI_Inventory.instance.maxSlots;

        for (int i = 0; i < slotCount; i++)
        {
            SHI_InventorySlot slot = Instantiate(slotPrefab, slotParent);
            slot.ClearSlot(); // �ʱ⿡�� ����д�
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
