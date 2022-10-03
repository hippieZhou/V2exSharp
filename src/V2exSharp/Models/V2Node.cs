using System.Text.Json.Serialization;

namespace V2exSharp.Models;

public class V2Node
{
    public string Name { get; set; }
    public string Title { get; set; }
    public int Id { get; set; }
    [JsonPropertyName("avatar_large")] public string AvatarLarge { get; set; }
    [JsonPropertyName("avatar_normal")] public string AvatarNormal { get; set; }
    public string Url { get; set; }
    public int Topics { get; set; }
    public string Footer { get; set; }
    public string Header { get; set; }

    [JsonPropertyName("title_alternative")]
    public string TitleAlternative { get; set; }

    [JsonPropertyName("avatar_mini")] public string AvatarMini { get; set; }
    public int Stars { get; set; }
    public string[] Aliases { get; set; }
    public bool Root { get; set; }
    [JsonPropertyName("parent_node_name")] public string ParentNodeName { get; set; }
}