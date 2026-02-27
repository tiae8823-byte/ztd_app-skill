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

    public TodoTask Create(string content, TodoTaskStatus status = TodoTaskStatus.Todo)
    {
        var task = new TodoTask
        {
            Content = content,
            Status = status
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
}
