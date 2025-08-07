using UnityEngine;

public class SHI_Itemget : MonoBehaviour
{
    public SHI_Inventory inventory;
    private void OnTriggerEnter(Collider other)
    {
        // 아이템인지 확인
        if (!other.CompareTag("Item")) return;

        // 아이템 컴포넌트 가져오기
        SHI_ItemBase item = other.GetComponent<SHI_ItemBase>();
        if (item == null)
        {
            Debug.LogWarning("아이템이 SHI_ItemBase 컴포넌트를 가지고 있지 않습니다.");
            return;
        }

        // 인벤토리 접근
        
            inventory = LGH_TestGameManager.Instance.localPlayer?.GetComponent<SHI_Inventory>();
        
        if (inventory == null)
        {
            Debug.LogError("SHI_Inventory 인스턴스를 찾을 수 없습니다.");
            return;
        }

        // 인벤토리 여유 확인
        if (inventory.IsFull())
        {
            Debug.Log("인벤토리가 가득 찼습니다.");
            return;
        }

        // 아이템 추가 시도
        bool added = inventory.AddItem(item);
        if (added)
        {
            // 아이템 습득 후 비활성화 (또는 파괴)
            item.gameObject.SetActive(false); // 아이템 오브젝트 비활성화
            //Destroy(other.gameObject); // 아이템 오브젝트 제거
            //item.gameObject.SetActive(false);
        }
    }
}