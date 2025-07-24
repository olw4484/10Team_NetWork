using UnityEngine;

public class KMS_HQ : MonoBehaviour
{
    [Header("HQ ���� ������")]
    public KMS_HQDataSO data;
    public int teamId;

    private int currentHP;
    private float spawnTimer;

    private void Start()
    {
        currentHP = data.maxHP;
    }

    // ������ ó�� (���ݹ��� ���)
    public void TakeDamage(int damage)
    {
        currentHP -= damage;
        if (currentHP <= 0)
        {
            OnDestroyed();
        }
    }

    private void OnDestroyed()
    {
        Debug.Log($"{gameObject.name} HQ �ı���!");
        EventManager.Instance.HQDestroyed(teamId); // �ı��� �� ID ����
        Destroy(gameObject);
    }
}
