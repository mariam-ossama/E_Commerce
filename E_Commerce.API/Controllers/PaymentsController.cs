using E_Commerce.Application.Contracts;
using E_Commerce.Application.DTOs.Baskets;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace E_Commerce.API.Controllers
{
    public class PaymentsController : ApiBaseController
    {
        private readonly IPaymentService _paymentService;

        // Create Or Update PaymentIntent
        // Must be Authorized
        // POST BaseUrl/api/Payments/basketId
        // basketId => BasketDto Updated With PaymentIntentId And ClientSecret
        public PaymentsController(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }
        [Authorize]
        [HttpPost("{basketId}")]
        public async Task<ActionResult<BasketDto>> CreateOrUpdatePaymentIntent(string basketId, CancellationToken ct)
        {
            return ToActionResult(await _paymentService.CreateOrUpdatePaymentIntentAsync(basketId, ct));
        }

    }
}
