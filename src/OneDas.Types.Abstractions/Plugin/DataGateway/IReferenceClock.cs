using System;

namespace OneDas.Plugin
{
    public interface IReferenceClock
    {
        DateTime GetUtcDateTime();
        long GetTimerDrift();
    }
}
