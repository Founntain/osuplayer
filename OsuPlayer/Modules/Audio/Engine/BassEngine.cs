using System.Diagnostics;
using Avalonia.Threading;
using ManagedBass;
using ManagedBass.DirectX8;
using ManagedBass.Fx;
using OsuPlayer.Data.OsuPlayer.Classes;
using OsuPlayer.IO.Storage.Equalizer;
using OsuPlayer.Modules.Audio.Interfaces;
using OsuPlayer.Services;

namespace OsuPlayer.Modules.Audio.Engine;

/// <summary>
/// Audio engine for the osu!player using <see cref="ManagedBass" />
/// </summary>
public sealed class BassEngine : OsuPlayerService, IAudioEngine
{
    public override string ServiceName => "BASS_ENGINE";

    public List<AudioDevice> AvailableAudioDevices { get; } = new();
    public IAudioEngine.ChannelReachedEndHandler? ChannelReachedEnd { private get; set; }
    public BindableArray<decimal> EqGains { get; } = new(10, 1);

    public Bindable<double> ChannelLength { get; } = new();
    public Bindable<double> ChannelPosition { get; } = new();
    public Bindable<double> Volume { get; } = new();
    public Bindable<bool> IsPlaying { get; } = new();
    public Bindable<double> PlaybackSpeed { get; } = new();

    private readonly SyncProcedure _endTrackSyncProc;
    private readonly DispatcherTimer _positionTimer = new(DispatcherPriority.ApplicationIdle);

    private int _decodeStreamHandle;
    private int _fxStream;

    private bool _inChannelSet;
    private bool _inChannelTimerUpdate;
    private bool _isEqEnabled;
    private DXParamEQ? _paramEq;
    private double _playbackSpeed;
    private int _sampleFrequency = 44100;

    public bool IsEqEnabled
    {
        get => _isEqEnabled;
        set
        {
            var oldValue = _isEqEnabled;

            _isEqEnabled = value;

            if (oldValue != _isEqEnabled) SetAllEq();
        }
    }

    public BassEngine()
    {
        _positionTimer.Interval = TimeSpan.FromMilliseconds((double) 1000 / 60);
        _positionTimer.Tick += PositionTimer_Tick;
        _positionTimer.Start();

        _endTrackSyncProc = EndTrack;

        IsEqEnabled = new Config().Container.IsEqEnabled;

        ChannelPosition.BindValueChanged(d =>
        {
            if (_inChannelSet) return;

            _inChannelSet = true; // Avoid recursion

            if (!_inChannelTimerUpdate)
                Bass.ChannelSetPosition(_fxStream, Bass.ChannelSeconds2Bytes(_fxStream, d.NewValue));

            _inChannelSet = false;
        });

        Volume.BindValueChanged(d =>
        {
            try
            {
                Bass.GlobalStreamVolume = (int) (d.NewValue * 100);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }
        }, true, true);

        EqGains.BindCollectionChanged((_, _) => SetAllEq());

        InitAudioDevices();
    }

    public void Stop()
    {
        CloseFile();
    }

    public void Pause()
    {
        if (Bass.ChannelPause(_fxStream))
            IsPlaying.Value = false;
    }

    public void Play()
    {
        IsPlaying.Value = PlayCurrentStream();
    }

    public void PlayPause()
    {
        if (IsPlaying.Value)
        {
            if (Bass.ChannelPause(_fxStream))
                IsPlaying.Value = false;
        }
        else
        {
            IsPlaying.Value = PlayCurrentStream();
        }
    }

    public void SetPlaybackSpeed(double speed)
    {
        SetPlaybackSpeedOptions(speed);

        _playbackSpeed = speed;
    }

    public void UpdatePlaybackMethod()
    {
        SetPlaybackSpeedOptions(_playbackSpeed);
    }

    public bool OpenFile(string path)
    {
        Stop();

        if (!File.Exists(path)) return false;

        // Create Stream
        _decodeStreamHandle = Bass.CreateStream(path, 0, 0, BassFlags.Decode | BassFlags.Float);
        _fxStream = BassFx.TempoCreate(_decodeStreamHandle, BassFlags.FxFreeSource | BassFlags.Float);
        ChannelLength.Value = Bass.ChannelBytes2Seconds(_fxStream, Bass.ChannelGetLength(_fxStream));

        if (_fxStream != 0)
        {
            SetupStream();
            return true;
        }

        _decodeStreamHandle = 0;
        _fxStream = 0;

        return false;
    }

    public void CloseFile()
    {
        if (_fxStream == 0) return;

        Bass.ChannelStop(_fxStream);
        Bass.StreamFree(_decodeStreamHandle);

        _fxStream = 0;
        _decodeStreamHandle = 0;

        ChannelPosition.Value = 0;
    }

    /// <summary>
    /// Sets the output device for the player
    /// <remarks>If the index is -1 sets the output device to the default device set in the os.</remarks>
    /// </summary>
    /// <param name="audioDevice"></param>
    public void SetDevice(AudioDevice? audioDevice)
    {
        var audioDevices = GetAudioDevices().ToList();
        var index = audioDevice == null ? -1 : audioDevices.IndexOf(audioDevices.FirstOrDefault(x => x.Driver == audioDevice.Driver));

        if (index == -1)
            for (var i = 0; i < audioDevices.Count; i++)
            {
                var deviceInfo = audioDevices[i];

                if (!deviceInfo.IsDefault) continue;

                index = i;
                break;
            }

        Bass.CurrentDevice = index;
        Bass.ChannelSetDevice(_fxStream, index);

        using var config = new Config();
        config.Container.SelectedAudioDeviceDriver = audioDevices[index].Driver;

        LogToConsole($"DEVICE {index} | LOADING PLAYBACK {Bass.LastError}");
    }

