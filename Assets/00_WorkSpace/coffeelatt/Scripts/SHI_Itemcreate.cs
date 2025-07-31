using UnityEngine;
using static UnityEditor.Progress;

public class SHI_Itemcreate : MonoBehaviour
{
    public GameObject[] Createitem;

    public Transform spawnpoint;

    public SHI_Inventory inventory;

    private void Start() => InitUseInventory();
    private void InitUseInventory()
    {
        if (inventory == null)
        {
            inventory = LGH_TestGameManager.Instance.localPlayer?.GetComponent<SHI_Inventory>();
        }
    }

    private void Update()
    {
        for (int i = 0; i <= Createitem.Length; i++)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1 + i))
            {
                create(i);
            }
        }
    }

    public void create(int index)
    {
        InitUseInventory();
        if (inventory == null)
        {
#if UNITY_EDITOR
            Debug.Log($"inventory 인벤토리가 없습니다.");
#endif
            return;
        }

        if (index >= 0 && index < Createitem.Length)
        {
            GameObject send = Instantiate(Createitem[index], spawnpoint.position, Quaternion.identity);
            SHI_ItemBase item = send.GetComponent<SHI_ItemBase>();

            Debug.Log($"아이템{index + 1}생성됨");
            if (item != null)
            {
                bool success = inventory.AddItem(item);
                if (success)
                {
                    send.SetActive(false); // 월드에 안 보이게 처리 (또는 Destroy(send); 로 파괴)
                    Debug.Log($"{item.itemNameEnum} 인벤토리에 추가됨");
                }
                else
                {
                    Debug.LogWarning("인벤토리에 아이템 추가 실패");
                }
            }
            else
            {
                Debug.LogError("프리팹에 SHI_ItemBase 컴포넌트가 없음");
            }
        }
    }
}
