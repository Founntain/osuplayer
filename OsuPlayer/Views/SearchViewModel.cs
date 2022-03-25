using System.Collections.ObjectModel;
using System.Reactive.Disposables;
using OsuPlayer.IO.DbReader;
using OsuPlayer.IO.DbReader.DataModels;
using OsuPlayer.ViewModels;
using ReactiveUI;

namespace OsuPlayer.Views;

public class SearchViewModel : BaseViewModel
{
    private string _filterText;

    public SearchViewModel()
    {
        Activator = new ViewModelActivator();
        this.WhenActivated(disposables => { Disposable.Create(() => { }).DisposeWith(disposables); });
    }

    public string FilterText
    {
        get => _filterText;
        set => this.RaiseAndSetIfChanged(ref _filterText, value);
    }

    public ReadOnlyObservableCollection<MinimalMapEntry> FilteredSongEntries => Core.Instance.Player.FilteredSongEntries!;
}