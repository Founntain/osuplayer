using OsuPlayer.Interfaces.Service;
using OsuPlayer.Modules;

namespace OsuPlayer.Windows;

public partial class FluentAppWindow : FluentReactiveWindow<FluentAppWindowViewModel>
{
    private readonly ILoggingService _loggingService;
    private readonly IProfileManagerService _profileManager;

    public FluentAppWindow(FluentAppWindowViewModel viewModel, ILoggingService loggingService)
    {
        InitializeComponent();

        TitleBar.ExtendsContentIntoTitleBar = false;

        ViewModel = viewModel;

        _profileManager = ViewModel.ProfileManager;
        _loggingService = loggingService;

        ViewModel.MainView = ViewModel.HomeView;
    }
}