using System;
using System.Collections.Generic;

namespace ExamPaper.Service.Repositories.Interfaces;

public interface IRepository<T>
{
    T Create(T entity);
    T GetById(Guid id);
    IEnumerable<T> GetAll();
    void Update(Guid id);
    void Delete(Guid id);

}    