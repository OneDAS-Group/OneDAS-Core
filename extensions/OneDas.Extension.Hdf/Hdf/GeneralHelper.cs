using System;
using System.Collections.Generic;
using System.Linq;

namespace OneDas.Extension.Hdf
{
    public static class GeneralHelper
    {
        public static (ulong, ulong) CalculateChunkParameters(ulong periodInSeconds, decimal samplesPerSecond)
        {
            var maxChunkLength = 4096UL;
            var length = (ulong)(periodInSeconds * samplesPerSecond);
            var chunkLength = GeneralHelper.FindLargestDivisor(length, maxChunkLength);
            var chunkCount = length / chunkLength;

            return (chunkLength, chunkCount);
        }

        private static ulong FindLargestDivisor(ulong length, ulong limit)
        {
            if (length < limit)
                return length;

            var primes = GeneralHelper.Factorize(length)
                .GroupBy(value => value)
                .Select(group => group.Aggregate(1UL, (previous, next) => previous * next))
                .Where(prime => prime <= limit)
                .ToList();

            var products = GeneralHelper.Powerset(primes)
                .Select(combo => combo.Aggregate(1UL, (previous, next) => previous * next))
                .ToList();

            products.Sort();

            var result = products.LastOrDefault(value => value <= limit);

            return result;
        }

        private static List<ulong> Factorize(ulong number)
        {
            var primes = new List<ulong>();

            for (ulong div = 2; number > 1; div++)
            {
                if (number % div == 0)
                {
                    while (number % div == 0)
                    {
                        number /= div;
                        primes.Add(div);
                    }
                }
            }

            return primes;
        }

        private static List<List<T>> Powerset<T>(List<T> list)
        {
            var comboCount = (int)Math.Pow(2, list.Count) - 1;
            var result = new List<List<T>>();

            for (int i = 1; i < comboCount + 1; i++)
            {
                result.Add(new List<T>());

                for (int j = 0; j < list.Count; j++)
                {
                    if ((i >> j) % 2 != 0)
                        result.Last().Add(list[j]);
                }
            }

            return result;
        }
    }
}