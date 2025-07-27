using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using Photon.Pun;

public class JHT_TeamManager : MonoBehaviour
{
    [SerializeField] private Transform redTeam;
    [SerializeField] private Transform blueTeam;
    public void TeamInit()
    {
        SetTeam();
    }

    public Transform SetTeam()
    {
        //모두가 공유해야하기 때문에 두개의 카운트 customproperty로 설정
        int red = PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey("RedCount") 
            ? (int)PhotonNetwork.CurrentRoom.CustomProperties["RedCount"] : 0;

        int blue = PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey("BlueCount") 
            ? (int)PhotonNetwork.CurrentRoom.CustomProperties["BlueCount"] : 0;

        ExitGames.Client.Photon.Hashtable roomProps = new();

        if (blue < red)
        {
            // Blue팀으로 배정
            SetTeamCustomProperty(TeamSetting.Blue);
            blue++;
            roomProps["BlueCount"] = blue;
            PhotonNetwork.CurrentRoom.SetCustomProperties(roomProps);
            return blueTeam;
        }
        else
        {
            // Red팀으로 배정
            SetTeamCustomProperty(TeamSetting.Red);
            red++;
            roomProps["RedCount"] = red;
            PhotonNetwork.CurrentRoom.SetCustomProperties(roomProps);
            return redTeam;
        }
    }


    private void SetTeamCustomProperty(TeamSetting teamSettng)
    {
        ExitGames.Client.Photon.Hashtable team = new();
        team["Team"] = teamSettng;
        PhotonNetwork.LocalPlayer.SetCustomProperties(team);
    }

    public void SetPlayerView()
    {

    }
}

public enum TeamSetting
{
    Red,
    Blue
}