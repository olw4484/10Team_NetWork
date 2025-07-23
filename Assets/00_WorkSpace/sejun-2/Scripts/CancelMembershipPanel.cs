using Firebase.Auth;
using Firebase.Extensions;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CancelMembershipPanel : MonoBehaviour
{
    [SerializeField] GameObject lobbyPanel;
    [SerializeField] TMP_Text idText;
    [SerializeField] Button backButton;
    [SerializeField] Button DeleteUserButton;

    private void Awake()
    {
        backButton.onClick.AddListener(Back);
        DeleteUserButton.onClick.AddListener(CancelMembership);
    }

    private void Back()
    {
        gameObject.SetActive(false);
        lobbyPanel.SetActive(true);
    }

    private void CancelMembership()
    {
        FirebaseUser user = FirebaseManager.Auth.CurrentUser;
        user.DeleteAsync()
            .ContinueWithOnMainThread(task =>
            {
                if (task.IsCanceled)
                {
                    Debug.LogError("���� ���� ��ҵ�");
                    return;
                }
                if (task.IsFaulted)
                {
                    Debug.LogError($"���� ���� ����. ���� : {task.Exception}");
                    return;
                }

                Debug.Log("���� ���� ����");
                FirebaseManager.Auth.SignOut();
                // ȸ�� Ż�� �� �κ�� ���ư���
                gameObject.SetActive(false);
                lobbyPanel.SetActive(true);
        });
    }



}
