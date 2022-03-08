using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using OsuPlayer.Data;
using OsuPlayer.IO;
using OsuPlayer.UI_Extensions;
using OsuPlayer.ViewModels;
using ReactiveUI;

namespace OsuPlayer.Views;

public partial class HomeView : ReactiveUserControl<HomeViewModel>
{
    public HomeView()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        this.WhenActivated(disposables => 
        {
            HomeViewInitialized();
        });
        AvaloniaXamlLoader.Load(this);
    }
    
    private async void HomeViewInitialized()
    {
        var osuPath = Core.Instance.Config.OsuPath;

        if (string.IsNullOrWhiteSpace(osuPath))
        {
            await MessageBox.ShowDialogAsync(Core.Instance.MainWindow, "You have to select your osu!.db file, before you can start listening to your songs.\nPlease head to the settings to select your osu!.db.");
            
            return;
        }

        //ViewModel!.Songs = new ObservableCollection<SongEntry>(songs);
    }
}