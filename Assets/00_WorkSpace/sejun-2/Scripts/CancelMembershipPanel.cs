using Firebase.Auth;
using Firebase.Extensions;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CancelMembershipPanel : MonoBehaviour
{
    [SerializeField] GameObject lobbyPanel;
    [SerializeField] TMP_Text idText;
    [SerializeField] Button backButton;
    [SerializeField] Button DeleteUserButton;

    private void Awake()
    {
        backButton.onClick.AddListener(Back);
        DeleteUserButton.onClick.AddListener(CancelMembership);
    }

    private void Back()
    {
        gameObject.SetActive(false);
        lobbyPanel.SetActive(true);
    }

    private void CancelMembership()
    {
        FirebaseUser user = FirebaseManager.Auth.CurrentUser;
        user.DeleteAsync()
            .ContinueWithOnMainThread(task =>
            {
                if (task.IsCanceled)
                {
                    Debug.LogError("유저 삭제 취소됨");
                    return;
                }
                if (task.IsFaulted)
                {
                    Debug.LogError($"유저 삭제 실패. 이유 : {task.Exception}");
                    return;
                }

                Debug.Log("유저 삭제 성공");
                FirebaseManager.Auth.SignOut();
                // 회원 탈퇴 후 로비로 돌아가기
                gameObject.SetActive(false);
                lobbyPanel.SetActive(true);
        });
    }



}
