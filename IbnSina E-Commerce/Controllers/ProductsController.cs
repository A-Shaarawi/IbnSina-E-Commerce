using IbnSina.Application.DTOs;
using IbnSina.Application.Interfaces;
using IbnSina.Domain.Entities;
using IbnSina.Infrastructure.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace IbnSina.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly IProductRepository _productRepository;
    private readonly ICategoryRepository _categoryRepository;

    public ProductsController(IProductRepository productRepository, ICategoryRepository categoryRepository)
    {
        _productRepository = productRepository;
        _categoryRepository = categoryRepository;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(string? search, int? categoryId, bool activeOnly = false)
    {
        if (categoryId.HasValue)
        {
            var category = await _categoryRepository.GetByIdAsync(categoryId.Value);
            if (category == null)
                return NotFound(new { message = $"No category was found with ID {categoryId.Value}." });
        }

        var products = await _productRepository.GetAllAsync(search, categoryId, activeOnly);

        if (!products.Any())
        {
            var message = !string.IsNullOrWhiteSpace(search)
                ? "No products found matching your search. Try different keywords."
                : categoryId.HasValue
                    ? $"No products found under category ID {categoryId.Value}."
                    : "No products found.";

            return Ok(new {message});
        }


        var result = products.Select(p => new ProductResponseDto
        {
            Id = p.Id,
            Name = p.Name,
            Description = p.Description,
            StockQuantity = p.StockQuantity,
            Price = p.Price,
            IsInStock = p.IsInStock,
            CategoryId = p.CategoryId,
            CategoryName = p.Category.Name,
            CreatedAt = p.CreatedAt
        });

        return Ok(result);
    }
    [Authorize(Roles = "Admin,SuperAdmin")]
    [HttpPost]
    public async Task<IActionResult> Create(CreateProductDto dto)
    {
        var product = new Product(dto.Name, dto.Description, dto.StockQuantity, dto.Price, dto.CategoryId);

        await _productRepository.AddAsync(product);

        var saved = await _productRepository.GetByIdAsync(product.Id);

        var result = new ProductResponseDto
        {
            Id = saved!.Id,
            Name = saved.Name,
            Description = saved.Description,
            StockQuantity = saved.StockQuantity,
            Price = saved.Price,
            IsInStock = saved.IsInStock,
            CategoryId = saved.CategoryId,
            CategoryName = saved.Category.Name,
            CreatedAt = saved.CreatedAt
        };

        return Ok(result);
    }
    [Authorize(Roles = "Admin,SuperAdmin")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var product = await _productRepository.GetByIdAsync(id);
        if (product == null)
            return NotFound();
        await _productRepository.DeleteAsync(product);
        return NoContent();
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Get(int id)
    {
        var product = await _productRepository.GetByIdAsync(id);
        if (product == null)
            return NotFound(new { message = $"No product was found with ID {id}. Please check and try again." });

        var result = new ProductResponseDto
        {
            Id = product.Id,
            Name = product.Name,
            Description = product.Description,
            StockQuantity = product.StockQuantity,
            Price = product.Price,
            IsInStock = product.IsInStock,
            CategoryId = product.CategoryId,
            CategoryName = product.Category.Name,
            CreatedAt = product.CreatedAt
        };
        return Ok(result);
    }
    [Authorize(Roles = "Admin,SuperAdmin")]
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, UpdateProductDto dto)
    {
        var product = await _productRepository.GetByIdAsync(id);
        if (product == null)
            return NotFound(new { message = $"No product was found with ID {id}. Please check and try again." });

        product.UpdateDetails(dto.Name, dto.Description, dto.Price, dto.CategoryId);
        await _productRepository.UpdateAsync(product);

        var updated = await _productRepository.GetByIdAsync(id);

        var result = new ProductResponseDto
        {
            Id = updated!.Id,
            Name = updated.Name,
            Description = updated.Description,
            StockQuantity = updated.StockQuantity,
            Price = updated.Price,
            IsInStock = updated.IsInStock,
            CategoryId = updated.CategoryId,
            CategoryName = updated.Category.Name,
            CreatedAt = updated.CreatedAt
        };
        return Ok(result);
    }
    [Authorize(Roles = "Admin,SuperAdmin")]
    [HttpPatch("{id}")]
    public async Task<IActionResult> Patch(int id, PatchProductDto dto)
    {
        var product = await _productRepository.GetByIdAsync(id);
        if (product == null)
            return NotFound(new { message = $"No product was found with ID {id}. Please check and try again." });

        product.PatchDetails(dto.Name, dto.Description, dto.Price, dto.CategoryId);
        await _productRepository.UpdateAsync(product);

        var updated = await _productRepository.GetByIdAsync(id);

        var result = new ProductResponseDto
        {
            Id = updated!.Id,
            Name = updated.Name,
            Description = updated.Description,
            StockQuantity = updated.StockQuantity,
            Price = updated.Price,
            IsInStock = updated.IsInStock,
            CategoryId = updated.CategoryId,
            CategoryName = updated.Category.Name,
            CreatedAt = updated.CreatedAt
        };

        return Ok(result);
    }
}