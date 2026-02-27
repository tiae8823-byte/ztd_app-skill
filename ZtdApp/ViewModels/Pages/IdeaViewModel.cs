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
    private readonly TaskManager _taskManager;
    private readonly NoteManager _noteManager;

    [ObservableProperty]
    private string _inputContent = string.Empty;

    public ObservableCollection<Idea> Ideas { get; } = new();

    public IdeaViewModel(IdeaManager ideaManager, TaskManager taskManager, NoteManager noteManager)
    {
        _ideaManager = ideaManager;
        _taskManager = taskManager;
        _noteManager = noteManager;
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
    private void ToggleExpand(Idea idea)
    {
        idea.IsExpanded = !idea.IsExpanded;
    }

    [RelayCommand]
    private void DeleteIdea(string id)
    {
        _ideaManager.Delete(id);
        LoadIdeas();
    }

    [RelayCommand]
    private void ConvertToTodo(string id)
    {
        var idea = Ideas.FirstOrDefault(i => i.Id == id);
        if (idea == null) return;

        _taskManager.Create(idea.Content, TodoTaskStatus.Todo);
        _ideaManager.Delete(id);
        LoadIdeas();
    }

    [RelayCommand]
    private void ConvertToNote(string id)
    {
        var idea = Ideas.FirstOrDefault(i => i.Id == id);
        if (idea == null) return;

        _noteManager.Create(idea.Content);
        _ideaManager.Delete(id);
        LoadIdeas();
    }

    [RelayCommand]
    private void QuickComplete(string id)
    {
        var idea = Ideas.FirstOrDefault(i => i.Id == id);
        if (idea == null) return;

        // 快速完成：创建任务并标记为已完成
        _taskManager.Create(idea.Content, TodoTaskStatus.Done);
        _ideaManager.Delete(id);
        LoadIdeas();
    }

    public void LoadIdeas()
    {
        Ideas.Clear();
        foreach (var idea in _ideaManager.GetAll())
        {
            Ideas.Add(idea);
        }
    }
}
