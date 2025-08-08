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
    public int currentGear = 0; // 추후 사용 가능성이 있음.


    // 싱글턴 인스턴스
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    // 미니언 생성시 필요한 자원 조건
    public bool HasEnoughResource(MinionType type)
    {
        return currentGold >= GetCost(type);
    }

    // 미니언 생성시 자원 소모
    public void ConsumeResource(MinionType type)
    {
        currentGold -= GetCost(type);
    }

    // 미니언 생성시 소모될 자원의 값
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
    /// 미니언 처치시 얻을 수 있는 자원의 타입과 양
    /// SO를 추적해서 MinionDataSO.goldReward 값을 받아옴
    /// </summary>
    /// <param name="type">획득할 자원의 종류 (예: Gold, Gear)</param>
    /// <param name="amount">획득할 자원의 양</param>

    public void AddResource(ResourceType type, int amount)
    {
        switch (type)
        {
            case ResourceType.Gold:
                currentGold += amount;
                Debug.Log($"[Resource] 골드 +{amount} → 현재: {currentGold}");
                break;

            case ResourceType.Gear:
                currentGear += amount;
                Debug.Log($"[Resource] 기어 +{amount} → 현재: {currentGear}");
                break;

                // 더 많은 자원이 생기면 여기 추가
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
    /// 네트워크 환경에서 자원 추가
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

