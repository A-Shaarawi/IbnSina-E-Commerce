using IbnSina.Application.Interfaces;
using IbnSina.Domain.Entities;
using IbnSina.Application.DTOs;
using Microsoft.AspNetCore.Mvc;
namespace IbnSina_E_Commerce.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CartsController : ControllerBase
    {
        private readonly ICartItemRepository _cartItemRepository;
        public CartsController(ICartItemRepository cartItemRepository)
        {
            _cartItemRepository = cartItemRepository;
        }
        [HttpGet]
        public async Task<IActionResult> GetCartItems(int userId)
        {
            var cartItems = await _cartItemRepository.GetAllAsync(userId);
            return new OkObjectResult(cartItems);
        }
        [HttpPost]
        public async Task<IActionResult> AddToCart(CreateCartItemDto dto)
        {
            var cartItem = new CartItem(dto.UserId, dto.ProductId, dto.Quantity);
            await _cartItemRepository.AddAsync(cartItem);

            var saved = await _cartItemRepository.GetByIdAsync(cartItem.Id, dto.UserId);

            return Ok(saved);
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> RemoveFromCart(int id, int userId)
        {
            var cartItem = await _cartItemRepository.GetByIdAsync(id, userId);
            if (cartItem == null)
            {
                return NotFound(new { message = $"No Cart Item was found with ID {id} for User ID {userId}. Please check and try again." });
            }
            await _cartItemRepository.DeleteAsync(cartItem);
            return NoContent();
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCartItem(int id, int userId, UpdateCartItemDto dto)
        {
            var cartItem = await _cartItemRepository.GetByIdAsync(id, userId);
            if (cartItem == null)
                return NotFound(new { message = $"No Cart Item was found with ID {id} for User ID {userId}. Please check and try again." });

            cartItem.UpdateQuantity(dto.Quantity);
            await _cartItemRepository.UpdateAsync(cartItem);

            return Ok(cartItem);
        }
        [HttpDelete("Clear")]
        public async Task<IActionResult> ClearCart(int userId)
        {
            await _cartItemRepository.ClearAsync(userId);
            return NoContent();
        }
    }
}
