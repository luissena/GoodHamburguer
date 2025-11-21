using GoodHamburguer.Api.DTOs;
using GoodHamburguer.Domain.Entities;
using GoodHamburguer.Domain.Exceptions;
using GoodHamburguer.Domain.Repositories;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace GoodHamburguer.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[SwaggerTag("Operations for order management")]
public class OrdersController : ControllerBase
{
    private readonly IOrderRepository _orderRepository;
    private readonly ICartRepository _cartRepository;
    private readonly IDiscountRepository _discountRepository;
    private readonly IProductRepository _productRepository;
    private readonly IOrderQuantityRuleRepository _quantityRuleRepository;
    private readonly ILogger<OrdersController> _logger;

    public OrdersController(
        IOrderRepository orderRepository,
        ICartRepository cartRepository,
        IDiscountRepository discountRepository,
        IProductRepository productRepository,
        IOrderQuantityRuleRepository quantityRuleRepository,
        ILogger<OrdersController> logger)
    {
        _orderRepository = orderRepository;
        _cartRepository = cartRepository;
        _discountRepository = discountRepository;
        _productRepository = productRepository;
        _quantityRuleRepository = quantityRuleRepository;
        _logger = logger;
    }

    [HttpPost]
    [SwaggerOperation(
        Summary = "Create a new order",
        Description = "Creates a new order from an existing cart. The order is created with a snapshot of cart items and automatically calculates applicable discounts."
    )]
    [SwaggerResponse(201, "Order created successfully", typeof(OrderDto))]
    [SwaggerResponse(400, "Validation error, cart not found, empty cart or domain rule violation")]
    public async Task<ActionResult<OrderDto>> Create([FromBody] CreateOrderDto dto, CancellationToken cancellationToken)
    {
        try
        {
            var cart = await _cartRepository.GetByIdAsync(dto.CartId, cancellationToken);

            if (cart == null)
            {
                return BadRequest(new { error = "Cart not found." });
            }

            if (cart.IsEmpty())
            {
                return BadRequest(new { error = "It is not possible to create an order with an empty cart." });
            }

            var availableDiscounts = await _discountRepository.GetActiveAsync(cancellationToken);
            var productIds = cart.Items.Select(item => item.ProductId).Distinct().ToList();
            var productsDictionary = new Dictionary<Guid, Product?>();

            foreach (var productId in productIds)
            {
                var product = await _productRepository.GetByIdAsync(productId, cancellationToken);
                productsDictionary[productId] = product;
            }

            Product? GetProductById(Guid id) => productsDictionary.TryGetValue(id, out var product) ? product : null;

            var quantityRules = await _quantityRuleRepository.GetAllAsync(cancellationToken);
            var ruleValueObjects = quantityRules.Select(r => r.ToValueObject()).ToList();
            var validator = new GoodHamburguer.Domain.OrderQuantityValidator(ruleValueObjects);
            
            var discountAmount = cart.GetDiscountAmount(availableDiscounts, GetProductById);
            var order = Order.CreateFromCart(cart, discountAmount, validator, GetProductById);

            await _orderRepository.CreateAsync(order, cancellationToken);

            var orderDto = MapToDto(order);
            return CreatedAtAction(nameof(GetById), new { id = order.Id }, orderDto);
        }
        catch (DomainException ex)
        {
            _logger.LogWarning(ex, "Domain error while creating order");
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpGet]
    [SwaggerOperation(
        Summary = "List all orders",
        Description = "Returns a list with all orders registered in the system."
    )]
    [SwaggerResponse(200, "Order list returned successfully", typeof(IEnumerable<OrderDto>))]
    public async Task<ActionResult<IEnumerable<OrderDto>>> GetAll(CancellationToken cancellationToken)
    {
        var orders = await _orderRepository.GetAllAsync(cancellationToken);
        var orderDtos = orders.Select(MapToDto);
        return Ok(orderDtos);
    }

    [HttpGet("{id}")]
    [SwaggerOperation(
        Summary = "Get order by ID",
        Description = "Returns the complete details of a specific order, including items, status and values."
    )]
    [SwaggerResponse(200, "Order found", typeof(OrderDto))]
    [SwaggerResponse(404, "Order not found")]
    public async Task<ActionResult<OrderDto>> GetById(
        [SwaggerParameter("Unique order ID", Required = true)] Guid id, 
        CancellationToken cancellationToken)
    {
        var order = await _orderRepository.GetByIdAsync(id, cancellationToken);

        if (order == null)
        {
            return NotFound(new { error = "Order not found." });
        }

        var orderDto = MapToDto(order);
        return Ok(orderDto);
    }

    [HttpGet("status/{status}")]
    [SwaggerOperation(
        Summary = "List orders by status",
        Description = "Returns a list of orders filtered by status. Valid statuses: Pending, Confirmed, Preparing, Ready, Delivered, Cancelled."
    )]
    [SwaggerResponse(200, "Order list returned successfully", typeof(IEnumerable<OrderDto>))]
    [SwaggerResponse(400, "Invalid status")]
    public async Task<ActionResult<IEnumerable<OrderDto>>> GetByStatus(
        [SwaggerParameter("Order status (Pending, Confirmed, Preparing, Ready, Delivered, Cancelled)", Required = true)] string status, 
        CancellationToken cancellationToken)
    {
        if (!Enum.TryParse<OrderStatus>(status, ignoreCase: true, out var orderStatus))
        {
            return BadRequest(new { error = "Invalid status." });
        }

        var orders = await _orderRepository.GetByStatusAsync(orderStatus, cancellationToken);
        var orderDtos = orders.Select(MapToDto);
        return Ok(orderDtos);
    }

    [HttpPut("{id}/status")]
    [SwaggerOperation(
        Summary = "Update order status",
        Description = "Updates the status of an order. It is not possible to change the status of delivered or cancelled orders."
    )]
    [SwaggerResponse(200, "Status updated successfully", typeof(OrderDto))]
    [SwaggerResponse(400, "Validation error, invalid status or domain rule violation")]
    [SwaggerResponse(404, "Order not found")]
    public async Task<ActionResult<OrderDto>> UpdateStatus(
        [SwaggerParameter("Unique order ID", Required = true)] Guid id, 
        [FromBody] UpdateOrderStatusDto dto, 
        CancellationToken cancellationToken)
    {
        try
        {
            var order = await _orderRepository.GetByIdAsync(id, cancellationToken);

            if (order == null)
            {
                return NotFound(new { error = "Order not found." });
            }

            if (!Enum.TryParse<OrderStatus>(dto.Status, ignoreCase: true, out var orderStatus))
            {
                return BadRequest(new { error = "Invalid status." });
            }

            order.UpdateStatus(orderStatus);
            await _orderRepository.UpdateAsync(order, cancellationToken);

            var orderDto = MapToDto(order);
            return Ok(orderDto);
        }
        catch (DomainException ex)
        {
            _logger.LogWarning(ex, "Domain error while updating order status");
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpPut("{id}/cancel")]
    [SwaggerOperation(
        Summary = "Cancel order",
        Description = "Cancels an order. It is not possible to cancel already delivered orders."
    )]
    [SwaggerResponse(200, "Order cancelled successfully", typeof(OrderDto))]
    [SwaggerResponse(400, "Error cancelling order")]
    [SwaggerResponse(404, "Order not found")]
    public async Task<ActionResult<OrderDto>> Cancel(
        [SwaggerParameter("Unique order ID", Required = true)] Guid id, 
        CancellationToken cancellationToken)
    {
        try
        {
            var order = await _orderRepository.GetByIdAsync(id, cancellationToken);

            if (order == null)
            {
                return NotFound(new { error = "Order not found." });
            }

            order.Cancel();
            await _orderRepository.UpdateAsync(order, cancellationToken);

            var orderDto = MapToDto(order);
            return Ok(orderDto);
        }
        catch (DomainException ex)
        {
            _logger.LogWarning(ex, "Domain error while cancelling order");
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpDelete("{id}")]
    [SwaggerOperation(
        Summary = "Delete order",
        Description = "Removes an order from the system."
    )]
    [SwaggerResponse(204, "Order deleted successfully")]
    [SwaggerResponse(404, "Order not found")]
    public async Task<IActionResult> Delete(
        [SwaggerParameter("Unique order ID", Required = true)] Guid id, 
        CancellationToken cancellationToken)
    {
        var order = await _orderRepository.GetByIdAsync(id, cancellationToken);

        if (order == null)
        {
            return NotFound(new { error = "Order not found." });
        }

        await _orderRepository.DeleteAsync(order, cancellationToken);
        return NoContent();
    }

    private static OrderDto MapToDto(Order order)
    {
        var itemDtos = order.Items.Select(item => new OrderItemDto(
            item.Id,
            item.ProductId,
            item.ProductName,
            item.UnitPrice,
            item.Quantity,
            item.GetSubtotal()
        ));

        return new OrderDto(
            order.Id,
            itemDtos,
            order.Status.ToString(),
            order.CreatedAt,
            order.Subtotal,
            order.DiscountAmount,
            order.Total
        );
    }
}

