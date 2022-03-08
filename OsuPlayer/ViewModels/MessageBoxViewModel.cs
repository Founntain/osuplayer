using Avalonia.Controls;
using ReactiveUI;

namespace OsuPlayer.ViewModels;

public class MessageBoxViewModel : BaseViewModel
{
    private readonly Window _window;
    
    private string _messageBoxTitle;
    private string _messageBoxText;

    public MessageBoxViewModel()
    {
        
    }
    
    public MessageBoxViewModel(Window window, string messageBoxText, string? messageBoxTitle = null)
    {
        _window = window;
        MessageBoxText = messageBoxText;
        MessageBoxTitle = messageBoxTitle ?? "Hey, listen!";
    }
    
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

    public void CloseMessageBox() => _window.Close();
}