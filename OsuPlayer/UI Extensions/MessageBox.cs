using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;

namespace OsuPlayer.UI_Extensions;

public static class MessageBox
{
    public static async Task ShowDialogAsync(Window window, string text, string? title = null)
    {
        var box = new MessageBoxWindow(text, title);
        
        await box.ShowDialog(window);
    }
}