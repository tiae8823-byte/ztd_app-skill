using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ZtdApp.Models;
using ZtdApp.Services;

namespace ZtdApp.ViewModels.Pages;

public partial class NotesViewModel : ObservableObject
{
    private readonly NoteManager _noteManager;

    [ObservableProperty]
    private string _inputContent = string.Empty;

    public ObservableCollection<Note> Notes { get; } = new();

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
            return;

        _noteManager?.Create(InputContent);
        InputContent = string.Empty;
        LoadNotes();
    }

    [RelayCommand]
    private void DeleteNote(string id)
    {
        _noteManager?.Delete(id);
        LoadNotes();
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
