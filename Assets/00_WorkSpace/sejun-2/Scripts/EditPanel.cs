using Firebase.Auth;
using Firebase.Database;
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

    // Firebase에서는 딕셔너리 지원.
    [SerializeField] Dictionary<string, object> dictionary = new Dictionary<string, object>();

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

                FirebaseUser user = FirebaseAuth.DefaultInstance.CurrentUser; // 현재 로그인된 Firebase 사용자를 가져옵니다.                                                           
                DatabaseReference root = FirebaseDatabase.DefaultInstance.RootReference;    // Firebase 데이터베이스의 루트 참조를 가져옵니다.
                DatabaseReference userInfo = root.Child("UserData").Child(user.UserId);
                DatabaseReference nameRef = userInfo.Child("name");   // 하나만 바꾸고 싶을때
                nameRef.SetValueAsync(nameInput.text);
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
