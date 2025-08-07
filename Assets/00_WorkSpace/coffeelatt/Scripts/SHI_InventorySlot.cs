using UnityEngine;
using UnityEngine.UI;

public class SHI_InventorySlot : MonoBehaviour
{
    public Image icon;
    public Outline outline;
    public Button useButton;
    [Header("��������Debug")]
    [SerializeField] private SHI_ItemBase currentItem;
    private SHI_ItemManager inventoryManager;
    public SHI_Inventory inventory; // �κ��丮 ����

    private void Start()
    {
        /*inventoryManager = GameObject.Find("itemmanager").GetComponent<SHI_ItemManager>();
        inventory = GameObject.Find("inventory").GetComponent<SHI_Inventory>();
        useButton.onClick.AddListener(OnUseButtonClicked);
        currentItem = GetComponent<SHI_ItemBase>();
        useButton.interactable = inventoryManager.stat.CurHP.Value > 0;*/
    }

    public void InitSlot(SHI_ItemManager inventoryManager, SHI_Inventory inventory)
    {
        this.inventoryManager = inventoryManager;
        this.inventory = inventory;

        useButton.onClick.AddListener(OnUseButtonClicked);
        currentItem = GetComponent<SHI_ItemBase>();
        useButton.interactable = inventoryManager.stat.CurHP.Value > 0;
       
        
        
    }

    public void SetItem(SHI_ItemBase item)
    {
        Debug.Log($"������ {item.itemNameEnum}�� ���Կ� �����Ǿ����ϴ�.");
        currentItem = item;
        icon.sprite = item.GetComponent<SpriteRenderer>()?.sprite; // ������ �����տ� �̹��� �ʿ�
        //icon.sprite = item._Image;
        icon.enabled = true;
        useButton.interactable = true;
        if (outline == null)
            outline = GetComponent<Outline>();

        if (outline != null)
            outline.enabled = (currentItem.type == 2);
    }

    public void ClearSlot()
    {
        currentItem = null;
        icon.sprite = null;
        icon.enabled = false;
        useButton.interactable = false;
        outline.enabled = false; // ������ �ƿ����� ��Ȱ��ȭ
    }

    private void OnUseButtonClicked()
    {
        if (currentItem != null)
        {

            bool success = inventoryManager.UseItem(currentItem);
            if (currentItem.type == 2)
                outline.enabled = true; // ������ �ƿ����� Ȱ��ȭ

            else if (currentItem.type == 1)
                outline.enabled = false; // ������ �ƿ����� ��Ȱ��ȭ

            if (success)
            {
                inventory.RemoveItem(currentItem);
                // if (currentItem.type <= 0)
                //ClearSlot();
            }
        }
    }
}
