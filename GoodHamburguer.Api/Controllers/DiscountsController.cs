using GoodHamburguer.Api.DTOs;
using GoodHamburguer.Domain.Entities;
using GoodHamburguer.Domain.Exceptions;
using GoodHamburguer.Domain.Repositories;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace GoodHamburguer.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[SwaggerTag("Operations for discount and promotion management")]
public class DiscountsController : ControllerBase
{
    private readonly IDiscountRepository _discountRepository;
    private readonly ILogger<DiscountsController> _logger;

    public DiscountsController(
        IDiscountRepository discountRepository,
        ILogger<DiscountsController> logger)
    {
        _discountRepository = discountRepository;
        _logger = logger;
    }

    [HttpPost]
    [SwaggerOperation(
        Summary = "Create a new discount",
        Description = "Creates a new discount with custom conditions. Conditions can be based on specific products, categories or combinations."
    )]
    [SwaggerResponse(201, "Discount created successfully", typeof(DiscountDto))]
    [SwaggerResponse(400, "Validation error or domain rule violation")]
    public async Task<ActionResult<DiscountDto>> Create([FromBody] CreateDiscountDto dto, CancellationToken cancellationToken)
    {
        try
        {
            var discount = Discount.Create(dto.Name, dto.Percentage);

            foreach (var conditionDto in dto.Conditions)
            {
                DiscountCondition condition;

                if (conditionDto.ProductId.HasValue && conditionDto.CategoryId.HasValue)
                {
                    condition = DiscountCondition.CreateForProductAndCategory(
                        conditionDto.ProductId.Value,
                        conditionDto.CategoryId.Value,
                        conditionDto.MinimumQuantity);
                }
                else if (conditionDto.ProductId.HasValue)
                {
                    condition = DiscountCondition.CreateForProduct(
                        conditionDto.ProductId.Value,
                        conditionDto.MinimumQuantity);
                }
                else if (conditionDto.CategoryId.HasValue)
                {
                    condition = DiscountCondition.CreateForCategory(
                        conditionDto.CategoryId.Value,
                        conditionDto.MinimumQuantity);
                }
                else
                {
                    return BadRequest(new { error = "The condition must have at least ProductId or CategoryId." });
                }

                discount.AddCondition(condition);
            }

            await _discountRepository.CreateAsync(discount, cancellationToken);

            var discountDto = MapToDto(discount);
            return CreatedAtAction(nameof(GetById), new { id = discount.Id }, discountDto);
        }
        catch (DomainException ex)
        {
            _logger.LogWarning(ex, "Domain error while creating discount");
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpGet]
    [SwaggerOperation(
        Summary = "List all discounts",
        Description = "Returns a list with all discounts registered in the system, including active and inactive."
    )]
    [SwaggerResponse(200, "Discount list returned successfully", typeof(IEnumerable<DiscountDto>))]
    public async Task<ActionResult<IEnumerable<DiscountDto>>> GetAll(CancellationToken cancellationToken)
    {
        var discounts = await _discountRepository.GetAllAsync(cancellationToken);
        var discountDtos = discounts.Select(MapToDto);
        return Ok(discountDtos);
    }

    [HttpGet("{id}")]
    [SwaggerOperation(
        Summary = "Get discount by ID",
        Description = "Returns the details of a specific discount, including its conditions."
    )]
    [SwaggerResponse(200, "Discount found", typeof(DiscountDto))]
    [SwaggerResponse(404, "Discount not found")]
    public async Task<ActionResult<DiscountDto>> GetById(
        [SwaggerParameter("Unique discount ID", Required = true)] Guid id, 
        CancellationToken cancellationToken)
    {
        var discount = await _discountRepository.GetByIdAsync(id, cancellationToken);

        if (discount == null)
        {
            return NotFound(new { error = "Discount not found." });
        }

        var discountDto = MapToDto(discount);
        return Ok(discountDto);
    }

    [HttpPut("{id}")]
    [SwaggerOperation(
        Summary = "Update discount",
        Description = "Updates an existing discount. All fields are optional - only provided fields will be updated."
    )]
    [SwaggerResponse(200, "Discount updated successfully", typeof(DiscountDto))]
    [SwaggerResponse(400, "Validation error or domain rule violation")]
    [SwaggerResponse(404, "Discount not found")]
    public async Task<ActionResult<DiscountDto>> Update(
        [SwaggerParameter("Unique discount ID", Required = true)] Guid id, 
        [FromBody] UpdateDiscountDto dto, 
        CancellationToken cancellationToken)
    {
        try
        {
            var discount = await _discountRepository.GetByIdAsync(id, cancellationToken);

            if (discount == null)
            {
                return NotFound(new { error = "Discount not found." });
            }

            if (dto.Name != null)
            {
                discount.UpdateName(dto.Name);
            }

            if (dto.Percentage.HasValue)
            {
                discount.UpdatePercentage(dto.Percentage.Value);
            }

            if (dto.IsActive.HasValue)
            {
                if (dto.IsActive.Value)
                {
                    discount.Activate();
                }
                else
                {
                    discount.Deactivate();
                }
            }

            await _discountRepository.UpdateAsync(discount, cancellationToken);

            var discountDto = MapToDto(discount);
            return Ok(discountDto);
        }
        catch (DomainException ex)
        {
            _logger.LogWarning(ex, "Domain error while updating discount");
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpDelete("{id}")]
    [SwaggerOperation(
        Summary = "Delete discount",
        Description = "Removes a discount from the system."
    )]
    [SwaggerResponse(204, "Discount deleted successfully")]
    [SwaggerResponse(404, "Discount not found")]
    public async Task<IActionResult> Delete(
        [SwaggerParameter("Unique discount ID", Required = true)] Guid id, 
        CancellationToken cancellationToken)
    {
        var discount = await _discountRepository.GetByIdAsync(id, cancellationToken);

        if (discount == null)
        {
            return NotFound(new { error = "Discount not found." });
        }

        await _discountRepository.DeleteAsync(discount, cancellationToken);
        return NoContent();
    }

    private static DiscountDto MapToDto(Discount discount)
    {
        var conditionDtos = discount.Conditions.Select(c => new DiscountConditionDto(
            c.ProductId,
            c.CategoryId,
            c.MinimumQuantity
        ));

        return new DiscountDto(
            discount.Id,
            discount.Name,
            discount.Percentage,
            discount.IsActive,
            conditionDtos
        );
    }
}

