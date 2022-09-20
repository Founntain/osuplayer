using Avalonia;
using Avalonia.Controls;
using Avalonia.Platform;
using Avalonia.VisualTree;
using OsuPlayer.Windows;

namespace OsuPlayer.Modules.Hotkeys;

public class HotkeyInitializer
{
    private MainWindow _mainWindow;

    private IHotkeyImplementation? _hotkeyImplementation;
    
    public HotkeyInitializer(MainWindow mainWindow)
    {
        _mainWindow = mainWindow;

        _hotkeyImplementation = GetHotkeyImplementation();

        if(_hotkeyImplementation != default)
            _hotkeyImplementation.Handle = GetHandle();
    }

    private IntPtr GetHandle()
    {
        var topLevel = (TopLevel) _mainWindow.GetVisualRoot();

        var platImpl = (IWindowImpl) topLevel.PlatformImpl;

        return platImpl.Handle.Handle;
    }

    private IHotkeyImplementation? GetHotkeyImplementation()
    {
        var os = AvaloniaLocator.Current.GetService<IRuntimePlatform>()?.GetRuntimeInfo().OperatingSystem;

        switch (os)
        {
            case OperatingSystemType.WinNT:
                return new HotkeyWindowsImplementation();
            default:
                return default;
        }
    }

    public void SetHotkeys(List<Hotkey> hotkeys)
    {
        if (_hotkeyImplementation == default) 
            return;
        
        _hotkeyImplementation.Hotkeys = hotkeys;
    }
    
    public void RegisterHotkeys()
    {
        _hotkeyImplementation?.InitializeHotkeys();
    }

    public void UnregisterHotkeys()
    {
        _hotkeyImplementation?.DeInitializeHotkeys();
    }
}