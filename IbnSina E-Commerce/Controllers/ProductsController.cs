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
            Quantity = p.Quantity,
            Price = p.Price,
            IsInStock = p.IsInStock,
            CategoryId = p.CategoryId,
            CategoryName = p.Category.Name
        });

        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateProductDto dto)
    {
        var product = new Product(dto.Name, dto.Description, dto.Quantity, dto.Price, dto.CategoryId);

        await _productRepository.AddAsync(product);

        // reload it with Category populated, since AddAsync only saved CategoryId
        var saved = await _productRepository.GetByIdAsync(product.Id);

        var result = new ProductResponseDto
        {
            Id = saved!.Id,
            Name = saved.Name,
            Description = saved.Description,
            Quantity = saved.Quantity,
            Price = saved.Price,
            IsInStock = saved.IsInStock,
            CategoryId = saved.CategoryId,
            CategoryName = saved.Category.Name
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
            Quantity = product.Quantity,
            Price = product.Price,
            IsInStock = product.IsInStock,
            CategoryId = product.CategoryId,
            CategoryName = product.Category.Name
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

        var result = new ProductResponseDto
        {
            Id = product.Id,
            Name = product.Name,
            Description = product.Description,
            Quantity = product.Quantity,
            Price = product.Price,
            IsInStock = product.IsInStock,
            CategoryId = product.CategoryId,
            CategoryName = product.Category.Name
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

        var updated = await _productRepository.GetByIdAsync(id); // reload in case CategoryId changed

        var result = new ProductResponseDto
        {
            Id = updated!.Id,
            Name = updated.Name,
            Description = updated.Description,
            Quantity = updated.Quantity,
            Price = updated.Price,
            IsInStock = updated.IsInStock,
            CategoryId = updated.CategoryId,
            CategoryName = updated.Category.Name
        };

        return Ok(result);
    }
}