namespace V2exSharp.Models;

public class V2Response<T> where T : class
{
    public bool Success { get; set; }
    public string Message { get; set; }
    public T Result { get; set; }
}