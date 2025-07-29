using UnityEngine;

public class LoginPresenter
{
    private readonly ILoginView view;
    private readonly IAuthService authService;

    public LoginPresenter(ILoginView view, IAuthService authService)
    {
        this.view = view;
        this.authService = authService;
    }

    public void OnClickLogin()
    {
        authService.Login(view.Email, view.Password,
            onSuccess: HandleLoginSuccess,
            onFail: view.ShowLoginFail);
    }

    private void HandleLoginSuccess(UserModel user)
    {
        if (!user.IsEmailVerified)
            view.ShowVerifyEmailPanel();
        else if (string.IsNullOrEmpty(user.Nickname))
            view.ShowNicknamePanel();
        else
            view.ShowLobby();
    }

    public void OnClickSignUp() => view.ShowSignUpPanel();

    public void OnClickResetPassword()
    {
        authService.SendPasswordReset(view.Email,
            onSuccess: () => Debug.Log("이메일 전송 성공"),
            onFail: view.ShowLoginFail);
    }

    public void OnClickGoogleLogin()
    {
        // 미구현
        Debug.Log("Test");
    }
}
