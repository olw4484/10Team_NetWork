using Firebase.Auth;
using Firebase.Extensions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EmailPanel : MonoBehaviour
{
    [SerializeField] GameObject loginPanel;
    //[SerializeField] GameObject nicknamePanel;
    [SerializeField] GameObject lobbyPanel;

    [SerializeField] Button backButton;

    private void Awake()
    {
        backButton.onClick.AddListener(Back);
    }

    private void OnEnable()
    {
        FirebaseManager.Auth.CurrentUser.SendEmailVerificationAsync()
            .ContinueWithOnMainThread(task =>
            {
                if (task.IsCanceled)
                {
                    Debug.LogError("인증 이메일 전송이 취소됨");
                    return;
                }
                if (task.IsFaulted)
                {
                    Debug.LogError($"인증 이메일 전송이 실패. 이유 : {task.Exception}");
                    return;
                }

                Debug.Log("인증 이메일 전송 성공");

                emailVerificationRoutine = StartCoroutine(EmailVerificationRoutine());  // 이메일 인증 확인을 위한 코루틴 시작
            });
    }

    private void Back()
    {
        FirebaseManager.Auth.SignOut();
        loginPanel.SetActive(true);
        gameObject.SetActive(false);
    }

    Coroutine emailVerificationRoutine; // 이메일 인증 확인을 위한 코루틴
    IEnumerator EmailVerificationRoutine()
    {
        FirebaseUser user = FirebaseManager.Auth.CurrentUser;   // 현재 사용자 정보 가져오기
        WaitForSeconds delay = new WaitForSeconds(2f);  // 2초마다 이메일 인증 확인

        while (true)    // 무한 루프를 통해 이메일 인증 상태를 주기적으로 확인
        {
            yield return delay; // 2초 대기

            user.ReloadAsync(); // 사용자 정보를 새로고침하여 최신 상태를 반영
            if (user.IsEmailVerified)   // 이메일 인증이 완료되었는지 확인
            {
                Debug.Log("인증 완료");
                lobbyPanel.SetActive(true);
                gameObject.SetActive(false);
                StopCoroutine(emailVerificationRoutine);    // 코루틴 중지
            }
            else
            {
                Debug.Log("인증 대기중...");
            }
        }
    }
}
