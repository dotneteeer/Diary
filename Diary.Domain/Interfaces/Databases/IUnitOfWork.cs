using Diary.Domain.Entity;
using Diary.Domain.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore.Storage;

namespace Diary.Domain.Interfaces.Databases;

public interface IUnitOfWork : IStateSaveChanges
{
    Task<IDbContextTransaction> BeginTransactionAsync();

    IBaseRepository<User> Users { get; set; }
    
    IBaseRepository<Role> Roles { get; set; }
    
    IBaseRepository<UserRole> UserRoles { get; set; }
}