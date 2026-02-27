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
        CurrentPage = IdeaViewModel;
        CurrentTitle = "想法收集";
    }

    [RelayCommand]
    private void NavigateToTodos()
    {
        CurrentPage = TodoViewModel;
        CurrentTitle = "待办列表";
    }

    [RelayCommand]
    private void NavigateToToday()
    {
        CurrentPage = TodayViewModel;
        CurrentTitle = "今日待办";
    }

    [RelayCommand]
    private void NavigateToNotes()
    {
        CurrentPage = NotesViewModel;
        CurrentTitle = "笔记库";
    }

    [RelayCommand]
    private void NavigateToReview()
    {
        CurrentPage = WeeklyReviewViewModel;
        CurrentTitle = "每周回顾";
    }
}
