//TODO: REIMPLEMENT

 // using System;
 // using System.Diagnostics;
 // using System.IO;
 // using System.Threading.Tasks;
 // using Newtonsoft.Json;
 // using OsuPlayerPlus.Classes.API.ApiEndpoints;
 // using OsuPlayerPlus.Classes.Extensions;
 // using OsuPlayerPlus.Properties;
 //
 // namespace OsuPlayerPlus.Classes.Online;
 //
 // public class Profile
 // {
 //     public User User { get; set; }
 //     public string ProfilePicture { get; set; }
 //
 //     public void CreateNewProfile()
 //     {
 //         User = new User();
 //     }
 //
 //     public async Task<User> FetchProfileAsync()
 //     {
 //         if (string.IsNullOrWhiteSpace(OsuPlayer.Profile?.User?.Name))
 //             return null;
 //
 //         var response = await ApiAsync.GetProfileByNameAsync(OsuPlayer.Profile.User.Name);
 //
 //         if (response == null) return null;
 //
 //         User = response;
 //
 //         return User;
 //     }
 //
 //     private void CheckIfAppdataFolderExists()
 //     {
 //         if (!Directory.Exists($"{OsuPlayer.Constants.AppdataLocation}/Founntain/osu!player plus"))
 //             Directory.CreateDirectory($"{OsuPlayer.Constants.AppdataLocation}/Founntain/osu!player plus");
 //     }
 //
 //     public void SaveProfile()
 //     {
 //         CheckIfAppdataFolderExists();
 //
 //         File.WriteAllText($"{OsuPlayer.Constants.AppdataLocation}/Founntain/osu!player plus/user.json",
 //             JsonConvert.SerializeObject(User));
 //     }
 //
 //     public async Task LoadProfile()
 //     {
 //         CheckIfAppdataFolderExists();
 //
 //         if (!File.Exists($"{OsuPlayer.Constants.AppdataLocation}/Founntain/osu!player plus/user.json"))
 //         {
 //             File.WriteAllText($"{OsuPlayer.Constants.AppdataLocation}/Founntain/osu!player plus/user.json",
 //                 JsonConvert.SerializeObject(
 //                     Settings.Default.profileId != string.Empty
 //                         ? new User {Id = Guid.Parse(Settings.Default.profileId)}
 //                         : new User()
 //                 ));
 //
 //             Settings.Default.profileId = Settings.Default.profileId != string.Empty
 //                 ? string.Empty
 //                 : Settings.Default.profileId;
 //         }
 //
 //         User localUser;
 //
 //         try
 //         {
 //             localUser = JsonConvert.DeserializeObject<User>(
 //                 File.ReadAllText($"{OsuPlayer.Constants.AppdataLocation}/Founntain/osu!player plus/user.json")
 //             );
 //         }
 //         catch (JsonSerializationException)
 //         {
 //             File.Delete($"{OsuPlayer.Constants.AppdataLocation}/Founntain/osu!player plus/user.json");
 //
 //             User = null;
 //             return;
 //         }
 //
 //         Debug.WriteLine("Trying to load profile");
 //
 //         try
 //         {
 //             User = localUser.Id != Guid.Empty
 //                 ? await ApiAsync.GetProfileAsync(localUser.Id.ToString())
 //                 : null;
 //
 //             if (User != null)
 //                 File.WriteAllText($"{OsuPlayer.Constants.AppdataLocation}/Founntain/osu!player plus/user.json",
 //                     JsonConvert.SerializeObject(User));
 //         }
 //         catch (Exception)
 //         {
 //             User = null;
 //         }
 //
 //         Debug.WriteLine(User == null
 //             ? "No profile found\n"
 //             : $"Profile {User.Name} loaded with GUID {User.Id}\n");
 //
 //         if (User == null)
 //         {
 //             OsuPlayerMessageBox.Show(
 //                 "If you never had an osu!player profile, ignore this message. \n\nosu!player could not load your profile. Did your profile got deleted? Please check if your profile still shows up in the user list. If so please contact Founntain to recover your profile. DO NOT create a new profile, if your old one still exists. Recover it first!. You find the contact informations down below in the settings view\n\nIf your profile got deleted and no recovery is possible, please create a new profile.",
 //                 "Profile not found");
 //             return;
 //         }
 //
 //         var profilePic = await ApiAsync.GetProfilePictureAsync(User.Name);
 //
 //         if (string.IsNullOrWhiteSpace(profilePic)) return;
 //
 //         ProfilePicture = profilePic.Substring(1, profilePic.Length - 2);
 //     }
 // }