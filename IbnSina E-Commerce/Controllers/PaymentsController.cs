using IbnSina.Application.Interfaces;
using IbnSina.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace IbnSina.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class PaymentsController : ControllerBase
{
    private readonly IOrderRepository _orderRepository;
    private readonly IPaymentService _paymentService;
    private readonly IPaymentRepository _paymentRepository;

    public PaymentsController(
        IOrderRepository orderRepository,
        IPaymentService paymentService,
        IPaymentRepository paymentRepository)
    {
        _orderRepository = orderRepository;
        _paymentService = paymentService;
        _paymentRepository = paymentRepository;
    }

    [HttpPost("orders/{orderId}/pay")]
    public async Task<IActionResult> Pay(int orderId)
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

        var order = await _orderRepository.GetByIdAsync(orderId, userId);
        if (order == null)
            return NotFound(new { message = "Order not found." });

        var existingPayment = await _paymentRepository.GetByOrderIdAsync(orderId);
        if (existingPayment != null && existingPayment.Status == IbnSina.Domain.Entities.PaymentStatus.Succeeded)
            return BadRequest(new { message = "This order has already been paid." });

        var result = await _paymentService.CreatePaymentIntentAsync(orderId, order.TotalAmount);

        var payment = new Payment(orderId, result.PaymentIntentId, order.TotalAmount);
        await _paymentRepository.AddAsync(payment);

        return Ok(new
        {
            result.ClientSecret,
            result.Amount,
            result.PaymentIntentId,
            currency = "usd"
        });
    }
}