    private void SetPlaybackSpeedOptions(double speed)
    {
        using var config = new Config();

        PlaybackSpeed.Value = speed;

        if (config.Container.UsePitch)
        {
            // Resetting Tempo effect, else speed overlaps
            Bass.ChannelSetAttribute(_fxStream, ChannelAttribute.Tempo,
                0);

            Bass.ChannelSetAttribute(_fxStream, ChannelAttribute.TempoFrequency,
                _sampleFrequency * (1 + speed));
        }
        else
        {
            // Resetting TempoFrequency effect, else speed overlaps
            Bass.ChannelSetAttribute(_fxStream, ChannelAttribute.TempoFrequency,
                _sampleFrequency * (1 + 0));

            Bass.ChannelSetAttribute(_fxStream, ChannelAttribute.Tempo,
                speed * 100);
        }
    }

    private void InitAudioDevices()
    {
        AvailableAudioDevices.Clear();

        var counter = 1;

        foreach (var deviceInfo in GetAudioDevices().Skip(1))
        {
            var success = Bass.Init(counter);

            LogToConsole($"INIT DEVICE {deviceInfo} | SUCCESSFUL: {success} | CODE: {Bass.LastError}");

            if (success) AvailableAudioDevices.Add(deviceInfo);

            counter++;
        }
    }

    private void EndTrack(int handle, int channel, int data, IntPtr user)
    {
        ChannelReachedEnd?.Invoke().Wait();
    }

    private void SetupStream()
    {
        // Obtain the sample rate of the stream
        var info = Bass.ChannelGetInfo(_fxStream);

        _sampleFrequency = info.Frequency;

        //SetEqBands();

        SetPlaybackSpeedOptions(_playbackSpeed);

        InitEq();

        var config = new Config();
        SetDevice(AvailableAudioDevices.FirstOrDefault(x => x.Driver == config.Container.SelectedAudioDeviceDriver));

        // Set the stream to call Stop() when it ends.
        var syncHandle = Bass.ChannelSetSync(_fxStream,
            SyncFlags.End,
            0,
            _endTrackSyncProc,
            IntPtr.Zero);

        if (syncHandle == 0)
            throw new ArgumentException(@"Error establishing End Sync on file stream.");
    }

    private bool PlayCurrentStream()
    {
        // Play Stream
        if (_fxStream != 0 && Bass.ChannelPlay(_fxStream)) return true;

        return false;
    }

    /// <summary>
    /// Inits the eq on the <see cref="_fxStream" />
    /// </summary>
    private void InitEq()
    {
        if (_fxStream == 0) return;

        _paramEq = new DXParamEQ(_fxStream);

        _paramEq.AddBand(80);
        _paramEq.AddBand(125);
        _paramEq.AddBand(200);
        _paramEq.AddBand(300);
        _paramEq.AddBand(500);
        _paramEq.AddBand(1000);
        _paramEq.AddBand(2000);
        _paramEq.AddBand(4000);
        _paramEq.AddBand(8000);
        _paramEq.AddBand(16000);

        using var eqStorage = new EqStorage();

        eqStorage.Container.LastUsedEqId ??= eqStorage.Container.EqPresets?.First().Id;

        EqGains.Set(eqStorage.Container.EqPresets?.FirstOrDefault(x => x.Id == eqStorage.Container.LastUsedEqId)?.Gain);

        SetAllEq();
    }

    /// <summary>
    /// Sets all eq bands with values from <see cref="EqGains" />
    /// </summary>
    private void SetAllEq()
    {
        if (_paramEq == null) return;

        for (var i = 0; i < EqGains.Length; i++)
            _paramEq?.UpdateBand(i, (double) (IsEqEnabled ? EqGains[i] : 0));
    }

    /// <summary>
    /// Gets all audio device infos
    /// </summary>
    /// <returns>a list of <see cref="DeviceInfo" /> containing the devices</returns>
    private List<DeviceInfo> GetDeviceInfos()
    {
        var list = new List<DeviceInfo>();

        for (var i = 0; i < Bass.DeviceCount; i++)
            list.Add(Bass.GetDeviceInfo(i));

        return list;
    }

    /// <summary>
    /// Gets all audio devices
    /// </summary>
    /// <returns>an <see cref="IEnumerable{T}" /> of <see cref="AudioDevice" /> containing all found devices on the computer</returns>
    private IEnumerable<AudioDevice> GetAudioDevices()
    {
        foreach (var info in GetDeviceInfos()) yield return new AudioDevice(info);
    }

    private void PositionTimer_Tick(object sender, EventArgs e)
    {
        if (!IsPlaying.Value) return;

        if (_fxStream == 0)
        {
            ChannelPosition.Value = 0;
        }
        else
        {
            _inChannelTimerUpdate = true;

            var bytes = Bass.ChannelGetPosition(_fxStream);

            if (bytes >= 0)
                ChannelPosition.Value = Bass.ChannelBytes2Seconds(_fxStream, bytes);

            _inChannelTimerUpdate = false;
        }
    }
}