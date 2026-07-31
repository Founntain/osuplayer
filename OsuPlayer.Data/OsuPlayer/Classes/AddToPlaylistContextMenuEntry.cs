using System.Windows.Input;

namespace OsuPlayer.Data.OsuPlayer.Classes;

public class AddToPlaylistContextMenuEntry
{
    public string Name { get; set; }
    public ICommand Action { get; set; }

    public AddToPlaylistContextMenuEntry(string name, Action<string> action)
    {
        Name = name;
        Action = new RelayCommand(action);
    }

    private class RelayCommand : ICommand
    {
        private readonly Action<string> _execute;

        public RelayCommand(Action<string> execute)
        {
            _execute = execute;
        }

        public bool CanExecute(object? parameter) => true;

        public void Execute(object? parameter)
        {
            _execute?.Invoke(parameter?.ToString() ?? string.Empty);
        }

        public event EventHandler? CanExecuteChanged;
    }
}