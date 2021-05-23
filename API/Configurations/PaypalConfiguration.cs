using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Configurations
{
    public class PaypalConfiguration
    {
        public Credentials Credentials { get; set; }
        public Payee Payee { get; set; }
        public WebSite WebSite { get; set; }
        public bool SandboxEnvironment { get; set; }
    }

    public class Credentials 
    {
        public string Account { get; set; }
        public string ClientID { get; set; }
        public string Secret { get; set; }
    }

    public class Payee 
    {
        public string BrandName { get; set; }
        public string CountryCallingCode { get; set; }
        public string ExtensionNumber { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
    }

    public class WebSite 
    {
        public string ReturnUrl { get; set; }
        public string CancelUrl { get; set; }
        public string BaseUrl { get; set; }
        public string WebUrl { get; set; }
    }


}
