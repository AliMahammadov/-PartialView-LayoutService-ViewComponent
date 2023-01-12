using System.ComponentModel.DataAnnotations;

namespace Pronia.Models
{
    public class Product
    {

        public int Id { get; set; }
    
        [MinLength( 2, ErrorMessage = "uzuluq 2 ola bilmez"), MaxLength(40, ErrorMessage = "uzuluq 40 ola bilmez")]
        public string Name { get; set; }
        public float Price { get; set; }
        public string ImgUrl { get; set; }
        public int CategoryId { get; set; }
        public Category Category { get; set; }
    }
}
