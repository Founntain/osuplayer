using DynamicData;
using OsuPlayer.Extensions.Bindables;
using OsuPlayer.IO.DbReader.DataModels;

namespace OsuPlayer.IO.Importer;

public interface ISongSource
{
    public Bindable<SourceList<IMapEntryBase>> SongSource { get; }
    public List<IMapEntryBase>? SongSourceList { get; }
}