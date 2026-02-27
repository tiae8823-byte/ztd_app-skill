using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Input;
using ZtdApp.ViewModels;

namespace ZtdApp.Views;

public sealed partial class IdeaPage : Page
{
    public IdeaViewModel ViewModel { get; }

    public IdeaPage(IdeaViewModel viewModel)
    {
        ViewModel = viewModel;
        InitializeComponent();
    }

    private void OnKeyDown(object sender, KeyRoutedEventArgs e)
    {
        if (e.Key == Windows.System.VirtualKey.Enter)
        {
            ViewModel.AddIdeaCommand.Execute(null);
        }
    }
}
