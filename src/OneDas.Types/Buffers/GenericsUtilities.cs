using System;
using System.Linq.Expressions;
using System.Reflection.Emit;

namespace OneDas.Buffers
{
    public static class GenericToDouble<T>
    {
        private static Func<T, double> _to_double_function = GenericToDouble<T>.EmitToDoubleConverter();

        private static Func<T, double> EmitToDoubleConverter()
        {
            var method = new DynamicMethod(string.Empty, typeof(double), new Type[] { typeof(T) });
            var ilGenerator = method.GetILGenerator();

            ilGenerator.Emit(OpCodes.Ldarg_0);

            if (typeof(T) != typeof(double))
                ilGenerator.Emit(OpCodes.Conv_R8);

            ilGenerator.Emit(OpCodes.Ret);

            return (Func<T, double>)method.CreateDelegate(typeof(Func<T, double>));
        }

        public static double ToDouble(T value)
        {
            return _to_double_function(value);
        }
    }

    public static class GenericAdd<T>
    {
        private static Func<T, T, T> _add_function = GenericAdd<T>.EmitAddFunction();

        private static Func<T, T, T> EmitAddFunction()
        {
            var _parameterA = Expression.Parameter(typeof(T), "a");
            var _parameterB = Expression.Parameter(typeof(T), "b");

            var _body = Expression.Add(_parameterA, _parameterB);

            return Expression.Lambda<Func<T, T, T>>(_body, _parameterA, _parameterB).Compile();
        }

        public static T Add(T a, T b)
        {
            return _add_function(a, b);
        }
    }

    public static class GenericBitOr<T>
    {
        private static Func<T, T, T> _bit_or_function = GenericBitOr<T>.EmitBitOrFunction();

        private static Func<T, T, T> EmitBitOrFunction()
        {
            var _parameterA = Expression.Parameter(typeof(T), "a");
            var _parameterB = Expression.Parameter(typeof(T), "b");

            var _body = Expression.Or(_parameterA, _parameterB);

            return Expression.Lambda<Func<T, T, T>>(_body, _parameterA, _parameterB).Compile();
        }

        public static T BitOr(T a, T b)
        {
            return _bit_or_function(a, b);
        }
    }

    public static class GenericBitAnd<T>
    {
        private static Func<T, T, T> _bit_and_function = GenericBitAnd<T>.EmitBitAndFunction();

        private static Func<T, T, T> EmitBitAndFunction()
        {
            var _parameterA = Expression.Parameter(typeof(T), "a");
            var _parameterB = Expression.Parameter(typeof(T), "b");

            var _body = Expression.And(_parameterA, _parameterB);

            return Expression.Lambda<Func<T, T, T>>(_body, _parameterA, _parameterB).Compile();
        }

        public static T BitAnd(T a, T b)
        {
            return _bit_and_function(a, b);
        }
    }
}