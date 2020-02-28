using System.Collections.Generic;

namespace OneDas.DataManagement.BlazorExplorer.Core
{
    public class SampleRateStringComparer : IComparer<string>
    {
        public int Compare(string x, string y)
        {
            if (x.Contains("Hz") && !y.Contains("Hz"))
                return -1;

            else if (!x.Contains("Hz") && y.Contains("Hz"))
                return 1;

            else
            {
                if (x.Contains("Hz"))
                {
                    x = x.Split(' ')[0];
                    y = y.Split(' ')[0];

                    if (int.Parse(x) < int.Parse(y))
                        return 1;
                    else if (int.Parse(x) > int.Parse(y))
                        return -1;
                    else
                        return 0;
                }
                else
                {
                    x = x.Split(' ')[0];
                    y = y.Split(' ')[0];

                    if (int.Parse(x) < int.Parse(y))
                        return -1;
                    else if (int.Parse(x) > int.Parse(y))
                        return 1;
                    else
                        return 0;
                }
            }
        }
    }
}
