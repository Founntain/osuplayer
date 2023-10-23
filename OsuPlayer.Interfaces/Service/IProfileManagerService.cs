using OsuPlayer.Data.DataModels;

namespace OsuPlayer.Interfaces.Service;

public interface IProfileManagerService
{
    public User? User { get; set; }

    public Task Login(string username, string password);
    public Task Login(string token);
}