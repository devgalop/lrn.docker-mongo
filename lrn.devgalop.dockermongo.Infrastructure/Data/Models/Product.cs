using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace lrn.devgalop.dockermongo.Infrastructure.Data.Models
{
    public class Product
    {
        public ObjectId Id { get; set; }

        [BsonElement("uuid")]    
        [Required(ErrorMessage = "Product unique identifier cannot be null or empty")]
        public string UUID { get; set; } = string.Empty;

        [BsonElement("name")]
        [Required(ErrorMessage = "Product name could not be null or empty")]
        public string Name { get; set; } = string.Empty;

        [BsonElement("detail")]
        [MaxLength(150, ErrorMessage = "Product description has too many characters. Max length is 150.")]
        public string? Description { get; set; }

        [BsonElement("registration_date")]
        public DateTime RegistrationDate { get; set; }

        [BsonElement("update_date")]
        public DateTime UpdateDate { get; set; }

        [BsonElement("status")]
        public bool Status { get; set; } = true;

        [BsonElement("unit_price")]
        [Range(0,double.MaxValue, ErrorMessage = $"Prices cannot be negative")]
        public double UnitPrice { get; set; }

        [BsonElement("sell_unit_price")]
        [Range(0,double.MaxValue, ErrorMessage = $"Prices cannot be negative")]
        public double SellUnitPrice { get; set; }
    }
}