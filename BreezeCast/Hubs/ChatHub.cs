using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace BreezeCast.Hubs
{
    public class ChatHub : Hub
    {
        // Thread safe dictionary to store the last message time for each client
        private static readonly ConcurrentDictionary<string, DateTime> _lastMessageTimeByIP = new();

        // Client message cooldown
        private static readonly TimeSpan _messageCooldown = TimeSpan.FromSeconds(5);

        private string GetClientIP()
        {
            // Get the IP address of the client
            return Context.GetHttpContext()?.Connection?.RemoteIpAddress?.ToString() ?? "unknown";
        }

        public override Task OnConnectedAsync()
        {
            _lastMessageTimeByIP[Context.ConnectionId] = DateTime.MinValue;
            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception? exception)
        {
            string ip = GetClientIP();
            _lastMessageTimeByIP.TryRemove(Context.ConnectionId, out _);
            return base.OnDisconnectedAsync(exception);
        }

        public async Task SendMessage(string user, string message)
        {
            var now = DateTime.UtcNow;

            // Ambil waktu terakhir kirim dari client ini
            if (_lastMessageTimeByIP.TryGetValue(Context.ConnectionId, out var lastTime))
            {
                var elapsed = now - lastTime;

                // Kalau belum cukup waktu, kasih notifikasi rate limit
                if (elapsed < _messageCooldown)
                {
                    var remaining = (_messageCooldown - elapsed).TotalSeconds;
                    await Clients.Caller.SendAsync("RateLimitExceeded",
                        $"⏳ Please wait {remaining:F1} seconds before sending another message.");
                    return;
                }
            }

            // Update waktu terakhir kirim pesan
            _lastMessageTimeByIP[Context.ConnectionId] = now;

            // Broadcast pesan ke semua client
            await Clients.All.SendAsync("ReceiveMessage", user, message);
        }
    }
}
