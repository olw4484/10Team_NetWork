using Unity.VisualScripting.ReorderableList;
using UnityEngine;
using UnityEngine.UI;

public class SHI_InventorySlot : MonoBehaviour
{
    public Image icon;
    public Button useButton;
    [Header("��������Debug")]
    [SerializeField] private SHI_ItemBase currentItem;
    private SHI_ItemManager inventoryManager;

    private void Start()
    {
        inventoryManager = GameObject.Find("itemmanager").GetComponent<SHI_ItemManager>();
        useButton.onClick.AddListener(OnUseButtonClicked);
        currentItem = GetComponent<SHI_ItemBase>();
        
    }

    public void SetItem(SHI_ItemBase item)
    {
        currentItem = item;
        icon.sprite = item.GetComponent<SpriteRenderer>()?.sprite; // ������ �����տ� �̹��� �ʿ�
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
            if (currentItem.type == 0)
            ClearSlot();
        }
    }
}
