//TODO: REIMPLEMENT PARTY MANAGER

// using System;
// using System.Diagnostics;
// using System.Linq;
// using System.Threading.Tasks;
// using System.Windows.Controls;
// using System.Windows.Threading;
// using OsuPlayer.Data.API.Models.Party;
// using OsuPlayer.Data.API.Models.User;
// using OsuPlayer.Data.OsuPlayer.Enums;
// using OsuPlayerPlus.Classes.API.ApiEndpoints;
// using OsuPlayerPlus.Classes.Audio;
// using OsuPlayerPlus.Classes.Extensions;
// using OsuPlayerPlus.Views;
// using Image = System.Drawing.Image;
//
// namespace OsuPlayerPlus.Classes.Online;
//
// public sealed class PartyManager
// {
//     public Image[] PartyAvatars;
//     public TextBlock[] PartyNames;
//
//     public PartyManager()
//     {
//         HostUpdate = new DispatcherTimer {Interval = TimeSpan.FromSeconds(1)};
//         HostUpdate.Tick += PartyUpdate_Tick;
//     }
//
//     public DashboardRefined Dashboard { private get; set; }
//     public Player Player { get; set; }
//
//     private DispatcherTimer HostUpdate { get; }
//
//     private bool IsHost { get; set; }
//     public bool IsInParty { get; set; }
//
//     private string Username { get; set; }
//     private string PartyClientId { get; set; }
//     public string PartyId { get; set; }
//     private string HostPartyId { get; set; }
//
//     public PartyModel Party { get; set; }
//
//     public async Task Create()
//     {
//         if (Player == null) return;
//
//         if (Player.Songs.Count == 0) return;
//
//         if (IsInParty || IsHost)
//         {
//             OsuPlayerMessageBox.Show(
//                 OsuPlayer.LanguageService.LoadControlLanguageWithKey("message.currentlyInParty"));
//             return;
//         }
//
//         if (OsuPlayer.Profile?.User == null)
//         {
//             OsuPlayerMessageBox.Show("You need a osu!player plus profile to create a party");
//             return;
//         }
//
//         var partyToCreate = new
//         {
//             Song = Player.CurrentSong.Checksum,
//             Songname = Player.CurrentSong.SongName,
//             Hostname = OsuPlayer.Profile?.User?.Name ?? Environment.UserName
//         };
//
//         var party = await ApiAsync.ApiRequestAsync<PartyModel>("createParty", "parties", partyToCreate);
//         //var party = Api.ApiRequest<RaveParty>("createDummyParty", "osuplayerparty", JsonConvert.SerializeObject(partyToCreate));
//
//         if (party == null)
//         {
//             OsuPlayerMessageBox.Show(
//                 OsuPlayer.LanguageService.LoadControlLanguageWithKey("message.partyCreatedApiError"));
//             return;
//         }
//
//         Debug.WriteLine($"Party created with ID {party.PartyId}");
//
//         party.Hostname = OsuPlayer.Profile.User?.Name ?? Username;
//
//         PartyId = party.PartyId.ToString();
//         HostPartyId = PartyId;
//         IsHost = true;
//         IsInParty = true;
//         Party = party;
//
//         //TODO: Check For Discord Fix
//         // if (OsuPlayer.Config.ConfigStorage.UseDiscord)
//         //     OsuPlayer.DiscordManager.UpdateActivityWithParty();
//
//         await SendHostData();
//
//         HostUpdate.Start();
//     }
//
//     public async Task Join(string id)
//     {
//         if (OsuPlayer.Profile.User == null)
//         {
//             OsuPlayerMessageBox.Show(
//                 OsuPlayer.LanguageService.LoadControlLanguageWithKey("message.partyNoValidProfile"));
//             return;
//         }
//
//         if (IsInParty)
//         {
//             Debug.WriteLine("Currently inside a party. Leaving it...");
//             await Leave();
//         }
//
//         HostPartyId = id;
//
//         Debug.WriteLine(@"Joining party: {0}", HostPartyId);
//
//         var join = new Join
//         {
//             JoinId = Guid.Parse(HostPartyId),
//             Username = OsuPlayer.Profile.User.Name,
//             Profilename = OsuPlayer.Profile.User.Name
//         };
//
//         await ApiAsync.ApiRequestAsync<string>("joinParty", "parties", join);
//
//         var party = await ApiAsync.GetPartyAsync(HostPartyId);
//
//         PartyClientId = party.Clients.Last().ClientId.ToString();
//
//         if (Player.GetSongByChecksum(party.Song) == null)
//             Debug.WriteLine(@"Sadly, you don't have the song with the checksum {0} installed :(", party.Song);
//         else
//             try
//             {
//                 await Player.Play(Player.GetSongByChecksum(party.Song));
//             }
//             catch (Exception ex)
//             {
//                 Debug.WriteLine(@"Can't play song :( | " + ex);
//             }
//
//         //TODO: Check For Discord Fix
//         // if (OsuPlayer.Config.ConfigStorage.UseDiscord)
//         //     OsuPlayer.DiscordManager.UpdateActivityWithParty();
//
//         PartyId = party.PartyId.ToString();
//         Party = party;
//
//         IsHost = false;
//         IsInParty = true;
//
//         HostUpdate.Start();
//     }
//
//     public async Task Leave()
//     {
//         try
//         {
//             if (HostUpdate.IsEnabled) HostUpdate.Stop();
//
//             if (Party == null || !IsInParty)
//             {
//                 HostUpdate.Stop();
//                 return;
//             }
//
//             if (IsHost)
//             {
//                 await ApiAsync.ApiRequestAsync<string>("destroyParty", "parties", Party);
//             }
//             else
//             {
//                 var partyClient = new PartyClientModel
//                 {
//                     PartyId = Party.PartyId.ToString(),
//                     ClientId = PartyClientId
//                 };
//
//                 await ApiAsync.ApiRequestAsync<string>("leaveParty", "parties", partyClient);
//             }
//
//             PartyId = string.Empty;
//             IsInParty = false;
//             Party = null;
//
//             if (OsuPlayer.Config.ConfigStorage.UseDiscord)
//             {
//                 //TODO: Check For Discord Fix
//                 // OsuPlayer.DiscordManager.UpdateActivity(Player.CurrentSong.TitleString, Player.CurrentSong.ArtistString);
//
//                 //TODO: Readd Partylabel
//                 // OsuPlayer.DashboardRefined.PartyLabel.Text = string.Empty;
//             }
//
//             HostPartyId = string.Empty;
//             IsHost = false;
//         }
//         catch (Exception ex)
//         {
//             Debug.WriteLine(ex.ToString());
//
//             HostUpdate.Stop();
//         }
//     }
//
//     public async Task KickPartyMember(ClientModel clientToKick)
//     {
//         var partyClient = new PartyClientModel
//         {
//             ClientId = clientToKick.ClientId.ToString(),
//             PartyId = PartyId
//         };
//
//         var response = await ApiAsync.ApiRequestAsync<int>("leaveparty", "parties", partyClient);
//
//         if (response != 1)
//             return;
//
//         if (!OsuPlayer.CheckIfDashboardPageIsOfType<PartyView>()) return;
//
//         var view = OsuPlayer.GetViewOfType<PartyView>();
//         await view.ReloadPartyMembers();
//     }
//
//     public async Task SendHostData()
//     {
//         if (Party == null) return;
//
//         Party.Song = Player.CurrentSong.Checksum;
//         Party.Songname = Player.CurrentSong.ToString();
//         Party.Speed = Dashboard.PlaybackSpeedSlider.Value;
//
//         if (Player.PlayerState == Playstate.Paused)
//         {
//             Party.IsPaused = true;
//             Party.Timestamp = Player.Engine.ChannelPosition;
//         }
//         else
//         {
//             Party.IsPaused = false;
//             Party.Timestamp = -1;
//         }
//
//         var response = await ApiAsync.ApiRequestAsync<int>("updateParty", "parties", Party);
//
//         if (response != 1) return;
//
//         if (!OsuPlayer.CheckIfDashboardPageIsOfType<PartyView>()) return;
//
//         var view = OsuPlayer.GetViewOfType<PartyView>();
//
//         view.PartyCurrentSongLabel.Text = Party.Songname;
//         view.PartyCurrentSongLabel.ToolTip = Party.Songname;
//         view.PartyMembersItemsControl.ItemsSource = Party.Clients;
//         view.KickPartyMemberCombobox.ItemsSource =
//             Party.Clients.Where(x => x.Username != Party.Hostname).Select(x => x.Username);
//     }
//
//     private async void PartyUpdate_Tick(object source, EventArgs e)
//     {
//         if (string.IsNullOrEmpty(HostPartyId) && !IsHost) return;
//
//         var response = await ApiAsync.GetPartyAsync(Party.PartyId.ToString());
//
//         if (response == null)
//         {
//             OsuPlayerMessageBox.Show(OsuPlayer.LanguageService.LoadControlLanguageWithKey("message.apiDown"));
//
//             await Leave();
//         }
//
//         //Get the RaveParty object from the Api
//         var party = response;
//
//         if (OsuPlayer.Player.SongTitle == null) return;
//
//         if (party == null)
//         {
//             OsuPlayerMessageBox.Show(string.Format(
//                 OsuPlayer.LanguageService.LoadControlLanguageWithKey("message.partyClosed"), HostPartyId));
//
//             //TODO: Check For Discord Fix
//             // if (OsuPlayer.Config.ConfigStorage.UseDiscord)
//             //     OsuPlayer.DiscordManager.UpdateActivity(Player.CurrentSong.TitleString, Player.CurrentSong.ArtistString);
//
//             await Leave();
//
//             return;
//         }
//
//         var partySize = party.Clients.Count;
//
//         //Check if party size updated and you are the host
//         //If so restart current song for all, so all are synced with the new person
//         if (partySize > Party.Clients.Count && IsHost)
//         {
//             //TODO: Check For Discord Fix
//             // if(OsuPlayer.Config.ConfigStorage.UseDiscord)
//             //     OsuPlayer.DiscordManager.UpdateActivityWithParty();
//
//             Party = party;
//
//             return;
//         }
//
//         if (OsuPlayer.CheckIfDashboardPageIsOfType<PartyView>())
//         {
//             var view = OsuPlayer.GetViewOfType<PartyView>();
//
//             view.PartyCurrentSongLabel.Text = party.Songname;
//             view.PartyCurrentSongLabel.ToolTip = party.Songname;
//             await view.LoadPartyData();
//         }
//
//         //If host, don't do anything just update presence
//         if (IsHost)
//         {
//             if (Party.Song != OsuPlayer.Player.CurrentSong.Checksum)
//             {
//                 await SendHostData();
//
//                 if (OsuPlayer.CheckIfDashboardPageIsOfType<PartyView>())
//                 {
//                     var view = OsuPlayer.GetViewOfType<PartyView>();
//
//                     view.PartyCurrentSongLabel.Text = party.Songname;
//                     view.PartyCurrentSongLabel.ToolTip = party.Songname;
//                     await view.LoadPartyData();
//                 }
//             }
//
//             //TODO: Check For Discord Fix
//             // if (OsuPlayer.Config.ConfigStorage.UseDiscord)
//             //     OsuPlayer.DiscordManager.UpdateActivityWithParty();
//
//             Party = party;
//
//             return;
//         }
//
//         //Check if user is still in party, if not he was kicked
//         var found = party.Clients.ToList()
//             .FindIndex(x => x.ClientId.ToString().Equals(PartyClientId));
//
//         if (found == -1)
//         {
//             await Leave();
//
//             OsuPlayerMessageBox.Show(OsuPlayer.LanguageService.LoadControlLanguageWithKey("message.partyKicked"));
//
//             return;
//         }
//
//         //Check if the user has the song if not, display download button
//         HostPartyId = party.PartyId.ToString();
//
//         //Debug.WriteLine($"{HostPartyId}, {party.Song}");
//
//         // ReSharper disable once CompareOfFloatsByEqualityOperator
//         if (party.Speed != 44100) Dashboard.PlaybackSpeedSlider.Value = party.Speed;
//
//         //TODO: Check For Discord Fix
//         // if (OsuPlayer.Config.ConfigStorage.UseDiscord)
//         //     OsuPlayer.DiscordManager.UpdateActivityWithParty();
//
//         //Check if party is paused and jump to the current paused timestamp
//         //For syncing purpose
//         if (party.IsPaused && OsuPlayer.Player.PlayerState == Playstate.Playing)
//         {
//             OsuPlayer.Player.Engine.ChannelPosition = party.Timestamp;
//             Dashboard.PlaybackSpeedSlider.Value = party.Speed;
//             OsuPlayer.Player.Pause();
//         }
//         else if (OsuPlayer.Player.PlayerState == Playstate.Paused && !party.IsPaused)
//         {
//             OsuPlayer.Player.Pause();
//         }
//
//         Debug.WriteLine($"CurrentPartySong: {Party.Song} | {party.Song}");
//         Debug.WriteLine("Is new song: " + (party.Song != Party.Song));
//
//         if (Party.Song == OsuPlayer.Player.CurrentSong.Checksum)
//         {
//             Party = party;
//             return;
//         }
//
//         Party = party;
//
//         if (OsuPlayer.Player.GetSongByChecksum(party.Song) == null) return;
//
//         await OsuPlayer.Player.PlaySongFromCheckSum(party.Song);
//     }
// }

