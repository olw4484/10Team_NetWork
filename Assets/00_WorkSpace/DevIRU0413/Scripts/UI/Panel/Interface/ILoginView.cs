public interface ILoginView
{
    string Email { get; }
    string Password { get; }

    void ShowLoginFail(string message);
    void ShowVerifyEmailPanel();
    void ShowNicknamePanel();
    void ShowLobby();
    void HideSelf();
    void ShowSignUpPanel();
}
