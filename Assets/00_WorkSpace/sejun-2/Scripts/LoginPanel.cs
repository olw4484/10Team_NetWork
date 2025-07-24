using Firebase.Auth;
using Firebase.Extensions;
using Google;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LoginPanel : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] GameObject signUpPanel;
    [SerializeField] GameObject lobbyPanel;
	[SerializeField] GameObject emailPanel;
	[SerializeField] GameObject nicknamePanel;
	[SerializeField] GameObject LoginFailPanel;
    [Header("Input Fields")]
    [SerializeField] TMP_InputField idInput;
    [SerializeField] TMP_InputField passInput;
    [Header("Buttons")]
    [SerializeField] Button signUpButton;
    [SerializeField] Button loginButton;
	[SerializeField] Button resetPassButton;
    [SerializeField] Button googleLoginButton;

    private void Awake()
    {
        signUpButton.onClick.AddListener(SignUp);
        loginButton.onClick.AddListener(Login);
		resetPassButton.onClick.AddListener(ResetPass);
        googleLoginButton.onClick.AddListener(GoogleLogin);
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
                //AuthResult result = task.Result;
                //FirebaseUser user = result.User;
                //Debug.Log($"������ �̸��� : {user.Email}");
                //Debug.Log($"������ �г��� : {user.DisplayName}");

                //1.�̹� �̸��� ������ ��ģ ������ ���� �κ��
                FirebaseUser user = task.Result.User;
                if (user.IsEmailVerified == true)
                {
                    // 1-1. ���� �г��� ������ ���� ���� ���
                    if (user.DisplayName == "")
                    {
                        nicknamePanel.SetActive(true);
                        gameObject.SetActive(false);
                    }
                    // 1-2. �г��� ������ �Ϸ��� ���
                    else
                    {
                        lobbyPanel.SetActive(true);
                        gameObject.SetActive(false);
                    }
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

    // Google �α��� ��ư Ŭ�� �� ȣ��Ǵ� �Լ�
    private void GoogleLogin()
    {
        //Firebase.Auth.Credential credential =
        //    Firebase.Auth.GoogleAuthProvider.GetCredential(googleIdToken, googleAccessToken);

        //auth.CurrentUser.LinkWithCredentialAsync(credential).ContinueWith(task => {
        //    if (task.IsCanceled)
        //    {
        //        Debug.LogError("LinkWithCredentialAsync was canceled.");
        //        return;
        //    }
        //    if (task.IsFaulted)
        //    {
        //        Debug.LogError("LinkWithCredentialAsync encountered an error: " + task.Exception);
        //        return;
        //    }

        //    Firebase.Auth.AuthResult result = task.Result;
        //    Debug.LogFormat("Credentials successfully linked to Firebase user: {0} ({1})",
        //        result.User.DisplayName, result.User.UserId);
        //});
    }
}
