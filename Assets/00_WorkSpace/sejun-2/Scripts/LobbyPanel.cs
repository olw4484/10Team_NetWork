using Firebase.Auth;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LobbyPanel : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] GameObject loginPannel;
    [SerializeField] GameObject editPanel;
    [SerializeField] GameObject deleteUserPanel;
    [Header("Texts")]
    [SerializeField] TMP_Text emailText;
    [SerializeField] TMP_Text nameText;
    [SerializeField] TMP_Text passwordText;
    [SerializeField] TMP_Text userIdText;
    [Header("Buttons")]
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
        passwordText.text = "��й�ȣ�� ���Ȼ� ǥ�õ��� �ʽ��ϴ�"; // ��й�ȣ�� ���Ȼ� ǥ������ ����
        userIdText.text = user.UserId; // ����� ID ǥ��

        //�����̳� �÷��� ����� �� �ٸ� ���ι��̴��� �α����� ���,
        //foreach (IUserInfo userInfo in user.ProviderData) // ����� �������� ���ι��̴� ������ ��������
        //{
        //    if (userInfo.ProviderId == "password") // ���ι��̴� ID�� "password"�� ���
        //    {
        //        emailText.text = userInfo.Email; // �̸��� �ؽ�Ʈ ������Ʈ
        //        break; // ���� ����
        //    }
        //}

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
        deleteUserPanel.SetActive(true);
        gameObject.SetActive(false);
    }

}
