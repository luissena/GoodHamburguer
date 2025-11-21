using Microsoft.AspNetCore.Components;
using GoodHamburguer.Client.Models;
using GoodHamburguer.Client.Services;

namespace GoodHamburguer.Client.Pages;

public partial class Sandwiches
{
    [Inject]
    private IProductService ProductService { get; set; } = null!;

    [Inject]
    private ICategoryService CategoryService { get; set; } = null!;

    [Inject]
    private ICartService CartService { get; set; } = null!;

    [Inject]
    private ICartStateService CartStateService { get; set; } = null!;

    [Inject]
    private IOrderQuantityRuleService OrderQuantityRuleService { get; set; } = null!;

    [Inject]
    private NavigationManager NavigationManager { get; set; } = null!;

    private PagedResultDto<ProductDto>? ProductsList { get; set; }
    private bool IsLoading { get; set; } = true;
    private bool IsAddingToCart { get; set; }
    private string? ErrorMessage { get; set; }
    private Guid? SandwichesCategoryId { get; set; }
    private int CurrentPage { get; set; } = 1;
    private const int PageSize = 12;

    protected override async Task OnInitializedAsync()
    {
        var categories = await CategoryService.GetCategoriesAsync();
        var sandwichesCategory = categories.FirstOrDefault(c => c.Name.Equals("Sandwiches", StringComparison.OrdinalIgnoreCase));
        if (sandwichesCategory != null)
        {
            SandwichesCategoryId = sandwichesCategory.Id;
        }
        
        await LoadSandwichesAsync();
    }

    private async Task LoadSandwichesAsync()
    {
        try
        {
            IsLoading = true;
            ErrorMessage = null;

            if (SandwichesCategoryId == null)
            {
                ErrorMessage = "Sandwiches category not found.";
                ProductsList = new PagedResultDto<ProductDto>(Enumerable.Empty<ProductDto>(), 1, PageSize, 0, 0);
                return;
            }

            ProductsList = await ProductService.GetProductsByCategoryAsync(SandwichesCategoryId.Value, CurrentPage, PageSize);
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Error loading sandwiches: {ex.Message}";
            ProductsList = new PagedResultDto<ProductDto>(Enumerable.Empty<ProductDto>(), 1, PageSize, 0, 0);
        }
        finally
        {
            IsLoading = false;
        }
    }

    private async Task ChangePage(int page)
    {
        if (page < 1 || (ProductsList != null && page > ProductsList.TotalPages))
        {
            return;
        }

        CurrentPage = page;
        await LoadSandwichesAsync();
    }

    private async Task HandleAddToCart(ProductDto product)
    {
        try
        {
            IsAddingToCart = true;
            ErrorMessage = null;

            var cartId = CartStateService.GetCurrentCartId();
            
            if (cartId == null)
            {
                var cart = await CartService.CreateCartAsync();
                cartId = cart.Id;
                CartStateService.SetCurrentCartId(cartId.Value);
            }

            var currentCart = await CartService.GetCartByIdAsync(cartId.Value);
            if (currentCart != null)
            {
                var validationError = await ValidateProductQuantityRulesAsync(product, currentCart);
                if (validationError != null)
                {
                    ErrorMessage = validationError;
                    return;
                }
            }

            await CartService.AddItemToCartAsync(cartId.Value, product.Id, 1);
            NavigationManager.NavigateTo("/cart");
        }
        catch (HttpRequestException ex)
        {
            ErrorMessage = ex.Message;
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Error adding product to cart: {ex.Message}";
        }
        finally
        {
            IsAddingToCart = false;
        }
    }

    private async Task<string?> ValidateProductQuantityRulesAsync(ProductDto product, CartDto cart)
    {
        try
        {
            var rules = await OrderQuantityRuleService.GetRulesAsync();
            
            var cartProductIds = cart.Items.Select(i => i.ProductId).Distinct().ToList();
            var cartProducts = new Dictionary<Guid, ProductDto>();
            
            foreach (var productId in cartProductIds)
            {
                var cartProduct = await ProductService.GetProductByIdAsync(productId);
                if (cartProduct != null)
                {
                    cartProducts[productId] = cartProduct;
                }
            }
            
            foreach (var rule in rules)
            {
                bool ruleApplies = false;
                
                if (rule.ProductId.HasValue && rule.ProductId.Value == product.Id)
                {
                    ruleApplies = true;
                }
                else if (rule.CategoryId.HasValue && rule.CategoryId.Value == product.Category.Id)
                {
                    ruleApplies = true;
                }

                if (ruleApplies)
                {
                    int totalQuantity = 0;
                    
                    foreach (var item in cart.Items)
                    {
                        if (rule.ProductId.HasValue)
                        {
                            if (item.ProductId == rule.ProductId.Value)
                            {
                                totalQuantity += item.Quantity;
                            }
                        }
                        else if (rule.CategoryId.HasValue)
                        {
                            if (cartProducts.TryGetValue(item.ProductId, out var cartProduct))
                            {
                                if (cartProduct.Category.Id == rule.CategoryId.Value)
                                {
                                    totalQuantity += item.Quantity;
                                }
                            }
                        }
                    }

                    totalQuantity += 1;

                    if (totalQuantity > rule.MaxQuantity)
                    {
                        return $"You already have {rule.RuleName} in your cart. Only {rule.MaxQuantity} per order is allowed.";
                    }
                }
            }
        }
        catch
        {
        }

        return null;
    }

    private void ClearError()
    {
        ErrorMessage = null;
    }
}

