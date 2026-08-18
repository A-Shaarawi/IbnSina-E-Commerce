using IbnSina.Application.DTOs;
using IbnSina.Application.Interfaces;
using Microsoft.Extensions.Configuration;
using Stripe;

namespace IbnSina.Infrastructure.Services;

public class StripePaymentService : IPaymentService
{
    public StripePaymentService(IConfiguration configuration)
    {
        StripeConfiguration.ApiKey = configuration["Stripe:SecretKey"];
    }

    public async Task<PaymentIntentDto> CreatePaymentIntentAsync(int orderId, decimal amount, string currency = "usd")
    {
        var options = new PaymentIntentCreateOptions
        {
            Amount = (long)(amount * 100),
            Currency = currency,
            Metadata = new Dictionary<string, string>
            {
                { "OrderId", orderId.ToString() }
            }
        };

        var service = new PaymentIntentService();
        var paymentIntent = await service.CreateAsync(options);

        return new PaymentIntentDto
        {
            ClientSecret = paymentIntent.ClientSecret!,
            PaymentIntentId = paymentIntent.Id,
            Amount = amount
        };
    }
}