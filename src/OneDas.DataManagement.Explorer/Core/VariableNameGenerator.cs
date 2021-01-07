using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace OneDas.DataManagement.Explorer.Core
{
    public static class VariableNameGenerator
    {
        #region Records

        record RandomWords([property: JsonPropertyName("data")] List<string> Data);

        #endregion

        #region Fields

        private static Random _random;
        private static RandomWords _randomWords;

        #endregion

        #region Constructors

        static VariableNameGenerator()
        {
            _random = new Random();

            var blob = ResourceLoader.GetResourceBlob("OneDas.DataManagement.Explorer.Resources.RandomWords.json");
            var options = new JsonSerializerOptions() { ReadCommentHandling = JsonCommentHandling.Skip };

            _randomWords = JsonSerializer.Deserialize<RandomWords>(blob, options);
        }

        #endregion

        #region Methods

        public static string Generate()
        {
            var i = _random.Next(0, _randomWords.Data.Count);
            int j = i;

            while (j == i)
            {
                j = _random.Next(0, _randomWords.Data.Count);
            }

            var first = VariableNameGenerator.FirstCharToUpper(_randomWords.Data[i]);
            var second = VariableNameGenerator.FirstCharToUpper(_randomWords.Data[j]);

            return $"{first}{second}";
        }

        private static string FirstCharToUpper(string value)
        {
            return value.First().ToString().ToUpper() + value.Substring(1);
        }

        #endregion
    }
}
