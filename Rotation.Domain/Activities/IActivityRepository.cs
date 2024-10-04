using Rotation.Domain.SeedWork;

namespace Rotation.Domain.Activities;

public interface IActivityRepository
    : IRepository<IActivity>
{
    Task<IActivity[]> GetToAlertAsync(CancellationToken cancellationToken);
}
