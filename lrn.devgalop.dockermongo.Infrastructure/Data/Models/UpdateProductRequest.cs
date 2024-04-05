using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace lrn.devgalop.dockermongo.Infrastructure.Data.Models
{
    public class UpdateProductRequest
    {
        [Required(ErrorMessage = "Product name could not be null or empty")]
        public string Name { get; set; } = string.Empty;
        
        [MaxLength(150, ErrorMessage = "Product description has too many characters. Max length is 150.")]
        public string? Description { get; set; }

        public DateTime UpdateDate { get; set; } = DateTime.Now;

        public bool Status { get; set; } = true;

        [Range(0,double.MaxValue, ErrorMessage = $"Prices cannot be negative")]
        public double UnitPrice { get; set; }

        [Range(0,double.MaxValue, ErrorMessage = $"Prices cannot be negative")]
        public double SellUnitPrice { get; set; }
    }
}