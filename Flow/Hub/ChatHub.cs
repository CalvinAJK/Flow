using Microsoft.AspNetCore.SignalR;

/*namespace Flow;
*/
public class ChatHub : Hub
{
    public override async Task OnConnectedAsync()
    {
        await Clients.All.SendAsync("ReceiveMessage", $"{Context.ConnectionId} has joined");
    }
    /*public async Task SendMessage(string user, string message)
    {
        await Clients.All.SendAsync("ReceiveMessage", user, message);
    }*/
    public async Task SendMessageToOrganization(string user, string message, int organizationId)
    {
        await Clients.Group($"Organization_{organizationId}").SendAsync("ReceiveMessage", user, message);
        /*await Clients.All.SendAsync("ReceiveMessage", user, message);*/
    }

    public async Task SendMessageToTeam(string user, string message, int teamId)
    {
        await Clients.Group($"Team_{teamId}").SendAsync("ReceiveMessage", user, message);
    }

    public async Task JoinOrganizationGroup(int organizationId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, $"Organization_{organizationId}");
    }

    public async Task JoinTeam(int teamId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, $"Team_{teamId}");
    }
}
