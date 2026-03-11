using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ZtdApp.Models;

namespace ZtdApp.ViewModels.Pages;

public partial class TestViewModel : ObservableObject
{
    [ObservableProperty]
    private string _inputContent = string.Empty;

    public ObservableCollection<TestCard> TestCards { get; } = new();

    public TestViewModel()
    {
    }

    [RelayCommand]
    private void AddTestItem()
    {
        if (string.IsNullOrWhiteSpace(InputContent))
            return;

        var card = new TestCard { Content = InputContent };
        TestCards.Insert(0, card);
        InputContent = string.Empty;
    }

    [RelayCommand]
    private void DeleteTestCard(string id)
    {
        var card = TestCards.FirstOrDefault(c => c.Id == id);
        if (card != null)
        {
            TestCards.Remove(card);
        }
    }

    [RelayCommand]
    private void ToggleTestCardExpand(TestCard card)
    {
        if (card == null) return;

        foreach (var c in TestCards)
        {
            if (c != card && c.IsExpanded)
                c.IsExpanded = false;
        }

        card.IsExpanded = !card.IsExpanded;
    }
}
