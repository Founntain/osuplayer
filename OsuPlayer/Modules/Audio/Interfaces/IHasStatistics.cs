using System.ComponentModel;
using LiveChartsCore.Defaults;

namespace OsuPlayer.Modules.Audio.Interfaces;

public interface IHasStatistics
{
    public BindableList<ObservableValue> GraphValues { get; }

    public event PropertyChangedEventHandler? UserDataChanged;
}