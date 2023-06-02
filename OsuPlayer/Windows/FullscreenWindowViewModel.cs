using Avalonia.Media.Imaging;
using Avalonia.Threading;
using Nein.Base;
using OsuPlayer.Modules;
using OsuPlayer.Modules.Audio.Interfaces;
using ReactiveUI;

namespace OsuPlayer.Windows;

public class FullscreenWindowViewModel : BaseWindowViewModel
{
    public readonly IPlayer Player;
    public readonly Bindable<IMapEntry?> CurrentSong = new();

    private Bitmap? _currentSongImage;

    public Bitmap? CurrentSongImage
    {
        get => _currentSongImage;
        set
        {
            _currentSongImage?.Dispose();
            this.RaiseAndSetIfChanged(ref _currentSongImage, value);
        }
    }

    public string TitleText => CurrentSong.Value?.Title ?? "No song is playing";
    
    public string ArtistText => CurrentSong.Value?.Artist ?? "please select from song list";
    
    public FullscreenWindowViewModel(IPlayer player)
    {
        Player = player;
        
        CurrentSong.BindTo(Player.CurrentSong);
        CurrentSong.BindValueChanged(_ =>
        {
            this.RaisePropertyChanged(nameof(TitleText));
            this.RaisePropertyChanged(nameof(ArtistText));
        });
        
        Player.CurrentSongImage.BindValueChanged(d =>
        {
            Dispatcher.UIThread.Post(() =>
            {
                CurrentSongImage?.Dispose();
            
                if (!string.IsNullOrEmpty(d.NewValue) && File.Exists(d.NewValue))
                {
                    CurrentSongImage = BitmapExtensions.BlurBitmap(d.NewValue, blurRadius: 25, opacity: 0.75f);
                
                    return;
                }

                CurrentSongImage = null;
            });
        }, true, true);
    }
}