using ZtdApp.Data;
using ZtdApp.Models;

namespace ZtdApp.Services;

public class TaskManager
{
    private readonly TaskRepository _repository;

    public TaskManager(TaskRepository repository)
    {
        _repository = repository;
    }

    public TodoTask Create(string content, TodoTaskStatus status = TodoTaskStatus.Todo, string? timeTag = null, string? categoryTag = null)
    {
        var task = new TodoTask
        {
            Content = content,
            Status = status,
            TimeTag = timeTag,
            CategoryTag = categoryTag
        };
        _repository.Add(task);
        return task;
    }

    public List<TodoTask> GetByStatus(TodoTaskStatus status)
    {
        return _repository.GetByStatus(status);
    }

    public void Delete(string id)
    {
        _repository.Delete(id);
    }

    public void MoveToStatus(string id, TodoTaskStatus status)
    {
        _repository.UpdateStatus(id, status);
    }

    public void Complete(string id)
    {
        _repository.Complete(id);
    }

    public List<TodoTask> GetThisWeekCompleted()
    {
        return _repository.GetThisWeekCompleted();
    }
}
