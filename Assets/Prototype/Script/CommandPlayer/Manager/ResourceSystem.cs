using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceSystem : MonoBehaviour, IManager
{
    public enum ResourceType { Gold, Gear }
    public static ResourceSystem Instance { get; private set; }

    public bool IsDontDestroy => true;

    [Header("Resource Settings")]
    public int currentGold = 0;
    public int currentGear = 0; // ���� ��� ���ɼ��� ����.


    // �̱��� �ν��Ͻ�
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    // �̴Ͼ� ������ �ʿ��� �ڿ� ����
    public bool HasEnoughResource(MinionType type)
    {
        return currentGold >= GetCost(type);
    }

    // �̴Ͼ� ������ �ڿ� �Ҹ�
    public void ConsumeResource(MinionType type)
    {
        currentGold -= GetCost(type);
    }

    // �̴Ͼ� ������ �Ҹ�� �ڿ��� ��
    public int GetCost(MinionType type)
    {
        return type switch
        {
            MinionType.Melee => 50,
            MinionType.Ranged => 75,
            MinionType.Elite => 150,
            _ => 0
        };
    }

    public event Action<ResourceType, int> OnResourceChanged;
    /// <summary>
    /// �̴Ͼ� óġ�� ���� �� �ִ� �ڿ��� Ÿ�԰� ��
    /// SO�� �����ؼ� MinionDataSO.goldReward ���� �޾ƿ�
    /// </summary>
    /// <param name="type">ȹ���� �ڿ��� ���� (��: Gold, Gear)</param>
    /// <param name="amount">ȹ���� �ڿ��� ��</param>

    public void AddResource(ResourceType type, int amount)
    {
        switch (type)
        {
            case ResourceType.Gold:
                currentGold += amount;
                Debug.Log($"[Resource] ��� +{amount} �� ����: {currentGold}");
                break;

            case ResourceType.Gear:
                currentGear += amount;
                Debug.Log($"[Resource] ��� +{amount} �� ����: {currentGear}");
                break;

                // �� ���� �ڿ��� ����� ���� �߰�
        }

        OnResourceChanged?.Invoke(type, GetResource(type));

    }

    public void AddResourceToPlayer(CommandPlayer player, ResourceType type, int amount)
    {
        if (type == ResourceType.Gold)
        {
            player.photonView.RPC("RpcAddGold", RpcTarget.All, amount);
        }
    }

    #region RPC_GOLD
    /// <summary>
    /// ��Ʈ��ũ ȯ�濡�� �ڿ� �߰�
    /// </summary>
    [PunRPC]
    public void RpcAddResource(int type, int amount)
    {
        AddResource((ResourceType)type, amount);
    }

    public void AddResourceNetwork(ResourceType type, int amount, CommandPlayer commandPlayer)
    {
        var photonView = ((MonoBehaviour)commandPlayer).GetComponent<PhotonView>();
        if (PhotonNetwork.InRoom)
        {
            photonView.RPC("RpcAddResource", RpcTarget.All, (int)type, amount);
        }
        else
        {
            AddResource(type, amount);
        }
    }
    #endregion

    public int GetResource(ResourceType type)
    {
        return type switch
        {
            ResourceType.Gold => currentGold,
            ResourceType.Gear => currentGear,
            _ => 0
        };
    }

    public void Initialize()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void Cleanup()
    {
        if (PhotonNetwork.IsConnected)
        {
            PhotonNetwork.Disconnect();
        }
    }

    public GameObject GetGameObject() => this.gameObject;
}

