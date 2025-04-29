using System;

namespace V2exSharp.Exceptions;

public class ServerErrorException(string message) : Exception($"Server Error: {message}");
