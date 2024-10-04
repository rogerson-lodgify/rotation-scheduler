using MediatR;
using Rotation.Domain.Activities;
using Rotation.Domain.SeedWork;
using Rotation.Infra.Services.Slack;

namespace Rotation.API.Activities.Features;

public class MonitorActivitiesJob
    : IHostedService, IDisposable
{
    private readonly CancellationTokenSource cts = new();
    private readonly IServiceScopeFactory _scopeFactory;
    private Task _executingTask;

    public MonitorActivitiesJob(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _executingTask = MonitorAsync(cts.Token);

        return _executingTask.IsCompleted
            ? _executingTask
            : Task.CompletedTask;
    }

    private async Task MonitorAsync(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            using var scope = _scopeFactory.CreateScope();
            var repository = scope.ServiceProvider.GetRequiredService<IActivityRepository>();
            var slack = scope.ServiceProvider.GetRequiredService<ISlackService>();
            var sender = scope.ServiceProvider.GetRequiredService<ISender>();

            var activities = await repository.GetToAlertAsync(cancellationToken);
            if (activities.Length != 0)
            {
                var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
                foreach (var activity in activities)
                {
                    var resume = await sender.Send(new GetNextUserOnRotation.GetActivityQuery(activity.Id),
                        cancellationToken);
                    await slack.SendMessageAsync(new SlackServiceModels.SlackMessage
                    {
                        Text = resume.ToString(),
                        Channel = ""
                    }, cancellationToken);

                    activity.Rotate();
                }

                await unitOfWork.SaveChangesAsync(cancellationToken);
            }

            await Task.Delay(TimeSpan.FromDays(1), cancellationToken);
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public void Dispose()
    {
        throw new NotImplementedException();
    }
}