namespace OsuPlayer.Modules.Audio.Interfaces;

/// <summary>
/// This interface provides method(s) regarding opening and closing audio files.
/// </summary>
public interface ICanOpenFiles
{
    /// <summary>
    /// Opens an audio file and creates a stream handle containing the decoded audio stream to play
    /// </summary>
    /// <param name="path">the path to the audio file to be opened</param>
    /// <returns>true if opening succeeded, false else</returns>
    /// <exception cref="ArgumentException">if the sync handle could not be created</exception>
    public bool OpenFile(string path);

    /// <summary>
    /// Closes the current audio file stream handle
    /// </summary>
    public void CloseFile();
}