using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.Progress;

public class SHI_InventoryUI : MonoBehaviour
{
    public GameObject slotPrefab; // �κ��丮 ���� ������
    public Transform slotParent;  // ������ ���� �θ� (ex. GridPanel)
    [SerializeField]
    private List<SHI_InventorySlot> slots = new List<SHI_InventorySlot>();
    public SHI_Inventory inventory;



    private void Start()
    {
        inventory = SHI_Inventory.instance;
        //for (int i = 0; i < SHI_Inventory.instance.maxSlots; i++)
        //{
        //    slots.Add(Instantiate(slotPrefab, slotParent));
            
        //}
            
        SHI_Inventory.instance.itemadd.AddListener(UpdateUI);
        SHI_Inventory.instance.itemremove.RemoveListener(UpdateUI);
       InitSlots(inventory.maxSlots);
    }
    private void InitSlots(int count)
    {
        for (int i = 0; i < count; i++)
        {
            GameObject go = Instantiate(slotPrefab, slotParent);
            SHI_InventorySlot slot = go.GetComponent<SHI_InventorySlot>();
            slots.Add(slot);
            //slot.ClearSlot(); // �ʱ�ȭ
        }
    }
    
    // UI ����
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