using System.Collections.Generic;
using UnityEngine;
using static SHI_ItemBase;

public class SHI_Inventory : MonoBehaviour
{
    public static SHI_Inventory instance;
    public List<SHI_ItemBase> items = new List<SHI_ItemBase>(); // 소지 아이템 리스트
    public int maxSlots = 6; // 인벤토리 최대 슬롯 수

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
            Debug.Log("인벤토리가 가득 찼습니다.");
            return false;
        }

        items.Add(newItem);
       itemadd.Invoke();
        Debug.Log($"{newItem.itemNameEnum} 인벤토리에 추가됨.");
        return true;
    }

    public void RemoveItem(SHI_ItemBase item)
    {
        if (items.Contains(item)&&item.type<=0)
        {
            items.Remove(item);
            itemremove.Invoke();
            Debug.Log($"{item.itemNameEnum} 인벤토리에서 제거됨.");
        }
    }
    


    public void UseItemFromInventory(SHI_ItemBase item)
    {
        SHI_ItemManager manager = FindObjectOfType<SHI_ItemManager>();
        if (manager != null)
        {
            manager.UseItem(item); // 효과 적용
            SHI_Inventory.instance.RemoveItem(item); // 인벤토리에서 제거
            //Destroy(item.gameObject); // 프리팹 제거 
        }
    }
}