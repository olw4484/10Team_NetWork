using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerManager : MonoBehaviour, IManager
{
    public static PlayerManager Instance { get; private set; }
    public List<CommandPlayer> AllPlayers { get; private set; } = new List<CommandPlayer>();

    public bool IsDontDestroy => false;

    public CommandPlayer GetCommandPlayerByTeam(int teamId)
    {
        return AllPlayers.FirstOrDefault(p => p.teamId == teamId);
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