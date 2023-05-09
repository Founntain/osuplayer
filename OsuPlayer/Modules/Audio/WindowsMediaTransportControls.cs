using Windows.Media;
using Windows.Media.Playback;
using Windows.Storage;
using Windows.Storage.Streams;
using OsuPlayer.Data.OsuPlayer.Enums;
using OsuPlayer.Modules.Audio.Interfaces;

namespace OsuPlayer.Modules.Audio;

public class WindowsMediaTransportControls
{
    private readonly MediaPlayer _mediaPlayer;
    private readonly SystemMediaTransportControls _mediaTransportControls;
    private readonly IPlayer _player;

    public WindowsMediaTransportControls(IPlayer player)
    {
        _player = player;

        _mediaPlayer = new MediaPlayer();

        _mediaTransportControls = _mediaPlayer.SystemMediaTransportControls;
        _mediaPlayer.CommandManager.IsEnabled = false;
        _mediaPlayer.Play();

        _mediaTransportControls.IsEnabled = true;
        _mediaTransportControls.IsPlayEnabled = true;
        _mediaTransportControls.IsPauseEnabled = true;
        _mediaTransportControls.IsNextEnabled = true;
        _mediaTransportControls.IsPreviousEnabled = true;

        _mediaTransportControls.ButtonPressed += MediaTransportControls_ButtonPressed;
    }

    private void MediaTransportControls_ButtonPressed(SystemMediaTransportControls sender, SystemMediaTransportControlsButtonPressedEventArgs args)
    {
        switch (args.Button)
        {
            case SystemMediaTransportControlsButton.Play:
                _player.Play();
                _mediaPlayer.Play();
                break;
            case SystemMediaTransportControlsButton.Pause:
                _player.Pause();
                _mediaPlayer.Pause();
                break;
            case SystemMediaTransportControlsButton.Stop:
                _player.Stop();
                break;
            case SystemMediaTransportControlsButton.Next:
                _player.NextSong(PlayDirection.Forward);
                break;
            case SystemMediaTransportControlsButton.Previous:
                _player.NextSong(PlayDirection.Backwards);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    public void UpdatePlayingStatus(bool isPlaying)
    {
        if (isPlaying)
        {
            _mediaPlayer.Play();
            _mediaTransportControls.PlaybackStatus = MediaPlaybackStatus.Playing;
        }
        else
        {
            _mediaPlayer.Pause();
            _mediaTransportControls.PlaybackStatus = MediaPlaybackStatus.Paused;
        }
    }

    public async void SetMetadata(IMapEntry fullMapEntry)
    {
        var metadata = _mediaTransportControls.DisplayUpdater;

        metadata.Type = MediaPlaybackType.Music;
        metadata.MusicProperties.Title = fullMapEntry.Title;
        metadata.MusicProperties.Artist = fullMapEntry.Artist;

        try
        {
            if (!string.IsNullOrEmpty(_player.CurrentSongImage.Value) && File.Exists(_player.CurrentSongImage.Value))
            {
                var x = await StorageFile.GetFileFromPathAsync(_player.CurrentSongImage.Value ?? "");
                metadata.Thumbnail = RandomAccessStreamReference.CreateFromFile(x);
            }
            else
            {
                metadata.Thumbnail = null;
            }
        }
        catch (Exception)
        {
            metadata.Thumbnail = null;
        }

        metadata.Update();

        _mediaPlayer.Play();
    }
}