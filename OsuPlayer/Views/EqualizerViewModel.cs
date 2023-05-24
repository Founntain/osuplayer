using System.Collections.Specialized;
using System.Reactive.Disposables;
using Avalonia.Threading;
using Nein.Base;
using OsuPlayer.Data.OsuPlayer.Classes;
using OsuPlayer.IO.Storage.Equalizer;
using OsuPlayer.Modules.Audio.Interfaces;
using ReactiveUI;

namespace OsuPlayer.Views;

public class EqualizerViewModel : BaseViewModel
{
    private readonly IHasEffects _player;
    public readonly BindableArray<decimal> Frequencies = new(10, 1);
    private List<EqPreset>? _eqPresets = new EqStorage().Container.EqPresets;
    private bool _isDeleteEqPresetPopupOpen;
    private bool _isNewEqPresetPopupOpen;
    private bool _isRenameEqPresetPopupOpen;
    private string _newEqPresetNameText = string.Empty;

    private EqPreset? _selectedPreset;

    public bool IsEqEnabled
    {
        get => _player.IsEqEnabled;
        set
        {
            using (var config = new Config())
            {
                config.Container.IsEqEnabled = value;
            }

            _player.IsEqEnabled = value;

            this.RaisePropertyChanged();
        }
    }

    public List<EqPreset>? EqPresets
    {
        get => _eqPresets;
        set
        {
            _eqPresets = value;
            this.RaisePropertyChanged();
            this.RaisePropertyChanged(nameof(SelectedPreset));
        }
    }

    public EqPreset? SelectedPreset
    {
        get => _selectedPreset;
        set
        {
            //if (value == default) return;

            _selectedPreset = value;
            using (var eqStorage = new EqStorage())
            {
                eqStorage.Container.LastUsedEqId = value?.Id;

                Frequencies.Set(eqStorage.Container.EqPresets?.FirstOrDefault(x => x.Id == value?.Id)?.Gain);
            }

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

    public bool IsNewEqPresetPopupOpen
    {
        get => _isNewEqPresetPopupOpen;
        set => this.RaiseAndSetIfChanged(ref _isNewEqPresetPopupOpen, value);
    }

    public string NewEqPresetNameText
    {
        get => _newEqPresetNameText;
        set => this.RaiseAndSetIfChanged(ref _newEqPresetNameText, value);
    }

    public bool IsDeleteEqPresetPopupOpen
    {
        get => _isDeleteEqPresetPopupOpen;
        set => this.RaiseAndSetIfChanged(ref _isDeleteEqPresetPopupOpen, value);
    }

    public bool IsRenameEqPresetPopupOpen
    {
        get => _isRenameEqPresetPopupOpen;
        set => this.RaiseAndSetIfChanged(ref _isRenameEqPresetPopupOpen, value);
    }

    public EqualizerViewModel(IHasEffects player)
    {
        Activator = new ViewModelActivator();
        _player = player;

        Frequencies.BindTo(_player.EqGains);
        Frequencies.BindCollectionChanged((sender, args) =>
        {
            Dispatcher.UIThread.Post(() => UpdateEq(sender, args));
        });

        using (var eqStorage = new EqStorage())
        {
            eqStorage.Container.LastUsedEqId ??= eqStorage.Container.EqPresets?.First().Id;
            SelectedPreset = eqStorage.Container.EqPresets?.FirstOrDefault(x => x.Id == eqStorage.Container.LastUsedEqId);
        }

        this.WhenActivated(disposables => { Disposable.Create(() => { }).DisposeWith(disposables); });
    }

    private void UpdateEq(object? sender, NotifyCollectionChangedEventArgs args)
    {
        if (EqPresets?.FirstOrDefault(x => x.Name == "Flat (Default)") == default)
        {
            EqPresets ??= new List<EqPreset>();

            EqPresets.Insert(0, EqPreset.Flat);

            using var eqPresets = new EqStorage();

            eqPresets.Container.EqPresets = EqPresets;
        }

        if (SelectedPreset == default) return;

        if (SelectedPreset.Name == "Flat (Default)" && Frequencies.Array.Any(x => x != 0))
        {
            SelectedPreset = EqPresets.FirstOrDefault(x => x.Name == "Custom");

            if (SelectedPreset == default)
            {
                var newCustom = EqPreset.Custom;
                newCustom.Gain = (decimal[]) Frequencies.Array.Clone();

                EqPresets.Insert(1, newCustom);

                SelectedPreset = EqPresets[1];
            }
        }

        SelectedPreset.Gain = (decimal[]) Frequencies.Array.Clone();

        using (var eqPresets = new EqStorage())
        {
            eqPresets.Container.EqPresets = EqPresets;
        }
    }

    #region EqFrequencies

    public decimal F80
    {
        get => Frequencies[0];
        set
        {
            Frequencies[0] = value;
            this.RaisePropertyChanged();
        }
    }

    public decimal F125
    {
        get => Frequencies[1];
        set
        {
            Frequencies[1] = value;
            this.RaisePropertyChanged();
        }
    }

    public decimal F200
    {
        get => Frequencies[2];
        set
        {
            Frequencies[2] = value;
            this.RaisePropertyChanged();
        }
    }

    public decimal F300
    {
        get => Frequencies[3];
        set
        {
            Frequencies[3] = value;
            this.RaisePropertyChanged();
        }
    }

    public decimal F500
    {
        get => Frequencies[4];
        set
        {
            Frequencies[4] = value;
            this.RaisePropertyChanged();
        }
    }

    public decimal F1000
    {
        get => Frequencies[5];
        set
        {
            Frequencies[5] = value;
            this.RaisePropertyChanged();
        }
    }

    public decimal F2000
    {
        get => Frequencies[6];
        set
        {
            Frequencies[6] = value;
            this.RaisePropertyChanged();
        }
    }

    public decimal F4000
    {
        get => Frequencies[7];
        set
        {
            Frequencies[7] = value;
            this.RaisePropertyChanged();
        }
    }

    public decimal F8000
    {
        get => Frequencies[8];
        set
        {
            Frequencies[8] = value;
            this.RaisePropertyChanged();
        }
    }

    public decimal F16000
    {
        get => Frequencies[9];
        set
        {
            Frequencies[9] = value;
            this.RaisePropertyChanged();
        }
    }

    #endregion
}