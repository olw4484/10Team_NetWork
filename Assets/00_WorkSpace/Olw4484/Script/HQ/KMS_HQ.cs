using UnityEngine;

public class KMS_HQ : MonoBehaviour
{
    [Header("HQ 설정 데이터")]
    public KMS_HQDataSO data;
    public int teamId;

    private int currentHP;
    private float spawnTimer;

    private void Start()
    {
        currentHP = data.maxHP;
    }

    // 데미지 처리 (공격받을 경우)
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
        Debug.Log($"{gameObject.name} HQ 파괴됨!");
        EventManager.Instance.HQDestroyed(teamId); // 파괴된 팀 ID 전송
        Destroy(gameObject);
    }
}
