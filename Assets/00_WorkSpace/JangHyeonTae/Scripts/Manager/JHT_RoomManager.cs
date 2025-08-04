using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class JHT_RoomManager : MonoBehaviour, IManager
{

    [SerializeField] private GameObject playerPanelPrefab;
    public Dictionary<int, JHT_PlayerPanelItem> playerPanelDic = new();
    
    public Action<bool> OnStartButtonActive;
    public Func<RectTransform> OnSetRedParent;
    public Func<RectTransform> OnSetBlueParent;
    public Action OnLeaveRoom;
    public Action<int> OnChangeScene;

    #region IManager

    public bool IsDontDestroy => false;


    public void Initialize()
    {
        Init();
    }

    public void Cleanup()
    {
        Outit();
    }

    public GameObject GetGameObject() => this.gameObject;

    #endregion

    private void Init()
    {
        ManagerGroup.Instance.GetManager<JHT_TeamManager>().OnChangeTeam += ChangeTeam;
        OnLeaveRoom += LeaveRoom;
    }

    private void Outit()
    {
        ManagerGroup.Instance.GetManager<JHT_TeamManager>().OnChangeTeam -= ChangeTeam;
    }


    #region 원래는 다른플레이어 패널 생성 이었지만 지금은 마스터 클라이언트가 바뀔시에만 사용(OnPlayerPropertiesUpdate에서 다른 플레이어 생성)
    public void PlayerPanelSpawn(Player player)
    {
        if (playerPanelDic.TryGetValue(player.ActorNumber, out JHT_PlayerPanelItem panel))
        {
            OnStartButtonActive?.Invoke(true);
            panel.Init(player);
            return;
        }

        GameObject obj = Instantiate(playerPanelPrefab);
        obj.transform.SetParent(SetPanelParent(player));
        JHT_PlayerPanelItem playerPanel = obj.GetComponent<JHT_PlayerPanelItem>();
        playerPanel.Init(player);
        playerPanelDic.Add(player.ActorNumber, playerPanel);
    }
    #endregion

    #region 플레이어 패널 생성
    public void PlayerPanelSpawn()
    {
        PhotonNetwork.AutomaticallySyncScene = true;

        if (!PhotonNetwork.IsMasterClient)
        {
            OnStartButtonActive?.Invoke(false);
        }

        foreach (Player player in PhotonNetwork.PlayerList)
        {
            if (player.CustomProperties.TryGetValue("Team", out object team))
            {
                GameObject obj = Instantiate(playerPanelPrefab);
                obj.transform.SetParent(SetPanelParent(player));
                JHT_PlayerPanelItem playerPanel = obj.GetComponent<JHT_PlayerPanelItem>();
                playerPanel.Init(player);
                playerPanelDic.Add(player.ActorNumber, playerPanel);

                if (player.CustomProperties.ContainsKey("HeroIndex"))
                    playerPanel.SetChangeCharacter(player);

                if (player.CustomProperties.ContainsKey("IsReady"))
                    playerPanel.CheckReady(player);
            }
            else
            {
                Debug.Log($"플레이어 {player.NickName}에 대한 정보 없음");
            }
        }

    }
    #endregion


    #region 팀바꾸기
    public void ChangeTeam(Player player, int red, int blue)
    {
        PhotonNetwork.AutomaticallySyncScene = true;

        if (playerPanelDic.TryGetValue(player.ActorNumber, out var panel))
        {
            Destroy(panel.gameObject);
            playerPanelDic.Remove(player.ActorNumber);
        }

        if (!PhotonNetwork.IsMasterClient)
        {
            OnStartButtonActive?.Invoke(false);
        }

        StartCoroutine(SetTeamCor(player,red,blue));

    }

    private IEnumerator SetTeamCor(Player player,int _red,int _blue)
    {

        while (!PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey("RedCount") ||
           !PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey("BlueCount"))
        {
            yield return null;
        }

        ManagerGroup.Instance.GetManager<JHT_TeamManager>().SetChangePlayerTeam(player, _red, _blue);

        while (!player.CustomProperties.ContainsKey("Team"))
            yield return null;

        yield return new WaitForSeconds(0.2f);
        GameObject obj = Instantiate(playerPanelPrefab);
        obj.transform.SetParent(SetPanelParent(player));
        JHT_PlayerPanelItem playerPanel = obj.GetComponent<JHT_PlayerPanelItem>();
        playerPanel.Init(player);
        playerPanelDic.Add(player.ActorNumber, playerPanel);
    }


    //팀바꾸기 동기화(OnPlayerPropertiesUpdate에서 생성)
    public void OtherPlayerChangeTeam(Player player)
    {
        if (player == PhotonNetwork.LocalPlayer)
            return;

        PhotonNetwork.AutomaticallySyncScene = true;

        if (playerPanelDic.TryGetValue(player.ActorNumber, out var panel))
        {
            Destroy(panel.gameObject);
            playerPanelDic.Remove(player.ActorNumber);
        }

        StartCoroutine(OtherPlayerSetTeamCor(player));
    }
    private IEnumerator OtherPlayerSetTeamCor(Player player)
    {
        while (!player.CustomProperties.ContainsKey("Team"))
            yield return null;

        yield return new WaitForSeconds(0.1f);

        GameObject obj = Instantiate(playerPanelPrefab);
        obj.transform.SetParent(SetPanelParent(player));
        JHT_PlayerPanelItem newPanel = obj.GetComponent<JHT_PlayerPanelItem>();
        newPanel.Init(player);
        playerPanelDic.Add(player.ActorNumber, newPanel);
    }
    #endregion

    #region 모든 오브젝트의 부모 설정 -> CustomProperty에서 받아서 사용
    public Transform SetPanelParent(Player player)
    {
        if (player.CustomProperties.TryGetValue("Team", out object value))
        {
            if ((int)value == (int)TeamSetting.Blue)
            {
                return OnSetBlueParent?.Invoke();
            }
            else
            {
                return OnSetRedParent?.Invoke();
            }
        }
        else
        {
            Debug.LogError($"TeamManager SetParentFromCustomProperty 팀 정보 없음 {PhotonNetwork.LocalPlayer.ActorNumber}");
            return null;
        }
    }
    #endregion

    #region 다른 플레이어가 떠났을경우
    public void PlayerLeaveRoom(Player player)
    {
        if (playerPanelDic.TryGetValue(player.ActorNumber, out JHT_PlayerPanelItem obj))
        {
            playerPanelDic.Remove(player.ActorNumber);
            Destroy(obj.gameObject);
        }

    }
    #endregion


    #region 게임시작

    public void GameStart()
    {
        Debug.Log("게임씬 로딩 시도 중");

        if (PhotonNetwork.IsMasterClient && AllPlayerReadyCheck()
            && (int)PhotonNetwork.CurrentRoom.CustomProperties["RedCount"] == 2
            && (int)PhotonNetwork.CurrentRoom.CustomProperties["BlueCount"] == 2)
        {
            Debug.Log("조건 충족 → GameScenes 로드");
            SetGameCustomProperty(true);
            PhotonNetwork.LoadLevel("GameScenes");
            //ManagerGroup.Instance.GetManager<YSJ_SystemManager>().LoadSceneWithPreActions("GameScenes");
        }
        else
        {
            Debug.Log("조건 미충족 → 씬 로드 안 함");
        }
    }

    public void SetGameCustomProperty(bool _value)
    {
        ExitGames.Client.Photon.Hashtable gameStart = new();
        bool isStart = _value;
        gameStart["GamePlay"] = isStart;
        PhotonNetwork.LocalPlayer.SetCustomProperties(gameStart);

    }


    public bool AllPlayerReadyCheck()
    {
        foreach (Player player in PhotonNetwork.PlayerList)
        {
            if (!player.CustomProperties.TryGetValue("IsReady", out object value) || !(bool)value)
            {
                return false;
            }
        }

        return true;
    }

    #endregion

    #region 내가 방을 나갔을경우
    public void LeaveRoom()
    {
        foreach (Player player in PhotonNetwork.PlayerList)
        {
            if (playerPanelDic.ContainsKey(player.ActorNumber))
                Destroy(playerPanelDic[player.ActorNumber].gameObject);
        }
        playerPanelDic.Clear();


        ExitGames.Client.Photon.Hashtable props = new();

        if ((TeamSetting)PhotonNetwork.LocalPlayer.CustomProperties["Team"] == TeamSetting.Blue)
        {
            int currentBlue = (int)PhotonNetwork.CurrentRoom.CustomProperties["BlueCount"];
            if (currentBlue > 0)
            {
                props["BlueCount"] = currentBlue - 1;
                PhotonNetwork.CurrentRoom.SetCustomProperties(props);
                Debug.Log($"[RoomManager - PlayerLeaveRoom] : {(int)PhotonNetwork.CurrentRoom.CustomProperties["BlueCount"]}");
            }
        }
        else if ((TeamSetting)PhotonNetwork.LocalPlayer.CustomProperties["Team"] == TeamSetting.Red)
        {
            int currentRed = (int)PhotonNetwork.CurrentRoom.CustomProperties["RedCount"];
            if (currentRed > 0)
            {
                props["RedCount"] = currentRed - 1;
                PhotonNetwork.CurrentRoom.SetCustomProperties(props);
                Debug.Log($"[RoomManager - PlayerLeaveRoom] : {(int)PhotonNetwork.CurrentRoom.CustomProperties["RedCount"]}");
            }
        }
        else
        {
            Debug.LogError("[RoomManager - PlayerLeaveRoom] : Not set team");
        }

        if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue("Team", out object value))
        {
            if ((TeamSetting)value == TeamSetting.Blue || (TeamSetting)value == TeamSetting.Red)
            {
                props["Team"] = null;
                PhotonNetwork.LocalPlayer.SetCustomProperties(props);
            }
        }

        

        PhotonNetwork.LeaveRoom();
    }
    #endregion

    //스페이스바 누르면 해당 플레이어 - 팀 정보 나옴
    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue("Team", out object teamObj) && teamObj != null)
            {
                Debug.Log($"{PhotonNetwork.LocalPlayer.NickName} 번 팀원 , 팀 : {teamObj.ToString()}");
            }
            else
            {
                Debug.Log($"{PhotonNetwork.LocalPlayer.NickName}의 팀 정보 없음.");
            }
        }
    }

    
}
