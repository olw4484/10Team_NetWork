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
        //��ΰ� �����ؾ��ϱ� ������ �ΰ��� ī��Ʈ customproperty�� ����
        int red = PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey("RedCount") 
            ? (int)PhotonNetwork.CurrentRoom.CustomProperties["RedCount"] : 0;

        int blue = PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey("BlueCount") 
            ? (int)PhotonNetwork.CurrentRoom.CustomProperties["BlueCount"] : 0;

        ExitGames.Client.Photon.Hashtable roomProps = new();

        if (blue < red)
        {
            // Blue������ ����
            SetTeamCustomProperty(TeamSetting.Blue);
            blue++;
            roomProps["BlueCount"] = blue;
            PhotonNetwork.CurrentRoom.SetCustomProperties(roomProps);
            return blueTeam;
        }
        else
        {
            // Red������ ����
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