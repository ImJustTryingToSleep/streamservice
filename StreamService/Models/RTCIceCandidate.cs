namespace StreamService.Models;

public class RTCIceCandidate
{
    public string Candidate { get; set; }
    public string SdpMid { get; set; }
    public int SdpMLineIndex { get; set; }
}