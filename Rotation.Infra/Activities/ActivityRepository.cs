using Microsoft.EntityFrameworkCore;
using Rotation.Application.Features.Activities;
using Rotation.Domain.Activities;
using Rotation.Infra.Persistence;

namespace Rotation.Infra.Activities;

internal class ActivityRepository
    : IActivityRepository
{
    private readonly DatabaseContext _databaseContext;

    public ActivityRepository(DatabaseContext databaseContext)
    {
        _databaseContext = databaseContext;
    }

    public async Task<IActivity> AddAsync(IActivity entity, CancellationToken cancellationToken = default)
    {
        var activity = await _databaseContext.Set<Activity>().AddAsync(entity as Activity, cancellationToken);

        return activity.Entity;
    }

    public async Task<IEnumerable<IActivity>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _databaseContext.Set<Activity>().Include(u => u.Users).ToListAsync(cancellationToken);
    }

    public async Task<IActivity[]> GetToAlertAsync(CancellationToken cancellationToken = default)
    {
        return await _databaseContext.Set<Activity>()
            .Where(w => w.Duration.CurrentEnd() < DateTime.Now)
            .ToArrayAsync(cancellationToken);
    }

    public async Task<IActivity?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _databaseContext.Set<Activity>().Include(u => u.Users).Where(w => w.Id == id)
            .FirstOrDefaultAsync(cancellationToken);
    }
}