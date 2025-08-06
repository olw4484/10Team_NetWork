using Firebase.Auth;
using Firebase.Extensions;
using Firebase.Database;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SignUpPanel : MonoBehaviour
{
    [SerializeField] GameObject loginPanel;
    [SerializeField] GameObject iDCheckPanel;

    [SerializeField] TMP_InputField idInput;
    [SerializeField] TMP_InputField nicknameInput;
    [SerializeField] TMP_InputField passInput;
    [SerializeField] TMP_InputField passConfirmInput;

    [SerializeField] Button signUpButton;
    [SerializeField] Button cancelButton;
    [SerializeField] Button IDCheckButton;

    // Firebase에서는 딕셔너리 지원.
    [SerializeField] Dictionary<string, object> dictionary = new Dictionary<string, object>();

    private void Awake()
    {
        signUpButton.onClick.AddListener(SignUp);
        cancelButton.onClick.AddListener(Cancel);
        IDCheckButton.onClick.AddListener(IdCheck);
    }

    private void SignUp()
    {
        if (passInput.text != passConfirmInput.text)
        {
            Debug.LogError("패스워드가 일치하지 않습니다");
            return;
        }
        if (nicknameInput.text == "")
        {
            Debug.LogError("닉네임을 설정해주세요");
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
                                Debug.LogError("유저 닉네임 설정 취소됨");
                                return;
                            }
                            if (task.IsFaulted)
                            {
                                Debug.LogError($"유저 닉네임 설정 실패. 이유 : {task.Exception}");
                                return;
                            }
                            Debug.Log("유저 닉네임 설정 성공");
                        });
                }

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

                SetData(); // Firebase에 사용자 데이터 저장

                loginPanel.SetActive(true);
                gameObject.SetActive(false);
            });
    }

    private void IdCheck()
    {
        string email = idInput.text.Trim();

        if (string.IsNullOrEmpty(email))
        {
            Debug.LogError("이메일이 입력되지 않았습니다.");
            return;
        }

        FirebaseManager.Auth.FetchProvidersForEmailAsync(email).ContinueWithOnMainThread(task =>
        {
            if (task.IsCanceled)
            {
                Debug.LogError("이메일 ID 체크가 취소됨");
                return;
            }
            if (task.IsFaulted)
            {
                Debug.LogError($"이메일 ID 체크 실패함. 이유 : {task.Exception}");
                return;
            }

            // IEnumerable<string> 으로 받은 후 List로 변환
            List<string> providers = new List<string>(task.Result);

            // [디버그] providers 로그 찍기
            Debug.Log($"이메일 {email}의 providers.Count: {providers.Count}");
            foreach (var provider in providers)
            {
                Debug.Log($"Provider: {provider}");
            }

            if (providers.Count > 0)
            {
                Debug.LogError("이미 사용 중인 이메일입니다.");
                iDCheckPanel.SetActive(true);
                iDCheckPanel.GetComponentInChildren<TMP_Text>().text = "이미 사용 중인 이메일입니다.";
                gameObject.SetActive(false);
                return;
            }
            else
            {
                Debug.Log("사용 가능한 이메일입니다.");
                iDCheckPanel.SetActive(true);
                iDCheckPanel.GetComponentInChildren<TMP_Text>().text = "사용 가능한 이메일입니다.";
                gameObject.SetActive(false);
                return;
            }
        });
    }

    private void Cancel()
    {
        loginPanel.SetActive(true);
        gameObject.SetActive(false);
    }

    private void SetData() // dictionary 사용하여 Firebase에 데이터를 저장하는 함수
    {
        // Firebase 데이터베이스의 루트 참조를 가져옵니다.
        FirebaseUser user = FirebaseAuth.DefaultInstance.CurrentUser;
        // Firebase 데이터베이스의 루트 참조를 가져옵니다.
        DatabaseReference root = FirebaseDatabase.DefaultInstance.RootReference;
        DatabaseReference userInfo = root.Child("UserData").Child(user.UserId);
        // 딕셔너리에 데이터를 추가합니다.
        dictionary["name"] = nicknameInput.text;
        dictionary["level"] = 1;
        dictionary["gameCount"] = 0;
        dictionary["winsCount"] = 0;
        userInfo.SetValueAsync(dictionary);// Firebase 데이터베이스에 text 변수를 저장합니다.
    }


}
