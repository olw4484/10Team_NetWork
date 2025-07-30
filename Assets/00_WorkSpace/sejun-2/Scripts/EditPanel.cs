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
        passInput.text = ""; // ��й�ȣ �Է� �ʵ�� �ʱ�ȭ
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
                    Debug.LogError("�г��� ���� ���");
                    return;
                }
                if (task.IsFaulted)
                {
                    Debug.LogError($"�г��� ���� ����. ���� : {task.Exception}");
                    return;
                }

                Debug.Log("�г��� ���� ����");
            });
    }

    private void ChangePassword()
    {
        if (passInput.text != passConfirmInput.text)
        {
            Debug.LogError("��й�ȣ�� ��ġ���� ����");
            return;
        }

        FirebaseUser user = FirebaseManager.Auth.CurrentUser;
        user.UpdatePasswordAsync(passInput.text)
            .ContinueWithOnMainThread(task =>
            {
                if (task.IsCanceled)
                {
                    Debug.LogError("��й�ȣ ���� ���");
                    return;
                }
                if (task.IsFaulted)
                {
                    Debug.LogError($"��й�ȣ ���� ����. ���� : {task.Exception}");
                    return;
                }

                Debug.Log("��й�ȣ ���� ����");
            });
    }

    private void Back()
    {
        lobbyPanel.SetActive(true);
        gameObject.SetActive(false);
    }
}
