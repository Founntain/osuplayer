namespace OsuPlayer.Data.API.Models.User;

public sealed class EditUserModel
{
    public UserModel UserModel { get; set; }
    public string Password { get; set; }
    public string NewPassword { get; set; }
}