using System.Collections.Generic;
using System.Diagnostics;

namespace V2exSharp.Models;

// https://v2ex.com/api/nodes/s2.json

[DebuggerDisplay("{DebuggerDisplay}")]
public class NodesInfo : List<NodesInfo.Node>
{
    private string DebuggerDisplay => $"Count = {this.Count}";

    [DebuggerDisplay("{Text}")]
    public class Node
    {
        public string? Text { get; set; } = null!;
        public int Topics { get; set; }
        public string? Id { get; set; }
        public string? Name { get; set; }
        public string? Title { get; set; }
        public string[]? Aliases { get; set; }
    }
}
