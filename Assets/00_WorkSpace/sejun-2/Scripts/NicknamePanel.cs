using Firebase.Auth;
using Firebase.Extensions;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NicknamePanel : MonoBehaviour
{
    [SerializeField] GameObject loginPanel;
    [SerializeField] GameObject lobbyPanel;

    [SerializeField] TMP_InputField nicknameInput;

    [SerializeField] Button confirmButton;
    [SerializeField] Button backButton;

    private void Awake()
    {
        confirmButton.onClick.AddListener(Confirm);
        backButton.onClick.AddListener(Back);
    }

    private void Confirm()
    {
        UserProfile profile = new UserProfile();
        profile.DisplayName = nicknameInput.text;

        FirebaseUser user = FirebaseManager.Auth.CurrentUser;
        user.UpdateUserProfileAsync(profile)
            .ContinueWithOnMainThread(task =>
            {
                if (task.IsCanceled)
                {
                    Debug.LogError("���� �г��� ���� ��ҵ�");
                    return;
                }
                if (task.IsFaulted)
                {
                    Debug.LogError($"���� �г��� ���� ����. ���� : {task.Exception}");
                    return;
                }

                Debug.Log("���� �г��� ���� ����");
                lobbyPanel.SetActive(true);
                gameObject.SetActive(false);
            });
    }

    private void Back()
    {
        FirebaseManager.Auth.SignOut();
        loginPanel.SetActive(true);
        gameObject.SetActive(false);
    }
}
