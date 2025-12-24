using Microsoft.AspNetCore.SignalR;

namespace BlazorSignalRApp.Hubs;

public class ChatHub : Hub
{
    private readonly NotifierService _notifierService;

    public ChatHub(NotifierService notifierService)
    {
        _notifierService = notifierService;
    }

    public async Task SendMessage(string user, string message)
    {
        await Clients.All.SendAsync("ReceiveMessage", user, message);
    }

    public override async Task OnConnectedAsync()
    {
        // Subscribe to notifier service events when client connects
        _notifierService.Notify += NotifyHandler;
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        // Unsubscribe from notifier service events when client disconnects
        _notifierService.Notify -= NotifyHandler;
        await base.OnDisconnectedAsync(exception);
    }

    private async Task NotifyHandler(string key, int value)
    {
        // Replay the notifier service events through SignalR
        await Clients.All.SendAsync("ReceiveMessage", "Notifier", $"{key}: {value}");
    }
}
