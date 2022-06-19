﻿namespace Domain
{
    public interface IRepository<T>
    {
        Task<Guid> Create(T model);
        Task<T?> Update(T model);
        Task<bool> Delete(Guid id);
        Task<IEnumerable<T>> GetAll();
        Task<IQueryable<T>> GetAllAsQuaryable();
        Task<T?> Get(Guid id);
    }
}