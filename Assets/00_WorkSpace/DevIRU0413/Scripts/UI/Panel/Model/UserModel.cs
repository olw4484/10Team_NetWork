public class UserModel
{
    public string Email { get; }
    public string Nickname { get; }
    public bool IsEmailVerified { get; }

    public UserModel(string email, string nickname, bool isEmailVerified)
    {
        Email = email;
        Nickname = nickname;
        IsEmailVerified = isEmailVerified;
    }
}
