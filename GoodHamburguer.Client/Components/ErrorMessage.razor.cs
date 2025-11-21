using Microsoft.AspNetCore.Components;

namespace GoodHamburguer.Client.Components;

public partial class ErrorMessage
{
    [Parameter]
    public string? Message { get; set; }

    [Parameter]
    public EventCallback OnDismiss { get; set; }

    private async Task HandleDismiss()
    {
        if (OnDismiss.HasDelegate)
        {
            await OnDismiss.InvokeAsync();
        }
    }
}

