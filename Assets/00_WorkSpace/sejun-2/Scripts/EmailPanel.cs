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
                    Debug.LogError("���� �̸��� ������ ��ҵ�");
                    return;
                }
                if (task.IsFaulted)
                {
                    Debug.LogError($"���� �̸��� ������ ����. ���� : {task.Exception}");
                    return;
                }

                Debug.Log("���� �̸��� ���� ����");

                emailVerificationRoutine = StartCoroutine(EmailVerificationRoutine());  // �̸��� ���� Ȯ���� ���� �ڷ�ƾ ����
            });
    }

    private void Back()
    {
        FirebaseManager.Auth.SignOut();
        loginPanel.SetActive(true);
        gameObject.SetActive(false);
    }

    Coroutine emailVerificationRoutine; // �̸��� ���� Ȯ���� ���� �ڷ�ƾ
    IEnumerator EmailVerificationRoutine()
    {
        FirebaseUser user = FirebaseManager.Auth.CurrentUser;   // ���� ����� ���� ��������
        WaitForSeconds delay = new WaitForSeconds(2f);  // 2�ʸ��� �̸��� ���� Ȯ��

        while (true)    // ���� ������ ���� �̸��� ���� ���¸� �ֱ������� Ȯ��
        {
            yield return delay; // 2�� ���

            user.ReloadAsync(); // ����� ������ ���ΰ�ħ�Ͽ� �ֽ� ���¸� �ݿ�
            if (user.IsEmailVerified)   // �̸��� ������ �Ϸ�Ǿ����� Ȯ��
            {
                Debug.Log("���� �Ϸ�");
                lobbyPanel.SetActive(true);
                gameObject.SetActive(false);
                StopCoroutine(emailVerificationRoutine);    // �ڷ�ƾ ����
            }
            else
            {
                Debug.Log("���� �����...");
            }
        }
    }
}
