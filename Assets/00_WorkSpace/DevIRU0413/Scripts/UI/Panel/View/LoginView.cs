using System.Collections.Generic;
using System.Reflection;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LoginView : JHT_BaseUI, ILoginView
{
    private enum LoginUI
    {
        SignUpButton,
        LoginButton,
        ResetPassButton,
        GoogleLoginButton,

        IDInputField,
        PassInputField,
    }

    [SerializeField] private LoginFailPanel loginFailPanel;
    [SerializeField] private EmailPanel emailPanel;
    [SerializeField] private NicknamePanel nicknamePanel;
    [SerializeField] private LobbyPanel lobbyPanel;
    [SerializeField] private SignUpPanel signUpPanel;


    private LoginPresenter _presenter;
    private UIBinder<LoginUI> _binder;


    public void Init(LoginPresenter presenter)
    {
        InitBaseUI();

        _presenter = presenter;
        _binder = new UIBinder<LoginUI>(this);

        _binder.Get<Button>(LoginUI.LoginButton).onClick.AddListener(presenter.OnClickLogin);
        _binder.Get<Button>(LoginUI.SignUpButton).onClick.AddListener(presenter.OnClickSignUp);
        _binder.Get<Button>(LoginUI.ResetPassButton).onClick.AddListener(presenter.OnClickResetPassword);
        _binder.Get<Button>(LoginUI.GoogleLoginButton).onClick.AddListener(presenter.OnClickGoogleLogin);
    }

    public string Email => _binder.Get<TMP_InputField>(LoginUI.IDInputField).text;
    public string Password => _binder.Get<TMP_InputField>(LoginUI.PassInputField).text;

    public void ShowLoginFail(string message)
    {
        Debug.LogError($"로그인 실패: {message}");
        loginFailPanel.gameObject.SetActive(true);
        HideSelf();
    }

    public void ShowVerifyEmailPanel()
    {
        emailPanel.gameObject.SetActive(true);
        HideSelf();
    }

    public void ShowNicknamePanel()
    {
        nicknamePanel.gameObject.SetActive(true);
        HideSelf();
    }

    public void ShowLobby()
    {
        lobbyPanel.gameObject.SetActive(true);
        HideSelf();
    }

    public void ShowSignUpPanel()
    {
        signUpPanel.gameObject.SetActive(true);
        HideSelf();
    }

    public void HideSelf()
    {
        gameObject.SetActive(false);
    }
}
