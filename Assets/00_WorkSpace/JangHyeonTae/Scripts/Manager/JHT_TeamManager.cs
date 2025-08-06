using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using Photon.Pun;
using System;
using TMPro;

public enum TeamSetting
{
    Red,
    Blue,
    None
}

public class JHT_TeamManager : MonoBehaviour, IManager
{
    [SerializeField] private GameObject blueFullPopup;
    [SerializeField] private GameObject redFullPopup;

    public int redCount;
    public int blueCount;


    public Action<Player> OnRedSelect;
    public Action<Player> OnBlueSelect;
    public event Action OnCantChangeRed;
    public event Action OnCantChangeBlue;
    public Action<Player, int, int> OnChangeTeam;


    private bool isRoom;
    public Action<bool> OnState;

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
        OnRedSelect += RedTeamSelect;
        OnBlueSelect += BlueTeamSelect;

        // 팀바꾸기 실패시
        OnCantChangeRed += CantRedChange;
        OnCantChangeBlue += CantBlueChange;
    }

    private void Outit()
    {
        OnRedSelect -= RedTeamSelect;
        OnBlueSelect -= BlueTeamSelect;

        // 팀바꾸기 실패시
        OnCantChangeRed -= CantRedChange;
        OnCantChangeBlue -= CantBlueChange;
    }

    #region Red/Blue 팀 값설정
    //blueCount 값만 올리기
    public void BlueTeamSelect(Player player)
    {
        StartCoroutine(AddBlueValueCor());

        if (blueCount >= 2)
        {
            Debug.Log("[TeamManager - BlueTeamSelect] : 블루팀 못감");
            OnCantChangeBlue?.Invoke();
            return;
        }

        if (player.CustomProperties.TryGetValue("CurState", out object value))
        {

            //플레이어 게임상태 가져와서 구분
            if ((CurrentState)value == CurrentState.InRoom)
            {
                if ((TeamSetting)player.CustomProperties["Team"] == TeamSetting.Blue)
                    return;

                OnChangeTeam?.Invoke(player, -1, 1);
            }
            else
            {
                blueCount++;
                SetTeamCount(redCount, blueCount);
            }
        }
        else
        {
            Debug.Log($"현재 {player.NickName}에 대한 상태 없음");
        }
    }


    IEnumerator AddBlueValueCor()
    {
        yield return new WaitForSeconds(0.1f);
        blueCount = (int)PhotonNetwork.CurrentRoom.CustomProperties["BlueCount"];
    }


    //redCount 값만 올리기
    public void RedTeamSelect(Player player)
    {
        StartCoroutine(AddRedValueCor());

        if (redCount >= 2)
        {
            Debug.Log("[TeamManager - RedTeamSelect] : 레드팀 못감");
            OnCantChangeRed?.Invoke();
            return;
        }

        if (player.CustomProperties.TryGetValue("CurState", out object value))
        {
            //플레이어 게임상태 가져와서 구분
            if ((CurrentState)value == CurrentState.InRoom)
            {
                if ((TeamSetting)player.CustomProperties["Team"] == TeamSetting.Red)
                    return;

                OnChangeTeam?.Invoke(player, 1, -1);
            }
            else
            {
                redCount++;
                SetTeamCount(redCount, blueCount);
            }
        }
        else
        {
            Debug.Log($"현재 {player.NickName}에 대한 상태 없음");
        }
    }

    IEnumerator AddRedValueCor()
    {
        yield return new WaitForSeconds(0.1f);

        redCount = (int)PhotonNetwork.CurrentRoom.CustomProperties["RedCount"];
    }
    #endregion

    #region 플레이어 시작시 팀 구분
    //red,blue Count 받아온 값으로 team정하기
    public void SetPlayerTeam(Player player)
    {
        TeamSetting setting;

        int red = (int)PhotonNetwork.CurrentRoom.CustomProperties["RedCount"];
        int blue = (int)PhotonNetwork.CurrentRoom.CustomProperties["BlueCount"];

        if (red > blue)
        {
            OnBlueSelect?.Invoke(player);
            setting = TeamSetting.Blue;
        }
        else
        {
            OnRedSelect?.Invoke(player);
            setting = TeamSetting.Red;
        }

        SetTeam(setting, player);
    }
    #endregion

    #region 플레이어 팀 선택시 팀 변경
    public void SetChangePlayerTeam(Player player, int redSelect, int blueSelect)
    {
        TeamSetting setting;

        int red = (int)PhotonNetwork.CurrentRoom.CustomProperties["RedCount"];
        int blue = (int)PhotonNetwork.CurrentRoom.CustomProperties["BlueCount"];

        red += redSelect;
        blue += blueSelect;

        if (redSelect > 0)
        {
            setting = TeamSetting.Red;
        }
        else
        {
            setting = TeamSetting.Blue;
        }

        SetTeamCount(red, blue);
        SetTeam(setting, player);
    }
    #endregion


    #region 룸에 red,blue Count 생성
    public void SetTeamCount(int red, int blue)
    {
        redCount = red;
        blueCount = blue;

        ExitGames.Client.Photon.Hashtable teamCount = new();
        teamCount["RedCount"] = redCount;
        teamCount["BlueCount"] = blueCount;
        PhotonNetwork.CurrentRoom.SetCustomProperties(teamCount);
    }

    public void SetTeam(TeamSetting setting,Player player)
    {
        ExitGames.Client.Photon.Hashtable props = new();
        props["Team"] = setting;
        player.SetCustomProperties(props);
    }

    #endregion

    #region PopUp
    public void CantRedChange()
    {
        YSJ_UISpawnFactory.SpawnPopup(redFullPopup);
    }

    public void CantBlueChange()
    {
        YSJ_UISpawnFactory.SpawnPopup(blueFullPopup);
    }

    #endregion
}
