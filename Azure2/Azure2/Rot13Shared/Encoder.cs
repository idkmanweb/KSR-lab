using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rot13Shared
{
    public static class Encoder
    {
        public static string Encode(string input)
        {
            char Rot13Char(char c)
            {
                if (c >= 'a' && c <= 'z')
                    return (char)('a' + (c - 'a' + 13) % 26);
                if (c >= 'A' && c <= 'Z')
                    return (char)('A' + (c - 'A' + 13) % 26);
                return c;
            }

            return new string(input.Select(Rot13Char).ToArray());
        }
    }
}
