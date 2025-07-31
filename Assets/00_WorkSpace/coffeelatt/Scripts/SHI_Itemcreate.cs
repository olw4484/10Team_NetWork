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
            Debug.Log($"inventory �κ��丮�� �����ϴ�.");
#endif
            return;
        }

        if (index >= 0 && index < Createitem.Length)
        {
            GameObject send = Instantiate(Createitem[index], spawnpoint.position, Quaternion.identity);
            SHI_ItemBase item = send.GetComponent<SHI_ItemBase>();

            Debug.Log($"������{index + 1}������");
            if (item != null)
            {
                bool success = inventory.AddItem(item);
                if (success)
                {
                    send.SetActive(false); // ���忡 �� ���̰� ó�� (�Ǵ� Destroy(send); �� �ı�)
                    Debug.Log($"{item.itemNameEnum} �κ��丮�� �߰���");
                }
                else
                {
                    Debug.LogWarning("�κ��丮�� ������ �߰� ����");
                }
            }
            else
            {
                Debug.LogError("�����տ� SHI_ItemBase ������Ʈ�� ����");
            }
        }
    }
}
