using System.Windows;
using System.Windows.Input;
using ZtdApp.ViewModels;

namespace ZtdApp.Views;

public partial class MainWindow : Window
{
    public MainWindowViewModel ViewModel { get; }

    public MainWindow(MainWindowViewModel viewModel)
    {
        ViewModel = viewModel;
        DataContext = ViewModel;
        InitializeComponent();
    }

    private void OnKeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Enter)
        {
            // 根据 CurrentPage 类型决定执行哪个命令
            if (ViewModel.CurrentPage is ViewModels.Pages.IdeaViewModel ideaVM)
            {
                ideaVM.AddIdeaCommand.Execute(null);
            }
            else if (ViewModel.CurrentPage is ViewModels.Pages.NotesViewModel notesVM)
            {
                notesVM.AddNoteCommand.Execute(null);
            }
            e.Handled = true;
        }
    }
}
