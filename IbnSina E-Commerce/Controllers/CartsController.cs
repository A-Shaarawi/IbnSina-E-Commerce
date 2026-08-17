using IbnSina.Application.Interfaces;
using IbnSina.Domain.Entities;
using IbnSina.Application.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace IbnSina_E_Commerce.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class CartsController : ControllerBase
    {
        private readonly ICartItemRepository _cartItemRepository;
        private readonly IProductRepository _productRepository;
        public CartsController(ICartItemRepository cartItemRepository, IProductRepository productRepository)
        {
            _cartItemRepository = cartItemRepository;
            _productRepository = productRepository;
        }

        private int GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            return int.Parse(userIdClaim!.Value);
        }

        [HttpGet]
        public async Task<IActionResult> GetCartItems()
        {
            var userId = GetCurrentUserId();
            var cartItems = await _cartItemRepository.GetAllAsync(userId);
            return Ok(cartItems);
        }

        [HttpPost]
        public async Task<IActionResult> AddToCart(CreateCartItemDto dto)
        {
            var userId = GetCurrentUserId();

            var product = await _productRepository.GetByIdAsync(dto.ProductId);
            if (product == null)
                return NotFound(new { message = $"No product was found with ID {dto.ProductId}." });

            if (product.StockQuantity < dto.Quantity)
            {
                return BadRequest(new
                {
                    message = $"Only {product.StockQuantity} unit(s) of '{product.Name}' available. You requested {dto.Quantity}."
                });
            }

            var cartItem = new CartItem(userId, dto.ProductId, dto.Quantity);
            await _cartItemRepository.AddAsync(cartItem);

            var saved = await _cartItemRepository.GetByIdAsync(cartItem.Id, userId);
            return Ok(saved);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> RemoveFromCart(int id)
        {
            var userId = GetCurrentUserId();
            var cartItem = await _cartItemRepository.GetByIdAsync(id, userId);
            if (cartItem == null)
                return NotFound(new { message = $"No Cart Item was found with ID {id}. Please check and try again." });

            await _cartItemRepository.DeleteAsync(cartItem);
            return NoContent();
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCartItem(int id, UpdateCartItemDto dto)
        {
            var userId = GetCurrentUserId();
            var cartItem = await _cartItemRepository.GetByIdAsync(id, userId);
            if (cartItem == null)
                return NotFound(new { message = $"No Cart Item was found with ID {id}. Please check and try again." });

            var product = await _productRepository.GetByIdAsync(cartItem.ProductId);
            if (product == null)
                return NotFound(new { message = "The product for this cart item no longer exists." });

            if (product.StockQuantity < dto.Quantity)
            {
                return BadRequest(new
                {
                    message = $"Only {product.StockQuantity} unit(s) of '{product.Name}' available. You requested {dto.Quantity}."
                });
            }

            cartItem.UpdateQuantity(dto.Quantity);
            await _cartItemRepository.UpdateAsync(cartItem);

            return Ok(cartItem);
        }

        [HttpDelete("clear")]
        public async Task<IActionResult> ClearCart()
        {
            var userId = GetCurrentUserId();
            await _cartItemRepository.ClearAsync(userId);
            return NoContent();
        }
    }
}