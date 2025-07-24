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

    private void Awake()    // �ʱ�ȭ �޼���
    {
        logoutButton.onClick.AddListener(Logout);
        editProfileButton.onClick.AddListener(EditProfile);
        deleteUserButton.onClick.AddListener(DeleteUser);
    }

    private void OnEnable() // �г��� ȭ�鿡 ��Ÿ�� ������ ȣ���
    {
        FirebaseUser user = FirebaseManager.Auth.CurrentUser;   // ���� �α����� ����� ���� ��������

        emailText.text = user.Email;    
        nameText.text = user.DisplayName;
        userIdText.text = user.UserId;
    }

    private void Logout()   // �α׾ƿ� �޼���
    {
        FirebaseManager.Auth.SignOut(); // Firebase���� �α׾ƿ�
        loginPannel.SetActive(true);
        gameObject.SetActive(false);
    }

    private void EditProfile()  // ������ ���� �޼���
    {
        editPanel.SetActive(true);
        gameObject.SetActive(false);
    }

    private void DeleteUser()   // ȸ�� Ż�� �޼���
    {
        cancelMembershipPanel.SetActive(true);
        gameObject.SetActive(false);
    }
}
