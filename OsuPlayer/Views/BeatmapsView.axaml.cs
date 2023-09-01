using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Nein.Base;
using Nein.Extensions;
using OsuPlayer.Api.Data.API.RequestModels.Beatmap;
using OsuPlayer.Api.Data.API.ResponseModels;
using OsuPlayer.Extensions;
using OsuPlayer.Network.API.Service.NorthFox;
using OsuPlayer.Network.API.Service.NorthFox.Endpoints;
using Splat;

namespace OsuPlayer.Views;

public partial class BeatmapsView : ReactiveControl<BeatmapsViewModel>
{
    public BeatmapsView()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }

    private async void SearchBeatmaps_OnClick(object? sender, RoutedEventArgs e)
    {
        if (ViewModel == null) return;

        await ViewModel.SearchBeatmaps();
    }

    private void ToggleFilterMenu_OnClick(object? sender, RoutedEventArgs e)
    {
        if (ViewModel == null) return;
        
        ViewModel.IsFilterMenuOpen = !ViewModel.IsFilterMenuOpen;
    }

    private async void PreviousPage_OnClick(object? sender, RoutedEventArgs e)
    {
        if(ViewModel == null) return;

        var newPage = ViewModel.CurrentPage - 1;

        if (newPage <= 0)
        {
            newPage = ViewModel.TotalPages;
        }

        await ViewModel.SearchBeatmaps(newPage);
    }

    private async void NextPage_OnClick(object? sender, RoutedEventArgs e)
    {
        if(ViewModel == null) return;

        var newPage = ViewModel.CurrentPage + 1;

        if (newPage > ViewModel.TotalPages)
        {
            newPage = 1;
        }

        await ViewModel.SearchBeatmaps(newPage);
    }

    private async void CurrentPage_OnKeyUp(object? sender, KeyEventArgs e)
    {
        if(e.Key != Key.Return || e.Key == Key.Enter || ViewModel == null) return;
        
        if(ViewModel.CurrentPage > ViewModel.TotalPages)
            ViewModel.CurrentPage = ViewModel.TotalPages;

        await ViewModel.SearchBeatmaps(ViewModel.CurrentPage);
    }
}