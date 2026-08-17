using IbnSina.Application.Interfaces;
using IbnSina.Domain.Entities;
using IbnSina.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace IbnSina.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class OrdersController : ControllerBase
{
    private readonly IOrderRepository _orderRepository;
    private readonly ICartItemRepository _cartItemRepository;
    private readonly IProductRepository _productRepository;
    private readonly AppDbContext _context;

    public OrdersController(
        IOrderRepository orderRepository,
        ICartItemRepository cartItemRepository,
        IProductRepository productRepository,
        AppDbContext context)
    {
        _orderRepository = orderRepository;
        _cartItemRepository = cartItemRepository;
        _productRepository = productRepository;
        _context = context;
    }

    private int GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        return int.Parse(userIdClaim!.Value);
    }

    [HttpPost]
    public async Task<IActionResult> Checkout()
    {
        var userId = GetCurrentUserId();

        // Step 2: Load cart with product details
        var cartItems = await _cartItemRepository.GetAllAsync(userId);

        // Step 3: Empty cart -> 400
        if (!cartItems.Any())
            return BadRequest(new { message = "Your cart is empty. Add items before checking out." });

        // Step 4: For each line, check stock is sufficient
        foreach (var line in cartItems)
        {
            if (line.Product.StockQuantity < line.Quantity)
            {
                return BadRequest(new
                {
                    message = $"Insufficient stock for '{line.Product.Name}'. Available: {line.Product.StockQuantity}, requested: {line.Quantity}."
                });
            }
        }

        // Step 5: Begin DB transaction
        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            // Step 6: Create Order + OrderItems (name/price snapshot)
            var order = new Order(userId);
            foreach (var line in cartItems)
            {
                var orderItem = new OrderItem(line.ProductId, line.Product.Name, line.Quantity, line.Product.Price);
                order.AddItem(orderItem);
            }

            await _orderRepository.AddAsync(order);

            // Step 7: Decrease stock + clear cart
            foreach (var line in cartItems)
            {
                line.Product.DecreaseStock(line.Quantity);
                await _productRepository.UpdateAsync(line.Product);
            }

            await _cartItemRepository.ClearAsync(userId);

            order.Complete();
            await _context.SaveChangesAsync();

            // Step 8: Commit
            await transaction.CommitAsync();

            return StatusCode(201, new
            {
                order.Id,
                order.OrderDate,
                order.Status,
                order.TotalAmount,
                Items = order.OrderItems.Select(oi => new
                {
                    oi.ProductId,
                    oi.ProductName,
                    oi.Quantity,
                    oi.UnitPrice,
                    oi.Subtotal
                })
            });
        }
        catch
        {
            // If anything fails mid-way -> rollback everything
            await transaction.RollbackAsync();
            throw;
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var userId = GetCurrentUserId();
        var orders = await _orderRepository.GetAllAsync(userId);

        var result = orders.Select(o => new
        {
            o.Id,
            o.OrderDate,
            o.Status,
            o.TotalAmount,
            Items = o.OrderItems.Select(oi => new { oi.ProductId, oi.ProductName, oi.Quantity, oi.UnitPrice, oi.Subtotal })
        });

        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var userId = GetCurrentUserId();
        var order = await _orderRepository.GetByIdAsync(id, userId);
        if (order == null)
            return NotFound(new { message = "Order not found." });

        return Ok(new
        {
            order.Id,
            order.OrderDate,
            order.Status,
            order.TotalAmount,
            Items = order.OrderItems.Select(oi => new { oi.ProductId, oi.ProductName, oi.Quantity, oi.UnitPrice, oi.Subtotal })
        });
    }
}