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

    private void Awake()    // �ʱ�ȭ �޼���
    {
        logoutButton.onClick.AddListener(Logout);
        editProfileButton.onClick.AddListener(EditProfile);
        deleteUserButton.onClick.AddListener(DeleteUser);
        ChangePasswordButton.onClick.AddListener(ChangePassword);
    }

    private void OnEnable() // �г��� ȭ�鿡 ��Ÿ�� ������ ȣ���
    {
        FirebaseUser user = FirebaseManager.Auth.CurrentUser;   // ���� �α����� ����� ���� ��������

        emailText.text = user.Email;    
        nameText.text = user.DisplayName;

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
        DeleteUserPanel.SetActive(true);
        gameObject.SetActive(false);
    }

    private void ChangePassword() // ��й�ȣ ���� �޼���
    {
        Debug.Log("��й�ȣ ����.");
        FirebaseUser user = FirebaseManager.Auth.CurrentUser; // ���� ����� ���� ��������
        string newPassword = NewPasswordInput.text;
        if (user != null)
        {
            user.UpdatePasswordAsync(newPassword).ContinueWith(task => {
                if (task.IsCanceled)
                {
                    Debug.LogError("UpdatePasswordAsync�� ��ҵǾ����ϴ�..");
                    return;
                }
                if (task.IsFaulted)
                {
                    Debug.LogError("UpdatePasswordAsync���� ������ �߻��߽��ϴ�.: " + task.Exception);
                    return;
                }

                Debug.Log("��й�ȣ�� ���������� ������Ʈ�Ǿ����ϴ�.");
            });
        }

    }
}
