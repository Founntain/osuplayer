using Avalonia.Controls;
using OsuPlayer.Base.ViewModels;
using ReactiveUI;

namespace OsuPlayer.UI_Extensions;

public class MessageBoxViewModel : BaseWindowViewModel
{
    private readonly Window _window;
    private string _messageBoxText;

    private string _messageBoxTitle;

    public string MessageBoxTitle
    {
        get => _messageBoxTitle;
        set => this.RaiseAndSetIfChanged(ref _messageBoxTitle, value);
    }

    public string MessageBoxText
    {
        get => _messageBoxText;
        set => this.RaiseAndSetIfChanged(ref _messageBoxText, value);
    }

    public MessageBoxViewModel()
    {
    }

    public MessageBoxViewModel(Window window, string messageBoxText, string? messageBoxTitle = null)
    {
        _window = window;
        MessageBoxText = messageBoxText;
        MessageBoxTitle = messageBoxTitle ?? "Hey, listen!";
    }

    public void CloseMessageBox()
    {
        _window.Close();
    }
}