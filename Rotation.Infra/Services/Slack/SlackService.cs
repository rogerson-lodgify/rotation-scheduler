using SlackNet;
using SlackNet.WebApi;
using static Rotation.Infra.Services.Slack.SlackServiceModels;

namespace Rotation.Infra.Services.Slack;

public class SlackService
    : ISlackService
{
    private readonly ISlackApiClient _slack;

    public SlackService(ISlackApiClient slack)
    {
        _slack = slack;
    }

    public async Task SendMessageAsync(SlackMessage msg, CancellationToken cancellationToken)
    {
        await _slack.Chat.PostMessage(new Message 
        { 
            Text = msg.Text, 
            Channel = msg.Channel
        }, cancellationToken);
    }
}


