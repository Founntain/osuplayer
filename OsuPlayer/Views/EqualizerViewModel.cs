using System.Reactive.Disposables;
using OsuPlayer.Extensions.Bindables;
using OsuPlayer.IO.Storage.Config;
using OsuPlayer.Modules.Audio;
using OsuPlayer.ViewModels;
using ReactiveUI;

namespace OsuPlayer.Views;

public class EqualizerViewModel : BaseViewModel
{
    private readonly BindableArray<double> _frequencies = new(10);
    private readonly Player _player;

    public EqualizerViewModel(Player player)
    {
        Activator = new ViewModelActivator();
        _player = player;

        _frequencies.BindTo(_player.EqGains);

        this.WhenActivated(disposables => { Disposable.Create(() => { }).DisposeWith(disposables); });
    }

    public double F80
    {
        get => _frequencies[0];
        set
        {
            _frequencies[0] = value;
            this.RaisePropertyChanged();
        }
    }

    public double F125
    {
        get => _frequencies[1];
        set
        {
            _frequencies[1] = value;
            this.RaisePropertyChanged();
        }
    }

    public double F200
    {
        get => _frequencies[2];
        set
        {
            _frequencies[2] = value;
            this.RaisePropertyChanged();
        }
    }

    public double F300
    {
        get => _frequencies[3];
        set
        {
            _frequencies[3] = value;
            this.RaisePropertyChanged();
        }
    }

    public double F500
    {
        get => _frequencies[4];
        set
        {
            _frequencies[4] = value;
            this.RaisePropertyChanged();
        }
    }

    public double F1000
    {
        get => _frequencies[5];
        set
        {
            _frequencies[5] = value;
            this.RaisePropertyChanged();
        }
    }

    public double F2000
    {
        get => _frequencies[6];
        set
        {
            _frequencies[6] = value;
            this.RaisePropertyChanged();
        }
    }

    public double F4000
    {
        get => _frequencies[7];
        set
        {
            _frequencies[7] = value;
            this.RaisePropertyChanged();
        }
    }

    public double F8000
    {
        get => _frequencies[8];
        set
        {
            _frequencies[8] = value;
            this.RaisePropertyChanged();
        }
    }

    public double F16000
    {
        get => _frequencies[9];
        set
        {
            _frequencies[9] = value;
            this.RaisePropertyChanged();
        }
    }

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
}