using AutoMapper;
using E_Commerce.Application.Common;
using E_Commerce.Application.Contracts;
using E_Commerce.Application.DTOs.Baskets;
using E_Commerce.Application.Specifications;
using E_Commerce.Domain.Contracts;
using E_Commerce.Domain.Entities.Orders;
using E_Commerce.Domain.Entities.Products;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Application.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly IBasketRepository _basketRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPaymentGateway _paymentGateway;
        private readonly IMapper _mapper;
        private readonly PaymentGatewaySettings _paymentGatewaySettings;

        public PaymentService(IBasketRepository basketRepository,
                              IUnitOfWork unitOfWork,
                              IPaymentGateway paymentGateway,
                              IOptions<PaymentGatewaySettings> options,
                              IMapper mapper)
        {
            _basketRepository = basketRepository;
            _unitOfWork = unitOfWork;
            _paymentGateway = paymentGateway;
            _mapper = mapper;
            _paymentGatewaySettings = options.Value;
        }
        public async Task<Result<BasketDto>> CreateOrUpdatePaymentIntentAsync(string basketId, CancellationToken ct = default)
        {
            // 1. Get basket and validate it
            var basket = await _basketRepository.GetBasketAsync(basketId,ct);
            if(basket == null)
                return Error.NotFound("Basket Is Not Found", $"Basket With Id {basketId} Is Not Found");
            if (basket.Items.Count == 0)
                return Error.Validation("Basket Is Empty", "Cannot Create PaymentIntent With Empty Basket");

            // 2. Get Delivery Method and Calculate Cost
            if (!basket.DeliveryMethodId.HasValue)
                return Error.Validation("Delivery Method Id Is Required");
            var deliveryMethod = await _unitOfWork.GetRepository<DeliveryMethod, int>()
                .GetByIdAsync(basket.DeliveryMethodId.Value, ct);
            if(deliveryMethod == null)
                return Error.NotFound("Delivery Method Is Not Found");
            basket.ShippingPrice = deliveryMethod.Price;

            // 3. Validate Product Prices
            var productIds = basket.Items.Select(x => x.Id).ToHashSet();
            var products = (await _unitOfWork.GetRepository<Product, int>()
                .GetAllAsync(new ProductWithIdSpecifications(productIds), ct)).ToDictionary(x=>x.Id);
            foreach(var item in basket.Items)
            {
                if (!products.TryGetValue(item.Id, out var product))
                    return Error.NotFound("Product Not Found", $"Product With Id {item.Id} Is Not Found");
                item.Price = product.Price;
            }

            // 4. Calculate Total Amount
            var subTotal = basket.Items.Sum(i => i.Price * i.Quantity);
            var amount = (long)((subTotal + deliveryMethod.Price) * 100m);

            // 5.1. PaymentIntentId Empty => Create - Put PaymentIntentId and ClientSecret in Basket
            if(string.IsNullOrEmpty(basket.PaymentIntentId))
            {
                // Create
                var result = await _paymentGateway.CreatePaymentIntentAsync(amount, _paymentGatewaySettings.DefaultCurrency, ct);
                basket.PaymentIntentId = result.PaymentIntentId;
                basket.ClientSecret = result.ClientSecret;
            }
            // 5.2. PaymentIntentId Is NOT Empty => Update PaymentIntent
            else
            {
                // Update
                var result = await _paymentGateway.UpdatePaymentIntentAsync(amount, basket.PaymentIntentId, ct);
            }
            await _basketRepository.CreateOrUpdateBasketAsync(basket, ct:ct);

            // 6. Return BasketDto UPDATED
            return _mapper.Map<BasketDto>(basket);
        }
    }
}
