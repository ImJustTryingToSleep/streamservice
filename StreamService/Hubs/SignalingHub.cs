using Microsoft.AspNetCore.SignalR;
using StreamService.Models;

namespace StreamService.Hubs;

public class SignalingHub : Hub
{
    private readonly IWebHostEnvironment _env;
    public SignalingHub(IWebHostEnvironment env)
    {
        _env = env;
    }

    //private static readonly List<VideoData> _videoDataList = new List<VideoData>();
    
    // Метод для получения данных видео из клиента
    public async Task SendVideoData(VideoData videoData)
    {
        // Сохранение видеоданных в списке
        //_videoDataList.Add(videoData);
        
        // Отправляем данные в группу
        await Clients.Group("videoGroup").SendAsync("video-data", videoData);
    }

    // public async Task SendExistingVideoDataToNewClient()
    // {
    //     foreach (var videoData in _videoDataList)
    //     {
    //         await Clients.Caller.SendAsync("video-data", videoData); // Отправляем старые данные
    //     }
    // }

    public override async Task OnConnectedAsync()
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, "videoGroup"); // Добавляем нового клиента в группу
        //await SendExistingVideoDataToNewClient();
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, "videoGroup"); // Удаляем клиента из группы, если он отключается
        await base.OnDisconnectedAsync(exception);
    }
}