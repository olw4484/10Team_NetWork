using Firebase.Auth;
using Firebase.Extensions;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class EditPanel : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] GameObject lobbyPanel;
    [Header("Input Fields")]
    [SerializeField] TMP_InputField nameInput;
    [SerializeField] TMP_InputField passInput;
    [SerializeField] TMP_InputField passConfirmInput;
    [Header("Texts")]
    [SerializeField] TMP_Text emailText;
    [SerializeField] TMP_Text userIdText;
    [Header("Buttons")]
    [SerializeField] Button nicknameConfirmButton;
    [SerializeField] Button passConfirmButton;
    [SerializeField] Button backButton;

    private void Awake()
    {
        nicknameConfirmButton.onClick.AddListener(ChangeNickname);
        passConfirmButton.onClick.AddListener(ChangePassword);
        backButton.onClick.AddListener(Back);
    }

    private void OnEnable()
    {
        FirebaseUser user = FirebaseManager.Auth.CurrentUser;

        emailText.text = user.Email;
        passInput.text = ""; // 비밀번호 입력 필드는 초기화
        nameInput.text = user.DisplayName;
        userIdText.text = user.UserId;
    }


    private void ChangeNickname()
    {
        UserProfile profile = new UserProfile();
        profile.DisplayName = nameInput.text;

        FirebaseUser user = FirebaseManager.Auth.CurrentUser;
        user.UpdateUserProfileAsync(profile)
            .ContinueWithOnMainThread(task =>
            {
                if (task.IsCanceled)
                {
                    Debug.LogError("닉네임 변경 취소");
                    return;
                }
                if (task.IsFaulted)
                {
                    Debug.LogError($"닉네임 변경 실패. 이유 : {task.Exception}");
                    return;
                }

                Debug.Log("닉네임 변경 성공");
            });
    }

    private void ChangePassword()
    {
        if (passInput.text != passConfirmInput.text)
        {
            Debug.LogError("비밀번호가 일치하지 않음");
            return;
        }

        FirebaseUser user = FirebaseManager.Auth.CurrentUser;
        user.UpdatePasswordAsync(passInput.text)
            .ContinueWithOnMainThread(task =>
            {
                if (task.IsCanceled)
                {
                    Debug.LogError("비밀번호 변경 취소");
                    return;
                }
                if (task.IsFaulted)
                {
                    Debug.LogError($"비밀번호 변경 실패. 이유 : {task.Exception}");
                    return;
                }

                Debug.Log("비밀번호 변경 성공");
            });
    }

    private void Back()
    {
        lobbyPanel.SetActive(true);
        gameObject.SetActive(false);
    }
}
