using System.Threading.Tasks;
using Avalonia.Controls;

namespace OsuPlayer.UI_Extensions;

public static class MessageBox
{
    public static void Show(string text, string? title = null)
    {
        var box = new MessageBoxWindow(text, title);

        box.Show();
    }

    public static void Show(Window window, string text, string? title = null)
    {
        var box = new MessageBoxWindow(text, title);

        box.Show(window);
    }

    public static async Task ShowDialogAsync(Window window, string text, string? title = null)
    {
        var box = new MessageBoxWindow(text, title);

        await box.ShowDialog(window);
    }
}