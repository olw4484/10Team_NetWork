using Firebase.Auth;
using Firebase.Extensions;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SignUpPanel : MonoBehaviour
{
    [SerializeField] GameObject loginPanel;
    //[SerializeField] GameObject iDCheckOkPanel;
    //[SerializeField] GameObject iDCheckFailPanel;

    [SerializeField] TMP_InputField idInput;
    [SerializeField] TMP_InputField nicknameInput;
    [SerializeField] TMP_InputField passInput;
    [SerializeField] TMP_InputField passConfirmInput;

    [SerializeField] Button signUpButton;
    [SerializeField] Button cancelButton;
    [SerializeField] Button IDCheckButton;

    private void Awake()
    {
        signUpButton.onClick.AddListener(SignUp);
        cancelButton.onClick.AddListener(Cancel);
        //IDCheckButton.onClick.AddListener();
    }

    private void SignUp()
    {
        if (passInput.text != passConfirmInput.text)
        {
            Debug.LogError("�н����尡 ��ġ���� �ʽ��ϴ�");
            return;
        }
        if (nicknameInput.text == "")
        {
            Debug.LogError("�г����� �������ּ���");
            return;
        }

        FirebaseManager.Auth.CreateUserWithEmailAndPasswordAsync(idInput.text, passInput.text)
            .ContinueWithOnMainThread(task =>
            {

                if (nicknameInput.text != "")
                {
                    UserProfile profile = new UserProfile();
                    profile.DisplayName = nicknameInput.text;

                    FirebaseUser user = FirebaseManager.Auth.CurrentUser;
                    user.UpdateUserProfileAsync(profile)
                        .ContinueWithOnMainThread(task =>
                        {
                            if (task.IsCanceled)
                            {
                                Debug.LogError("���� �г��� ���� ��ҵ�");
                                return;
                            }
                            if (task.IsFaulted)
                            {
                                Debug.LogError($"���� �г��� ���� ����. ���� : {task.Exception}");
                                return;
                            }

                            Debug.Log("���� �г��� ���� ����");

                        });
                }

                if (task.IsCanceled)
                {
                    Debug.LogError("�̸��� ������ ��ҵ�"); 
                    return;
                }
                if (task.IsFaulted)
                {
                    Debug.LogError($"�̸��� ���� ������. ���� : {task.Exception}");
                    return;
                }
          
                Debug.Log("�̸��� ���� ����!");
                loginPanel.SetActive(true);
                gameObject.SetActive(false);
            });

    }

    private void Cancel()
    {
        loginPanel.SetActive(true);
        gameObject.SetActive(false);
    }

    


}
