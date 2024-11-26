using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using StreamService.Models;

namespace StreamService.Hubs;

public class SignalingHub : Hub
{
    // Для подключения нескольких стримеров нужно будет реализовывать функционал "комнат"
    private static string _broadcaster;
    private readonly ILogger<SignalingHub> _logger;

    public SignalingHub(ILogger<SignalingHub> logger)
    {
        _logger = logger;
    }
    
    /// <summary>
    /// Регистрация текущего клиента, как стримера. Затем уведомляет подключенных клиентов, что стример доступен
    /// </summary>
    public async Task RegisterBroadcaster()
    {
        _broadcaster = Context.ConnectionId;
        _logger.LogInformation($"Created broadcaster Id: {_broadcaster}");
        
        await Clients.Others.SendAsync("broadcaster");
    }

    /// <summary>
    /// Регистрация нового зрителя. Уведомляет стримера о подключении нового зрителя
    /// </summary>
    /// <exception cref="InvalidOperationException"></exception>
    public async Task? RegisterWatcher()
    {
        if (_broadcaster == null)
        {
            throw new InvalidOperationException("No broadcaster is currently available.");
        }
        
        _logger.LogInformation($"Created watcher Id: {Context.ConnectionId}");
        
        await Clients.Client(_broadcaster).SendAsync("watcher", Context.ConnectionId);
    }

    /// <summary>
    /// Обрабатывает отправку предложения SDP (Session Description Protocol) от вещателя определенному зрителю
    /// </summary>
    /// <param name="id">зритель</param>
    /// <param name="message">SDP сообщение</param>
    /// <exception cref="ArgumentException"></exception>
    public async Task SendOffer(string id, string message)
    {
        try
        {
            var description = JsonConvert.DeserializeObject<RTCSessionDescription>(message);

            await Clients.Client(id).SendAsync("offer", Context.ConnectionId, description);
        }
        catch (ArgumentException ex)
        {
            _logger.LogError(ex, "Connection ID or message cannot be null or empty");
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occured while sending offer");
            throw;
        }
    }

    /// <summary>
    /// Обрабатывает отправку ответа SDP от наблюдателя обратно вещателю.
    /// </summary>
    /// <param name="id">стример</param>
    /// <param name="message">SDP сообщение</param>
    public async Task SendAnswer(string id, string message)
    {
        try
        {
            //Десериализует ответ из формата JSON и отправляет его стримеру
            var description = JsonConvert.DeserializeObject<RTCSessionDescription>(message);

            await Clients.Client(id).SendAsync("answer", Context.ConnectionId, description);
        }
        catch (ArgumentException ex)
        {
            _logger.LogError(ex, "Connection ID or message cannot be null or empty");
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occured while sending answer");
        }
    }
    /// <summary>
    /// Обрабатывает обмен кандидатами ICE между одноранговыми узлами.
    /// Кандидаты ICE используются для поиска наилучшего пути для потока мультимедиа.
    /// </summary>
    /// <param name="id">клиент</param>
    /// <param name="message">ICE кандидат</param>
    /// <exception cref="ArgumentException"></exception>
    public async Task SendCandidate(string id, string message)
    {
        if (string.IsNullOrWhiteSpace(message))
        {
            throw new ArgumentException("Message cannot be null or empty.", nameof(message));
        }
        var candidate = JsonConvert.DeserializeObject<RTCIceCandidate>(message);
        
        await Clients.Client(id).SendAsync("candidate", Context.ConnectionId, candidate);
    }
    /// <summary>
    /// Отправляет сообщение стримеру о том, что зритель отключился.
    /// </summary>
    /// <param name="exception"></param>
    public override async Task OnDisconnectedAsync(Exception exception)
    {
        await Clients.Client(_broadcaster).SendAsync("disconnectPeer", Context.ConnectionId);
        
        await base.OnDisconnectedAsync(exception);
    }
}