namespace V2exSharp.Models;

public class UnitInfo
{
    public string Url { get; internal set; } = null!;

    internal static UnitInfo Parse(string html) => new();
}
