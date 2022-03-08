using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using OsuPlayer.IO;
using OsuPlayer.UI_Extensions;
using OsuPlayer.ViewModels;

namespace OsuPlayer.Views;

public partial class HomeView : UserControl
{
    public HomeView()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
    
    private async void HomeView_OnInitialized(object? sender, EventArgs e)
    {
        //TODO: We need an event that runs when activated and MainWindow is available for the messagebox to work.
        if (Core.MainWindow == default) return;
        
        var osuPath = Core.Config.OsuPath;

        if (string.IsNullOrWhiteSpace(osuPath))
        {
            await MessageBox.ShowDialogAsync(Core.MainWindow, "You have to select your osu!.db file, before you can start listening to your songs.\nPlease head to the settings to select your osu!.db.");
            
            return;
        }
        
        var viewModel = (HomeViewModel) DataContext!;

        var songs = await new SongImporter().ImportSongs(osuPath);

        viewModel.Songs = new ObservableCollection<SongEntry>(songs);
    }
}