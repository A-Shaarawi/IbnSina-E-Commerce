using IbnSina.Application.DTOs;
using IbnSina.Application.Interfaces;
using IbnSina.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace IbnSina.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly IProductRepository _productRepository;

    public ProductsController(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var products = await _productRepository.GetAllAsync();

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

    [HttpPost]
    public async Task<IActionResult> Create(CreateProductDto dto)
    {
        var product = new Product(dto.Name, dto.Description, dto.Quantity, dto.Price, dto.CategoryId);

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
            return NotFound();

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

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, UpdateProductDto dto)
    {
        var product = await _productRepository.GetByIdAsync(id);
        if (product == null)
            return NotFound();

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

    [HttpPatch("{id}")]
    public async Task<IActionResult> Patch(int id, PatchProductDto dto)
    {
        var product = await _productRepository.GetByIdAsync(id);
        if (product == null)
            return NotFound();

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