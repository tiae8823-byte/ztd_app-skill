using System.Collections.Generic;
using ZtdApp.Data;
using ZtdApp.Models;

namespace ZtdApp.Services;

public class IdeaManager
{
    private readonly IdeaRepository _repository;

    public IdeaManager(IdeaRepository repository)
    {
        _repository = repository;
    }

    public Idea Create(string content)
    {
        var idea = new Idea { Content = content };
        _repository.Add(idea);
        return idea;
    }

    public List<Idea> GetAll()
    {
        return _repository.GetAll();
    }

    public void Delete(string id)
    {
        _repository.Delete(id);
    }
}
