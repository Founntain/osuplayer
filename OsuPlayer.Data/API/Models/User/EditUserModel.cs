using OsuPlayer.Api.Data.API.EntityModels;

public sealed class EditUserModel
{
    public UserModel UserModel { get; set; }
    public string Password { get; set; }
    public string? NewPassword { get; set; }
}