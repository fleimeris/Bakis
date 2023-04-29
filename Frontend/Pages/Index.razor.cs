using System.ComponentModel.DataAnnotations;
using System.Net.Http.Headers;
using System.Text;
using Frontend.CustomValidators;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.WebAssembly.Http;
using MudBlazor;
using Newtonsoft.Json;

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

    private async Task OnSuccessValidation(EditContext context)
    {
        var httpClient = new HttpClient();
        httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        var request = new HttpRequestMessage(HttpMethod.Post, "http://localhost:5254/api/v1/Scan");
        //request.SetBrowserRequestMode(BrowserRequestMode.NoCors);
        request.Content = new StringContent(JsonConvert.SerializeObject(_request), Encoding.UTF8, "application/json");

        try
        {
            var response = await httpClient.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                _snackbar.Add("Error while sending request to the server.", Severity.Error);
                return;
            }

            var responseBody = await response.Content.ReadAsStringAsync();
            
            Console.WriteLine(responseBody);
        }
        catch (Exception e)
        {
            _snackbar.Add($"Error while sending request to the server: {e}", Severity.Error);
            Console.WriteLine(e);
        }
    }
}