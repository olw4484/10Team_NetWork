using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using Photon.Pun;
using System;
using TMPro;

public class JHT_TeamManager : MonoBehaviour
{
    [SerializeField] private Transform redTeam;
    [SerializeField] private Transform blueTeam;

    public int redCount;
    public int blueCount;


    public Action OnRedSelect;
    public Action OnBlueSelect;
    public Action OnCantChangeRed;
    public Action OnCantChangeBlue;
    public Action OnChangeTeam;

    private void Awake()
    {
        OnRedSelect += RedTeamSelect;
        OnBlueSelect += BlueTeamSelect;
        OnCantChangeRed += CantRedChange;
        OnCantChangeBlue += CantBlueChange;
    }

    private void OnDestroy()
    {
        OnRedSelect -= RedTeamSelect;
        OnBlueSelect -= BlueTeamSelect;
        OnCantChangeRed -= CantRedChange;
        OnCantChangeBlue -= CantBlueChange;
    }

    //blueCount 값만 올리기
    public void BlueTeamSelect()
    {
        if (blueCount >= 2)
        {
            OnCantChangeBlue?.Invoke();
            return;
        }

        blueCount++;

        ExitGames.Client.Photon.Hashtable teamCount = new();
        teamCount["BlueCount"] = blueCount;
        PhotonNetwork.CurrentRoom.SetCustomProperties(teamCount);
    }

    //redCount 값만 올리기
    public void RedTeamSelect()
    {
        if (redCount >= 2)
        {
            OnCantChangeRed?.Invoke();
            return;
        }

        redCount++;

        ExitGames.Client.Photon.Hashtable teamCount = new();
        teamCount["RedCount"] = redCount;
        PhotonNetwork.CurrentRoom.SetCustomProperties(teamCount);
    }

    public void CantRedChange()
    {
        JHT_UIManager.UIInstance.ShowPopUp<JHT_RedFullPanel>("RedFullText");
    }

    public void CantBlueChange()
    {
        JHT_UIManager.UIInstance.ShowPopUp<JHT_BlueFullPanel>("BlueFullText");
    }

    //red,blue Count 받아온 값으로 team정하기
    public void SetPlayerTeam(Player player)
    {
        TeamSetting setting;

        int red = (int)PhotonNetwork.CurrentRoom.CustomProperties["RedCount"];
        int blue = (int)PhotonNetwork.CurrentRoom.CustomProperties["BlueCount"];

        if (red > blue)
        {
            OnBlueSelect?.Invoke();
            setting = TeamSetting.Blue;
        }
        else
        {
            OnRedSelect?.Invoke();
            setting = TeamSetting.Red;
        }

        ExitGames.Client.Photon.Hashtable props = new();
        props["Team"] = setting;
        player.SetCustomProperties(props);
    }

    public void ChangeTeam()
    {

    }
}

public enum TeamSetting
{
    None,
    Red,
    Blue
}