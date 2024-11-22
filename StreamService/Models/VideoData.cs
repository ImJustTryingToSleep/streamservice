using System.Text.Json.Serialization;

namespace StreamService.Models;

public class VideoData
{
    public int Index { get; }
    public string Part { get; }
        
    [JsonConstructor]
    public VideoData(int index, string part)
    {
        Index = index;
        Part = part;
    }
}