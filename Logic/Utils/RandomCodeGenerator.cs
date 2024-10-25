using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logic.Utils
{
    public class RandomCodeGenerator
    {
        private static readonly Random random = new Random();
        private const string letters = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
        private const string numbers = "1234567890";

        public static string GenerateRandomCode(int length = 15, bool? isNumeric = null)
        {
            StringBuilder sb = new StringBuilder(length);
            var chars = isNumeric == true ? numbers : numbers + letters;
            for (int i = 0; i < length; i++)
            {
                sb.Append(chars[random.Next(chars.Length)]);
            }

            return sb.ToString();
        }
    }
}
