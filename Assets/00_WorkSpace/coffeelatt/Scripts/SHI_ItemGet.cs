using UnityEngine;

public class SHI_Itemget : MonoBehaviour
{
    public SHI_Inventory inventory;
    private void OnTriggerEnter(Collider other)
    {
        // ���������� Ȯ��
        if (!other.CompareTag("Item")) return;

        // ������ ������Ʈ ��������
        SHI_ItemBase item = other.GetComponent<SHI_ItemBase>();
        if (item == null)
        {
            Debug.LogWarning("�������� SHI_ItemBase ������Ʈ�� ������ ���� �ʽ��ϴ�.");
            return;
        }

        // �κ��丮 ����
        
            inventory = LGH_TestGameManager.Instance.localPlayer?.GetComponent<SHI_Inventory>();
        
        if (inventory == null)
        {
            Debug.LogError("SHI_Inventory �ν��Ͻ��� ã�� �� �����ϴ�.");
            return;
        }

        // �κ��丮 ���� Ȯ��
        if (inventory.IsFull())
        {
            Debug.Log("�κ��丮�� ���� á���ϴ�.");
            return;
        }

        // ������ �߰� �õ�
        bool added = inventory.AddItem(item);
        if (added)
        {
            // ������ ���� �� ��Ȱ��ȭ (�Ǵ� �ı�)
            item.gameObject.SetActive(false); // ������ ������Ʈ ��Ȱ��ȭ
            //Destroy(other.gameObject); // ������ ������Ʈ ����
            //item.gameObject.SetActive(false);
        }
    }
}