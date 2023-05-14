using System.ComponentModel.DataAnnotations;
using System.Net.Http.Headers;
using System.Text;
using Frontend.CustomValidators;
using Humanizer;
using Humanizer.Localisation;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using MudBlazor;
using MudBlazor.Utilities;
using Newtonsoft.Json;
using RestAPI.Domain.Data.Models;

namespace Frontend.Pages;

public class ScanWebsiteRequest
{
    [Required]
    [UrlValidator("Provided url was not valid")]
    public string? WebsiteUrl { get; set; }
}

public partial class Index
{
    [Inject] private ISnackbar _snackbar { get; set; }
    [Inject] private IJSRuntime _jsRuntime { get; set; }
    
    private ScanWebsiteRequest _request = new();

    private bool _hasData = false;
    private ScanResult? _scanResult;
    private string? _scanResultJson;

    private async Task OnSuccessValidation(EditContext context)
    {
        var httpClient = new HttpClient();
        httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        httpClient.Timeout = TimeSpan.FromHours(1);

        var request = new HttpRequestMessage(HttpMethod.Post, "http://localhost:5254/api/v1/Scan");
        request.Content = new StringContent(JsonConvert.SerializeObject(_request), Encoding.UTF8, "application/json");

        try
        {
            _snackbar.Add("Website is scanning.", Severity.Info);
            
            var response = await httpClient.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                _snackbar.Add("Error while sending request to the server.", Severity.Error);
                return;
            }

            _scanResultJson = await response.Content.ReadAsStringAsync();
            _scanResult = JsonConvert.DeserializeObject<ScanResult>(_scanResultJson);
            _hasData = true;
            await InvokeAsync(StateHasChanged);
        }
        catch (Exception e)
        {
            _snackbar.Add($"Error while sending request to the server: {e}", Severity.Error);
            Console.WriteLine(e);
        }
    }

    private async Task Reset()
    {
        _hasData = false;
        _scanResult = null;
        _scanResultJson = null;
        await InvokeAsync(StateHasChanged);
    }
    
    private async Task DownloadCsv()
    {
        var httpClient = new HttpClient();
        httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        httpClient.Timeout = TimeSpan.FromHours(1);

        var request = new HttpRequestMessage(HttpMethod.Post, "http://localhost:5254/api/v1/Download?what=0");
        request.Content = new StringContent(JsonConvert.SerializeObject(_scanResult), Encoding.UTF8, "application/json");

        try
        {
            var response = await httpClient.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                return;
            }

            var responseBytes = await response.Content.ReadAsByteArrayAsync();

            await _jsRuntime.InvokeVoidAsync("saveAsFile", "report.csv", "text/csv", responseBytes);
        }
        catch (Exception e)
        {
            _snackbar.Add($"Failed to download file: {e}");
        }
    }

    private async Task DownloadJson()
    {
        await _jsRuntime.InvokeVoidAsync("saveAsFile", "report.json", "application/json",
            Encoding.UTF8.GetBytes(_scanResultJson));
    }

    private string HumanizeTimeSpan(Cookie cookie)
    {
        if (cookie.Session || cookie.Expires < 1f)
            return "Session";

        try
        {
            var nowDate = DateTime.Now;
            var parsedDate = DateTimeOffset.FromUnixTimeSeconds((long)cookie.Expires!).DateTime;
            
            Console.WriteLine(parsedDate);

            return nowDate >= parsedDate
                ? "Session"
                : (parsedDate - nowDate).Humanize(maxUnit: TimeUnit.Year, minUnit: TimeUnit.Day);
        }
        catch
        {
            return "Session";
        }
    }

    private string IsCookieGood(Cookie cookie)
    {
        try
        {
            if (!_scanResult!.RulesFound.Any(x => x.Cookie!.Name == cookie.Name))
                return "Valid";
        }
        catch
        {
            return "Valid";
        }

        var auditRule = _scanResult.RulesFound.FirstOrDefault(x => x.Cookie!.Name == cookie.Name);

        return auditRule!.Rule!.OnSuccess!;
    }
    
    private string CellStyleFunc(Cookie cookie)
    {
        var theme = new MudTheme();
        try
        {
            if (!_scanResult!.RulesFound.Any(x => x.Cookie!.Name == cookie.Name))
                return $"background-color: {theme.Palette.Success.ToString(MudColorOutputFormats.Hex)}";
        }
        catch
        {
            return $"background-color: {theme.Palette.Success.ToString(MudColorOutputFormats.Hex)}";
        }
     
        return $"background-color: {theme.Palette.Error.ToString(MudColorOutputFormats.Hex)}";
    }
}