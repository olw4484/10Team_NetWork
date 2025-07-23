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
    //[SerializeField] TMP_InputField nicknameInput;
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
            Debug.LogError("패스워드가 일치하지 않습니다");
            return;
        }

        FirebaseManager.Auth.CreateUserWithEmailAndPasswordAsync(idInput.text, passInput.text)
            .ContinueWithOnMainThread(task =>
            {
                if (task.IsCanceled)
                {
                    Debug.LogError("이메일 가입이 취소됨"); 
                    return;
                }
                if (task.IsFaulted)
                {
                    Debug.LogError($"이메일 가입 실패함. 이유 : {task.Exception}");
                    return;
                }

                Debug.Log("이메일 가입 성공!");
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
