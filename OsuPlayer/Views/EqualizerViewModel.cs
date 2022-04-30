using System;
using System.Collections.Generic;
using System.Reactive.Disposables;
using OsuPlayer.Extensions.Bindables;
using OsuPlayer.ViewModels;
using ReactiveUI;

namespace OsuPlayer.Views;

public class EqualizerViewModel : BaseViewModel
{
    private readonly double[] _frequencies = new double[10];

    public EqualizerViewModel()
    {
        Activator = new ViewModelActivator();

        this.WhenActivated(disposables => { Disposable.Create(() => { }).DisposeWith(disposables); });
    }

    public double F80
    {
        get => _frequencies[0];
        set => this.RaiseAndSetIfChanged(ref _frequencies[0], value);
    }

    public double F125
    {
        get => _frequencies[1];
        set => this.RaiseAndSetIfChanged(ref _frequencies[1], value);
    }

    public double F200
    {
        get => _frequencies[2];
        set => this.RaiseAndSetIfChanged(ref _frequencies[2], value);
    }

    public double F300
    {
        get => _frequencies[3];
        set => this.RaiseAndSetIfChanged(ref _frequencies[3], value);
    }

    public double F500
    {
        get => _frequencies[4];
        set => this.RaiseAndSetIfChanged(ref _frequencies[4], value);
    }

    public double F1000
    {
        get => _frequencies[5];
        set => this.RaiseAndSetIfChanged(ref _frequencies[5], value);
    }

    public double F2000
    {
        get => _frequencies[6];
        set => this.RaiseAndSetIfChanged(ref _frequencies[6], value);
    }

    public double F4000
    {
        get => _frequencies[7];
        set => this.RaiseAndSetIfChanged(ref _frequencies[7], value);
    }

    public double F8000
    {
        get => _frequencies[8];
        set => this.RaiseAndSetIfChanged(ref _frequencies[8], value);
    }

    public double F16000
    {
        get => _frequencies[9];
        set => this.RaiseAndSetIfChanged(ref _frequencies[9], value);
    }
}