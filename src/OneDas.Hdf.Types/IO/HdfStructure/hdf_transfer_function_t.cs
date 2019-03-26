using System.Runtime.InteropServices;

namespace OneDas.Hdf.IO
{
    /// <summary>
    /// Represents a structure to hold data about an EtherCAT slave to be written to a hierachical data format file.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct hdf_transfer_function_t
    {
        #region "Fields"

        [MarshalAs(UnmanagedType.LPStr)]
        public string date_time;
        [MarshalAs(UnmanagedType.LPStr)]
        public string type;
        [MarshalAs(UnmanagedType.LPStr)]
        public string option;
        [MarshalAs(UnmanagedType.LPStr)]
        public string argument;

        #endregion

        #region "Constructors"

        /// <summary>
        /// Creates as new <see cref="hdf_transfer_function_t"/>.
        /// </summary>
        /// <param name="date_time">The current <see cref="DateTime"/>.</param>
        /// <param name="type">The transfer function representation.</param>
        /// <param name="argument">The transfer function input argument.</param>
        /// <param name="option">The transfer function option.</param>
        public hdf_transfer_function_t(string date_time, string type, string option, string argument)
        {
            this.date_time = date_time;
            this.type = type;
            this.option = option;
            this.argument = argument;
        }

        #endregion

        /// <summary>
        /// Compares two copies of <see cref="hdf_transfer_function_t"/>.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns>Returns True if both copies are equal.</returns>
        public override bool Equals(object obj)
        {
            if (obj == null || !this.GetType().Equals(obj.GetType()))
            {
                return false;
            }
            else
            {
                hdf_transfer_function_t _hdf_transfer_function = (hdf_transfer_function_t)obj;

                return this.date_time == _hdf_transfer_function.date_time &&
                       this.type == _hdf_transfer_function.type &&
                       this.option == _hdf_transfer_function.option &&
                       this.argument == _hdf_transfer_function.argument;
            }
        }

        public override int GetHashCode()
        {
            return this.date_time.Length + this.type.Length + this.option.Length + this.argument.Length;
        }
    }
}