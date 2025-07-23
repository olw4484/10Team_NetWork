using Firebase.Auth;
using Firebase.Extensions;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LoginPanel : MonoBehaviour
{
    [SerializeField] GameObject signUpPanel;
    [SerializeField] GameObject lobbyPanel;
	[SerializeField] GameObject emailPanel;
	//[SerializeField] GameObject nicknamePanel;
	[SerializeField] GameObject LoginFailPanel;

    [SerializeField] TMP_InputField idInput;
    [SerializeField] TMP_InputField passInput;

    [SerializeField] Button signUpButton;
    [SerializeField] Button loginButton;
	[SerializeField] Button resetPassButton;

    private void Awake()
    {
        signUpButton.onClick.AddListener(SignUp);
        loginButton.onClick.AddListener(Login);
		resetPassButton.onClick.AddListener(ResetPass);
    }

    private void SignUp()
    {
        signUpPanel.SetActive(true);
        gameObject.SetActive(false);
    }

    private void Login()
    {
		FirebaseManager.Auth.SignInWithEmailAndPasswordAsync(idInput.text, passInput.text)
			.ContinueWithOnMainThread(task =>
			{
				if (task.IsCanceled)
				{
					Debug.LogError("�α����� ��ҵ�");
					return;
				}
				if (task.IsFaulted)
				{
                    // �α��� ���н� �α��� ���� �г� Ȱ��ȭ
					LoginFailPanel.SetActive(true);
					gameObject.SetActive(false);
                    Debug.LogError($"�α��ο� ������. ���� : {task.Exception}");
					return;
				}

				Debug.Log("�α��� ����");
				AuthResult result = task.Result;
                FirebaseUser user = result.User;

                Debug.Log($"������ �̸��� : {user.Email}");
                Debug.Log($"������ �г��� : {user.DisplayName}");


                //1.�̹� �̸��� ������ ��ģ ������ ���� �κ��
                if (user.IsEmailVerified == true)
                {
                    //// 1-1. ���� �г��� ������ ���� ���� ���
                    //if (user.DisplayName == "")
                    //{
                    //    nicknamePanel.SetActive(true);
                    //    gameObject.SetActive(false);
                    //}
                    //// 1-2. �г��� ������ �Ϸ��� ���
                    //else
                    //{
                    //    lobbyPanel.SetActive(true);
                    //    gameObject.SetActive(false);
                    //}
                    lobbyPanel.SetActive(true);
                    gameObject.SetActive(false);
                }
                //2.�̸��� ������ ���� ���� ���� ������ �̸��� ������ ���
                else
                {
                    emailPanel.SetActive(true);
                    gameObject.SetActive(false);
                }
            });
    }

	private void ResetPass()    // �н����� �缳�� �̸��� ����
    {
		FirebaseManager.Auth.SendPasswordResetEmailAsync(idInput.text)
			.ContinueWithOnMainThread(task =>
			{
				if (task.IsCanceled)
				{
					Debug.LogError("�н����� �缳�� �̸��� ���� ��ҵ�");
					return;
				}
				if (task.IsFaulted)
				{
					Debug.LogError($"�н����� �缳�� �̸��� ���� ����. ���� : {task.Exception}");
					return;
				}

				Debug.Log("�н����� �缳�� �̸��� ���� ����");
			});
	}
}
