namespace OsuPlayer.Data.API.Models.User
{
    public class CreateUserModel
    {
        public UserModel UserModel { get; set; }
        public string Password { get; set; }
    }
}