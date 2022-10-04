namespace V2exSharp.Models;

public class V2Token
{
    public string Token { get; set; }
    public string Scope { get; set; }
    public int Expiration { get; set; }
    public int GoodForDays { get; set; }
    public int TotalUsed { get; set; }
    public int LastUsed { get; set; }
    public int Created { get; set; }
}