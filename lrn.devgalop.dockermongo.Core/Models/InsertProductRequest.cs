using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace lrn.devgalop.dockermongo.Core.Models
{
    public class InsertProductRequest
    {
        [Required(ErrorMessage = "Product name could not be null or empty")]
        public string Name { get; set; } = string.Empty;

        [MaxLength(150, ErrorMessage = "Product description has too many characters. Max length is 150.")]
        public string? Description { get; set; }

        [Range(0,double.MaxValue, ErrorMessage = $"Prices cannot be negative")]
        public double UnitPrice { get; set; }

        [Range(0,double.MaxValue, ErrorMessage = $"Prices cannot be negative")]
        public double SellUnitPrice { get; set; }
    }
}