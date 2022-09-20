using Avalonia.Input;

namespace OsuPlayer.Modules.Hotkeys;

public interface IHotkeyImplementation
{
    public List<Hotkey> Hotkeys { get; set; }
    
    public IntPtr Handle { get; set; }
    
    public void InitializeHotkeys();
    public void DeInitializeHotkeys();

    public bool RegisterHotkey(Hotkey hotkey);
    public bool UnregisterHotkey(Hotkey hotkey);
}