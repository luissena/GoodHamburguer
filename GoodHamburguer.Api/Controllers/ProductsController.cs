using GoodHamburguer.Api.DTOs;
using GoodHamburguer.Domain.Entities;
using GoodHamburguer.Domain.Exceptions;
using GoodHamburguer.Domain.Repositories;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace GoodHamburguer.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[SwaggerTag("Operations for product management")]
public class ProductsController : ControllerBase
{
    private readonly IProductRepository _productRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly ILogger<ProductsController> _logger;

    public ProductsController(
        IProductRepository productRepository,
        ICategoryRepository categoryRepository,
        ILogger<ProductsController> logger)
    {
        _productRepository = productRepository;
        _categoryRepository = categoryRepository;
        _logger = logger;
    }

    [HttpPost]
    [SwaggerOperation(
        Summary = "Create a new product",
        Description = "Creates a new product in the system. The product must be associated with an existing category."
    )]
    [SwaggerResponse(201, "Product created successfully", typeof(ProductDto))]
    [SwaggerResponse(400, "Validation error, category not found or domain rule violation")]
    public async Task<ActionResult<ProductDto>> Create([FromBody] CreateProductDto dto, CancellationToken cancellationToken)
    {
        try
        {
            var category = await _categoryRepository.GetByIdAsync(dto.CategoryId, cancellationToken);

            if (category == null)
            {
                return BadRequest(new { error = "Category not found." });
            }

            var product = Product.Create(dto.Name, category, dto.Price);
            await _productRepository.CreateAsync(product, cancellationToken);

            var productDto = MapToDto(product);
            return CreatedAtAction(nameof(GetById), new { id = product.Id }, productDto);
        }
        catch (DomainException ex)
        {
            _logger.LogWarning(ex, "Domain error while creating product");
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpGet]
    [SwaggerOperation(
        Summary = "List products with pagination",
        Description = "Returns a paginated list of products. By default, returns the first page with 10 items."
    )]
    [SwaggerResponse(200, "Lista de produtos retornada com sucesso", typeof(PagedResultDto<ProductDto>))]
    [SwaggerResponse(400, "Parâmetros de paginação inválidos")]
    public async Task<ActionResult<PagedResultDto<ProductDto>>> GetAll(
        [SwaggerParameter("Page number (minimum: 1)", Required = false)] [FromQuery] int page = 1,
        [SwaggerParameter("Page size (minimum: 1, maximum: 100)", Required = false)] [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        if (page < 1)
        {
            return BadRequest(new { error = "Page must be greater than zero." });
        }

        if (pageSize < 1 || pageSize > 100)
        {
            return BadRequest(new { error = "Page size must be between 1 and 100." });
        }

        var allProducts = await _productRepository.GetAllAsync(cancellationToken);
        var totalCount = allProducts.Count();

        var products = allProducts
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        var productDtos = products.Select(MapToDto);
        var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

        var result = new PagedResultDto<ProductDto>(
            productDtos,
            page,
            pageSize,
            totalCount,
            totalPages
        );

        return Ok(result);
    }

    [HttpGet("category/{categoryId}")]
    [SwaggerOperation(
        Summary = "List products by category with pagination",
        Description = "Returns a paginated list of products filtered by category. By default, returns the first page with 10 items."
    )]
    [SwaggerResponse(200, "Lista de produtos retornada com sucesso", typeof(PagedResultDto<ProductDto>))]
    [SwaggerResponse(400, "Parâmetros de paginação inválidos")]
    [SwaggerResponse(404, "Category not found")]
    public async Task<ActionResult<PagedResultDto<ProductDto>>> GetByCategory(
        [SwaggerParameter("Unique category ID", Required = true)] Guid categoryId,
        [SwaggerParameter("Page number (minimum: 1)", Required = false)] [FromQuery] int page = 1,
        [SwaggerParameter("Page size (minimum: 1, maximum: 100)", Required = false)] [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        if (page < 1)
        {
            return BadRequest(new { error = "Page must be greater than zero." });
        }

        if (pageSize < 1 || pageSize > 100)
        {
            return BadRequest(new { error = "Page size must be between 1 and 100." });
        }

        var category = await _categoryRepository.GetByIdAsync(categoryId, cancellationToken);
        if (category == null)
        {
            return NotFound(new { error = "Category not found." });
        }

        var allProducts = await _productRepository.GetByCategoryIdAsync(categoryId, cancellationToken);
        var totalCount = allProducts.Count();

        var products = allProducts
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        var productDtos = products.Select(MapToDto);
        var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

        var result = new PagedResultDto<ProductDto>(
            productDtos,
            page,
            pageSize,
            totalCount,
            totalPages
        );

        return Ok(result);
    }

    [HttpGet("{id}")]
    [SwaggerOperation(
        Summary = "Get product by ID",
        Description = "Returns the details of a specific product using its ID."
    )]
    [SwaggerResponse(200, "Product found", typeof(ProductDto))]
    [SwaggerResponse(404, "Product not found")]
    public async Task<ActionResult<ProductDto>> GetById(
        [SwaggerParameter("Unique product ID", Required = true)] Guid id, 
        CancellationToken cancellationToken)
    {
        var product = await _productRepository.GetByIdAsync(id, cancellationToken);

        if (product == null)
        {
            return NotFound(new { error = "Product not found." });
        }

        var productDto = MapToDto(product);
        return Ok(productDto);
    }

    [HttpPut("{id}")]
    [SwaggerOperation(
        Summary = "Update product",
        Description = "Updates an existing product. All fields are optional - only provided fields will be updated."
    )]
    [SwaggerResponse(200, "Product updated successfully", typeof(ProductDto))]
    [SwaggerResponse(400, "Validation error, category not found or domain rule violation")]
    [SwaggerResponse(404, "Product not found")]
    public async Task<ActionResult<ProductDto>> Update(
        [SwaggerParameter("Unique product ID", Required = true)] Guid id, 
        [FromBody] UpdateProductDto dto, 
        CancellationToken cancellationToken)
    {
        try
        {
            var product = await _productRepository.GetByIdAsync(id, cancellationToken);

            if (product == null)
            {
                return NotFound(new { error = "Product not found." });
            }

            if (dto.Name != null)
            {
                product.UpdateName(dto.Name);
            }

            if (dto.CategoryId.HasValue)
            {
                var category = await _categoryRepository.GetByIdAsync(dto.CategoryId.Value, cancellationToken);
                if (category == null)
                {
                    return BadRequest(new { error = "Category not found." });
                }
                product.UpdateCategory(category);
            }

            if (dto.Price.HasValue)
            {
                product.UpdatePrice(dto.Price.Value);
            }

            await _productRepository.UpdateAsync(product, cancellationToken);

            var productDto = MapToDto(product);
            return Ok(productDto);
        }
        catch (DomainException ex)
        {
            _logger.LogWarning(ex, "Domain error while updating product");
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpDelete("{id}")]
    [SwaggerOperation(
        Summary = "Delete product",
        Description = "Removes a product from the system."
    )]
    [SwaggerResponse(204, "Product deleted successfully")]
    [SwaggerResponse(404, "Product not found")]
    public async Task<IActionResult> Delete(
        [SwaggerParameter("Unique product ID", Required = true)] Guid id, 
        CancellationToken cancellationToken)
    {
        var product = await _productRepository.GetByIdAsync(id, cancellationToken);

        if (product == null)
        {
            return NotFound(new { error = "Product not found." });
        }

        await _productRepository.DeleteAsync(product, cancellationToken);
        return NoContent();
    }

    private static ProductDto MapToDto(Product product)
    {
        return new ProductDto(
            product.Id,
            product.Name,
            new CategoryDto(product.Category.Id, product.Category.Name),
            product.Price
        );
    }
}

