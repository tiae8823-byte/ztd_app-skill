using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ZtdApp.Models;
using ZtdApp.Services;

namespace ZtdApp.ViewModels.Pages;

public partial class IdeaViewModel : ObservableObject
{
    private readonly IdeaManager _ideaManager;

    [ObservableProperty]
    private string _inputContent = string.Empty;

    public ObservableCollection<Idea> Ideas { get; } = new();

    public IdeaViewModel(IdeaManager ideaManager)
    {
        _ideaManager = ideaManager;
        LoadIdeas();
    }

    [RelayCommand]
    private void AddIdea()
    {
        if (string.IsNullOrWhiteSpace(InputContent))
            return;

        _ideaManager.Create(InputContent);
        InputContent = string.Empty;
        LoadIdeas();
    }

    [RelayCommand]
    private void DeleteIdea(string id)
    {
        _ideaManager.Delete(id);
        LoadIdeas();
    }

    private void LoadIdeas()
    {
        Ideas.Clear();
        foreach (var idea in _ideaManager.GetAll())
        {
            Ideas.Add(idea);
        }
    }
}
