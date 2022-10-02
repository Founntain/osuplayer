namespace OsuPlayer.Modules.Audio.Engine;

public interface ICanOpenFiles
{
    public Bindable<double> ChannelLength { get; }
    public Bindable<double> ChannelPosition { get; }

    public bool OpenFile(string path);
}