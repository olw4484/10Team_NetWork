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
    [SerializeField] GameObject iDCheckPanel;

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
        IDCheckButton.onClick.AddListener(IdCheck);
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

    private void IdCheck()
    {
        string email = idInput.text.Trim();

        if (string.IsNullOrEmpty(email))
        {
            Debug.LogError("�̸����� �Էµ��� �ʾҽ��ϴ�.");
            return;
        }

        FirebaseManager.Auth.FetchProvidersForEmailAsync(email).ContinueWithOnMainThread(task =>
        {
            if (task.IsCanceled)
            {
                Debug.LogError("�̸��� ID üũ�� ��ҵ�");
                return;
            }
            if (task.IsFaulted)
            {
                Debug.LogError($"�̸��� ID üũ ������. ���� : {task.Exception}");
                return;
            }

            // IEnumerable<string> ���� ���� �� List�� ��ȯ
            List<string> providers = new List<string>(task.Result);

            // [�����] providers �α� ���
            Debug.Log($"�̸��� {email}�� providers.Count: {providers.Count}");
            foreach (var provider in providers)
            {
                Debug.Log($"Provider: {provider}");
            }

            if (providers.Count > 0)
            {
                Debug.LogError("�̹� ��� ���� �̸����Դϴ�.");
                iDCheckPanel.SetActive(true);
                iDCheckPanel.GetComponentInChildren<TMP_Text>().text = "�̹� ��� ���� �̸����Դϴ�.";
                gameObject.SetActive(false);
                return;
            }
            else
            {
                Debug.Log("��� ������ �̸����Դϴ�.");
                iDCheckPanel.SetActive(true);
                iDCheckPanel.GetComponentInChildren<TMP_Text>().text = "��� ������ �̸����Դϴ�.";
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

    


}
