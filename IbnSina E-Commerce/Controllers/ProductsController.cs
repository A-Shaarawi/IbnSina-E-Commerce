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
}