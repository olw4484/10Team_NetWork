using Photon.Pun;
using UnityEngine;

public class GoldTickSystem : MonoBehaviour
{
    private CommandPlayer player; // CommandPlayer ���� ����
    public int goldPerTick = 10; // ƽ�� ��� ��差
    public float tickInterval = 5f; // ƽ ���� (��)
    private float nextTickTime; // ���� ƽ�� �߻��� �ð�

    void Awake()
    {
        // ������ CommandPlayer ������Ʈ�� ���� ������Ʈ�� ã�Ƽ� �Ҵ�
        // �� ������Ʈ�� ���� ������ player ������ ������ null�� �˴ϴ�.
        player = FindObjectOfType<CommandPlayer>();
        nextTickTime = Time.time + tickInterval;
    }

    void Update()
    {
        // player�� null�� �ƴ� ���� ������ ����
        if (player != null)
        {
            if (Time.time >= nextTickTime)
            {
                // ResourceSystem�� �ν��Ͻ��� �����ϴ��� Ȯ��
                if (ResourceSystem.Instance != null)
                {
                    ResourceSystem.Instance.AddResourceToPlayer(player, ResourceSystem.ResourceType.Gold, goldPerTick);
                    nextTickTime = Time.time + tickInterval;
                }
                else
                {
                    Debug.LogError("ResourceSystem.Instance�� �������� �ʽ��ϴ�.");
                }
            }
        }
        else
        {
            // player�� null�� ��� �α׸� ����Ͽ� ����뿡 ����
            Debug.LogError("CommandPlayer�� ���� �������� �ʽ��ϴ�. GoldTickSystem�� �ùٸ��� ������ �� �����ϴ�.");
        }
    }
}