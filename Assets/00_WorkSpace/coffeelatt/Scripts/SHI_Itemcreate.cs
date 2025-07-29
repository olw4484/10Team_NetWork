using UnityEngine;

public class SHI_Itemcreate : MonoBehaviour
{
    public GameObject[] Createitem;

    public Transform spawnpoint;

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
        if (index >= 0 && index < Createitem.Length)
        {
            GameObject send = Instantiate(Createitem[index], spawnpoint.position, Quaternion.identity);
            SHI_ItemBase item = send.GetComponent<SHI_ItemBase>();
            Debug.Log($"아이템{index + 1}생성됨");
            if (item != null)
            {
                bool success = SHI_Inventory.instance.AddItem(item);
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
