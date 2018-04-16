using System;

namespace OneDas.Core.Engine
{
    // still unclear if this is useful
    [Flags]
    public enum DataStorageFlags
    {
        IsValid = 0,                    // data are fully valid
        WithinLagTolerance = 1,         // data were collected within 2 x sample rate [drawback: no gain of information]
        NoSync = 2                      // time not synchronized
    }
}
