using System;
using System.Collections.Generic;

namespace OneDas.Extension.Mat73
{
    public static class Mat73Utilities
    {
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
