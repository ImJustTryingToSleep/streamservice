using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using StreamService.Models;

namespace StreamService.Hubs;

public class SignalingHub : Hub
{
    private static string _broadcaster;

    public async Task RegisterBroadcaster()
    {
        _broadcaster = Context.ConnectionId;
        Console.WriteLine($"SignalingHub.RegisterBroadcaster called {_broadcaster}");
        
        await Clients.Others.SendAsync("broadcaster");
    }

    public async Task? RegisterWatcher()
    {
        if (_broadcaster == null)
        {
            throw new InvalidOperationException("No broadcaster is currently available.");
        }
        
        Console.WriteLine($"SignalingHub.RegisterWatcher called {Context.ConnectionId}");
        await Clients.Client(_broadcaster).SendAsync("watcher", Context.ConnectionId);
    }

    public async Task SendOffer(string id, string message)
    {
        //Console.WriteLine($"SendOffer called with ID: {id}, message: {message}");
        
        if (string.IsNullOrEmpty(id))
        {
            throw new ArgumentException("Connection ID cannot be null or empty", nameof(id));
        }
        
        var description = JsonConvert.DeserializeObject<RTCSessionDescription>(message);
        await Clients.Client(id).SendAsync("offer", Context.ConnectionId, description);
    }

    public async Task SendAnswer(string id, string message)
    {
        Console.WriteLine($"SendAnswer called with ID: {id}, message: {message}");
        var description = JsonConvert.DeserializeObject<RTCSessionDescription>(message);
        await Clients.Client(id).SendAsync("answer", Context.ConnectionId, description);
    }

    public async Task SendCandidate(string id, string message)
    {
        //Console.WriteLine($"SendCandidate called with ID: {id}, message: {message}");
        //Console.WriteLine($"No client found with ID: {id}");
        
        if (string.IsNullOrWhiteSpace(message))
        {
            throw new ArgumentException("Message cannot be null or empty.", nameof(message));
        }
        var candidate = JsonConvert.DeserializeObject<RTCIceCandidate>(message);
        
        await Clients.Client(id).SendAsync("candidate", Context.ConnectionId, candidate);
    }

    public override async Task OnDisconnectedAsync(Exception exception)
    {
        await Clients.Client(_broadcaster).SendAsync("disconnectPeer", Context.ConnectionId);
        await base.OnDisconnectedAsync(exception);
    }
    
    public class RTCSessionDescription
    {
        public string Type { get; set; }
        public string Sdp { get; set; }
    }
    
    public class RTCIceCandidate
    {
        public string Candidate { get; set; }
        public string SdpMid { get; set; }
        public int SdpMLineIndex { get; set; }
    }
}