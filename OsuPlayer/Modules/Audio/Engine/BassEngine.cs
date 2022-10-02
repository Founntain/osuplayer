using System.ComponentModel;
using System.Diagnostics;
using Avalonia.Threading;
using DynamicData;
using ManagedBass;
using ManagedBass.DirectX8;
using ManagedBass.Fx;
using OsuPlayer.Data.OsuPlayer.Classes;
using OsuPlayer.IO.Storage.Equalizer;

namespace OsuPlayer.Modules.Audio.Engine;

/// <summary>
/// Audio engine for the osu!player using <see cref="ManagedBass" />
/// </summary>
public sealed class BassEngine : IAudioEngine
{
    private readonly SyncProcedure _endTrackSyncProc;

    private readonly DispatcherTimer _positionTimer = new(DispatcherPriority.ApplicationIdle);
    private int _activeStream;
    private bool _inChannelSet;
    private bool _inChannelTimerUpdate;
    private bool _isEqEnabled;
    private DXParamEQ? _paramEq;
    private double _playbackSpeed;
    private int _sampleFrequency = 44100;
    private int _streamFx;
    public List<AudioDevice> AvailableAudioDevices { get; } = new();
    public Bindable<double> ChannelLength { get; } = new();
    public Bindable<double> ChannelPosition { get; } = new();
    public BindableArray<decimal> EqGains { get; } = new(10, 1);
    public Bindable<double> Volume { get; } = new();

    private int ActiveStreamHandle
    {
        get => _activeStream;
        set
        {
            var oldValue = _activeStream;
            _activeStream = value;
            if (oldValue != _activeStream)
                NotifyPropertyChanged("ActiveStreamHandle");
        }
    }

    private int FxStream
    {
        get => _streamFx;
        set
        {
            var oldValue = _streamFx;
            _streamFx = value;
            if (oldValue != _streamFx)
                NotifyPropertyChanged("FXStream");
        }
    }

    public Bindable<bool> IsPlaying { get; } = new();

    public bool IsEqEnabled
    {
        get => _isEqEnabled;
        set
        {
            var oldValue = _isEqEnabled;
            _isEqEnabled = value;
            if (oldValue != _isEqEnabled)
            {
                NotifyPropertyChanged("EqEnabled");
                SetAllEq();
            }
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
                Bass.ChannelSetPosition(FxStream, Bass.ChannelSeconds2Bytes(FxStream, d.NewValue));

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
        }, true);

        IsPlaying.BindValueChanged(d => NotifyPropertyChanged("PlayState"), true);

        EqGains.BindCollectionChanged((sender, args) => SetAllEq());

