using IbnSina.Application.Interfaces;
using IbnSina.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace IbnSina.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CategoriesController : ControllerBase
{
    private readonly ICategoryRepository _categoryRepository;

    public CategoriesController(ICategoryRepository categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var categories = await _categoryRepository.GetAllAsync();
        return new OkObjectResult(categories);
    }
    [HttpPost]
    public async Task<IActionResult> Create(Category category)
    {
        await _categoryRepository.AddAsync(category);
        return new OkObjectResult(category);
    }

}

