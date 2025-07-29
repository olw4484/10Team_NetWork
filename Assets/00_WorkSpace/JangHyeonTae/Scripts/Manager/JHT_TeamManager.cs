using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using Photon.Pun;
using System;
using TMPro;

public enum TeamSetting
{
    None,
    Red,
    Blue
}

public class JHT_TeamManager : MonoBehaviour
{
    [SerializeField] private GameObject blueFullPopup;
    [SerializeField] private GameObject redFullPopup;

    public int redCount;
    public int blueCount;


    public Action<Player> OnRedSelect;
    public Action<Player> OnBlueSelect;
    public event Action OnCantChangeRed;
    public event Action OnCantChangeBlue;
    public Action<Player,int,int> OnChangeTeam;

    private CurrentState curState;

    private void Awake()
    {
        OnRedSelect += RedTeamSelect;
        OnBlueSelect += BlueTeamSelect;

        // ���ٲٱ� ���н�
        OnCantChangeRed += CantRedChange;
        OnCantChangeBlue += CantBlueChange;
    }

    private void OnDestroy()
    {
        OnRedSelect -= RedTeamSelect;
        OnBlueSelect -= BlueTeamSelect;

        // ���ٲٱ� ���н�
        OnCantChangeRed -= CantRedChange;
        OnCantChangeBlue -= CantBlueChange;
    }


    #region Red/Blue �� ������
    //blueCount ���� �ø���
    public void BlueTeamSelect(Player player)
    {
        StartCoroutine(AddBlueValueCor());

        if (blueCount >= 2)
        {
            OnCantChangeBlue?.Invoke();
            return;
        }

        if (player.CustomProperties.TryGetValue("CurState", out object value))
        {
            //�÷��̾� ���ӻ��� �����ͼ� ����
            if ((CurrentState)value == CurrentState.InRoom)
            {
                if ((TeamSetting)player.CustomProperties["Team"] == TeamSetting.Blue)
                    return;

                OnChangeTeam?.Invoke(player,-1,1);
            }
            else
            {
                blueCount++;
                SetTeamCount(redCount, blueCount);
            }
        }
        else
        {
            Debug.Log($"���� {player.NickName}�� ���� ���� ����");
        }
    }

    IEnumerator AddBlueValueCor()
    {
        yield return new WaitForSeconds(0.1f);
        blueCount = (int)PhotonNetwork.CurrentRoom.CustomProperties["BlueCount"];
    }


    //redCount ���� �ø���
    public void RedTeamSelect(Player player)
    {
        StartCoroutine(AddRedValueCor());

        if (redCount >= 2)
        {
            OnCantChangeRed?.Invoke();
            return;
        }

        if (player.CustomProperties.TryGetValue("CurState", out object value))
        {
            //�÷��̾� ���ӻ��� �����ͼ� ����
            if ((CurrentState)value == CurrentState.InRoom)
            {
                if ((TeamSetting)player.CustomProperties["Team"] == TeamSetting.Red)
                    return;

                OnChangeTeam?.Invoke(player,1,-1);
            }
            else
            {
                redCount++;
                SetTeamCount(redCount, blueCount);
            }
        }
        else
        {
            Debug.Log($"���� {player.NickName}�� ���� ���� ����");
        }
    }

    IEnumerator AddRedValueCor()
    {
        yield return new WaitForSeconds(0.1f);

        redCount = (int)PhotonNetwork.CurrentRoom.CustomProperties["RedCount"];
    }
    #endregion

    #region �÷��̾� ���۽� �� ����
    //red,blue Count �޾ƿ� ������ team���ϱ�
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

        ExitGames.Client.Photon.Hashtable props = new();
        props["Team"] = setting;
        player.SetCustomProperties(props);
    }
    #endregion

    #region �÷��̾� �� ���ý� �� ����
    public void SetChangePlayerTeam(Player player,int redSelect, int blueSelect)
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

        ExitGames.Client.Photon.Hashtable count = new();
        count["RedCount"] = red;
        count["BlueCount"] = blue;
        PhotonNetwork.CurrentRoom.SetCustomProperties(count);

        ExitGames.Client.Photon.Hashtable props = new();
        props["Team"] = setting;
        player.SetCustomProperties(props);
    }
    #endregion

    #region �뿡 red,blue Count ����
    public void SetTeamCount(int red, int blue)
    {
        redCount = red;
        blueCount = blue;

        ExitGames.Client.Photon.Hashtable teamCount = new();
        teamCount["RedCount"] = redCount;
        teamCount["BlueCount"] = blueCount;
        PhotonNetwork.CurrentRoom.SetCustomProperties(teamCount);
    }

    #endregion

    #region PopUp
    public void CantRedChange()
    {
        YSJ_UISpawnFactory.ShowPopup(redFullPopup);
    }

    public void CantBlueChange()
    {
        YSJ_UISpawnFactory.ShowPopup(blueFullPopup);
    }
    #endregion
}
