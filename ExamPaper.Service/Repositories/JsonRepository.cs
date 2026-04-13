using System;
using System.Collections.Generic;

using ExamPaper.Service.Repositories.Interfaces;

namespace ExamPaper.Service.Repositories;

public class JsonRepository<T> : IRepository<T> where T: class
{
    public T Create(T entity)
    {
        throw new System.NotImplementedException();
    }

    public T GetById(Guid id)
    {
        throw new System.NotImplementedException();
    }

    public IEnumerable<T> GetAll()
    {
        throw new System.NotImplementedException();
    }

    public void Update(Guid id)
    {
        throw new System.NotImplementedException();
    }

    public void Delete(Guid id)
    {
        throw new System.NotImplementedException();
    }
}