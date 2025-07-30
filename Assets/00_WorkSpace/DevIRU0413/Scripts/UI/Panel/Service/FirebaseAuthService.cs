using System;
using Firebase.Extensions;

public class FirebaseAuthService : IAuthService
{
    private bool IsReady =>
        FirebaseManager.Instance != null &&
        FirebaseManager.Auth != null;

    public void Login(string email, string password, Action<UserModel> onSuccess, Action<string> onFail)
    {
        if (!IsReady)
        {
            onFail?.Invoke("Firebase 초기화가 완료되지 않았습니다.");
            return;
        }

        FirebaseManager.Auth.SignInWithEmailAndPasswordAsync(email, password)
            .ContinueWithOnMainThread(task =>
            {
                if (task.IsCanceled || task.IsFaulted)
                {
                    onFail?.Invoke(task.Exception?.Message ?? "로그인 실패");
                    return;
                }

                var user = task.Result.User;
                var model = new UserModel(user.Email, user.DisplayName ?? "", user.IsEmailVerified);
                onSuccess?.Invoke(model);
            });
    }

    public void SendPasswordReset(string email, Action onSuccess, Action<string> onFail)
    {
        if (!IsReady)
        {
            onFail?.Invoke("Firebase 초기화가 완료되지 않았습니다.");
            return;
        }

        FirebaseManager.Auth.SendPasswordResetEmailAsync(email)
            .ContinueWithOnMainThread(task =>
            {
                if (task.IsCanceled || task.IsFaulted)
                {
                    onFail?.Invoke(task.Exception?.Message ?? "이메일 전송 실패");
                    return;
                }

                onSuccess?.Invoke();
            });
    }
}
