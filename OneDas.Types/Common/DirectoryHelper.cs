using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace OneDas.Common
{
    public static class DirectoryHelper
    {
        public static IEnumerable<string> EnumerateFiles(string directoryPath, string searchPatternExpression, SearchOption searchOption)
        {
            Regex regex;

            regex = new Regex(searchPatternExpression, RegexOptions.IgnoreCase);

            return Directory.EnumerateFiles(directoryPath, "*", searchOption).Where(x => regex.IsMatch(x));
        }
    }
}
