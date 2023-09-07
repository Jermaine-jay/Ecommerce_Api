using Microsoft.AspNetCore.Authentication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Services.Interfaces
{
    public interface IAuthServices
    {
        Task<object> GoogleAuth(AuthenticateResult authenticateResult);
    }
}
