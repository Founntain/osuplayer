using System.Threading.Tasks;
using Avalonia.Remote.Protocol.Input;

namespace OsuPlayer.Modules.Hotkeys;

public sealed class Hotkey
{
    public int Id { get; }
    public Key Key { get; set; }
    public Action Command { get; }
    public ModifierKey ModifierKey { get; } = ModifierKey.None;

    public Hotkey(int id, Key key, Action command)
    {
        Id = id;
        Key = key;
        Command = command;
    }
    
    public Hotkey(int id, Key key, Action command, ModifierKey modifierKey)
    {
        Id = id;
        Key = key;
        Command = command;
        ModifierKey = modifierKey;
    }

    public void RunCommand()
    {
        Command();
    }
}