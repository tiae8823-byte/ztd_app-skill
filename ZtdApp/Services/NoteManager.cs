using ZtdApp.Data;
using ZtdApp.Models;

namespace ZtdApp.Services;

public class NoteManager
{
    private readonly NoteRepository _repository;

    public NoteManager(NoteRepository repository)
    {
        _repository = repository;
    }

    public Note Create(string content, string? category = null)
    {
        var note = new Note
        {
            Content = content,
            Category = category
        };
        _repository.Add(note);
        return note;
    }

    public List<Note> GetAll()
    {
        return _repository.GetAll();
    }

    public void Delete(string id)
    {
        _repository.Delete(id);
    }
}
