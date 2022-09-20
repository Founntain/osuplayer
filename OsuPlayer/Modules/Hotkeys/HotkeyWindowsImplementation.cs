using System.Diagnostics;

namespace OsuPlayer.Modules.Hotkeys;

public class HotkeyWindowsImplementation : IHotkeyImplementation
{
    public List<Hotkey> Hotkeys { get; set; }
    public IntPtr Handle { get; set; }

    [System.Runtime.InteropServices.DllImport("user32.dll")]
    private static extern bool RegisterHotKey(IntPtr hWnd, int id, int fsModifiers, int vk);
    [System.Runtime.InteropServices.DllImport("user32.dll")]
    private static extern bool UnregisterHotKey(IntPtr hWnd, int id);
    
    public void InitializeHotkeys()
    {
        foreach (var hotkey in Hotkeys)
        {
            var result = RegisterHotkey(hotkey);
            
            Debug.WriteLine($"Hotkey {hotkey.Key.ToString()} {(result ? string.Empty : "not ")}successfully registered!");
        }
    }

    public void DeInitializeHotkeys()
    {
        foreach (var hotkey in Hotkeys)
        {
            UnregisterHotkey(hotkey);
        }
    }

    public bool RegisterHotkey(Hotkey hotkey)
    {
        return RegisterHotKey(Handle, hotkey.Id, (int) hotkey.ModifierKey, (int) hotkey.Key);
    }

    public bool UnregisterHotkey(Hotkey hotkey)
    {
        return UnregisterHotKey(Handle, hotkey.Id);
    }
}