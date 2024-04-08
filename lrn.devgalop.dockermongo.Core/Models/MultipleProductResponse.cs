using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace lrn.devgalop.dockermongo.Core.Models
{
    public class MultipleProductResponse : BaseResponse
    {
        public IEnumerable<ProductResponse>? Products {get; set;}
    }
}