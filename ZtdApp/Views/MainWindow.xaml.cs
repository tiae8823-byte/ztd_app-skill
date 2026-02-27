using System.Windows;
using System.Windows.Input;
using ZtdApp.ViewModels;

namespace ZtdApp.Views;

public partial class MainWindow : Window
{
    public IdeaViewModel ViewModel { get; }

    public MainWindow(IdeaViewModel viewModel)
    {
        ViewModel = viewModel;
        DataContext = ViewModel;
        InitializeComponent();

        // 设置输入框焦点
        IdeaInput.Focus();
    }

    private void OnKeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Enter)
        {
            ViewModel.AddIdeaCommand.Execute(null);
            e.Handled = true;
        }
    }
}
