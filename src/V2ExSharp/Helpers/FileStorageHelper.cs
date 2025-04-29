using System;
using System.IO;
using System.Text.Json;
using Microsoft.Extensions.Logging;

namespace V2exSharp.Helpers;

public static class FileStorageHelper
{
    public static T? Get<T>(string filePath, T? defaultValue, ILogger logger)
    {
        if (!File.Exists(filePath))
        {
            return defaultValue;
        }

        try
        {
            var result = File.ReadAllText(filePath);
            return string.IsNullOrEmpty(result) ? defaultValue : JsonSerializer.Deserialize<T>(result);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, ex.Message);
            return defaultValue;
        }
    }

    public static void Set<T>(string filePath, T value, ILogger logger)
    {
        var json = JsonSerializer.Serialize(value, new JsonSerializerOptions { WriteIndented = true });

        try
        {
            using var writer = File.CreateText(filePath);
            writer.WriteLine(json);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, ex.Message);
        }
    }
}