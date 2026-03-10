using System.Collections.ObjectModel;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ZtdApp.Models;
using ZtdApp.Services;
using MessageBox = System.Windows.MessageBox;

namespace ZtdApp.ViewModels.Pages;

public partial class NotesViewModel : ObservableObject
{
    private readonly NoteManager _noteManager;

    [ObservableProperty]
    private string _inputContent = string.Empty;

    [ObservableProperty]
    private string? _selectedCategory;

    // 编辑对话框
    [ObservableProperty]
    private bool _isEditDialogVisible;

    [ObservableProperty]
    private string _editNoteId = string.Empty;

    [ObservableProperty]
    private string _editContent = string.Empty;

    [ObservableProperty]
    private string? _editCategory;

    public ObservableCollection<Note> Notes { get; } = new();

    public List<string> Categories { get; } = new()
    {
        "工作",
        "个人",
        "学习",
        "运动",
        "其他"
    };

    public NotesViewModel(NoteManager noteManager)
    {
        _noteManager = noteManager;
        LoadNotes();
    }

    // 无参构造函数用于设计时
    public NotesViewModel()
    {
        _noteManager = null!;
    }

    [RelayCommand]
    private void AddNote()
    {
        if (string.IsNullOrWhiteSpace(InputContent))
        {
            MessageBox.Show("请输入笔记内容", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        _noteManager.Create(InputContent.Trim(), SelectedCategory);
        InputContent = string.Empty;
        SelectedCategory = null;
        LoadNotes();
    }

    [RelayCommand]
    private void OpenEditDialog(string id)
    {
        var note = Notes.FirstOrDefault(n => n.Id == id);
        if (note == null) return;

        EditNoteId = note.Id;
        EditContent = note.Content;
        EditCategory = note.Category;
        IsEditDialogVisible = true;
    }

    [RelayCommand]
    private void SaveEdit()
    {
        if (string.IsNullOrWhiteSpace(EditContent))
        {
            MessageBox.Show("请输入笔记内容", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        var note = Notes.FirstOrDefault(n => n.Id == EditNoteId);
        if (note != null)
        {
            note.Content = EditContent.Trim();
            note.Category = EditCategory;
            _noteManager.Update(note);
            LoadNotes();
        }

        IsEditDialogVisible = false;
    }

    [RelayCommand]
    private void CancelEdit()
    {
        IsEditDialogVisible = false;
    }

    [RelayCommand]
    private void DeleteNote(string id)
    {
        var result = MessageBox.Show(
            "确定要删除这条笔记吗？",
            "确认删除",
            MessageBoxButton.YesNo,
            MessageBoxImage.Question);

        if (result == MessageBoxResult.Yes)
        {
            _noteManager.Delete(id);
            LoadNotes();
        }
    }

    public void LoadNotes()
    {
        Notes.Clear();
        if (_noteManager == null) return;

        foreach (var note in _noteManager.GetAll())
        {
            Notes.Add(note);
        }
    }
}
