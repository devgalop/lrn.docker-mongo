using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace lrn.devgalop.dockermongo.Core.Models
{
    public class BasicProductResponse : BaseResponse
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public DateTime RegistrationDate { get; set; }
        public bool Status { get; set; }
        public double UnitPrice { get; set; }
        public double SellUnitPrice { get; set; }
    }
}