        InitAudioDevices();
    }

    private void InitAudioDevices()
    {
        AvailableAudioDevices.Clear();

        var counter = 1;

        foreach (var deviceInfo in GetAudioDevices())
        {
            var success = Bass.Init(counter);

            Console.WriteLine($"INIT: {deviceInfo} | {success} | {Bass.LastError}");

            if (success)
            {
                AvailableAudioDevices.Add(deviceInfo);
            }

            counter++;
        }
    }

    private void EndTrack(int handle, int channel, int data, IntPtr user)
    {
        NotifyPropertyChanged("SongEnd");
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    private void NotifyPropertyChanged(string info)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(info));
    }

    public void Stop()
    {
        ChannelPosition.Value = ChannelLength.Value;
        if (FxStream != 0)
        {
            Bass.ChannelStop(FxStream);
            Bass.ChannelSetPosition(FxStream, (long) ChannelPosition.Value);
            IsPlaying.Value = false;
        }
    }

    public void Pause()
    {
        if (Bass.ChannelPause(FxStream))
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
            if (Bass.ChannelPause(FxStream))
                IsPlaying.Value = false;
        }
        else
        {
            IsPlaying.Value = PlayCurrentStream();
        }
    }

    /// <summary>
    /// Sets the speed of the current <see cref="FxStream" />
    /// </summary>
    /// <param name="speed">a 0 based offset for the speed</param>
    public void SetPlaybackSpeed(double speed)
    {
        Bass.ChannelSetAttribute(FxStream, ChannelAttribute.TempoFrequency,
            _sampleFrequency * (1 + speed));
        _playbackSpeed = speed;
    }

    /// <summary>
    /// Opens an audio file and creates an <see cref="FxStream" /> containing the decoded audio stream
    /// </summary>
    /// <param name="path">the path to the audio file to be opened</param>
    /// <returns>true if opening succeeded, false else</returns>
    /// <exception cref="ArgumentException">if the sync handle could not be created</exception>
    public bool OpenFile(string path)
    {
        Stop();

        DisposeStreamIfNeeded();

        if (!File.Exists(path)) return false;

        // Create Stream
        ActiveStreamHandle = Bass.CreateStream(path, 0, 0, BassFlags.Decode | BassFlags.Float);
        FxStream = BassFx.TempoCreate(ActiveStreamHandle,
            BassFlags.FxFreeSource | BassFlags.Float | BassFlags.Loop);
        ChannelLength.Value = Bass.ChannelBytes2Seconds(FxStream, Bass.ChannelGetLength(FxStream));

        if (FxStream != 0)
        {
            SetupStream();
            return true;
        }

        ActiveStreamHandle = 0;
        FxStream = 0;

        return false;
    }

    private void DisposeStreamIfNeeded()
    {
        if (FxStream != 0)
        {
            ChannelPosition.Value = 0;
            Bass.StreamFree(ActiveStreamHandle);
            Bass.StreamFree(FxStream);
        }
    }

    private void SetupStream()
    {
        // Obtain the sample rate of the stream
        var info = Bass.ChannelGetInfo(FxStream);
        _sampleFrequency = info.Frequency;
        //SetEqBands();
        var speed = _sampleFrequency * (1 + _playbackSpeed);
        Bass.ChannelSetAttribute(FxStream, ChannelAttribute.TempoFrequency, speed);

        InitEq();

        var config = new Config();
        SetDevice(AvailableAudioDevices[config.Container.SelectedAudioDevice]);

        // Set the stream to call Stop() when it ends.
        var syncHandle = Bass.ChannelSetSync(FxStream,
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
        if (FxStream != 0 && Bass.ChannelPlay(FxStream)) return true;

        return false;
    }

    /// <summary>
    /// Inits the eq on the <see cref="FxStream" />
    /// </summary>
    private void InitEq()
    {
        if (FxStream == 0) return;

        _paramEq = new DXParamEQ(FxStream);

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

        using (var eqStorage = new EqStorage())
        {
            eqStorage.Container.LastUsedEqId ??= eqStorage.Container.EqPresets?.First().Id;

            EqGains.Set(eqStorage.Container.EqPresets?.FirstOrDefault(x => x.Id == eqStorage.Container.LastUsedEqId)?.Gain);
            SetAllEq();
        }
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

        list.RemoveAt(0);

        return list;
    }

    /// <summary>
    /// Sets the output device for the player
    /// <remarks>If the index is -1 sets the output device to the default device set in the os.</remarks>
    /// </summary>
    /// <param name="audioDevice"></param>
    public void SetDevice(AudioDevice audioDevice)
    {
        var audioDevices = GetAudioDevices().ToList();
        var index = audioDevices.IndexOf(audioDevices.FirstOrDefault(x => x.Driver == audioDevice.Driver));

        if (index == -1)
        {
            for (var i = 0; i < audioDevices.Count; i++)
            {
                var deviceInfo = audioDevices[i];

                if (!deviceInfo.IsDefault) continue;

                index = i;
                break;
            }
        }

        Bass.CurrentDevice = index + 1;
        Bass.ChannelSetDevice(FxStream, index + 1);

        var result = Bass.LastError == Errors.OK;

        using var config = new Config();
        config.Container.SelectedAudioDevice = index;

        Console.WriteLine($"SET: {index} | {result} | {Bass.LastError}");
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
        if (FxStream == 0)
        {
            ChannelPosition.Value = 0;
        }
        else
        {
            _inChannelTimerUpdate = true;
            ChannelPosition.Value = Bass.ChannelBytes2Seconds(FxStream, Bass.ChannelGetPosition(FxStream));
            _inChannelTimerUpdate = false;
        }
    }
}