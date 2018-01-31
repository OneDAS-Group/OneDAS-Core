using System;
using System.Collections.Generic;

namespace OneDas.Common
{
    public static class StringHelper
    {
        public static string Reverse(this string value)
        {
            char[] charArray;

            charArray = value.ToCharArray();
            Array.Reverse(charArray);

            return new string(charArray);
        }

        public static int[] ToCodePoints(this string value)
        {
            List<int> codePoints;

            if (value == null)
            {
                throw new ArgumentNullException("str");
            }
                
            codePoints = new List<int>(value.Length);

            for (int i = 0; i < value.Length; i++)
            {
                codePoints.Add(Char.ConvertToUtf32(value, i));

                if (Char.IsHighSurrogate(value[i]))
                {
                    i += 1;
                }
            }

            return codePoints.ToArray();
        }
    }
}
