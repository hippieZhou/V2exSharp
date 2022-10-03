using System.Text.Json.Serialization;

namespace V2exSharp.Models;

public class V2Topic
{
    public int Id { get; set; }
    public V2Node Node { get; set; }
    public V2Member Member { get; set; }
    [JsonPropertyName("last_reply_by")] public string LastReplyBy { get; set; }
    [JsonPropertyName("last_touched")] public int LastTouched { get; set; }
    public string Title { get; set; }
    public string Url { get; set; }
    public int Created { get; set; }
    public int Deleted { get; set; }
    public string Content { get; set; }
    [JsonPropertyName("content_rendered")] public string ContentRendered { get; set; }
    [JsonPropertyName("last_modified")] public int LastModified { get; set; }
    public int Replies { get; set; }
}