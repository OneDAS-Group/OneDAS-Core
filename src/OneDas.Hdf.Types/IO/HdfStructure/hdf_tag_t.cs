using System.Runtime.InteropServices;

namespace OneDas.Hdf.IO
{
    /// <summary>
    /// Represents a structure to hold data about an EtherCAT slave to be written to a hierachical data format file.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct hdf_tag_t
    {
        #region "Fields"

        [MarshalAs(UnmanagedType.LPStr)]
        public string date_time;
        [MarshalAs(UnmanagedType.LPStr)]
        public string name;
        [MarshalAs(UnmanagedType.LPStr)]
        public string mode;
        [MarshalAs(UnmanagedType.LPStr)]
        public string comment;

        #endregion

        #region "Constructors"

        /// <summary>
        /// Creates as new <see cref="hdf_tag_t"/>.
        /// </summary>
        /// <param name="date_time">The current <see cref="DateTime"/>.</param>
        /// <param name="type">The transfer function representation.</param>
        /// <param name="argument">The transfer function input argument.</param>
        /// <param name="option">The transfer function option.</param>
        public hdf_tag_t(string date_time, string name, string mode, string comment)
        {
            this.date_time = date_time;
            this.name = name;
            this.mode = mode;
            this.comment = comment;
        }

        #endregion

        /// <summary>
        /// Compares two copies of <see cref="hdf_tag_t"/>.
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
                hdf_tag_t _hdf_tag = (hdf_tag_t)obj;

                return this.date_time == _hdf_tag.date_time &&
                       this.name == _hdf_tag.name &&
                       this.mode == _hdf_tag.mode &&
                       this.comment == _hdf_tag.comment;
            }            
        }

        public override int GetHashCode()
        {
            return this.date_time.Length + this.name.Length + this.mode.Length + this.comment.Length;
        }
    }
}