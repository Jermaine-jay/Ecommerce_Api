using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Services.Configurations.Jwt
{
    public class JwtConfig
    {
        public string Secret { get; set; }
        public double Expires { get; set; }
        public string ImpersonationExpires { get; set; }
        public string Issuer { get; set; }
        public string Audience { get; set; }
    }
}
