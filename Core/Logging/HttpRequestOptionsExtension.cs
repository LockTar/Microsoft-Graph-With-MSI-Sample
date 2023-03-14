using Microsoft.Graph;

namespace Core.Logging;

public static class HttpRequestOptionsExtension
{
    private const string GraphRequestContextKey = "GraphRequestContext";

    public static GraphRequestContext? GetGraphRequestContext(this HttpRequestOptions options)
    {
        return options.FirstOrDefault(o => o.Key.Equals(GraphRequestContextKey)).Value as GraphRequestContext;

    }
}
