using Microsoft.AspNetCore.Components;
using GoodHamburguer.Client.Models;

namespace GoodHamburguer.Client.Components;

public partial class ProductCard
{
    [Parameter, EditorRequired]
    public ProductDto Product { get; set; } = null!;

    [Parameter]
    public EventCallback<ProductDto> OnAddToCart { get; set; }

    [Parameter]
    public bool IsLoading { get; set; }

    private async Task HandleAddToCart()
    {
        if (OnAddToCart.HasDelegate)
        {
            await OnAddToCart.InvokeAsync(Product);
        }
    }
}

