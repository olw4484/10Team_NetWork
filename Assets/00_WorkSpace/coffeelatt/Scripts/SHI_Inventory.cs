using System.Collections.Generic;
using UnityEngine;
using static SHI_ItemBase;

public class SHI_Inventory : MonoBehaviour
{
    public static SHI_Inventory instance;
    public List<SHI_ItemBase> items = new List<SHI_ItemBase>(); // ���� ������ ����Ʈ
    public int maxSlots = 6; // �κ��丮 �ִ� ���� ��

    public Events.VoidEvent itemadd =new Events.VoidEvent();
    public Events.VoidEvent itemremove = new Events.VoidEvent();
    private void Awake()
    {
        if (instance == null)
            instance = this;    
        else
            Destroy(gameObject);
    }

    public bool AddItem(SHI_ItemBase newItem)
    {
        if (items.Count >= maxSlots)
        {
            Debug.Log("�κ��丮�� ���� á���ϴ�.");
            return false;
        }

        items.Add(newItem);
       itemadd.Invoke();
        Debug.Log($"{newItem.itemNameEnum} �κ��丮�� �߰���.");
        return true;
    }

    public void RemoveItem(SHI_ItemBase item)
    {
        if (items.Contains(item)&&item.type<=0)
        {
            items.Remove(item);
            itemremove.Invoke();
            Debug.Log($"{item.itemNameEnum} �κ��丮���� ���ŵ�.");
        }
    }
    


    public void UseItemFromInventory(SHI_ItemBase item)
    {
        SHI_ItemManager manager = FindObjectOfType<SHI_ItemManager>();
        if (manager != null)
        {
            manager.UseItem(item); // ȿ�� ����
            SHI_Inventory.instance.RemoveItem(item); // �κ��丮���� ����
            //Destroy(item.gameObject); // ������ ���� 
        }
    }
}