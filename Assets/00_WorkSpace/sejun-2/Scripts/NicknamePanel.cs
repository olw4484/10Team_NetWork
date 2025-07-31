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
                    Debug.LogError("유저 닉네임 설정 취소됨");
                    return;
                }
                if (task.IsFaulted)
                {
                    Debug.LogError($"유저 닉네임 설정 실패. 이유 : {task.Exception}");
                    return;
                }

                Debug.Log("유저 닉네임 설정 성공");
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
