using System.Collections;
using Avalonia.Media.Imaging;
using OsuPlayer.Api.Data.API.EntityModels;
using OsuPlayer.Api.Data.API.RequestModels.User;

namespace OsuPlayer.Interfaces.Service.Endpoint;

public interface IOsuPlayerApiUserEndpoint
{
    Task<UserModel?> UpdateSongsPlayed(int amount, int beatmapSetId = -1);
    Task<UserModel?> UpdateXp(UpdateXpModel updateXpModel);
    Task<bool> SetOnlineStatus(UserOnlineStatusModel data);
    Task<Bitmap?> GetProfilePictureAsync(Guid currentUserUniqueId);
    Task<UserModel?> GetUserFromLoginToken();
    Task<UserModel?> EditUser(EditUserModel editUserModel);
    Task<bool> SaveProfilePicture(byte[] data);
    Task<Bitmap?> GetProfileBannerAsync(string? bannerUrl);
    Task<bool> DeleteUser();
    Task<List<UserModel>> GetAllUsers();
    Task<List<UserActivityModel>> GetActivityOfUser(Guid selectedUserUniqueId);
    Task<UserModel> Register(AddUserModel addUserModel);
}