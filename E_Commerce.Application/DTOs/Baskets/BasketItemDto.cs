using System.ComponentModel.DataAnnotations;

namespace E_Commerce.Application.DTOs.Baskets
{
    public class BasketItemDto
    {
        [Required(ErrorMessage ="Product Id is Required")]
        public int Id { get; set; }
        [Required(ErrorMessage = "Product Name is Required")]
        public string ProductName { get; set; } = default!;
        public string PictureUrl { get; set; } = default!;
        [Range(1, double.MaxValue, ErrorMessage ="Price Must be Positive")]
        public decimal Price { get; set; }
        [Range(1,50, ErrorMessage ="Quantity Must be Between 1 and 50")]
        public int Quantity { get; set; }
    }
}