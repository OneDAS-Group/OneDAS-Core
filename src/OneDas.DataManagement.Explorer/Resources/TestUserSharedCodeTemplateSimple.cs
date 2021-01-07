using System;

namespace OneDas.DataManagement.Explorer.Filters
{
    /// <summary>
    /// The purpose of this class is to provide shared code, i.e. predefined and 
    /// resuable functions. By default this class is static but you may change it
    /// to be instantiatable. Also, you may rename or create another class within
    /// this code file. All files of kind 'shared' get linked to other 'normal'
    /// code files to make their content available there.
    /// </summary>
    static class Shared
    {
        public static double AddOne(double value)
        {
            return value + 1;
        }
    }
};