using Microsoft.JSInterop;

namespace VaultForce.GridStack.Extensions;

/// <summary>
/// Internal use
/// </summary>
public static class JSLoader
{
    /// <summary>
    /// Loads the main JS file to run blazor apexcharts
    /// </summary>
    /// <param name="jsRuntime"></param>
    /// <param name="path"></param>
    public static async Task<IJSObjectReference> LoadAsync(IJSRuntime jsRuntime, string? path = null)
    {
        var javascriptPath = "./_content/VaultForce.GridStack/gridStackInterop.js";
        if (!string.IsNullOrWhiteSpace(path))
        {
            javascriptPath = path;
        }

        // load Module ftom ES6 script
        IJSObjectReference module = await jsRuntime.InvokeAsync<IJSObjectReference>("import", javascriptPath);
        return module;
    }
}