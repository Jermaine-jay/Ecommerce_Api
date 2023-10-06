using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Models.Enums
{
    public enum CountryCurrency
    {
        NigerianNaira = 1
    }

    public static class CurrencyTypeExtension
    {
        public static string? GetStringValue(this CountryCurrency currency)
        {
            return currency switch
            {
                CountryCurrency.NigerianNaira => "NGN",
                _ => null
            };
        }
    }
}
