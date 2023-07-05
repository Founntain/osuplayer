using System.Reactive.Disposables;
using Nein.Base;
using Nein.Extensions;
using OsuPlayer.Network.API.ModelExtensions;
using OsuPlayer.Network.API.Service.Endpoints;
using ReactiveUI;
using Splat;

namespace OsuPlayer.Views;

public class BeatmapsViewModel : BaseViewModel
{
    private ObservableCollection<ApiBeatmapModel> _beatmaps = new ();

    public ObservableCollection<ApiBeatmapModel> Beatmaps
    {
        get => _beatmaps;
        set => this.RaiseAndSetIfChanged(ref _beatmaps, value);
    }

    public BeatmapsViewModel()
    {
        Activator = new ViewModelActivator();
        
        this.WhenActivated(Block);
    }

    private async void Block(CompositeDisposable disposables)
    {
        Disposable.Create(() => { }).DisposeWith(disposables);

        var api = Locator.Current.GetService<NorthFox>();

        var beatmaps = await api.GetBeatmapsPaged(pageSize: 100);

        if (beatmaps == null || beatmaps.Count == 0)
        {
            return;
        }
        
        Beatmaps = beatmaps.OrderByDescending(x => x.TimesPlayed).ToObservableCollection();
    }
}