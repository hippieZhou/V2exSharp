using System.Text.Json.Serialization;

namespace V2exSharp.Models;

public class V2Reply
{
    public int Id { get; set; }
    public string Content { get; set; }
    [JsonPropertyName("content_rendered")] public string ContentRendered { get; set; }
    public long Created { get; set; }
    public V2Member Member { get; set; }
}