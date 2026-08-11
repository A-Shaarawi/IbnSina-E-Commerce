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

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var category = await _categoryRepository.GetByIdAsync(id);
        if (category == null)
        {
            return NotFound();
        }
        await _categoryRepository.DeleteAsync(category);
        return NoContent();
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Get(int id)
    {
        var category = await _categoryRepository.GetByIdAsync(id);
        if (category == null)
        {
            return NotFound();
        }
        return new OkObjectResult(category);
    }
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, Category category)
    {
        var existingCategory = await _categoryRepository.GetByIdAsync(id);
        if (existingCategory == null)
        {
            return NotFound();
        }

        existingCategory.SetName(category.Name);

        await _categoryRepository.UpdateAsync(existingCategory);

        return new OkObjectResult(existingCategory);
    }
}
