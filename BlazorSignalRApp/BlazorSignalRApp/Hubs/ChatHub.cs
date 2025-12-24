using Eddy;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace BlazorSignalRApp.Hubs;

public class ChatHub : Hub
{
    private readonly NotifierService _notifierService;
    private readonly ILogger<ChatHub> _logger;

    public ChatHub(NotifierService notifierService, ILogger<ChatHub> logger)
    {
        _notifierService = notifierService;
        _logger = logger;
    }

    public async Task SendMessage(string user, string message)
    {
        _logger.LogInformation("Sending message from {User}: {Message}", user, message);
        await Clients.All.SendAsync("ReceiveMessage", user, message);
    }

    public override async Task OnConnectedAsync()
    {
        // Log when a new client connects
        _logger.LogInformation("Client connected: {ConnectionId}", Context.ConnectionId);

        // Subscribe to notifier service events when client connects
        _notifierService.Notify += NotifyHandler;
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        // Unsubscribe from notifier service events when client disconnects
        _notifierService.Notify -= NotifyHandler;
        _logger.LogInformation("Client disconnected: {ConnectionId}", Context.ConnectionId);
        await base.OnDisconnectedAsync(exception);
    }

    private async Task NotifyHandler(string key, int value)
    {
        // Replay the notifier service events through SignalR
        _logger.LogInformation("Notifying client of {Key}: {Value}", key, value);
        await Clients.All.SendAsync("ReceiveMessage", "Notifier", $"{key}: {value}");
    }
}
