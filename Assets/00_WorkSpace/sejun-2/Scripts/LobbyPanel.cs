using Firebase.Auth;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LobbyPanel : MonoBehaviour
{
    [SerializeField] GameObject loginPannel;
    [SerializeField] GameObject editPanel;
    [SerializeField] GameObject DeleteUserPanel;

    [SerializeField] TMP_Text emailText;
    [SerializeField] TMP_Text nameText;
    [SerializeField] TMP_Text userIdText;
    [SerializeField] TMP_InputField NewPasswordInput;

    [SerializeField] Button ChangePasswordButton;
    [SerializeField] Button logoutButton;
    [SerializeField] Button editProfileButton;
    [SerializeField] Button deleteUserButton;

    private void Awake()    // 초기화 메서드
    {
        logoutButton.onClick.AddListener(Logout);
        editProfileButton.onClick.AddListener(EditProfile);
        deleteUserButton.onClick.AddListener(DeleteUser);
        ChangePasswordButton.onClick.AddListener(ChangePassword);
    }

    private void OnEnable() // 패널이 화면에 나타날 때마다 호출됨
    {
        FirebaseUser user = FirebaseManager.Auth.CurrentUser;   // 현재 로그인한 사용자 정보 가져오기

        emailText.text = user.Email;    
        nameText.text = user.DisplayName;

        userIdText.text = user.UserId; // 사용자 ID 표시

        //구글이나 플레이 스토어 등 다른 프로바이더로 로그인한 경우,
        //foreach (IUserInfo userInfo in user.ProviderData) // 사용자 정보에서 프로바이더 데이터 가져오기
        //{
        //    if (userInfo.ProviderId == "password") // 프로바이더 ID가 "password"인 경우
        //    {
        //        emailText.text = userInfo.Email; // 이메일 텍스트 업데이트
        //        break; // 루프 종료
        //    }
        //}

    }

    private void Logout()   // 로그아웃 메서드
    {
        FirebaseManager.Auth.SignOut(); // Firebase에서 로그아웃
        loginPannel.SetActive(true);
        gameObject.SetActive(false);
    }

    private void EditProfile()  // 프로필 수정 메서드
    {
        editPanel.SetActive(true);
        gameObject.SetActive(false);
    }

    private void DeleteUser()   // 회원 탈퇴 메서드
    {
        DeleteUserPanel.SetActive(true);
        gameObject.SetActive(false);
    }

    private void ChangePassword() // 비밀번호 변경 메서드
    {
        Debug.Log("비밀번호 변경.");
        FirebaseUser user = FirebaseManager.Auth.CurrentUser; // 현재 사용자 정보 가져오기
        string newPassword = NewPasswordInput.text;
        if (user != null)
        {
            user.UpdatePasswordAsync(newPassword).ContinueWith(task => {
                if (task.IsCanceled)
                {
                    Debug.LogError("UpdatePasswordAsync가 취소되었습니다..");
                    return;
                }
                if (task.IsFaulted)
                {
                    Debug.LogError("UpdatePasswordAsync에서 오류가 발생했습니다.: " + task.Exception);
                    return;
                }

                Debug.Log("비밀번호가 성공적으로 업데이트되었습니다.");
            });
        }

    }
}
