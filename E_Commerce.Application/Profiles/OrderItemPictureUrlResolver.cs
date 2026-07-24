using AutoMapper;
using E_Commerce.Application.DTOs.Orders;
using E_Commerce.Application.DTOs.Products;
using E_Commerce.Domain.Entities.Orders;
using E_Commerce.Domain.Entities.Products;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Application.Profiles
{
    internal class OrderItemPictureUrlResolver : IValueResolver<OrderItem, OrderItemDto, string>
    {
        private readonly UrlSettings _settings;
        public OrderItemPictureUrlResolver(IOptions<UrlSettings> options)
        {
            _settings = options.Value;
        }

        public string Resolve(OrderItem source, OrderItemDto destination, string destMember, ResolutionContext context)
        {
            // Source => images/products/FormalBlazer.jpg
            // Return => https://localhost:7198/Files/images/products/FormalBlazer.jpg
            var baseUrl = _settings.BaseUrl.TrimEnd('/');
            var path = source.Product.PictureUrl.TrimStart('/');
            return $"{baseUrl}/Files/{path}";
        }
    }
}
