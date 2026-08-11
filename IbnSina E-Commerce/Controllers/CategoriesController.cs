using IbnSina.Application.Interfaces;
using IbnSina.Domain.Entities;
using IbnSina.Application.DTOs;
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
    public async Task<IActionResult> GetAll(string? search)
    {
        var categories = await _categoryRepository.GetAllAsync(search);
        if (!categories.Any())
        {
            var message = !string.IsNullOrWhiteSpace(search)
                ? "No categories found matching your search. Try different keywords."
                : "No categories found.";

            return Ok(new {message});
        }
        return Ok(categories);
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
            return NotFound(new { message = $"No Category was found with ID {id}. Please check and try again." });
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
            return NotFound(new { message = $"No Category was found with ID {id}. Please check and try again." });
        }
        return new OkObjectResult(category);
    }
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, UpdateCategoryDto dto)
    {
        var existingCategory = await _categoryRepository.GetByIdAsync(id);
        if (existingCategory == null)
            return NotFound(new { message = $"No Category was found with ID {id}. Please check and try again." });

        existingCategory.UpdateDetails(dto.Name, dto.Description);

        await _categoryRepository.UpdateAsync(existingCategory);

        return Ok(existingCategory);
    }
}
