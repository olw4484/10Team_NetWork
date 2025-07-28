using Unity.VisualScripting.ReorderableList;
using UnityEngine;
using UnityEngine.UI;

public class SHI_InventorySlot : MonoBehaviour
{
    public Image icon;
    public Button useButton;

    private SHI_ItemBase currentItem;
    private SHI_ItemManager inventoryManager;

    private void Start()
    {
        inventoryManager = GetComponent<SHI_ItemManager>();
        useButton.onClick.AddListener(OnUseButtonClicked);
        currentItem = GetComponent<SHI_ItemBase>();
        icon = GetComponent<Image>();
    }

    public void SetItem(SHI_ItemBase item)
    {
        currentItem = item;
        icon.sprite = item.GetComponent<SpriteRenderer>()?.sprite; // 아이템 프리팹에 이미지 필요
        //icon.sprite = item._Image;
        icon.enabled = true;
        useButton.interactable = true;
    }

    public void ClearSlot()
    {
        currentItem = null;
        icon.sprite = null;
        icon.enabled = false;
        useButton.interactable = false;
    }

    private void OnUseButtonClicked()
    {
        if (currentItem != null)
        {
            inventoryManager.UseItem(currentItem);
            SHI_Inventory.instance.RemoveItem(currentItem);
        }
    }
}
