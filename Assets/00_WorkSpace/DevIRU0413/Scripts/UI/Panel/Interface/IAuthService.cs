using System;

public interface IAuthService
{
    void Login(string email, string password, Action<UserModel> onSuccess, Action<string> onFail);
    void SendPasswordReset(string email, Action onSuccess, Action<string> onFail);
}
