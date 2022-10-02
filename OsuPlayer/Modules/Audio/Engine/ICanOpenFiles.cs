namespace OsuPlayer.Modules.Audio.Engine;

public interface ICanOpenFiles
{
    /// <summary>
    /// Opens an audio file and creates a stream handle containing the decoded audio stream to play
    /// </summary>
    /// <param name="path">the path to the audio file to be opened</param>
    /// <returns>true if opening succeeded, false else</returns>
    /// <exception cref="ArgumentException">if the sync handle could not be created</exception>
    public bool OpenFile(string path);
}