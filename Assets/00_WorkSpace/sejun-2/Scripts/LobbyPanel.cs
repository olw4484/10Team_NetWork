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
    [SerializeField] GameObject cancelMembershipPanel;

    [SerializeField] TMP_Text emailText;
    [SerializeField] TMP_Text nameText;
    [SerializeField] TMP_Text userIdText;

    [SerializeField] Button logoutButton;
    [SerializeField] Button editProfileButton;
    [SerializeField] Button deleteUserButton;

    private void Awake()    // 초기화 메서드
    {
        logoutButton.onClick.AddListener(Logout);
        editProfileButton.onClick.AddListener(EditProfile);
        deleteUserButton.onClick.AddListener(DeleteUser);
    }

    private void OnEnable() // 패널이 화면에 나타날 때마다 호출됨
    {
        FirebaseUser user = FirebaseManager.Auth.CurrentUser;   // 현재 로그인한 사용자 정보 가져오기

        emailText.text = user.Email;    
        nameText.text = user.DisplayName;
        userIdText.text = user.UserId;
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
        cancelMembershipPanel.SetActive(true);
        gameObject.SetActive(false);
    }
}
