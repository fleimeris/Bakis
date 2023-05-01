using System.Net.Http.Headers;
using Frontend.Components;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using Newtonsoft.Json;

namespace Frontend.Pages;

public partial class AuditRules
{
    [Inject] private IDialogService _dialogService { get; set; }
    [Inject] private ISnackbar _snackbar { get; set; }
    private List<RestAPI.Domain.Data.Models.AuditRule> _auditRules = new();

    protected override async Task OnInitializedAsync()
    {
        await RefreshRules();
    }

    private async Task OnCreateButton()
    {
        var result = await _dialogService.ShowAsync<CreateAuditRule>("Create new audit rule");

        var isSubmitted = await result.Result;

        if ((bool)isSubmitted.Data)
        {
            await RefreshRules();
        }
    }

    private async Task RefreshRules()
    {
        var httpClient = new HttpClient();
        httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        httpClient.Timeout = TimeSpan.FromHours(1);

        var request = new HttpRequestMessage(HttpMethod.Get, "http://localhost:5254/api/v1/AuditRule");

        try
        {
            var result = await httpClient.SendAsync(request);

            if (!result.IsSuccessStatusCode)
            {
                _snackbar.Add("Failed to receive all audit rules", Severity.Error);
                return;
            }

            var responseString = await result.Content.ReadAsStringAsync();
            
            _auditRules.Clear();
            _auditRules = JsonConvert.DeserializeObject<List<RestAPI.Domain.Data.Models.AuditRule>>(responseString);
            await InvokeAsync(StateHasChanged);
        }
        catch (Exception e)
        {
            _snackbar.Add($"Failed to receive all audit rules: {e}", Severity.Error);
        }
    }
}