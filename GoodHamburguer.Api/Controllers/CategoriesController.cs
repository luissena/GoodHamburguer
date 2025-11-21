using GoodHamburguer.Api.DTOs;
using GoodHamburguer.Domain.Entities;
using GoodHamburguer.Domain.Exceptions;
using GoodHamburguer.Domain.Repositories;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace GoodHamburguer.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[SwaggerTag("Operations for product category management")]
public class CategoriesController : ControllerBase
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly ILogger<CategoriesController> _logger;

    public CategoriesController(
        ICategoryRepository categoryRepository,
        ILogger<CategoriesController> logger)
    {
        _categoryRepository = categoryRepository;
        _logger = logger;
    }

    [HttpPost]
    [SwaggerOperation(
        Summary = "Create a new category",
        Description = "Creates a new product category in the system."
    )]
    [SwaggerResponse(201, "Category created successfully", typeof(CategoryDto))]
    [SwaggerResponse(400, "Validation error or domain rule violation")]
    public async Task<ActionResult<CategoryDto>> Create([FromBody] CreateCategoryDto dto, CancellationToken cancellationToken)
    {
        try
        {
            var category = Category.Create(dto.Name);
            await _categoryRepository.CreateAsync(category, cancellationToken);

            var categoryDto = new CategoryDto(category.Id, category.Name);
            return CreatedAtAction(nameof(GetById), new { id = category.Id }, categoryDto);
        }
        catch (DomainException ex)
        {
            _logger.LogWarning(ex, "Domain error while creating category");
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpGet]
    [SwaggerOperation(
        Summary = "List all categories",
        Description = "Returns a list with all categories registered in the system."
    )]
    [SwaggerResponse(200, "Category list returned successfully", typeof(IEnumerable<CategoryDto>))]
    public async Task<ActionResult<IEnumerable<CategoryDto>>> GetAll(CancellationToken cancellationToken)
    {
        var categories = await _categoryRepository.GetAllAsync(cancellationToken);
        var categoryDtos = categories.Select(c => new CategoryDto(c.Id, c.Name));
        return Ok(categoryDtos);
    }

    [HttpGet("{id}")]
    [SwaggerOperation(
        Summary = "Get category by ID",
        Description = "Returns the details of a specific category using its ID."
    )]
    [SwaggerResponse(200, "Category found", typeof(CategoryDto))]
    [SwaggerResponse(404, "Category not found")]
    public async Task<ActionResult<CategoryDto>> GetById(
        [SwaggerParameter("Unique category ID", Required = true)] Guid id, 
        CancellationToken cancellationToken)
    {
        var category = await _categoryRepository.GetByIdAsync(id, cancellationToken);

        if (category == null)
        {
            return NotFound(new { error = "Category not found." });
        }

        var categoryDto = new CategoryDto(category.Id, category.Name);
        return Ok(categoryDto);
    }

    [HttpPut("{id}")]
    [SwaggerOperation(
        Summary = "Update category",
        Description = "Updates the name of an existing category."
    )]
    [SwaggerResponse(200, "Category updated successfully", typeof(CategoryDto))]
    [SwaggerResponse(400, "Validation error or domain rule violation")]
    [SwaggerResponse(404, "Category not found")]
    public async Task<ActionResult<CategoryDto>> Update(
        [SwaggerParameter("Unique category ID", Required = true)] Guid id, 
        [FromBody] UpdateCategoryDto dto, 
        CancellationToken cancellationToken)
    {
        try
        {
            var category = await _categoryRepository.GetByIdAsync(id, cancellationToken);

            if (category == null)
            {
                return NotFound(new { error = "Category not found." });
            }

            category.UpdateName(dto.Name);
            await _categoryRepository.UpdateAsync(category, cancellationToken);

            var categoryDto = new CategoryDto(category.Id, category.Name);
            return Ok(categoryDto);
        }
        catch (DomainException ex)
        {
            _logger.LogWarning(ex, "Domain error while updating category");
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpDelete("{id}")]
    [SwaggerOperation(
        Summary = "Delete category",
        Description = "Removes a category from the system."
    )]
    [SwaggerResponse(204, "Category deleted successfully")]
    [SwaggerResponse(404, "Category not found")]
    public async Task<IActionResult> Delete(
        [SwaggerParameter("Unique category ID", Required = true)] Guid id, 
        CancellationToken cancellationToken)
    {
        var category = await _categoryRepository.GetByIdAsync(id, cancellationToken);

        if (category == null)
        {
            return NotFound(new { error = "Category not found." });
        }

        await _categoryRepository.DeleteAsync(category, cancellationToken);
        return NoContent();
    }
}

