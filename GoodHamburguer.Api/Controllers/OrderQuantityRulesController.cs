using GoodHamburguer.Api.DTOs;
using GoodHamburguer.Domain.Entities;
using GoodHamburguer.Domain.Exceptions;
using GoodHamburguer.Domain.Repositories;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace GoodHamburguer.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[SwaggerTag("Operations for product/category quantity rule management")]
public class OrderQuantityRulesController : ControllerBase
{
    private readonly IOrderQuantityRuleRepository _ruleRepository;
    private readonly ILogger<OrderQuantityRulesController> _logger;

    public OrderQuantityRulesController(
        IOrderQuantityRuleRepository ruleRepository,
        ILogger<OrderQuantityRulesController> logger)
    {
        _ruleRepository = ruleRepository;
        _logger = logger;
    }

    [HttpPost]
    [SwaggerOperation(
        Summary = "Create a new quantity rule",
        Description = "Creates a new rule that limits the maximum quantity of a specific product or category in the order."
    )]
    [SwaggerResponse(201, "Rule created successfully", typeof(OrderQuantityRuleDto))]
    [SwaggerResponse(400, "Validation error or domain rule violation")]
    public async Task<ActionResult<OrderQuantityRuleDto>> Create([FromBody] CreateOrderQuantityRuleDto dto, CancellationToken cancellationToken)
    {
        try
        {
            if (!dto.ProductId.HasValue && !dto.CategoryId.HasValue)
            {
                return BadRequest(new { error = "The rule must have at least one ProductId or CategoryId." });
            }

            var rule = OrderQuantityRuleEntity.Create(dto.ProductId, dto.CategoryId, dto.MaxQuantity, dto.RuleName);
            await _ruleRepository.CreateAsync(rule, cancellationToken);

            var ruleDto = MapToDto(rule);
            return CreatedAtAction(nameof(GetById), new { id = rule.Id }, ruleDto);
        }
        catch (DomainException ex)
        {
            _logger.LogWarning(ex, "Domain error while creating quantity rule");
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpGet]
    [SwaggerOperation(
        Summary = "List all quantity rules",
        Description = "Returns a list with all quantity rules registered in the system."
    )]
    [SwaggerResponse(200, "Rule list returned successfully", typeof(IEnumerable<OrderQuantityRuleDto>))]
    public async Task<ActionResult<IEnumerable<OrderQuantityRuleDto>>> GetAll(CancellationToken cancellationToken)
    {
        var rules = await _ruleRepository.GetAllAsync(cancellationToken);
        var ruleDtos = rules.Select(MapToDto);
        return Ok(ruleDtos);
    }

    [HttpGet("{id}")]
    [SwaggerOperation(
        Summary = "Get quantity rule by ID",
        Description = "Returns the details of a specific rule using its ID."
    )]
    [SwaggerResponse(200, "Rule found", typeof(OrderQuantityRuleDto))]
    [SwaggerResponse(404, "Rule not found")]
    public async Task<ActionResult<OrderQuantityRuleDto>> GetById(
        [SwaggerParameter("Unique rule ID", Required = true)] Guid id, 
        CancellationToken cancellationToken)
    {
        var rule = await _ruleRepository.GetByIdAsync(id, cancellationToken);

        if (rule == null)
        {
            return NotFound(new { error = "Rule not found." });
        }

        var ruleDto = MapToDto(rule);
        return Ok(ruleDto);
    }

    [HttpPut("{id}")]
    [SwaggerOperation(
        Summary = "Update quantity rule",
        Description = "Updates an existing rule. All fields are optional - only provided fields will be updated."
    )]
    [SwaggerResponse(200, "Rule updated successfully", typeof(OrderQuantityRuleDto))]
    [SwaggerResponse(400, "Validation error or domain rule violation")]
    [SwaggerResponse(404, "Rule not found")]
    public async Task<ActionResult<OrderQuantityRuleDto>> Update(
        [SwaggerParameter("Unique rule ID", Required = true)] Guid id, 
        [FromBody] UpdateOrderQuantityRuleDto dto, 
        CancellationToken cancellationToken)
    {
        try
        {
            var rule = await _ruleRepository.GetByIdAsync(id, cancellationToken);

            if (rule == null)
            {
                return NotFound(new { error = "Rule not found." });
            }

            rule.Update(dto.ProductId, dto.CategoryId, dto.MaxQuantity, dto.RuleName);
            await _ruleRepository.UpdateAsync(rule, cancellationToken);

            var ruleDto = MapToDto(rule);
            return Ok(ruleDto);
        }
        catch (DomainException ex)
        {
            _logger.LogWarning(ex, "Domain error while updating quantity rule");
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpDelete("{id}")]
    [SwaggerOperation(
        Summary = "Delete quantity rule",
        Description = "Removes a specific rule from the system."
    )]
    [SwaggerResponse(204, "Rule deleted successfully")]
    [SwaggerResponse(404, "Rule not found")]
    public async Task<IActionResult> Delete(
        [SwaggerParameter("Unique rule ID", Required = true)] Guid id, 
        CancellationToken cancellationToken)
    {
        var rule = await _ruleRepository.GetByIdAsync(id, cancellationToken);

        if (rule == null)
        {
            return NotFound(new { error = "Rule not found." });
        }

        await _ruleRepository.DeleteAsync(rule, cancellationToken);
        return NoContent();
    }

    [HttpDelete]
    [SwaggerOperation(
        Summary = "Remove all quantity rules",
        Description = "Removes all rules from the system, allowing orders to be created without quantity restrictions."
    )]
    [SwaggerResponse(204, "All rules were removed successfully")]
    public async Task<IActionResult> ClearAll(CancellationToken cancellationToken)
    {
        await _ruleRepository.ClearAllAsync(cancellationToken);
        return NoContent();
    }

    private static OrderQuantityRuleDto MapToDto(OrderQuantityRuleEntity rule)
    {
        return new OrderQuantityRuleDto(
            rule.Id,
            rule.ProductId,
            rule.CategoryId,
            rule.MaxQuantity,
            rule.RuleName
        );
    }
}

