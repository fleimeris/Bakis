using Frontend.Components;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace Frontend.Pages;

public partial class AuditRules
{
    [Inject] private IDialogService _dialogService { get; set; }
    private readonly List<AuditRules> _auditRules = new();
    
    private async Task OnCreateButton()
    {
        var result = await _dialogService.ShowAsync<CreateAuditRule>("Create new audit rule");

        var isSubmitted = await result.Result;

        if ((bool)isSubmitted.Data)
        {
            
        }
    }
}