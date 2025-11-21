using Microsoft.AspNetCore.Components;
using GoodHamburguer.Client.Models;
using GoodHamburguer.Client.Services;

namespace GoodHamburguer.Client.Pages;

public partial class Cart
{
    [Inject]
    private ICartService CartService { get; set; } = null!;

    [Inject]
    private ICartStateService CartStateService { get; set; } = null!;

    [Inject]
    private IOrderService OrderService { get; set; } = null!;

    [Inject]
    private NavigationManager NavigationManager { get; set; } = null!;

    private CartDto? CurrentCart { get; set; }
    private bool IsLoading { get; set; } = true;
    private bool IsUpdating { get; set; }
    private bool IsRemoving { get; set; }
    private bool IsCheckingOut { get; set; }
    private string? ErrorMessage { get; set; }
    private Guid? CartId { get; set; }

    protected override async Task OnInitializedAsync()
    {
        await LoadOrCreateCartAsync();
    }

    private async Task LoadOrCreateCartAsync()
    {
        try
        {
            IsLoading = true;
            ErrorMessage = null;

            CartId = CartStateService.GetCurrentCartId();

            if (CartId == null)
            {
                var cart = await CartService.CreateCartAsync();
                CartId = cart.Id;
                CartStateService.SetCurrentCartId(cart.Id);
            }

            CurrentCart = await CartService.GetCartByIdAsync(CartId.Value);
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Error loading cart: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }

    private async Task UpdateQuantity(Guid productId, int newQuantity)
    {
        if (CartId == null || newQuantity < 1)
        {
            return;
        }

        try
        {
            IsUpdating = true;
            ErrorMessage = null;
            CurrentCart = await CartService.UpdateItemQuantityAsync(CartId.Value, productId, newQuantity);
        }
        catch (HttpRequestException ex)
        {
            ErrorMessage = ex.Message;
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Error updating quantity: {ex.Message}";
        }
        finally
        {
            IsUpdating = false;
        }
    }

    private async Task RemoveItem(Guid productId)
    {
        if (CartId == null)
        {
            return;
        }

        try
        {
            IsRemoving = true;
            ErrorMessage = null;
            CurrentCart = await CartService.RemoveItemFromCartAsync(CartId.Value, productId);
        }
        catch (HttpRequestException ex)
        {
            ErrorMessage = ex.Message;
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Error removing item: {ex.Message}";
        }
        finally
        {
            IsRemoving = false;
        }
    }

    private async Task HandleCheckout()
    {
        if (CartId == null || CurrentCart == null || !CurrentCart.Items.Any())
        {
            ErrorMessage = "Empty cart. Add products before checkout.";
            return;
        }

        try
        {
            IsCheckingOut = true;
            ErrorMessage = null;
            var order = await OrderService.CreateOrderAsync(CartId.Value);
            
            CartStateService.ClearCart();
            
            NavigationManager.NavigateTo("/orders");
        }
        catch (HttpRequestException ex)
        {
            ErrorMessage = ex.Message;
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Error completing order: {ex.Message}";
        }
        finally
        {
            IsCheckingOut = false;
        }
    }

    private void ClearError()
    {
        ErrorMessage = null;
    }
}
