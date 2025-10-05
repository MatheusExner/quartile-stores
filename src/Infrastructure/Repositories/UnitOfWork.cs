using Domain.Interfaces;
using Infrastructure.Context;

namespace Infrastructure.Repositories;

public class UnitOfWork(SqlServerDataContext context) : IUnitOfWork
{
    private readonly SqlServerDataContext _context = context;

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}
