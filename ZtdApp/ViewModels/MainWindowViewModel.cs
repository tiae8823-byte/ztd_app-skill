using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ZtdApp.ViewModels.Pages;

namespace ZtdApp.ViewModels;

public partial class MainWindowViewModel : ObservableObject
{
    [ObservableProperty]
    private object? _currentPage;

    [ObservableProperty]
    private string _currentTitle = "想法收集";

    public IdeaViewModel IdeaViewModel { get; }
    public TodoViewModel TodoViewModel { get; }
    public TodayViewModel TodayViewModel { get; }
    public NotesViewModel NotesViewModel { get; }
    public WeeklyReviewViewModel WeeklyReviewViewModel { get; }

    public MainWindowViewModel(
        IdeaViewModel ideaViewModel,
        TodoViewModel todoViewModel,
        TodayViewModel todayViewModel,
        NotesViewModel notesViewModel,
        WeeklyReviewViewModel weeklyReviewViewModel)
    {
        IdeaViewModel = ideaViewModel;
        TodoViewModel = todoViewModel;
        TodayViewModel = todayViewModel;
        NotesViewModel = notesViewModel;
        WeeklyReviewViewModel = weeklyReviewViewModel;

        // 默认显示想法收集页面
        CurrentPage = IdeaViewModel;
    }

    [RelayCommand]
    private void NavigateToIdeas()
    {
        IdeaViewModel.LoadIdeas();
        CurrentPage = IdeaViewModel;
        CurrentTitle = "想法收集";
    }

    [RelayCommand]
    private void NavigateToTodos()
    {
        TodoViewModel.LoadTodos();
        CurrentPage = TodoViewModel;
        CurrentTitle = "待办列表";
    }

    [RelayCommand]
    private void NavigateToToday()
    {
        TodayViewModel.LoadTasks();
        CurrentPage = TodayViewModel;
        CurrentTitle = "今日待办";
    }

    [RelayCommand]
    private void NavigateToNotes()
    {
        NotesViewModel.LoadNotes();
        CurrentPage = NotesViewModel;
        CurrentTitle = "笔记库";
    }

    [RelayCommand]
    private void NavigateToReview()
    {
        WeeklyReviewViewModel.RefreshData();
        CurrentPage = WeeklyReviewViewModel;
        CurrentTitle = "每周回顾";
    }
}
