using System;
using System.Collections.Generic;

namespace API.Responses
{
    public class OrderResponse
    {
        public string IdOrder { get; set; }
        public List<LinkResponse> LinkResponses { get; set; }
        public object Status { get; set; }
    }

    public class LinkResponse 
    {
        public string Url { get; set; }
        public string Method { get; set; }
        public string Rel { get; set; }
    }

}
