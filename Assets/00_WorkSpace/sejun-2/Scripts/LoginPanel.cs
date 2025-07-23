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
					Debug.LogError("로그인이 취소됨");
					return;
				}
				if (task.IsFaulted)
				{
                    // 로그인 실패시 로그인 실패 패널 활성화
					LoginFailPanel.SetActive(true);
					gameObject.SetActive(false);
                    Debug.LogError($"로그인에 실패함. 이유 : {task.Exception}");
					return;
				}

				Debug.Log("로그인 성공");
				AuthResult result = task.Result;
                FirebaseUser user = result.User;

                Debug.Log($"유저의 이메일 : {user.Email}");
                Debug.Log($"유저의 닉네임 : {user.DisplayName}");


                //1.이미 이메일 인증을 마친 유저인 경우는 로비로
                if (user.IsEmailVerified == true)
                {
                    //// 1-1. 아직 닉네임 설정을 하지 않은 경우
                    //if (user.DisplayName == "")
                    //{
                    //    nicknamePanel.SetActive(true);
                    //    gameObject.SetActive(false);
                    //}
                    //// 1-2. 닉네임 설정도 완료한 경우
                    //else
                    //{
                    //    lobbyPanel.SetActive(true);
                    //    gameObject.SetActive(false);
                    //}
                    lobbyPanel.SetActive(true);
                    gameObject.SetActive(false);
                }
                //2.이메일 인증이 아직 되지 않은 유저는 이메일 인증을 대기
                else
                {
                    emailPanel.SetActive(true);
                    gameObject.SetActive(false);
                }
            });
    }

	private void ResetPass()    // 패스워드 재설정 이메일 전송
    {
		FirebaseManager.Auth.SendPasswordResetEmailAsync(idInput.text)
			.ContinueWithOnMainThread(task =>
			{
				if (task.IsCanceled)
				{
					Debug.LogError("패스워드 재설정 이메일 전송 취소됨");
					return;
				}
				if (task.IsFaulted)
				{
					Debug.LogError($"패스워드 재설정 이메일 전송 실패. 이유 : {task.Exception}");
					return;
				}

				Debug.Log("패스워드 재설정 이메일 전송 성공");
			});
	}
}
