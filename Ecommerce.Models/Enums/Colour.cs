using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Models.Enums
{
    public enum Colour
    {
        AsDisplayed = 1,
        Black,
        White,
        Red,
        Green,
        Yellow,
        Blue,
        Purple,
    }

    public static class ColourExtension
    {
        public static string? GetStringValue(this Colour colour)
        {
            return colour switch
            {
                Colour.Black => "Black",
                Colour.White => "White",
                Colour.Red => "Red",
                Colour.Green => "Black",
                Colour.Yellow => "White",
                Colour.Blue => "Red",
                Colour.Purple => "Purple",
                Colour.AsDisplayed => "AsDisplayed",
                _ => null
            };
        }
    }
}
