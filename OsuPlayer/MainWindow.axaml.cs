using Avalonia.Controls;
using Avalonia.Interactivity;
using OsuPlayer.IO;
using OsuPlayer.Network.Online;
using OsuPlayer.UI_Extensions;

namespace OsuPlayer
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private async void Button_OnClick(object? sender, RoutedEventArgs e)
        {
            var x = new SongImporter();
            
            var songs = await x.ImportSongs("Y:\\osu!");
        }
    }
}