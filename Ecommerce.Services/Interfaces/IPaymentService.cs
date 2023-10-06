using Ecommerce.Services.Implementations;
using PayStack.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Services.Interfaces
{
    public interface IPaymentService
    {
        Task<string> AvailableSystem();
    }
}
