using System.ComponentModel.DataAnnotations;
using System.Net.Http.Headers;
using System.Text;
using Frontend.CustomValidators;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using MudBlazor;
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
    
    private ScanWebsiteRequest _request = new();

    private bool _hasData = false;
    private ScanResult? _scanResult;

    private async Task OnSuccessValidation(EditContext context)
    {
        var httpClient = new HttpClient();
        httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

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

            var responseBody = await response.Content.ReadAsStringAsync();
            _scanResult = JsonConvert.DeserializeObject<ScanResult>(responseBody);
            _hasData = true;
            await InvokeAsync(StateHasChanged);
            
            Console.WriteLine(responseBody);
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
        await InvokeAsync(StateHasChanged);
    }
}