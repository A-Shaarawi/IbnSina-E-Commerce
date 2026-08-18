using IbnSina.Application.DTOs;

namespace IbnSina.Application.Interfaces;

public interface IPaymentService
{
    Task<PaymentIntentDto> CreatePaymentIntentAsync(int orderId, decimal amount, string currency = "usd");
}