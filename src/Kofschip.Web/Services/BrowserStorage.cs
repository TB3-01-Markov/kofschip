using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.JSInterop;

namespace Kofschip.Web.Services;

/// <summary>
/// Reads and writes JSON in the browser's localStorage. Storage can be unavailable
/// (private mode, blocked, connection lost), so every call fails softly.
/// </summary>
public sealed class BrowserStorage(IJSRuntime js)
{
    private static readonly JsonSerializerOptions Options = new()
    {
        Converters = { new JsonStringEnumConverter() },
    };

    public async Task<T?> GetAsync<T>(string key)
    {
        try
        {
            var json = await js.InvokeAsync<string?>("localStorage.getItem", key);
            return string.IsNullOrEmpty(json) ? default : JsonSerializer.Deserialize<T>(json, Options);
        }
        catch (Exception ex) when (IsExpected(ex))
        {
            return default;
        }
    }

    public async Task SetAsync<T>(string key, T value)
    {
        try
        {
            await js.InvokeVoidAsync("localStorage.setItem", key, JsonSerializer.Serialize(value, Options));
        }
        catch (Exception ex) when (IsExpected(ex))
        {
            // Nothing to do: the app keeps working without stored data.
        }
    }

    public async Task RemoveAsync(string key)
    {
        try
        {
            await js.InvokeVoidAsync("localStorage.removeItem", key);
        }
        catch (Exception ex) when (IsExpected(ex))
        {
        }
    }

    private static bool IsExpected(Exception ex) =>
        ex is JSException or JsonException or JSDisconnectedException
            or OperationCanceledException or InvalidOperationException;
}
