using System.Collections.ObjectModel;
using System.Reactive.Disposables;
using OsuPlayer.IO.DbReader;
using ReactiveUI;

namespace OsuPlayer.ViewModels;

public class SearchViewModel : BaseViewModel, IActivatableViewModel
{
    public SearchViewModel()
    {
        Activator = new ViewModelActivator();
        this.WhenActivated(disposables => { Disposable.Create(() => { }).DisposeWith(disposables); });
    }

    private string _filterText;

    public string FilterText
    {
        get => _filterText;
        set => this.RaiseAndSetIfChanged(ref _filterText, value);
    }

    public ViewModelActivator Activator { get; }

    public ReadOnlyObservableCollection<MapEntry> FilteredSongEntries => Core.Instance.Player.FilteredSongEntries!;
}