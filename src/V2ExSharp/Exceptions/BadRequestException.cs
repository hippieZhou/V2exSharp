using System;

namespace V2exSharp.Exceptions;

public class BadRequestException(string message) : Exception($"Bad Request: {message}");