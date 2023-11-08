using Windows.UI.Input;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.VisualTree;
using Nein.Base;
using Nein.Extensions;
using OsuPlayer.Data.OsuPlayer.Classes;
using OsuPlayer.IO.Storage.Equalizer;
using OsuPlayer.UI_Extensions;
using OsuPlayer.Windows;
using ReactiveUI;
using Splat;
using TappedEventArgs = Avalonia.Input.TappedEventArgs;

namespace OsuPlayer.Views;

public partial class EqualizerView : ReactiveControl<EqualizerViewModel>
{
    private FluentAppWindow? _mainWindow;

    public EqualizerView()
    {
        InitializeComponent();

        _mainWindow = Locator.Current.GetRequiredService<FluentAppWindow>();
    }

    private void ResetSlider(object? sender, TappedEventArgs e)
    {
        if (sender is Slider slider)
            slider.Value = 0;
    }

    private void AddEqPreset_OnClick(object? sender, RoutedEventArgs e)
    {
        ViewModel.IsNewEqPresetPopupOpen = !ViewModel.IsNewEqPresetPopupOpen;
    }

    private void ConfirmNewEqPreset_OnClick(object? sender, RoutedEventArgs e)
    {
        if (string.IsNullOrWhiteSpace(ViewModel.NewEqPresetNameText)) return;

        var eqPreset = new EqPreset
        {
            Name = ViewModel.NewEqPresetNameText,
            Gain = (decimal[]) ViewModel.Frequencies.Array.Clone()
        };

        using var eqStorage = new EqStorage();

        if (eqStorage.Container.EqPresets == default) return;

        if (eqStorage.Container.EqPresets.Any(x => x.Name == eqPreset.Name)) return;

        eqStorage.Container.EqPresets.Add(eqPreset);

        ViewModel.IsNewEqPresetPopupOpen = false;
        ViewModel.EqPresets = eqStorage.Container.EqPresets;

        ViewModel.RaisePropertyChanged(nameof(ViewModel.EqPresets));

        ViewModel.SelectedPreset = ViewModel.EqPresets!.Last();
    }

    private async void DeleteEqPreset_OnClick(object? sender, RoutedEventArgs e)
    {
        if (_mainWindow == default) return;

        if (ViewModel.SelectedPreset?.Name is "Flat (Default)" or "Custom")
        {
            await MessageBox.ShowDialogAsync(_mainWindow, "No you can't delete the default and custom preset! Sorry :(");
            return;
        }

        ViewModel.IsDeleteEqPresetPopupOpen = !ViewModel.IsDeleteEqPresetPopupOpen;
    }

    private async void ConfirmDeleteEqPreset_OnClick(object? sender, RoutedEventArgs e)
    {
        await using (var eqStorage = new EqStorage())
        {
            eqStorage.Container.EqPresets?.Remove(eqStorage.Container.EqPresets?.FirstOrDefault(x => x.Id == ViewModel.SelectedPreset?.Id));

            ViewModel.EqPresets = eqStorage.Container.EqPresets;
        }

        ViewModel.RaisePropertyChanged(nameof(ViewModel.EqPresets));
        ViewModel.RaisePropertyChanged(nameof(ViewModel.SelectedPreset));

        ViewModel.IsDeleteEqPresetPopupOpen = false;

        ViewModel.SelectedPreset = ViewModel.EqPresets?.First();
    }

    private void CancelDeleteEqPreset_OnClick(object? sender, RoutedEventArgs e)
    {
        ViewModel.IsDeleteEqPresetPopupOpen = false;
    }

    private void ConfirmRenameEqPreset_OnClick(object? sender, RoutedEventArgs e)
    {
        if (ViewModel.SelectedPreset == default) return;

        var before = ViewModel.SelectedPreset;
        ViewModel.SelectedPreset = null;

        ViewModel.RaisePropertyChanged(nameof(ViewModel.EqPresets));
        ViewModel.RaisePropertyChanged(nameof(ViewModel.SelectedPreset));

        ViewModel.SelectedPreset = before;

        using (var eqStorage = new EqStorage())
        {
            eqStorage.Container.EqPresets = ViewModel.EqPresets;
        }

        ViewModel.IsRenameEqPresetPopupOpen = false;
    }

    private async void RenameEqPreset_OnClick(object? sender, RoutedEventArgs e)
    {
        if (_mainWindow == default) return;

        if (ViewModel.SelectedPreset?.Name is "Flat (Default)" or "Custom")
        {
            await MessageBox.ShowDialogAsync(_mainWindow, "No you can't rename the default and custom preset! Sorry :(");
            return;
        }

        ViewModel.IsRenameEqPresetPopupOpen = !ViewModel.IsRenameEqPresetPopupOpen;
    }
}