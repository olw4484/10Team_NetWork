using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class SHI_InventorySlot : MonoBehaviour
{
    public Image icon;
    public Outline outline;
    public Button useButton;
    [Header("정상착용Debug")]
    [SerializeField] private SHI_ItemBase currentItem;
    private SHI_ItemManager inventoryManager;
    public SHI_Inventory inventory; // 인벤토리 참조



    private void Start()
    {
        inventoryManager = GameObject.Find("itemmanager").GetComponent<SHI_ItemManager>();
        inventory = GameObject.Find("inventory").GetComponent<SHI_Inventory>();
        useButton.onClick.AddListener(OnUseButtonClicked);
        currentItem = GetComponent<SHI_ItemBase>();
        useButton.interactable = inventoryManager.stat.CurHP.Value > 0;
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
        outline.enabled = false; // 슬롯의 아웃라인 비활성화
    }

    private void OnUseButtonClicked()
    {
        if (currentItem != null)
        {

            bool success = inventoryManager.UseItem(currentItem);
            if (currentItem.type == 2)
                outline.enabled = true; // 슬롯의 아웃라인 활성화

            else
                outline.enabled = false; // 슬롯의 아웃라인 비활성화

            if (success)
            {
                inventory.RemoveItem(currentItem);
                if (currentItem.type <= 0)
                    ClearSlot();
            }
        }
    }
}
