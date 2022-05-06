using System.Threading.Tasks;
using Avalonia.Controls;

namespace OsuPlayer.UI_Extensions;

/// <summary>
/// Own implementation of a Messagebox
/// </summary>
public static class MessageBox
{
    /// <summary>
    /// Opens a new messagebox to display to the user
    /// </summary>
    /// <param name="text">The text inside of the messagebox</param>
    /// <param name="title">The title of the messagebox</param>
    public static void Show(string text, string? title = null)
    {
        var box = new MessageBoxWindow(text, title);

        box.Show();
    }

    /// <summary>
    /// Opens a new messagebox to display to the user
    /// </summary>
    /// <param name="window">The parent window</param>
    /// <param name="text">The text inside of the messagebox</param>
    /// <param name="title">The title of the messagebox</param>
    public static void Show(Window window, string text, string? title = null)
    {
        var box = new MessageBoxWindow(text, title);

        box.Show(window);
    }

    /// <summary>
    /// Opens a new messagebox dialog to display to the user
    /// </summary>
    /// <param name="window">The parent window</param>
    /// <param name="text">The text inside of the messagebox</param>
    /// <param name="title">The title of the messagebox</param>
    public static async Task ShowDialogAsync(Window window, string text, string? title = null)
    {
        var box = new MessageBoxWindow(text, title);

        await box.ShowDialog(window);
    }
}