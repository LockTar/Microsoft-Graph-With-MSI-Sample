namespace Core.Logging;

public class HttpRequestMessageLoggingHandler : DelegatingHandler
{
    private const string NotAvailableMessage = "Not available";

    public HttpRequestMessageLoggingHandler()
    {
    }

    /// <summary>
    /// Sends a HTTP request.
    /// </summary>
    /// <param name="httpRequest">The <see cref="HttpRequestMessage"/> to be sent.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> for the request.</param>
    /// <returns></returns>
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage httpRequest, CancellationToken cancellationToken)
    {
        var graphRequestContextRequest = httpRequest.Options.GetGraphRequestContext();

        // log the request before it goes out.
        Console.WriteLine($"Sending request => method: '{httpRequest.Method.Method}', uri: '{httpRequest.RequestUri?.AbsoluteUri}', requestId: {graphRequestContextRequest?.ClientRequestId ?? NotAvailableMessage}");

        // For privacy and security reasons no content logging
        // if (httpRequest.Content != null)
        //   _logger.LogInformation("Sending request: {HttpRequest}",
        //     await httpRequest.Content.ReadAsStringAsync(cancellationToken));

        // Always call base.SendAsync so that the request is forwarded through the pipeline.
        HttpResponseMessage httpResponse = await base.SendAsync(httpRequest, cancellationToken);

        var graphRequestContextResponse = httpResponse.RequestMessage?.Options.GetGraphRequestContext();

        // log the response as it comes back.
        Console.WriteLine($"Received response => httpStatusCode: {(int)httpResponse.StatusCode}, reason: '{httpResponse.ReasonPhrase}', requestId: {graphRequestContextResponse?.ClientRequestId ?? NotAvailableMessage}");

        // For privacy and security reasons no content logging
        // _logger.LogInformation("Received response: {HttpResponse}", await httpResponse.Content.ReadAsStringAsync(cancellationToken));

        return httpResponse;
    }
}