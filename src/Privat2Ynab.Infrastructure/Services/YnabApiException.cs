using System.Net;

namespace Privat2Ynab.Infrastructure.Services;

public sealed class YnabApiException : Exception
{
    public YnabApiException() :
        base("An unexpected YNAB API error occurred.")
    {
    }

    public YnabApiException(string message) :
        base(message)
    {
    }

    public YnabApiException(string message, Exception innerException) :
        base(message, innerException)
    {
    }

    public YnabApiException(int statusCode, string responseBody, string message) :
        base(BuildMessage(statusCode, message, responseBody))
    {
        StatusCode = statusCode;
        ResponseBody = responseBody;
    }

    public int StatusCode { get; }

    public string ResponseBody { get; } = string.Empty;

    private static string BuildMessage(int statusCode, string message, string responseBody) =>
        $"{message}. HTTP {(HttpStatusCode)statusCode} ({statusCode}). Response: {responseBody}";
}