using Avalonia.ReactiveUI;
using Avalonia.Threading;
using Nein.Base;
using Nein.Extensions;
using OsuPlayer.Modules.Audio.Interfaces;
using ReactiveUI;
using Splat;

namespace OsuPlayer.Views.CustomControls;

public partial class AudioVisualizerView : ReactiveUserControl<AudioVisualizerViewModel>
{
    private object _lockObj = new ();

    public AudioVisualizerView()
    {
        InitializeComponent();

        this.WhenActivated(_ =>
        {
            ViewModel.AudioVisualizerUpdateTimer.Interval = TimeSpan.FromMilliseconds(2);
            ViewModel.AudioVisualizerUpdateTimer.Tick += AudioVisualizerUpdateTimer_OnTick;

            ViewModel.AudioVisualizerUpdateTimer.Start();
        });
    }

    private void AudioVisualizerUpdateTimer_OnTick(object? sender, EventArgs e)
    {
        Dispatcher.UIThread.Invoke(() =>
        {
            using var config = new Config();

            // Do nothing if audio visualizer is disabled
            if (!config.Container.DisplayAudioVisualizer) return;

            var player = Locator.Current.GetRequiredService<IPlayer>();

            if (ViewModel == default) return;

            if (!player.IsPlaying.Value)
            {
                foreach (var t in ViewModel.SeriesValues.Where(x => x.Value != 0))
                {
                    t.Value = 0;
                }

                return;
            }

            lock (_lockObj)
            {
                // var audioEngine = Locator.Current.GetRequiredService<IAudioEngine>();

                var vData = ViewModel.AudioEngine.GetVisualizationData();

                for (var i = 0; i < vData.Length; i++)
                {
                    ViewModel.SeriesValues[i].Value = vData[i] * 5;
                }
            }
        });
    }
}