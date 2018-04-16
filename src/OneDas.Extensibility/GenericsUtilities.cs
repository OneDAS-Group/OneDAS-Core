using System;
using System.Reflection.Emit;

namespace OneDas
{
    public static class GenericToDouble<T>
    {
        private static Func<T, double> _to_double_function = GenericToDouble<T>.EmitToDoubleConverter();

        private static Func<T, double> EmitToDoubleConverter()
        {
            // check if convertible

            DynamicMethod method = new DynamicMethod(string.Empty, typeof(double), new Type[] { typeof(T) });
            ILGenerator ilGenerator = method.GetILGenerator();

            ilGenerator.Emit(OpCodes.Ldarg_0);

            if (typeof(T) != typeof(double))
            {
                ilGenerator.Emit(OpCodes.Conv_R8);
            }

            ilGenerator.Emit(OpCodes.Ret);

            return (Func<T, double>)method.CreateDelegate(typeof(Func<T, double>));
        }

        public static double ToDouble(T value)
        {
            return _to_double_function(value);
        }
    }
}