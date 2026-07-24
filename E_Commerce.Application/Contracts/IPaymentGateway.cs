using E_Commerce.Application.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Application.Contracts
{
    public interface IPaymentGateway
    {
        // Create PaymentIntent
        // amount + currency => PaymentIntentId + ClientSecret
        Task<PaymentIntentResult> CreatePaymentIntentAsync(decimal amount, string currency, CancellationToken ct = default);

        // Update PaymentIntent
        // PaymentIntentId + amount => PaymentIntentId + ClientSecret
        Task<PaymentIntentResult> UpdatePaymentIntentAsync(decimal amount, string paymentIntentId, CancellationToken ct = default);
    }
}
