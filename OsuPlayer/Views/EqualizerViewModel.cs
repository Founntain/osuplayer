using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Reactive.Disposables;
using OsuPlayer.Extensions.Bindables;
using OsuPlayer.Extensions.Equalizer;
using OsuPlayer.IO.Storage.Config;
using OsuPlayer.IO.Storage.Equalizer;
using OsuPlayer.Modules.Audio;
using OsuPlayer.ViewModels;
using ReactiveUI;

namespace OsuPlayer.Views;

public class EqualizerViewModel : BaseViewModel
{
    private readonly BindableArray<decimal> _frequencies = new(10, 1);
    private readonly Player _player;

    public EqualizerViewModel(Player player)
    {
        Activator = new ViewModelActivator();
        _player = player;

        _frequencies.BindTo(_player.EqGains);
        _frequencies.BindCollectionChanged(UpdateEq);

        this.WhenActivated(disposables => { Disposable.Create(() => { }).DisposeWith(disposables); });
    }

    private void UpdateEq(object sender, NotifyCollectionChangedEventArgs args)
    {
        using var eqPresets = new EqStorage();
        if (eqPresets.Container.EqPresets?.FirstOrDefault(x => x.Name == "Flat (Default)") == default)
        {
            eqPresets.Container.EqPresets ??= new List<EqPreset>();

            eqPresets.Container.EqPresets.Insert(0, EqPreset.Flat);

            EqPresets = eqPresets.Container.EqPresets;

            this.RaisePropertyChanged(nameof(EqPresets));
        }

        var freq = (BindableArray<decimal>) sender;

        if (eqPresets.Container.EqPresets == default || EqPresets == default) return;

        if (eqPresets.Container.EqPresets.FirstOrDefault(x => x.Gain.SequenceEqual(freq!.Array)) is { } found)
        {
            this.RaisePropertyChanged(nameof(SelectedPreset));
        }
        else
        {
            if (eqPresets.Container.EqPresets.FirstOrDefault(x => x.Name == "Custom") is { } custom)
            {
                custom.Gain = freq!.Array;
            }
            else
            {
                var newCustom = EqPreset.Custom;
                newCustom.Gain = freq!.Array;

                eqPresets.Container.EqPresets.Insert(1, newCustom);
            }

            EqPresets = eqPresets.Container.EqPresets;

            this.RaisePropertyChanged(nameof(EqPresets));
            this.RaisePropertyChanged(nameof(SelectedPreset));
        }
    }

    #region EqFrequencies

    public decimal F80
    {
        get => _frequencies[0];
        set
        {
            _frequencies[0] = value;
            this.RaisePropertyChanged();
        }
    }

    public decimal F125
    {
        get => _frequencies[1];
        set
        {
            _frequencies[1] = value;
            this.RaisePropertyChanged();
        }
    }

    public decimal F200
    {
        get => _frequencies[2];
        set
        {
            _frequencies[2] = value;
            this.RaisePropertyChanged();
        }
    }

    public decimal F300
    {
        get => _frequencies[3];
        set
        {
            _frequencies[3] = value;
            this.RaisePropertyChanged();
        }
    }

    public decimal F500
    {
        get => _frequencies[4];
        set
        {
            _frequencies[4] = value;
            this.RaisePropertyChanged();
        }
    }

    public decimal F1000
    {
        get => _frequencies[5];
        set
        {
            _frequencies[5] = value;
            this.RaisePropertyChanged();
        }
    }

    public decimal F2000
    {
        get => _frequencies[6];
        set
        {
            _frequencies[6] = value;
            this.RaisePropertyChanged();
        }
    }

    public decimal F4000
    {
        get => _frequencies[7];
        set
        {
            _frequencies[7] = value;
            this.RaisePropertyChanged();
        }
    }

    public decimal F8000
    {
        get => _frequencies[8];
        set
        {
            _frequencies[8] = value;
            this.RaisePropertyChanged();
        }
    }

    public decimal F16000
    {
        get => _frequencies[9];
        set
        {
            _frequencies[9] = value;
            this.RaisePropertyChanged();
        }
    }

    #endregion

    public bool IsEqEnabled
    {
        get => new Config().Container.IsEqEnabled;
        set
        {
            using (var config = new Config())
            {
                config.Container.IsEqEnabled = value;
            }

            _player.ToggleEq(value);

            this.RaisePropertyChanged();
        }
    }

    public List<EqPreset>? EqPresets { get; private set; } = new EqStorage().Container.EqPresets;

    public EqPreset? SelectedPreset
    {
        get => EqPresets?.FirstOrDefault(x => x.Gain.SequenceEqual(_frequencies.Array));
        set
        {
            if (value == default) return;

            var eq = new EqStorage();
            _frequencies.Set(eq.Container.EqPresets?.First(x => x.Name == value.Name).Gain);

            this.RaisePropertyChanged(nameof(F80));
            this.RaisePropertyChanged(nameof(F125));
            this.RaisePropertyChanged(nameof(F200));
            this.RaisePropertyChanged(nameof(F300));
            this.RaisePropertyChanged(nameof(F500));
            this.RaisePropertyChanged(nameof(F1000));
            this.RaisePropertyChanged(nameof(F2000));
            this.RaisePropertyChanged(nameof(F4000));
            this.RaisePropertyChanged(nameof(F8000));
            this.RaisePropertyChanged(nameof(F16000));
            this.RaisePropertyChanged();
        }
    }
}