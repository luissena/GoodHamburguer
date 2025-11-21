using Microsoft.AspNetCore.Components;
using GoodHamburguer.Client.Models;
using GoodHamburguer.Client.Services;

namespace GoodHamburguer.Client.Pages;

public partial class Orders
{
    [Inject]
    private IOrderService OrderService { get; set; } = null!;

    private IEnumerable<OrderDto>? OrdersList { get; set; }
    private bool IsLoading { get; set; } = true;
    private string? ErrorMessage { get; set; }

    protected override async Task OnInitializedAsync()
    {
        await LoadOrdersAsync();
    }

    private async Task LoadOrdersAsync()
    {
        try
        {
            IsLoading = true;
            ErrorMessage = null;
            OrdersList = await OrderService.GetOrdersAsync();
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Error loading orders: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }

    private string GetStatusText(string status)
    {
        return status switch
        {
            "Pending" => "Pending",
            "Confirmed" => "Confirmed",
            "Preparing" => "Preparing",
            "Ready" => "Ready",
            "Delivered" => "Delivered",
            "Cancelled" => "Cancelled",
            _ => status
        };
    }

    private string GetStatusBadgeColor(string status)
    {
        return status switch
        {
            "Pending" => "warning",
            "Confirmed" => "info",
            "Preparing" => "primary",
            "Ready" => "success",
            "Delivered" => "success",
            "Cancelled" => "danger",
            _ => "secondary"
        };
    }

    private void ClearError()
    {
        ErrorMessage = null;
    }
}

