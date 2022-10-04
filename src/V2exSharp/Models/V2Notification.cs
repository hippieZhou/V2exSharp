using System.Text.Json.Serialization;

namespace V2exSharp.Models;

public class V2Notification
{
    public int Id { get; set; }
    [JsonPropertyName("member_id")] public int MemberId { get; set; }
    [JsonPropertyName("for_member_id")] public int ForMemberId { get; set; }
    public string Text { get; set; }
    public string Payload { get; set; }
    [JsonPropertyName("payload_rendered")] public string PayloadRendered { get; set; }
    public int Created { get; set; }
    public V2Member Member { get; set; }
}