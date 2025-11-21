using GoodHamburguer.Api.DTOs;
using GoodHamburguer.Domain.Entities;
using GoodHamburguer.Domain.Exceptions;
using GoodHamburguer.Domain.Repositories;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace GoodHamburguer.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[SwaggerTag("Operations for shopping cart management")]
public class CartsController : ControllerBase
{
    private readonly ICartRepository _cartRepository;
    private readonly IProductRepository _productRepository;
    private readonly IDiscountRepository _discountRepository;
    private readonly IOrderQuantityRuleRepository _quantityRuleRepository;
    private readonly ILogger<CartsController> _logger;

    public CartsController(
        ICartRepository cartRepository,
        IProductRepository productRepository,
        IDiscountRepository discountRepository,
        IOrderQuantityRuleRepository quantityRuleRepository,
        ILogger<CartsController> logger)
    {
        _cartRepository = cartRepository;
        _productRepository = productRepository;
        _discountRepository = discountRepository;
        _quantityRuleRepository = quantityRuleRepository;
        _logger = logger;
    }

    [HttpPost]
    [SwaggerOperation(
        Summary = "Create a new cart",
        Description = "Creates a new empty shopping cart. Use this cart to add products later."
    )]
    [SwaggerResponse(201, "Cart created successfully", typeof(CartDto))]
    public async Task<ActionResult<CartDto>> Create(CancellationToken cancellationToken)
    {
        var cart = Cart.Create();
        await _cartRepository.CreateAsync(cart, cancellationToken);

        var cartDto = await MapToDtoAsync(cart, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = cart.Id }, cartDto);
    }

    [HttpGet("{id}")]
    [SwaggerOperation(
        Summary = "Get cart by ID",
        Description = "Returns the complete cart details, including items, subtotal, applied discounts and final total."
    )]
    [SwaggerResponse(200, "Cart found", typeof(CartDto))]
    [SwaggerResponse(404, "Cart not found")]
    public async Task<ActionResult<CartDto>> GetById(
        [SwaggerParameter("Unique cart ID", Required = true)] Guid id, 
        CancellationToken cancellationToken)
    {
        var cart = await _cartRepository.GetByIdAsync(id, cancellationToken);

        if (cart == null)
        {
            return NotFound(new { error = "Cart not found." });
        }

        var cartDto = await MapToDtoAsync(cart, cancellationToken);
        return Ok(cartDto);
    }

    [HttpPost("{id}/items")]
    [SwaggerOperation(
        Summary = "Add item to cart",
        Description = "Adds a product to the cart. If the product already exists in the cart, the quantity will be incremented."
    )]
    [SwaggerResponse(200, "Item added successfully", typeof(CartDto))]
    [SwaggerResponse(400, "Validation error, product not found or domain rule violation")]
    [SwaggerResponse(404, "Cart not found")]
    public async Task<ActionResult<CartDto>> AddItem(
        [SwaggerParameter("Unique cart ID", Required = true)] Guid id, 
        [FromBody] AddItemToCartDto dto, 
        CancellationToken cancellationToken)
    {
        try
        {
            var cart = await _cartRepository.GetByIdAsync(id, cancellationToken);

            if (cart == null)
            {
                return NotFound(new { error = "Cart not found." });
            }

            var product = await _productRepository.GetByIdAsync(dto.ProductId, cancellationToken);

            if (product == null)
            {
                return BadRequest(new { error = "Product not found." });
            }

            var quantityRules = await _quantityRuleRepository.GetAllAsync(cancellationToken);
            if (quantityRules.Any())
            {
                var ruleValueObjects = quantityRules.Select(r => r.ToValueObject()).ToList();
                var validator = new GoodHamburguer.Domain.OrderQuantityValidator(ruleValueObjects);
                
                var tempCart = Cart.Create(cart.Id);
                foreach (var item in cart.Items)
                {
                    var itemProduct = await _productRepository.GetByIdAsync(item.ProductId, cancellationToken);
                    if (itemProduct != null)
                    {
                        tempCart.AddItem(itemProduct, item.Quantity);
                    }
                }
                
                tempCart.AddItem(product, dto.Quantity);
                
                var allProductIds = tempCart.Items.Select(i => i.ProductId).Distinct().ToList();
                var productsDict = new Dictionary<Guid, Product?>();
                foreach (var pid in allProductIds)
                {
                    productsDict[pid] = await _productRepository.GetByIdAsync(pid, cancellationToken);
                }
                
                Product? GetProductByIdFull(Guid id) => productsDict.TryGetValue(id, out var p) ? p : null;
                
                tempCart.ValidateQuantityRules(validator, GetProductByIdFull);
            }

            cart.AddItem(product, dto.Quantity);
            await _cartRepository.UpdateAsync(cart, cancellationToken);

            var cartDto = await MapToDtoAsync(cart, cancellationToken);
            return Ok(cartDto);
        }
        catch (DomainException ex)
        {
            _logger.LogWarning(ex, "Domain error while adding item to cart");
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpPut("{id}/items/{productId}")]
    [SwaggerOperation(
        Summary = "Update item quantity in cart",
        Description = "Updates the quantity of a specific product in the cart."
    )]
    [SwaggerResponse(200, "Quantity updated successfully", typeof(CartDto))]
    [SwaggerResponse(400, "Validation error or domain rule violation")]
    [SwaggerResponse(404, "Cart or item not found")]
    public async Task<ActionResult<CartDto>> UpdateItemQuantity(
        [SwaggerParameter("Unique cart ID", Required = true)] Guid id,
        [SwaggerParameter("Unique product ID", Required = true)] Guid productId,
        [FromBody] UpdateItemQuantityDto dto,
        CancellationToken cancellationToken)
    {
        try
        {
            var cart = await _cartRepository.GetByIdAsync(id, cancellationToken);

            if (cart == null)
            {
                return NotFound(new { error = "Cart not found." });
            }

            var quantityRules = await _quantityRuleRepository.GetAllAsync(cancellationToken);
            if (quantityRules.Any())
            {
                var ruleValueObjects = quantityRules.Select(r => r.ToValueObject()).ToList();
                var validator = new GoodHamburguer.Domain.OrderQuantityValidator(ruleValueObjects);
                
                var tempCart = Cart.Create(cart.Id);
                foreach (var item in cart.Items)
                {
                    var itemProduct = await _productRepository.GetByIdAsync(item.ProductId, cancellationToken);
                    if (itemProduct != null)
                    {
                        if (item.ProductId == productId)
                        {
                            tempCart.AddItem(itemProduct, dto.Quantity);
                        }
                        else
                        {
                            tempCart.AddItem(itemProduct, item.Quantity);
                        }
                    }
                }
                
                var allProductIds = tempCart.Items.Select(i => i.ProductId).Distinct().ToList();
                var productsDict = new Dictionary<Guid, Product?>();
                foreach (var pid in allProductIds)
                {
                    productsDict[pid] = await _productRepository.GetByIdAsync(pid, cancellationToken);
                }
                
                Product? GetProductById(Guid id) => productsDict.TryGetValue(id, out var p) ? p : null;
                
                tempCart.ValidateQuantityRules(validator, GetProductById);
            }

            cart.UpdateItemQuantity(productId, dto.Quantity);
            await _cartRepository.UpdateAsync(cart, cancellationToken);

            var cartDto = await MapToDtoAsync(cart, cancellationToken);
            return Ok(cartDto);
        }
        catch (DomainException ex)
        {
            _logger.LogWarning(ex, "Domain error while updating item quantity");
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpDelete("{id}/items/{productId}")]
    [SwaggerOperation(
        Summary = "Remove item from cart",
        Description = "Removes a specific product from the cart."
    )]
    [SwaggerResponse(200, "Item removed successfully", typeof(CartDto))]
    [SwaggerResponse(400, "Error removing item")]
    [SwaggerResponse(404, "Cart not found")]
    public async Task<ActionResult<CartDto>> RemoveItem(
        [SwaggerParameter("Unique cart ID", Required = true)] Guid id, 
        [SwaggerParameter("Unique product ID", Required = true)] Guid productId, 
        CancellationToken cancellationToken)
    {
        try
        {
            var cart = await _cartRepository.GetByIdAsync(id, cancellationToken);

            if (cart == null)
            {
                return NotFound(new { error = "Cart not found." });
            }

            cart.RemoveItem(productId);
            await _cartRepository.UpdateAsync(cart, cancellationToken);

            var cartDto = await MapToDtoAsync(cart, cancellationToken);
            return Ok(cartDto);
        }
        catch (DomainException ex)
        {
            _logger.LogWarning(ex, "Domain error while removing item from cart");
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpDelete("{id}")]
    [SwaggerOperation(
        Summary = "Delete cart",
        Description = "Completely removes a cart from the system."
    )]
    [SwaggerResponse(204, "Cart deleted successfully")]
    [SwaggerResponse(404, "Cart not found")]
    public async Task<IActionResult> Delete(
        [SwaggerParameter("Unique cart ID", Required = true)] Guid id, 
        CancellationToken cancellationToken)
    {
        var cart = await _cartRepository.GetByIdAsync(id, cancellationToken);

        if (cart == null)
        {
            return NotFound(new { error = "Cart not found." });
        }

        await _cartRepository.DeleteAsync(cart, cancellationToken);
        return NoContent();
    }

    private async Task<CartDto> MapToDtoAsync(Cart cart, CancellationToken cancellationToken)
    {
        var subtotal = cart.GetTotal();
        
        var availableDiscounts = await _discountRepository.GetActiveAsync(cancellationToken);
        
        var productIds = cart.Items.Select(item => item.ProductId).Distinct().ToList();
        var productsDictionary = new Dictionary<Guid, Product?>();
        
        foreach (var productId in productIds)
        {
            var product = await _productRepository.GetByIdAsync(productId, cancellationToken);
            productsDictionary[productId] = product;
        }

        Product? GetProductById(Guid id) => productsDictionary.TryGetValue(id, out var product) ? product : null;
        
        var discountAmount = cart.GetDiscountAmount(availableDiscounts, GetProductById);
        var bestDiscount = cart.GetBestApplicableDiscount(availableDiscounts, GetProductById);
        var total = cart.GetTotalWithDiscount(availableDiscounts, GetProductById);

        var itemDtos = cart.Items.Select(item => new CartItemDto(
            item.Id,
            item.ProductId,
            item.ProductName,
            item.UnitPrice,
            item.Quantity,
            item.GetSubtotal()
        ));

        return new CartDto(
            cart.Id,
            itemDtos,
            subtotal,
            discountAmount > 0 ? discountAmount : null,
            bestDiscount?.Name,
            total,
            cart.GetTotalItems()
        );
    }
}

