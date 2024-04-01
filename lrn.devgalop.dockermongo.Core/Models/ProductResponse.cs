using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace lrn.devgalop.dockermongo.Core.Models
{
    public class ProductResponse : BaseResponse
    {
        public string Name { get; set; } = string.Empty;
        public double UnitPrice { get; set; }
        public int Stock { get; set; }
    }
}