using System.Diagnostics;
using LOVIT.Tracker.Services;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace LOVIT.Tracker.Pages;

[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
[IgnoreAntiforgeryToken]
public class ErrorModel : PageModel
{
    public string? RequestId { get; set; }

    public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);

    private readonly ILogger<ErrorModel> _logger;

    public ErrorModel(ILogger<ErrorModel> logger)
    {
        _logger = logger;
    }

    public async Task OnGet()
    {
        await RecordException();
        RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier;
    }

    public async Task OnPost()
    {
        await RecordException();
        RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier;
    }
    
    private async Task RecordException()
    {
        var exceptionHandlerPathFeature = HttpContext.Features.Get<IExceptionHandlerPathFeature>();
        var error = exceptionHandlerPathFeature?.Error;
        var errorMessage = error?.Message;
        var path = exceptionHandlerPathFeature?.Path;
        var queryString = this.Request.QueryString;
        _logger.LogError(error, errorMessage);
    }
}

