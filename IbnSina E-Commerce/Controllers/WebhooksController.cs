using IbnSina.Application.Interfaces;
using IbnSina.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Stripe;

namespace IbnSina.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class WebhooksController : ControllerBase
{
    private readonly IPaymentRepository _paymentRepository;
    private readonly IConfiguration _configuration;

    public WebhooksController(IPaymentRepository paymentRepository, IConfiguration configuration)
    {
        _paymentRepository = paymentRepository;
        _configuration = configuration;
    }

    [HttpPost("stripe")]
    public async Task<IActionResult> StripeWebhook()
    {
        var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();

        try
        {
            var stripeEvent = EventUtility.ConstructEvent(
                json,
                Request.Headers["Stripe-Signature"],
                _configuration["Stripe:WebhookSecret"]
            );

            if (stripeEvent.Type == EventTypes.PaymentIntentSucceeded)
            {
                var paymentIntent = stripeEvent.Data.Object as PaymentIntent;
                var payment = await _paymentRepository
                    .GetByStripePaymentIntentIdAsync(paymentIntent!.Id);

                if (payment != null)
                {
                    payment.MarkSucceeded();
                    await _paymentRepository.UpdateAsync(payment);
                }
            }
            else if (stripeEvent.Type == EventTypes.PaymentIntentPaymentFailed)
            {
                var paymentIntent = stripeEvent.Data.Object as PaymentIntent;
                var payment = await _paymentRepository
                    .GetByStripePaymentIntentIdAsync(paymentIntent!.Id);

                if (payment != null)
                {
                    payment.MarkFailed();
                    await _paymentRepository.UpdateAsync(payment);
                }
            }

            return Ok();
        }
        catch (StripeException)
        {
            return BadRequest();
        }
    }
}