using AutoMapper;
using E_Commerce.Application.Common;
using E_Commerce.Application.Contracts;
using E_Commerce.Application.DTOs.Orders;
using E_Commerce.Application.Specifications;
using E_Commerce.Domain.Contracts;
using E_Commerce.Domain.Entities.Orders;
using E_Commerce.Domain.Entities.Products;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Application.Services
{
    public class OrderService : IOrderService
    {
        private readonly IBasketRepository _basketRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public OrderService(IBasketRepository basketRepository,
                            IUnitOfWork unitOfWork,
                            IMapper mapper)
        {
            _basketRepository = basketRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        public async Task<Result<OrderToReturnDto>> CreateOrderAsync(OrderDto orderDto, string email, CancellationToken ct = default)
        {
            var basket = await _basketRepository.GetBasketAsync(orderDto.BasketId, ct);
            if (basket == null)
                return Error.NotFound("Basket Not Found", $"Basket with Id {orderDto.BasketId} Is Not Found");
            if (basket.Items.Count == 0)
                return Error.Validation("Basket Is Empty", $"Cannot Create Order With Basket Id {basket.Id}");

            //  1) Get Order Items
            var orderItems = new List<OrderItem>(basket.Items.Count);
            var productIds = basket.Items.Select(x => x.Id).ToHashSet();
            var products = (await _unitOfWork.GetRepository<Product, int>()
                .GetAllAsync(new ProductWithIdSpecifications(productIds), ct)).ToDictionary(x=>x.Id);
            foreach (var item in basket.Items)
            {
                //var product = await _unitOfWork.GetRepository<Product, int>().GetByIdAsync(item.Id); // N + 1 Problem!!!!!
                //var product = await _unitOfWork.GetRepository<Product, int>().GetAllAsync(ct); // Works but not best solution!!
                // Get product
                if (!products.TryGetValue(item.Id, out var product))
                    return Error.NotFound("Product Not Found", $"Product With Id {item.Id} Is Not Found");
                orderItems.Add(new OrderItem()
                {
                    Price = product.Price,
                    Quantity = item.Quantity,
                    Product = new ProductItemOrdered()
                    {
                        PictureUrl = product.PictureUrl,
                        ProductId = product.Id,
                        ProductName = product.Name
                    }
                });
            }
            //  2) Get ShipToAddress (Order Address)
            var orderAddress = _mapper.Map<OrderAddress>(orderDto.ShipToAddress);
            //  3) Get DeliverMethod
            var deliveryMethod = await _unitOfWork.GetRepository<DeliveryMethod, int>().GetByIdAsync(orderDto.DeliveryMethodId);
            if (deliveryMethod == null)
                return Error.NotFound("DeliveryMethod Not Found", $"Delivery Method With Id {orderDto.DeliveryMethodId} Is Not Found");
            //  4) Sub Total
            var subTotal = orderItems.Sum(x => x.Quantity * x.Price);
            var order = new Order(email,orderAddress,orderItems, deliveryMethod, subTotal);
            _unitOfWork.GetRepository<Order, Guid>().Add(order);
            var result = await _unitOfWork.SaveChangesAsync(ct);
            if(result == 0)
            {
                // fail
                return Error.Failure("Failed To Save Order", $"Cannot Create Order");
            }
            else
            {
                // success
                await _basketRepository.DeleteBasketAsync(orderDto.BasketId, ct);
                return _mapper.Map<OrderToReturnDto>(order);
            }
        }

        public async Task<Result<IReadOnlyList<OrderToReturnDto>>> GetAllOrdersForUserAsync(string email, CancellationToken ct = default)
        {
            var orders = await _unitOfWork.GetRepository<Order, Guid>()
                .GetAllAsync(new OrderSpecifications(email), ct);
            if(orders.Any())
            {
                return Result<IReadOnlyList<OrderToReturnDto>>.Ok(_mapper.Map<IReadOnlyList<OrderToReturnDto>>(orders));
            }
            else
            {
                return Error.NotFound("Orders Not Found", $"No Orders Found For User With Email {email}");
            }
        }

        public async Task<Result<IReadOnlyList<DeliveryMethodDto>>> GetDeliveryMethodsAsync(CancellationToken ct = default)
        {
            var deliverMethods = await _unitOfWork.GetRepository<DeliveryMethod, int>().GetAllAsync(ct);
            if (deliverMethods.Any())
                return Result<IReadOnlyList<DeliveryMethodDto>>
                    .Ok(_mapper.Map<IReadOnlyList<DeliveryMethodDto>>(deliverMethods));
            else
                return Error.NotFound("No Deliver Methods Found");
        }

        public async Task<Result<OrderToReturnDto>> GetOrderByIdAndEmailForUserAsync(Guid id, string email, CancellationToken ct = default)
        {
            var order = await _unitOfWork.GetRepository<Order, Guid>().GetByIdAsync(new OrderSpecifications(id,email),id, ct);
            if(order == null)
            {
                return Error.NotFound("Order Is Not Found", $"Order with Id {id} is Not Found");
            }
            else
            {
                return _mapper.Map<OrderToReturnDto>(order);
            }
        }
    }
}
