﻿using DataAccess.Models;

namespace Domain.Interfaces
{
    public interface IFileInfoRepository
    {
        public Task<Guid> Create(DbFileInfo fileInfo);
        public Task<DbFileInfo?> Get(Guid id);
        public Task<List<DbFileInfo>> GetAll();
    }
}
